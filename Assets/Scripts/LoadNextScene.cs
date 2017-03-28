using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadNextScene : MonoBehaviour {

	public Slider Loading;
	public bool MainMenuOnStart=false;
	public Text dotdotdotBox;

	void Awake(){
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width,Screen.height);
	}

	void Start () {
		if(MainMenuOnStart)
			LoadLevel ("MainMenu");

		//for the dot dot dot box
		PreviousTime=Time.time;
	}
	private int called=0;
	public void LoadLevel(string LevelName){
		if (Application.CanStreamedLevelBeLoaded (LevelName))
			StartCoroutine ("LoadALevel", LevelName);
		else {
			Debug.LogError ("WARNING. Scene name "+LevelName+" incorrent, OR scene missing!");
		}

	}

	private AsyncOperation async = null; // When assigned, load is in progress.
	private IEnumerator LoadALevel(string levelName) {
		//float currentT=Time.time;
		async = Application.LoadLevelAsync(levelName);
		//Debug.Log("Time to load "+(Time.time-currentT).ToString());
		yield return async;
	}

	private float PreviousTime;
	private int NoDots=1;
	void Update(){
		if (async != null) {
			Loading.value=async.progress;
		}
		if(Time.time-PreviousTime>=.5f){
			NoDots++;
			if(NoDots>=4)
				NoDots=1;
			string TextToDotDisplay="";
			int i=0;
			for(i=0;i<NoDots;i++){
				TextToDotDisplay+=".";
			}
			dotdotdotBox.text=TextToDotDisplay;
			PreviousTime=Time.time;
		}
	}
}
