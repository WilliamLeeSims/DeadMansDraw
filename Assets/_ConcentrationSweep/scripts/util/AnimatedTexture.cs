using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AnimatedTexture : MonoBehaviour
{
	public Material MaterialToAnimate;
  [Header( "Scale" ) ]
  public Vector2 BaseScale;
  public Vector2 ScaleVariance;
  public Vector2 ScaleSpeed;
  public Vector2 ScaleTimeOffset;
  [Header( "Offset" )]
  public Vector2 BaseOffset;
  public Vector2 OffsetVariance;
  public Vector2 OffsetSpeed;
  public Vector2 OffsetTimeOffset;

  void Update ()
	{
    float scaleX = BaseScale.x + ScaleVariance.x * Mathf.Sin( Time.time * ScaleSpeed.x + ScaleTimeOffset.x );
    float scaleY = BaseScale.y + ScaleVariance.y * Mathf.Sin( Time.time * ScaleSpeed.y + ScaleTimeOffset.y );

    float offsetX = BaseOffset.x + OffsetVariance.x * Mathf.Sin( Time.time * OffsetSpeed.x + OffsetTimeOffset.x );
    float offsetY = BaseOffset.y + OffsetVariance.y * Mathf.Sin( Time.time * OffsetSpeed.y + OffsetTimeOffset.y );

    MaterialToAnimate.mainTextureScale = Vector2.right * scaleX + Vector2.up * scaleY;
    MaterialToAnimate.mainTextureOffset = Vector2.right * offsetX + Vector2.up * offsetY;
	}
}