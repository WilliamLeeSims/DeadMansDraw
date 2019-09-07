using BUtility;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniExtensions;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
  public int TablePosition;
  public Button JoinButton;
  public Button LeaveButton;
  public Image PlaceRibbon;

  public TextMeshProUGUI Score;

  public void Render( View v, Game g, Player p )
  {
    if( g.State == GameState.Login )
    {
      Score.gameObject.SetActive( false );
      JoinButton.gameObject.SetActive( !p.Playing );
      LeaveButton.gameObject.SetActive( p.Playing );
      PlaceRibbon.gameObject.SetActive( false );
    }
    else if( g.State == GameState.Player_Picking ||
             g.State == GameState.GameOver )
    {
      Score.gameObject.SetActive( true );
      JoinButton.gameObject.SetActive( false );
      LeaveButton.gameObject.SetActive( false );

      Score.text = string.Format(
        "Matches: <size=125%><b>{0}</b></size>\nBombs: <b>{1}</b>",
        p.MatchesFound, p.BombsExploded );

      PlaceRibbon.gameObject.SetActive( p.Place.HasValue );
      if( p.Place.HasValue )
        PlaceRibbon.sprite = v.PlaceRibbons[p.Place.Value];
    }
  }

  public void Join()
  {
    Control.I.SetPlayerPlaying( TablePosition, true );
  }
  public void Leave()
  {
    Control.I.SetPlayerPlaying( TablePosition, false );
  }
}