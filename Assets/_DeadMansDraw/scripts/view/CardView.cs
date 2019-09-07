using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using System;
using BUtility;

public class CardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public bool Enabled;
  public RectTransform Target;
  public RectTransform DisplayLayer;

  [HideInInspector] public Action InteractCb;

  protected bool QTapping = false;
  protected bool QDragging = false;

  protected RectTransform myRT;

  private int myNumTouches;
  private Vector2 myOffset;

  virtual protected void Start()
  {
    myRT = transform as RectTransform;
    myRT.SetParent( DisplayLayer, true );
    myRT.localScale = Vector3.one;
  }

  public void FullReset()
  {
    Enabled = false;
    Target = null;
    myNumTouches = 0;
    QTapping = false;
    QDragging = false;
  }

  public void SetTarget( RectTransform target, bool qEnabled = true )
  {
    Enabled = qEnabled;
    if( target == null )
    {
      FullReset();
      gameObject.SetActive( false );
    }
    else
    {
      gameObject.SetActive( true );
      Target = target;
    }
  }

  Vector3 myVelocity = Vector3.zero;
  float mySpringReset = 0.002f;
  float myVelocityReset = 0.03f;
  float myAngleVelocity = 0f;
  void LateUpdate()
  {
    if( !QTapping && !QDragging && Target != null )
    {
      var diff = Target.position - myRT.position;
      var dist = diff.magnitude;
      if( dist < mySpringReset && myVelocity.magnitude < myVelocityReset )
      {
        myRT.position = Target.position;
        myVelocity = Vector3.zero;
      }
      else
      {
        Vector3 force = Vector3.zero;
        if( dist > 25 ) // ERROR!
        {
          myRT.position = Target.position;
          myVelocity = Vector3.zero;
        }
        else if( dist >= mySpringReset )
        {
          var unit = diff / dist;
          force = View.I.SpringForce * Mathf.Pow( dist, 1.5f ) * unit;
        }
        if( myVelocity.magnitude >= 5 ) // ERROR-ish!
        {
          myVelocity = myVelocity.normalized * 1f;
        }
        else if( myVelocity.sqrMagnitude > 0 )
        {
          force += -myVelocity.sqrMagnitude * View.I.AirResistance * myVelocity.normalized;
        }
        myVelocity += force * Time.deltaTime;
        myRT.position += myVelocity * Time.deltaTime;
      }

      float ang = Mathf.SmoothDampAngle( myRT.eulerAngles.z, Target.eulerAngles.z, ref myAngleVelocity, 0.5f );
      myRT.rotation = Quaternion.Euler( 0, 0, ang );
    }
  }

  public void OnPointerDown( PointerEventData data )
  {
    if( !Enabled )
      return;

    ++myNumTouches;
    if( myNumTouches == 1 )
    {
      QTapping = true;
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
        DisplayLayer, data.position, data.pressEventCamera, out localPointerPosition
    ) )
    {
      myRT.localPosition = localPointerPosition - myOffset;
    }
  }

  public virtual void OnEndDrag( PointerEventData data )
  {
    if( !Enabled )
      return;

    if( QDragging )
      Callback();
  }

  public void OnPointerUp( PointerEventData eventData )
  {
    if( !Enabled )
      return;

    if( myNumTouches > 0 )
      --myNumTouches;

    if( QTapping )
      Callback();
  }

  void Callback()
  {
    myNumTouches = 0;
    QTapping = false;
    QDragging = false;

    if( InteractCb != null && Enabled )
      InteractCb();
  }
}