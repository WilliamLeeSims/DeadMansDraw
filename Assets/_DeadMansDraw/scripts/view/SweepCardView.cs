using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SweepCardView : MonoBehaviour
{ 
  public RectTransform CardRectTransform;
  public Image SymbolSprite;
  public TextMeshProUGUI BombCountLabel;
  public RectTransform BombCountRT;
  public GameObject BombAnimation;

  int X;
  int Y;
  int Symbol;
  bool CountShown;
  
  float myGoalFlip = -1;
  float myCurrFlip = -1;
  float myFlipVel = 0;

  void LateUpdate()
  {
    myCurrFlip = Mathf.SmoothDamp( myCurrFlip, myGoalFlip, ref myFlipVel, 0.3f );

    float xScale = myCurrFlip * myCurrFlip;
    CardRectTransform.localScale = new Vector3( xScale, 1, 1 );

    float textScale = 0.625f - 0.375f * myCurrFlip;
    BombCountRT.localScale = new Vector3( textScale, textScale, 1 );

    if( myCurrFlip < 0 )
    {
      SymbolSprite.gameObject.SetActive( false );
      BombAnimation.gameObject.SetActive( false );
      BombCountLabel.gameObject.SetActive( CountShown );
    }
    else
    {
      SymbolSprite.gameObject.SetActive( Symbol >= 0 );
      BombAnimation.gameObject.SetActive( Symbol < 0 );
      BombCountLabel.gameObject.SetActive( CountShown );
    }
  }

  public void AttemptFlip()
  {
    Control.I.Reveal( X, Y );
  }

  public void Set( int x, int y, int? symbol, int bombCount, bool seen )
  {
    X = x;
    Y = y;
    if( symbol.HasValue )
      Symbol = symbol.Value;
    CountShown = seen;

    if( Symbol >= 10 )
      SymbolSprite.sprite = View.I.Pets[Symbol-10];
    else if( Symbol >= 0 )
      SymbolSprite.sprite = View.I.Desserts[Symbol];
    BombCountLabel.text = bombCount == 6 ? "6." : bombCount.ToString();
    BombCountLabel.color = View.I.BombCountColors[bombCount];
  }

  public void SetFlip( bool qFlipped )
  {
    myGoalFlip = qFlipped ? 1 : -1;
  }
}
