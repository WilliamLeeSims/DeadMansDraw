using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
  [Header( "Y-Axis Rotation vs Match Camera" )]
  public bool LookAtCamera;

  void OnEnable()
  {
    CameraRenderCallbacks.onPreCull += billboard;
  }

  void OnDisable()
  {
    CameraRenderCallbacks.onPreCull -= billboard;
  }

  void billboard( Camera who )
  {
    if( LookAtCamera )
      transform.LookAt( who.transform );
    else
      transform.forward = who.transform.forward;
  }
}
