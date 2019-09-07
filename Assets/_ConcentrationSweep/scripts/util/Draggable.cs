using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using System;
using BUtility;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public bool Enabled = true;

  public bool ConstrainExtentToParentExtent;
  public bool Rotatable;
  public bool Flippable;

  public Action InteractCb;

  protected bool QTapping = false;
  protected bool QFlipping = false;
  protected bool QDragging = false;

  protected RectTransform myParentRT;
  protected RectTransform myRT;
  protected Vector3 myOrigPos;
  protected Quaternion myOrigAng;
  protected Vector3 myOrigScale;

  private Vector2 myOffset;
  private int myNumTouches = 0;
  private Vector2[] myTouches = new Vector2[2];

  void Awake()
  {
    myParentRT = transform.parent as RectTransform;
    myRT = transform as RectTransform;

    myOrigPos = myRT.localPosition;
    myOrigAng = myRT.localRotation;
    myOrigScale = myRT.localScale;
  }

  public void Reset()
  {
    if( !Enabled )
      return;

    myRT.localPosition = myOrigPos;
    myRT.localRotation = myOrigAng;
    myRT.localScale = myOrigScale;
  }

  public void OnPointerDown( PointerEventData data )
  {
    if( !Enabled )
      return;

    ++myNumTouches;
    if( myNumTouches == 1 )
    {
      QTapping = true;
      myTouches[0] = data.position;
    }
    else if( myNumTouches == 2 )
    {
      QTapping = false;
      QDragging = false;
      QFlipping = true;
      myTouches[1] = data.position;
      Flip( -1 * Vector2.right + Vector2.up );
    }
  }

  public virtual void OnBeginDrag( PointerEventData data )
  {
    if( !Enabled )
      return;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      myRT, data.position, data.pressEventCamera, out myOffset );

    QDragging = true;
    QTapping = false;
    QFlipping = false;

    myRT.SetAsLastSibling();

    OnDrag( data );
  }

  public virtual void OnDrag( PointerEventData data )
  {
    if( !Enabled )
      return;

    if( !QDragging )
      return;

    Vector2 localPointerPosition;
    if( RectTransformUtility.ScreenPointToLocalPointInRectangle(
        myParentRT, data.position, data.pressEventCamera, out localPointerPosition
    ) )
    {
      myRT.localPosition = localPointerPosition - myOffset;
    }
  }

  public virtual void OnEndDrag( PointerEventData data )
  {
    if( !Enabled )
      return;

    FixBounding();
  }

  public void OnPointerUp( PointerEventData eventData )
  {
    if( !Enabled )
      return;

    if( myNumTouches > 0 )
      --myNumTouches;

    if( QTapping )
      Rotate();
  }

  void FixBounding()
  {
    if( !ConstrainExtentToParentExtent )
      return;

    myNumTouches = 0;
    QTapping = false;
    QFlipping = false;
    QDragging = false;

    bool qFixPosition = false;
    Vector3 fixedPosition = myRT.localPosition;
    if( myRT.localPosition.x < -256 )
    {
      qFixPosition = true;
      fixedPosition.x = -256;
    }
    else if( myRT.localPosition.x > 256 )
    {
      qFixPosition = true;
      fixedPosition.x = 256;
    }
    if( myRT.localPosition.y < -256 )
    {
      qFixPosition = true;
      fixedPosition.y = -256;
    }
    else if( myRT.localPosition.y > 256 )
    {
      qFixPosition = true;
      fixedPosition.y = 256;
    }
    if( qFixPosition )
      myRT.DOLocalMove( fixedPosition, 0.25f );

    if( InteractCb != null )
      InteractCb();
  }
  public void Rotate()
  {
    if( !Rotatable )
      return;

    myRT.DOComplete();
    myRT.DOLocalRotate( -90 * Vector3.forward, 0.25f ).SetRelative();

    FixBounding();
  }
  public void Flip( Vector3 axis )
  {
    if( !Flippable )
      return;

    myRT.DOComplete();
    var ls = myRT.localScale;
    axis = axis.Rotate( -myRT.localRotation.eulerAngles.z * 2 );
    ls.x *= axis.x;
    ls.y *= axis.y;
    
    myRT.DOScale( ls, 0.25f );

    FixBounding();
  }
}