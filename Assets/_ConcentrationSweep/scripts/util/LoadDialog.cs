using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UniExtensions;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;

public class LoadDialog : MonoBehaviour
{
  public GameObject SaveGameList;
  public GameObject SavedGameEntryPrefab;

  public void Populate()
  {
    SaveGameList.DestroyChildren();

    int delay = 0;
    foreach( string dirName in Directory.GetDirectories( Application.persistentDataPath + "/savedGames" ).Reverse() )
    {
      GameObject saveEntry = Instantiate( SavedGameEntryPrefab, SaveGameList.transform );

      var entry = saveEntry.GetComponent<LoadDialogEntry>();

      DateTime gameStart = DateTime.MinValue;
      string strippedDirName;
      try
      {
        strippedDirName = dirName.Split( '\\' ).Last();
        string[] dateParts = strippedDirName.Split( '_' ); // yyyy_MM_dd_HH_mm_ss
        int year = int.Parse( dateParts[0] );
        int month = int.Parse( dateParts[1] );
        int day = int.Parse( dateParts[2] );
        int hour = int.Parse( dateParts[3] );
        int minute = int.Parse( dateParts[4] );
        int second = int.Parse( dateParts[5] );
        gameStart = new DateTime( year, month, day, hour, minute, second );
      }
      catch( Exception )
      {
        Debug.Log( "Skipping directory: " + dirName );
        continue;
      }

      string url = "file:///" + dirName.Replace( "\\", "/" ).Replace( " ", "%20" ) + "/Snapshot.png";

      entry.AddListener( () => loadGame( strippedDirName ) );
      entry.Render( gameStart, url, 0.5f * delay++ );
    }
  }

  public void loadGame( string name )
  {
    gameObject.SetActive( false );
    //GameControl.I.ContinueGame( name );
  }
}