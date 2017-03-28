using UnityEditor;

[CustomEditor(typeof(Boss))] 
public class BossEditorScript : Editor {
	public override void OnInspectorGUI(){
		Boss myTarget = (Boss)target;
		EditorGUILayout.LabelField("Current Stage is ",myTarget.CurrentStagePassBack);
		EditorGUILayout.LabelField("Current state is ",myTarget.CurrentStatePassBack);

		int stage = 2;
		float previousHealth=myTarget.maxHealth;
		EditorGUILayout.LabelField("To get to each stage, the Boss's health needs to be less than");
		foreach(float current in myTarget.CurrentStageHealthPercentageRequired)
		{
			float newHealth=current*myTarget.maxHealth;
			EditorGUILayout.LabelField("Stage "+stage, newHealth.ToString()+" Having removed "+(previousHealth-newHealth).ToString()+" Health");
			previousHealth=newHealth;
			stage++;
		}
		DrawDefaultInspector();
	}
}