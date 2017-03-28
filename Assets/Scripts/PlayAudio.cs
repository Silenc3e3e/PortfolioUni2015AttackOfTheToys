using UnityEngine;
//using UnityEngine.Audio;
using System.Collections;

public class PlayAudio : MonoBehaviour {

    public AudioSource[] ClipToPlay;

	public void ConnectedAudioPlay(int No)
    {
        ClipToPlay[No].Play();
    }
}
