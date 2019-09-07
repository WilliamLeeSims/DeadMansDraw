using UnityEngine;
using System.Collections;
using BUtility;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardFlip : MonoBehaviour
{
  public Sprite[] Cards;
  public float Delay;
  public bool Repeat;

  Image myImage;
  int myIter = 0;
  float myCountdown = 0;

  void Start()
  {
    myImage = GetComponent<Image>();
  }

  void Update()
  {
    if( Cards == null || Cards.Length == 0 || Delay <= 0.001f )
      return;

    myCountdown -= Time.deltaTime;

    if( myCountdown <= 0 )
    {
      ++myIter;
      if( Repeat )
        myIter = myIter % Cards.Length;
      else if( myIter >= Cards.Length )
        myIter = Cards.Length - 1;
      myCountdown += Delay;
      myImage.sprite = Cards[myIter];
    }
  }
}
