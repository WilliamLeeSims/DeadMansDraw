using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class VersionInfo : MonoBehaviour
{
  public string versionNumber;
  public string buildName;

  void Start()
  {
    gameObject.GetComponent<TextMeshProUGUI>().text = "v." + buildName;
  }
#if UNITY_EDITOR
  public void Awake()
  {
    if( Application.isEditor && !Application.isPlaying )
    {
      buildName = versionNumber + " " + System.DateTime.Now.ToString( "yy.MM.dd" );
    }
  }
#endif
}