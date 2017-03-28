﻿using UnityEngine;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour {

	public static string[] LevelNames = {"One","NewWeapons", "NewDoors", "HeadsUp","Spears","Lure","DistressCall","ComeToAid","FinalApproach"};
	public string[] PublicLevelNames{
		get
		{
			return LevelNames;
		}
	}
	public static string BossLevelName = "TheFinalBattle";
	public GameObject PanelForTheLevels;
	public GameObject PanelForLevelsMask;
	private ScrollRect PanelScrollRect;
	public GameObject ALevelPrefab;
	public GameObject PrefabLoading;

	[Range (10f,500f)]
	public float preferableWidth;
	[Range (10f,500f)]
	public float preferableHeight;
	private float ParentHeight;
	private RectTransform PFTLsRect;

	void Start(){
        //SaveData(9999);
		if (PanelForLevelsMask != null) {
			PanelScrollRect = PanelForLevelsMask.gameObject.GetComponent<ScrollRect> ();
		}
		PFTLsRect=PanelForTheLevels.GetComponent<RectTransform>();
		ParentHeight=PanelForLevelsMask.GetComponent<RectTransform>().rect.height;
		DisplayLevels (LoadData());

		//PanelForTheLevels.GetComponent<LayoutGroup>().;
	}

	public void GoToMainMenuScene(){
		GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading,new Vector3(0,0,0),Quaternion.identity);
		LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene> ();
		if (LNSScript != null)
			LNSScript.LoadLevel ("MainMenu");
		else {
			Debug.LogWarning("Loading bar not found. Loading level Manually");
			Application.LoadLevel("MainMenu");
		}
	}

    public static string ReturnLatestUnlockedScene()
    {
        int levelNumToLoad = LoadData();
        if (LevelNames.Length <= levelNumToLoad)
            return BossLevelName;
        else
            return LevelNames[levelNumToLoad];
    }

	public void ResetUnlockedLevels(){
		SaveData (0);
	}

	public static void SaveData(int Num){
		string data = Num.ToString ();
		byte[] byteData = System.Text.Encoding.ASCII.GetBytes (data);
		string convertedData = System.Convert.ToBase64String (byteData);
//		Debug.Log (convertedData);

		PlayerPrefs.SetString("ReachedLevel", convertedData);
	}
	public static string[] ReturnLevelNames(){
		return LevelNames;//new string[]{"Fred","Bob","Alex","George","Luke","Dean","Max","Matt","Tahnee"};
	}
	public int LoadDataForGUIInspector(){
		return LoadData ();
	}
	public static int LoadData(){
		//grab the byte data from player prefs. default value is 1. but that 1 needs conversion to encrypted string first
		/*byte[] data =System.Convert.FromBase64String (
			PlayerPrefs.GetString (
				"ReachedLevel",
				//default value
				System.Convert.ToBase64String (
					System.Text.Encoding.ASCII.GetBytes (1.ToString ())
				)
			)
		);*/
		string defaultVal = System.Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes ("0"));
		string storedData = PlayerPrefs.GetString ("ReachedLevel", defaultVal);

//		Debug.Log ("defaultVal: " + defaultVal);
//		Debug.Log ("storedData: " + storedData);

		byte[] data = System.Convert.FromBase64String (storedData);


		string stringdata=System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
//		Debug.Log ("stringdata: " + stringdata);
		/*foreach (byte current in data) {
			stringdata+=System.Convert.ToString(current);
			
			Debug.Log ("Pulled data "+System.Convert.ToString(current));
		}*/
		int IntData;
		if(!int.TryParse(stringdata,out IntData)){
			//Data has either become corrupted. or player tried to edit the data
			byte[] ByteData = System.Text.Encoding.ASCII.GetBytes ("0");
			string StringToSave=System.Convert.ToBase64String (ByteData);
			PlayerPrefs.SetString("ReachedLevel", StringToSave);
			return 1;
		}
		return IntData;
	}

	void DisplayLevels(int MaxLevelUnlock){
		int UpTo= 0;//1 instead of 0 due to guarenteed boss level


		foreach(string current in LevelNames){
			//resizing parent
			if(!IsOdd(UpTo)){
				Vector2 rectSize = PFTLsRect.sizeDelta;
				rectSize = new Vector2 (rectSize.x+300, rectSize.y);
				PFTLsRect.sizeDelta = rectSize;
			}

			GameObject childObject = Instantiate(ALevelPrefab) as GameObject;
			childObject.transform.parent = PanelForTheLevels.transform;
			LevelPrefab thisChildLP=childObject.GetComponent<LevelPrefab>();
			thisChildLP.SceneDetails(current,UpTo+1);
			//is this level unlocked
			if(UpTo<=MaxLevelUnlock){
				thisChildLP.unlock();
			}
			UpTo++;
		}

		//Boss button
//		if(!IsOdd(UpTo)){
//			Vector2 rectSize = PanelForTheLevels.GetComponent<RectTransform>().sizeDelta;
//			rectSize = new Vector2 (rectSize.x+300, rectSize.y);
//			PanelForTheLevels.GetComponent<RectTransform>().sizeDelta = rectSize;
//		}
		GameObject BossButton = Instantiate(ALevelPrefab) as GameObject;
		BossButton.transform.parent = PanelForTheLevels.transform;
		LevelPrefab BossLP=BossButton.GetComponent<LevelPrefab>();
		BossLP.SceneDetails(BossLevelName,UpTo+1);
		if(UpTo<=MaxLevelUnlock){
			BossLP.unlock();
		}
		UpTo++;//used in the resizing below

		//Debug.Log("UpTo "+UpTo);


		//resizing parent
		//finding max No. cellRows
		float MaxNoLevelDisplay=Mathf.Floor(ParentHeight/preferableHeight);
		if(MaxNoLevelDisplay==1){
			MaxNoLevelDisplay++;
		}else if(MaxNoLevelDisplay<=0){
			MaxNoLevelDisplay=1;
		}

		//resizing cells
		float NewPrefabsHeight=ParentHeight/MaxNoLevelDisplay-1;
		float NewPrefabsWidth=(preferableWidth/preferableHeight)*NewPrefabsHeight;
		PanelForTheLevels.GetComponent<GridLayoutGroup>().cellSize=new Vector2(NewPrefabsWidth,NewPrefabsHeight);
		//resizing the width of parent
		PFTLsRect.sizeDelta = new Vector2 (Mathf.Ceil(UpTo/MaxNoLevelDisplay)*NewPrefabsWidth,PFTLsRect.sizeDelta.y);



		//snapping to latest unlock
		PanelScrollRect.horizontalNormalizedPosition = (float)(((float)MaxLevelUnlock/*-Minus*/)/((float)UpTo+1));
	}
	private bool IsOdd(int value)
	{
		return value % 2 != 0;
	}
}
