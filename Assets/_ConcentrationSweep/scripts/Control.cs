using System;
using UnityEngine;
using BUtility;
using UniExtensions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Control : MonoBehaviour
{
  static public Control I;

  public Game G;

  void Start()
  {
    I = this;

    ResetGame();
  }

  public void ResetGame()
  {
    G = new Game();

    View.I.Render( G );
  }

  public void Exit()
  {
    if( G == null || G.State == GameState.Login )
    {
      #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
      #else
        Application.Quit();
      #endif
    }
    else
    {
      SceneManager.LoadScene( 0 );
    }
  }

  public Player at( int tablePos )
  {
    return G.PotentialPlayers[tablePos];
  }

  public void SetPlayerPlaying( int tablePos, bool qPlaying )
  {
    var p = at( tablePos );
    p.Playing = qPlaying;
    View.I.Render( G );
  }

  public void ToggleOption( int optionNum )
  {
    int bit = 1 << optionNum;

    // This stops 0 sets from being selected.
    if( G.Sets == bit )
      bit = 3;

    G.Sets ^= bit;
    View.I.Render( G );
  }

  public void Play()
  {
    G.BuildPlayers();
    G.GenerateBoard();
    G.PickRandomPlayer();
    G.InitCurrentPlayer();

    View.I.TransitionToGame( G );
    View.I.Render( G );
  }

  public void Reveal( int x, int y )
  {
    if( G.State != GameState.Player_Picking )
      return;

    bool qActualReveal = G.Reveal( x, y );
    if( qActualReveal )
      Audio.PlayClick();

    View.I.Render( G );

    if( G.State == GameState.Flip_Result_Bomb )
    {
      DOVirtual.DelayedCall( 0.5f, () => {
        View.I.ExplodeBomb( G );
        Audio.PlayTsss();
        } );
      DOVirtual.DelayedCall( 1.1f, ProcessBomb );
    }
    else if( G.State == GameState.Flip_Result_Match )
    {
      DOVirtual.DelayedCall( 1f, ProcessMatch );
    }
  }

  public void ProcessBomb()
  {
    Audio.PlayExplosion();

    G.Result();

    DOVirtual.DelayedCall( 1f, NextPlayerTurn );

    View.I.Render( G );
  }

  public void ProcessMatch()
  {
    int matchesPrior = G.MatchesRemaining;
    G.Result();
    bool matchFound = (G.MatchesRemaining != matchesPrior);

    if( matchFound )
      Audio.PlayMatch();
    else
      Audio.PlayBadMatch();

    if( G.MatchesRemaining == 0 )
    {
      View.I.GiveMatch( G );
      DOVirtual.DelayedCall( 1f, () => {
        G.GameOver();
        Audio.PlayGameOver();
        View.I.Render( G );
      } );
    }
    else if( matchFound )
    {
      View.I.GiveMatch( G );
      DOVirtual.DelayedCall( 1f, CurrentPlayerAgain );
    }
    else
    {
      DOVirtual.DelayedCall( 1f, NextPlayerTurn );
    }
  }

  public void CurrentPlayerAgain()
  {
    G.InitCurrentPlayer();

    View.I.Render( G );
  }

  public void NextPlayerTurn()
  {
    G.InitNextPlayer();

    View.I.UpdateFairy( G );
    View.I.Render( G );
  }
}