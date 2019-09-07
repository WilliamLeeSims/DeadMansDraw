using BUtility;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniExtensions;
using UnityEngine;

public class Game
{
  public static int MIN_PLAYERS = 1;
  public static int MAX_PLAYERS = 12;

  public GameState State = GameState.Login;

  public Player[] PotentialPlayers = new Player[MAX_PLAYERS];
  public List<Player> Players;
  public int Sets = 1;

  public int?[,] Grid;
  public bool[,] Seen;
  public int Width;
  public int Height;
  public int Symbols;
  public int Copies;
  public int TotalBombs;

  public int BombsRemaining;
  public int MatchesRemaining;

  public Player CurrentPlayer;
  public int PickNumber;
  public Coord[] Picks = new Coord[2];

  public Game()
  {
    for( int i = 0; i < PotentialPlayers.Length; ++i )
      PotentialPlayers[i] = new Player { TablePos = i };
  }

  public Player at( int tablePos )
  {
    return PotentialPlayers[tablePos];
  }
  public Player next( Player prev )
  {
    int index = Players.IndexOf( prev );
    int newIndex = (index + 1) % Players.Count;
    return Players[newIndex];
  }

  public void Join( int tablePos )
  {
    PotentialPlayers[tablePos].Playing = true;
  }
  public void Leave( int tablePos )
  {
    PotentialPlayers[tablePos].Playing = false;
  }

  public void BuildPlayers()
  {
    Players = PotentialPlayers.Where( pp => pp.Playing ).ToList();
  }

  public void GenerateBoard()
  {
    Width = 10;
    Height = 5;
    int minSymbol = Sets == 2 ? 10 : 0;
    Symbols = Sets == 3 ? 20 : 10;
    Copies =  Sets == 3 ? 2 : 4;
    TotalBombs = Width * Height - Symbols * Copies;

    BombsRemaining = TotalBombs;
    MatchesRemaining = Symbols * (Copies / 2); // if copies is odd, fewer matches

    List<int> cards = new List<int>();
    for( int i = 0; i < Copies; ++i )
      cards.AddRange( Enumerable.Range( minSymbol, Symbols ) );
    for( int i = 0; i < TotalBombs; ++i )
      cards.Add( -1 );
    cards.Shuffle();

    Grid = new int?[Width, Height];
    Seen = new bool[Width, Height];
    for( int x = 0; x < Width; ++x )
      for( int y = 0; y < Height; ++y )
        Grid[x, y] = cards.Pop( -1 );
  }

  public int SweepNumber( int x, int y )
  {
    int bombs = 0;
    Coord start = new Coord( x, y );
    for( int dir = 0; dir < Coord.DX.Length; ++dir )
    {
      Coord end = start.atAngle( dir );
      if( end.X.Between( 0, Width - 1 ) && 
          end.Y.Between( 0, Height - 1 ) && 
          Grid[end.X, end.Y].HasValue &&
          Grid[end.X, end.Y].Value == -1 )
        ++bombs;
    }
    return bombs;
  }

  public void PickRandomPlayer()
  {
    CurrentPlayer = Players.GetRandom();
  }

  public void InitNextPlayer()
  {
    CurrentPlayer = next( CurrentPlayer );
    InitCurrentPlayer();
  }

  public void InitCurrentPlayer()
  {
    State = GameState.Player_Picking;
    PickNumber = 0;
  }

  public bool Reveal( int x, int y )
  {
    if( State != GameState.Player_Picking )
      return false;
    if( PickNumber == 1 && Picks[0].X == x && Picks[0].Y == y )
      return false;

    int? symbol = Grid[x, y];
    if( !symbol.HasValue )
      return false;

    Picks[PickNumber].Set( x, y );
    Seen[x, y] = true;
    ++PickNumber;

    if( symbol < 0 ) // Bomb
      State = GameState.Flip_Result_Bomb;
    else if( PickNumber == 2 ) // 2nd Card Safe
      State = GameState.Flip_Result_Match;

    return true;
  }

  public void Result()
  {
    if( State == GameState.Flip_Result_Bomb )
    {
      CurrentPlayer.BombsExploded++;
      BombsRemaining--;

      Grid[Picks[PickNumber - 1].X, Picks[PickNumber - 1].Y] = null;
    }
    else
    {
      if( Grid[Picks[0].X, Picks[0].Y] == Grid[Picks[1].X, Picks[1].Y] )
      {
        CurrentPlayer.MatchesFound++;
        MatchesRemaining--;

        for( int i = 0; i < PickNumber; ++i )
          Grid[Picks[i].X, Picks[i].Y] = null;
      }
    }
  }

  public void GameOver()
  {
    State = GameState.GameOver;
    PickNumber = 0;
    var order = Players.OrderByDescending( tp => tp.MatchesFound ).ThenBy( tp => tp.BombsExploded );
    int place = -1;
    Player previous = null;
    order.ForEach( ( p, i ) => {
      if( previous == null || 
          !(previous.MatchesFound == p.MatchesFound && previous.BombsExploded == p.BombsExploded) )
        place = i;
      p.Place = place;
      previous = p;
    } );
  }
}