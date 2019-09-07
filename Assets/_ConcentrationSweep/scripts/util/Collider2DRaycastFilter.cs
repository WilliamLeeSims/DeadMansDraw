using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( RectTransform ), typeof( Collider2D ) )]
public class Collider2DRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
  Collider2D[] myCollider;
  RectTransform myRectTransform;

  void Awake()
  {
    myCollider = GetComponents<Collider2D>();
    myRectTransform = GetComponent<RectTransform>();
  }

  public bool IsRaycastLocationValid( Vector2 screenPos, Camera eventCamera )
  {
    var worldPoint = Vector3.zero;
    var isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
        myRectTransform, screenPos, eventCamera,
        out worldPoint );
    if( isInside )
    {
      isInside = false;
      for( int i = myCollider.Length - 1; i >= 0 && !isInside; --i )
        isInside |= myCollider[i].OverlapPoint( worldPoint );
    }

    return isInside;
  }
}