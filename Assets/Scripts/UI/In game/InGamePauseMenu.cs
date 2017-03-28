using UnityEngine;
using UnityEngine.UI;
/*using System.Collections;*/

public class InGamePauseMenu : MonoBehaviour {
	
	public GameObject Main;
	public GameObject Options;
	public GameObject Load;
	public GameObject DeathMenu;
    public GameObject QuitMenu;
	public GameObject[] HUDs;
	public string MainMenuName;
	public GameObject PrefabLoading;

	void Start(){
        if (GameObject.FindGameObjectWithTag("CanvasHUD") != null)
        {
            Button PauseButton = GameObject.Find("ButtonPause").GetComponent<Button>();
            PauseButton.onClick.AddListener(()=>toMenu());
        }
        else
        {
            Debug.LogError("MARJOR ERROR: Canvas Pause-Menu required. Ref: InGamePauseMenu");
        }


		bool NotFoundPlayerHUD=true;
		foreach (GameObject current in HUDs){
			if(current!=null){
				if(current.tag=="CanvasHUD"){
					NotFoundPlayerHUD=false;
					break;
				}
			}
		}
		if(NotFoundPlayerHUD){
			int count=0;
			foreach (GameObject current in HUDs){
				if(current==null){
					HUDs[count]=GameObject.FindGameObjectWithTag("CanvasHUD");
					break;
				}
				count++;
			}
		}
		CloseAll ();
	}

	public void toMenu(){
		Main.SetActive (true);
		Options.SetActive (false);
		Load.SetActive (false);
        QuitMenu.SetActive(false);
        foreach (GameObject current in HUDs){
			if(current!=null)
				current.SetActive (false);
		}
		Time.timeScale=0f;
	}
	public void toOptions(){
		Main.SetActive (false);
		Options.SetActive (true);
		Load.SetActive (false);
        QuitMenu.SetActive(false);
        foreach (GameObject current in HUDs){
			if(current!=null)
				current.SetActive (false);
		}
		Time.timeScale=0f;
	}
	public void toLoadGame(){
		Main.SetActive (false);
		Options.SetActive (false);
		Load.SetActive (true);
        QuitMenu.SetActive(false);
        foreach (GameObject current in HUDs){
			if(current!=null)
				current.SetActive (false);
		}
		Time.timeScale=0f;
	}
	public void CloseAll(){
		Main.SetActive (false);
		Options.SetActive (false);
		Load.SetActive (false);
		DeathMenu.SetActive (false);
        QuitMenu.SetActive(false);
        foreach (GameObject current in HUDs){
			if(current!=null)
				current.SetActive (true);
		}
		Time.timeScale=1f;
	}
    public void GoToQuitPanel()
    {
        Main.SetActive(false);
        Options.SetActive(false);
        Load.SetActive(false);
        QuitMenu.SetActive(true);
        foreach (GameObject current in HUDs)
        {
            if (current != null)
                current.SetActive(false);
        }
    }
	public void GoToMainMenu(){
        Time.timeScale = 1f;

        GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading, new Vector3(0, 0, 0), Quaternion.identity);
        LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene>();
        if (LNSScript != null)
            LNSScript.LoadLevel("MainMenu");
        else
        {
            Debug.LogWarning("Loading bar not found. Loading level Manually");
            Application.LoadLevel("MainMenu");
        }
    }
	public void GoToLevelSelection(){
		Time.timeScale=1f;

		GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading,new Vector3(0,0,0),Quaternion.identity);
		LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene> ();
		if (LNSScript != null)
			LNSScript.LoadLevel ("Levels");
		else {
			Debug.LogWarning("Loading bar not found. Loading level Manually");
			Application.LoadLevel("Levels");
		}
	}
	public void DeathMenuActive(){
		Time.timeScale=1f;
		CloseAll();
		DeathMenu.SetActive(true);
	}
	public void ReloadLevel(){
		Time.timeScale=1f;
		GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading,new Vector3(0,0,0),Quaternion.identity);
		LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene> ();
		if (LNSScript != null)
			LNSScript.LoadLevel (Application.loadedLevelName);
		else {
			StartCoroutine ("LoadALevel",Application.loadedLevelName);
		}
	}
}
