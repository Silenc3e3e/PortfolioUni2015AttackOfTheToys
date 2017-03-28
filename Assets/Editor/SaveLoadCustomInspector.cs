using UnityEngine;
using System.Collections;
using UnityEditor;
	
[CustomEditor(typeof(SaveLoad))]
public class SaveLoadCustomInspector: Editor
{
	private int CurrentLevelNumber=-1;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		SaveLoad myScript = (SaveLoad)target;
		if(GUILayout.Button("Reset Unlocked Levels"))
		{
			myScript.ResetUnlockedLevels();
			CurrentLevelNumber=myScript.LoadDataForGUIInspector();
		}
		if(GUILayout.Button("Refresh current level unlocked"))
		{
			CurrentLevelNumber=myScript.LoadDataForGUIInspector();
		}

		GUILayout.Label ("Current Save: "+CurrentLevelNumber.ToString());
	}
}