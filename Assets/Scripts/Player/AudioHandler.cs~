using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioHandler : MonoBehaviour {

	public AudioMixer MasterMixer;
	public Slider MasterVolume;
	public Slider MusicVolume;
	public Slider SFXVolume;
	//public AudioSource Panting;
	public AudioSource OneLines;
	public AudioSource OnesLinesTrack2;

	public AudioClip[] SwordAttacks;
	private int SwordAttacks1PrevNum;
	private int SwordAttacks2PrevNum;

	private float PantingVol;
	private float MixerPantingVol=0;
	public float PositivePantingTransitionMuffle=.3f;
	public float NegativePantingTransitionMuffle=2.5f;

	public void PlayAttack(bool OnSecondTrack = false){
		//MasterMixer.;
		if(OnSecondTrack){
			int random = Random.Range(0,SwordAttacks.Length-1);
			while(random==SwordAttacks2PrevNum)
				random = Random.Range(0,SwordAttacks.Length-1);
			SwordAttacks2PrevNum=random;
			OnesLinesTrack2.clip=SwordAttacks[random];
			OnesLinesTrack2.Play();
		}else{
			int random = Random.Range(0,SwordAttacks.Length-1);
			while(random==SwordAttacks1PrevNum)
				random = Random.Range(0,SwordAttacks.Length-1);
			SwordAttacks1PrevNum=random;
			OneLines.clip=SwordAttacks[random];
			OneLines.Play();
		}
	}


	public GameObject PrefabMenuCanvasOBJ;
	void Start(){
		MasterMixer.GetFloat ("PantingVol",out MixerPantingVol);
		PantingVol = MixerPantingVol;

		//making sure there are no null references
		if(MasterVolume==null||MusicVolume==null||SFXVolume==null){
			GameObject CPMOBJ=GameObject.FindGameObjectWithTag("CanvasPauseMenu");

			if(PrefabMenuCanvasOBJ!=null && CPMOBJ==null){
				GameObject Spawn =(GameObject)Instantiate(PrefabMenuCanvasOBJ,new Vector3(0,0,0),Quaternion.identity);
				CPMOBJ=GameObject.FindGameObjectWithTag("CanvasPauseMenu");
				if(CPMOBJ==null)
					Debug.LogError("FATAL ERROR: Pause menu is missing the tag CanvasPauseMenu");
			}

			if(CPMOBJ!=null){
				InGamePauseMenu CPMIGPMScript= CPMOBJ.GetComponent<InGamePauseMenu>();
				CPMIGPMScript.Options.SetActive(true);
				if(MasterVolume==null){
					MasterVolume= GameObject.Find("PanelMasterVolume").GetComponentInChildren<Slider>();
				}
				if(MusicVolume==null){
					MusicVolume= GameObject.Find("PanelMusicVolume").GetComponentInChildren<Slider>();
				}
				if(SFXVolume==null){
					SFXVolume= GameObject.Find("PanelSFXVolume").GetComponentInChildren<Slider>();
				}
				CPMIGPMScript.Options.SetActive(false);
			}
			else{
				Debug.LogError("Could not find, or create Pause menu!");
			}
		}

		//getting saved volume levels etc

		//Master
		if (PlayerPrefs.HasKey ("MasterVolume")) {
			MasterMixer.SetFloat ("MasterVol", PlayerPrefs.GetFloat ("MasterVolume",0f));
			MasterVolume.value=PlayerPrefs.GetFloat ("MasterVolume",0f);
		}
		else {
			MasterVolume.value = 0;
			PlayerPrefs.SetFloat("MasterVolume",0);
			MasterMixer.SetFloat ("MasterVol",0);
		}
		//Music
		if (PlayerPrefs.HasKey ("MusicVolume")) {
			MasterMixer.SetFloat ("MusicVol", PlayerPrefs.GetFloat ("MusicVolume",0f));
			MusicVolume.value = PlayerPrefs.GetFloat ("MusicVolume",0f);
		}
		else {
			MusicVolume.value = 0;
			PlayerPrefs.SetFloat("MusicVolume",0);
			MasterMixer.SetFloat ("MusicVol",0);
		}
		//SFX
		if (PlayerPrefs.HasKey ("SFXVolume")) {
			MasterMixer.SetFloat ("SFXVol", PlayerPrefs.GetFloat ("SFXVolume",0f));
			SFXVolume.value = PlayerPrefs.GetFloat ("SFXVolume",0f);
		}
		else {
			SFXVolume.value = 0;
			PlayerPrefs.SetFloat("SFXVolume",0);
			MasterMixer.SetFloat ("SFXVol",0);
		}
		MasterVolume.onValueChanged.AddListener (MasterVolumeAdjust);
		MusicVolume.onValueChanged.AddListener (MusicVolumeAdjust);
		SFXVolume.onValueChanged.AddListener (SFXVolumeAdjust);

		PrevTime = Time.time;
		//test, to be removed
		UpdatePanting (0f);
	}
	public void MasterVolumeAdjust(float Volume){
		MasterMixer.SetFloat ("MasterVol", Volume);
		PlayerPrefs.SetFloat("MasterVolume",Volume);
		//Debug.Log("Adjusted volume to "+Volume);
	}
	public void MusicVolumeAdjust(float Volume){
		MasterMixer.SetFloat ("MusicVol", Volume);
		PlayerPrefs.SetFloat("MusicVolume",Volume);
	}
	public void SFXVolumeAdjust(float Volume){
		MasterMixer.SetFloat ("SFXVol", Volume);
		PlayerPrefs.SetFloat("SFXVolume",Volume);
	}

	public void UpdatePanting(float VolOneToHundred){
		PantingVol = VolOneToHundred-80f;
		//Debug.Log ("Vol passed through "+PantingVol);
	}
	public void UpdatePanting(float VolOneToHundred, float PositiveTransitionMuffle, float NegaiveTransitionMuffle){
		PantingVol = VolOneToHundred-80f;
		PositivePantingTransitionMuffle = PositiveTransitionMuffle;
		NegativePantingTransitionMuffle = NegaiveTransitionMuffle;
	}

	private float PrevTime;
	void Update(){
		float TimeDifference = Time.time-PrevTime;
		if (PantingVol != MixerPantingVol) {
			if(PantingVol>MixerPantingVol)
				MixerPantingVol=Mathf.Lerp(MixerPantingVol,PantingVol,TimeDifference/PositivePantingTransitionMuffle);
			else
				MixerPantingVol=Mathf.Lerp(MixerPantingVol,PantingVol,TimeDifference/NegativePantingTransitionMuffle);
			MasterMixer.SetFloat ("PantingVol", MixerPantingVol);
		}

		PrevTime = Time.time;


		//volume levels
		MasterMixer.SetFloat ("MasterVol",MasterVolume.value);
		MasterMixer.SetFloat ("MusicVol",MusicVolume.value);
		MasterMixer.SetFloat ("SFXVol",SFXVolume.value);
	}
}
