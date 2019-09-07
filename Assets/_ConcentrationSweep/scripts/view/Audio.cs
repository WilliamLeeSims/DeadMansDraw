using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour
{
  static Audio I;
  void Start() { I = this; }

  public static void PlayBadMatch() { I.BadMatch.Play(); }
  public static void PlayClick() { I.Click.Play(); }
  public static void PlayExplosion() { I.Explosion.Play(); }
  public static void PlayGameOver() { I.GameOver.Play(); }
  public static void PlayMatch() { I.Match.Play(); }
  public static void PlayTsss() { I.Tsss.Play(); }

  public GameAudioSource BadMatch;
  public GameAudioSource Click;
  public GameAudioSource Explosion;
  public GameAudioSource GameOver;
  public GameAudioSource Match;
  public GameAudioSource Tsss;
}