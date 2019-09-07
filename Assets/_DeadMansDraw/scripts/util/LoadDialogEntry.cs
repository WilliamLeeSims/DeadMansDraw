using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class LoadDialogEntry : MonoBehaviour
{
  public RawImage ScreenCapture;
  public TextMeshProUGUI DateText;
  public Button LoadButton;

  public void AddListener( UnityAction a )
  {
    LoadButton.onClick.AddListener( a );
  }
  public void Render( DateTime gameStart, string imageURL, float delay = 0 )
  {
    string timeString = " @ ";
    if( gameStart.Hour < 10 )
      timeString += "0" + gameStart.Hour;
    else
      timeString += gameStart.Hour;
    timeString += ":";
    if( gameStart.Minute < 10 )
      timeString += "0" + ((gameStart.Minute / 5) * 5);
    else
      timeString += ((gameStart.Minute / 5) * 5);

    DateTime midnightToday = DateTime.Today;
    if( gameStart >= midnightToday )
    {
      var diff = DateTime.Now - gameStart;
      if( diff.Hours >= 2 )
        DateText.text = diff.Hours + " hours ago";
      else if( diff.TotalMinutes >= 5 )
        DateText.text = Mathf.FloorToInt((float)diff.TotalMinutes) + " minutes ago";
      else
        DateText.text = "A few minutes ago";
    }
    else if( gameStart >= midnightToday.AddDays( -1 ) )
      DateText.text = "Yesterday" + timeString;
    else if( gameStart >= midnightToday.AddDays( -6 ) )
      DateText.text = gameStart.DayOfWeek.ToString() + timeString;
    else
      DateText.text = gameStart.ToString( "d MMM yy" ) + timeString;

    StartCoroutine( loadImage( imageURL, delay ) );
  }
  IEnumerator loadImage( string imageURL, float delay )
  {
    if( delay > 0 )
      yield return new WaitForSeconds( delay );

    var www = new WWW( imageURL );
    yield return www;
    if( String.IsNullOrEmpty( www.error ) )
      ScreenCapture.texture = www.texture;
  }
}
