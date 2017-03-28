using UnityEngine;
using System.Collections;

public class OnLevelEndData : MonoBehaviour {

	private int ThisLevelNumberPlusOne=0;
	public GameObject PrefabLoading;
	private string nextLevelToLoad="";

	void Start(){
		int count = 0;
		string LevelName = Application.loadedLevelName;
		string[] AllLevels=SaveLoad.ReturnLevelNames();
		foreach(string current in AllLevels){
			if(current==LevelName){
				ThisLevelNumberPlusOne = count+1;
				break;
			}
			else{
				count++;
			}
		}

        //prepare for the end of the level
        if (AllLevels.Length > ThisLevelNumberPlusOne)
            nextLevelToLoad = AllLevels[ThisLevelNumberPlusOne + 1];
        else
            nextLevelToLoad = SaveLoad.ReturnLatestUnlockedScene();
		if(nextLevelToLoad==null){
			if(!Application.CanStreamedLevelBeLoaded(nextLevelToLoad)){
				nextLevelToLoad="Levels";
				Debug.LogWarning("Error: could not get name of next level");
			}
		}
	}


	public void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") {
            //if this is a newer unlocked level. then unlock the next level
			Debug.Log("Save load data SLD: "+SaveLoad.LoadData ()+" TLN "+ThisLevelNumberPlusOne);
			if (SaveLoad.LoadData () <= ThisLevelNumberPlusOne) {
				SaveLoad.SaveData (ThisLevelNumberPlusOne);
			}

			GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading,new Vector3(0,0,0),Quaternion.identity);
			LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene> ();
			if (LNSScript != null)
				LNSScript.LoadLevel ("Levels");
			else {
				Debug.LogWarning("Loading bar not found. Loading level Manually");
				Application.LoadLevel("Levels");
			}
		}
	}
}