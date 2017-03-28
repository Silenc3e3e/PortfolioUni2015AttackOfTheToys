using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossUI : MonoBehaviour {
	
	public Canvas canvas;
	public GameObject panelPrefab;
	
	public float panelOffset = 0.35f;
	public GameObject panelRef;
	
	public float BeginFadeDistance;
	public float EndFadeDistance;
	private float BaseFadeDistance;
	private float FadeDivder;
	
	private Slider HealthSlider;
	
	private DepthUI depthUIRef;
	
	void Start () 
	{
		InitiateVariables();
	}
	public void UpdateHealthBar(float HealthUpToOne){
		if (HealthSlider != null)
			HealthSlider.value = HealthUpToOne;
		else
			InitiateVariables ();
	}
	public void Death(){
		Destroy(panelRef);
	}
	void InitiateVariables(){
		BaseFadeDistance=EndFadeDistance/(EndFadeDistance-BeginFadeDistance);
		FadeDivder=EndFadeDistance-BeginFadeDistance;
		if(canvas==null){
			canvas=GameObject.FindGameObjectWithTag("CanvasHUD").GetComponent<Canvas>();
		}
		if(panelRef==null){
			panelRef = Instantiate(panelPrefab) as GameObject;
			panelRef.transform.SetParent(canvas.transform, false);
			HealthSlider = panelRef.GetComponentInChildren<Slider>();
			depthUIRef = panelRef.GetComponent<DepthUI>();
//			if(canvas!=null)
//				canvas.GetComponent<ScreenSpaceCanvasManager>().AddToCanvas(panelRef);
		}
	}
	void OnEnable(){
		//Debug.Log("EUI on enable called");
		InitiateVariables();
	}
	
	
//	void Update () 
//	{		
//		Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + panelOffset, transform.position.z);
//		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
//		if (panelRef != null)
//			panelRef.transform.position = new Vector3 (screenPos.x, screenPos.y, screenPos.z);
//		else
//			InitiateVariables ();
//		
//		float distance = (worldPos - Camera.main.transform.position).magnitude;
//		depthUIRef.depth = -distance;
//		
//		float alpha = BaseFadeDistance - distance / FadeDivder;
//		depthUIRef.SetAlpha(alpha);
//	}
}
