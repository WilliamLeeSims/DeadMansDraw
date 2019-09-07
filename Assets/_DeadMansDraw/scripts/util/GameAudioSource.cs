using UnityEngine;
using System.Collections;
using BUtility;

[RequireComponent(typeof(AudioSource))]
public class GameAudioSource : MonoBehaviour
{
  public AudioClip[] Clips;
  public float MinPitch = 1;
  public float MaxPitch = 1;

  AudioSource mySource;

  void Start()
  {
    mySource = GetComponent<AudioSource>();
  }

  public void Play()
  {
    if( Clips == null )
      return;

    mySource.pitch = MinPitch + (MaxPitch - MinPitch) * Random.value;
    mySource.clip = Clips[MathUtil.RandomRange( Clips.Length )];
    mySource.Play();
  }
}
