using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Camera ) )]
public class CameraRenderCallbacks : MonoBehaviour
{
  public delegate void PreCullEvent( Camera me );
  public static PreCullEvent onPreCull;

  Camera myCamera;

  void Start()
  {
    myCamera = GetComponent<Camera>();
    Camera.onPreCull += XXXOnPreCull;
  }

  void XXXOnPreCull( Camera cam )
  {
    if( cam == myCamera )
      if( onPreCull != null )
        onPreCull( myCamera );
  }
}
