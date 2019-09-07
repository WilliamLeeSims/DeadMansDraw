using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using BUtility;
using System.Collections.Generic;
using DG.Tweening;
using System.Text;
using UnityEngine.EventSystems;

public class View : MonoBehaviour
{
  public static View I;

  [Header( "Main Menu" )]
  public Button StartButton;
  public Bob StartBombBobber;
  public GameObject OptionPanel;
  public GameObject[] Options;
  public Button ExitButton;

  [Header( "Game" )]
  public GameObject StatPanel;
  public TextMeshProUGUI GameStats;
  public Transform CurrentPlayerFairy;
  public SweepCardView[] CardViews;

  [Header( "Players" )]
  public PlayerView[] PlayerViews;

  [Header( "Prefabs" )]
  public float SpringForce;
  public float AirResistance;
  public Sprite[] Desserts;
  public Sprite[] Pets;
  public Sprite[] PlaceRibbons;
  public Color[] BombCountColors;
  public ParticleSystem Explosion;

  public void Start()
  {
    I = this;
  }

  public PlayerView at( int tablePos )
  {
    return PlayerViews[tablePos];
  }

  public void TurnOnFairy( Game g )
  {
    CurrentPlayerFairy.position = at( g.CurrentPlayer.TablePos ).transform.position + Vector3.forward;
    CurrentPlayerFairy.GetComponent<ParticleSystem>().Play( true );
  }
  public void UpdateFairy( Game g )
  {
    CurrentPlayerFairy
      .DOMove( at( g.CurrentPlayer.TablePos ).transform.position + Vector3.forward,
               2 );
  }
  public void TurnOffFairy()
  {
    CurrentPlayerFairy.GetComponent<ParticleSystem>().Stop( true );
  }

  public void TransitionToGame( Game g )
  {
    StartButton.gameObject.SetActive( false );
    StatPanel.SetActive( true );
    OptionPanel.SetActive( false );

    TurnOnFairy( g );

    for( int i = 0; i < Game.MAX_PLAYERS; ++i )
    {
      var p = g.at( i );
      PlayerViews[i].gameObject.SetActive( p.Playing );
    }

    foreach( var cv in CardViews )
      cv.gameObject.SetActive( true );
    renderGrid( g );
  }

  public void Render( Game g )
  {
    if( g.State == GameState.Login )
    {
      StatPanel.SetActive( false );
      StartButton.gameObject.SetActive( true );
      StartButton.interactable = g.PotentialPlayers.Count( tp => tp.Playing ) > 0;
      StartBombBobber.enabled = StartButton.interactable;
      OptionPanel.SetActive( true );
      Options[0].SetActive( (g.Sets & 1) > 0 );
      Options[1].SetActive( (g.Sets & 2) > 0 );

      for( int i = 0; i < Game.MAX_PLAYERS; ++i )
      {
        PlayerViews[i].gameObject.SetActive( true );
        PlayerViews[i].Render( this, g, g.at( i ) );
      }

      foreach( var cv in CardViews )
        cv.gameObject.SetActive( false );
    }
    else if( g.State == GameState.Player_Picking )
    {
      GameStats.text = string.Format(
        "Matches to Find: <b><size=125%>{0}</size></b>\nHidden Bombs: <b><size=125%>{1}</size></b>",
        g.MatchesRemaining, g.BombsRemaining );

      foreach( var p in g.Players )
        at( p.TablePos ).Render( this, g, p );

      renderGrid( g );
    }
    else if( g.State == GameState.Flip_Result_Bomb )
    {
      renderGrid( g );
    }
    else if( g.State == GameState.Flip_Result_Match )
    {
      renderGrid( g );
    }
    else if( g.State == GameState.GameOver )
    {
      GameStats.text = string.Format(
        "Matches to Find: <b><size=125%>{0}</size></b>\nHidden Bombs: <b><size=125%>{1}</size></b>",
        g.MatchesRemaining, g.BombsRemaining );

      renderGrid( g );
      TurnOffFairy();

      for( int i = 0; i < Game.MAX_PLAYERS; ++i )
      {
        var p = g.at( i );
        if( p.Playing )
          PlayerViews[i].Render( this, g, g.at( i ) );
      }
    }
  }

  void renderGrid( Game g )
  {
    for( int x = 0; x < g.Width; ++x )
    {
      for( int y = 0; y < g.Height; ++y )
      {
        var cv = CardViews[y * g.Width + x];
        cv.CardRectTransform.gameObject.SetActive( g.Grid[x, y].HasValue );
        cv.Set( x, y, g.Grid[x, y], g.SweepNumber( x, y ), g.Seen[x, y] );
        bool qFlip = (g.PickNumber > 0 && g.Picks[0].X == x && g.Picks[0].Y == y) ||
                     (g.PickNumber > 1 && g.Picks[1].X == x && g.Picks[1].Y == y);
        cv.SetFlip( qFlip );
      }
    }
  }

  public void ExplodeBomb( Game g )
  {
    var pick = g.Picks[g.PickNumber - 1];
    var cv = CardViews[pick.Y * g.Width + pick.X];
    cv.BombAnimation.GetComponent<CardFlip>().enabled = true;
    DOVirtual.DelayedCall( 0.5f, () => {
      Explosion.transform.position = cv.transform.position - 10 * Vector3.forward;
      Explosion.Play();
    } );
  }

  public void GiveMatch( Game g )
  {
    for( int i = 0; i < 2; ++i )
    {
      var pick = g.Picks[i];
      var cv = CardViews[pick.Y * g.Width + pick.X];
      var pv = at( g.CurrentPlayer.TablePos );
      cv.SymbolSprite.transform.DOMove( pv.transform.position, 0.85f );
      cv.SymbolSprite.transform.DOScale( Vector3.zero, 0.85f ).SetEase( Ease.InQuad );
    }
  }
}
