using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelPrefab : MonoBehaviour {

	public Button GoToLevel;
	public Image LockedImage;
	public Text DisplayLevelName;
	public Text DisplayLevelNumber;
	private string LevelToGoTo;
	private bool Unlocked=false;
	public GameObject PrefabLoading;

	void Awake(){
		GoToLevel.enabled=false;
	}

	public void SceneDetails(string SceneName, int SceneNumber){
		string DisplayName="";
		foreach(char current in SceneName){
			if (char.IsUpper(current) && DisplayName!=""){
				DisplayName+=" "+current;
			}else{
				DisplayName+=current;
			}
		}
		DisplayLevelName.text=DisplayName;
		DisplayLevelNumber.text=SceneNumber.ToString();
		LevelToGoTo=SceneName;
	}

	public void unlock(){
		GoToLevel.enabled=true;
		LockedImage.enabled=false;
		Unlocked=true;
	}

	public void ButtonClick(){
		if (Unlocked) {
			string LevelToLoadName="";
			char CharSpace= ' ';
			foreach(char current in LevelToGoTo){
				if (current!=CharSpace){
					LevelToLoadName+=current;
				}
			}
			//Debug.Log("Temporary note. Delete me| LevelToLoadName "+LevelToLoadName);
			LoadNextScene LNSScript=null;
			if(PrefabLoading!=null){
				GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading,new Vector3(0,0,0),Quaternion.identity);
				LNSScript = spawnedPrefab.GetComponent<LoadNextScene> ();
			}
			if (LNSScript != null)
				LNSScript.LoadLevel (LevelToLoadName);
			else {
				Debug.LogWarning("Loading bar not found. Loading level Manually");
				if(Application.CanStreamedLevelBeLoaded(LevelToLoadName)){
					Application.LoadLevel(LevelToLoadName);
				}else {
					Debug.LogError ("WARNING. Scene name incorrent, OR scene missing!");
				}
			}
		}
	}

}
