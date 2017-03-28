using UnityEngine;

public class MainMenuNavigation : MonoBehaviour {
	public GameObject Main;
	public GameObject Options;
	//public GameObject QuitMenu;
	public GameObject PrefabLoading;
	
	void Start(){
		toMenu ();
		Time.timeScale=1f;
	}
	
	public void toMenu(){
		Main.SetActive (true);
		Options.SetActive (false);
		//QuitMenu.SetActive (false);
	}
	public void toOptions(){
		Main.SetActive (false);
		Options.SetActive (true);
		//QuitMenu.SetActive (false);
	}

	public void toQuitMenu(){
		Main.SetActive (false);
		Options.SetActive (false);
		//QuitMenu.SetActive (true);
	}

	public void Quit(){
		Time.timeScale=1f;
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		//Application.LoadLevel (MainMenuName);
	}
	public void GoToLevelSelection(){
		GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading,new Vector3(0,0,0),Quaternion.identity);
		LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene> ();
		if (LNSScript != null)
			LNSScript.LoadLevel ("Levels");
		else {
			Debug.LogWarning("Loading bar not found. Loading level Manually");
			Application.LoadLevel("Levels");
		}
	}

    public void ResumeProgress()
    {
       string LevelToLoad= SaveLoad.ReturnLatestUnlockedScene();

        GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading, new Vector3(0, 0, 0), Quaternion.identity);
        LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene>();
        if (LNSScript != null)
            LNSScript.LoadLevel(LevelToLoad);
        else
        {
            Debug.LogWarning("Loading bar not found. Loading level Manually");
            Application.LoadLevel(LevelToLoad);
        }
    }
}
