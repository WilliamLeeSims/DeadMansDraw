using BUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
  public Vector3 Delta;
  public float TimeMultiplier;
  public float TimeOffset;

  float myElapsed;
  Transform myTransform;
  Transform myTargetTransform;
  
  void Start()
  {
    myTargetTransform = transform;
    int level = myTargetTransform.GetSiblingIndex();

    GameObject go = new GameObject( "Bob Mount for " + name );
    myTransform = go.transform;
    myTransform.position = myTargetTransform.position;
    go.transform.SetParent( myTargetTransform.parent, true );
    go.transform.SetSiblingIndex( level );

    myTargetTransform.SetParent( myTransform );

    myElapsed = TimeOffset;
    Update();
  }

  void Update()
  {
    myElapsed += Time.deltaTime * TimeMultiplier;
    myTargetTransform.localPosition = 
      Delta *
      MathUtil.Hacovercosine( myElapsed * MathUtil.TAU );
  }
}