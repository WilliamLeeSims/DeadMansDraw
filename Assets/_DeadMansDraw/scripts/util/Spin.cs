using UnityEngine;
using System.Collections;
using BUtility;

public class Spin : MonoBehaviour
{
  public Vector3 Axis;
  public bool WorldAxis;
  public float Delta;
  public float TimeMultiplier;
  public float TimeOffset;

  Transform myTransform;

  void Start()
  {
    myTransform = transform;
  }

  void Update()
  {
    myTransform.Rotate( 
      Axis, 
      360 * Delta * (TimeMultiplier * Time.deltaTime + TimeOffset), 
      WorldAxis ? Space.World : Space.Self );
  }
}
