using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour 
{	
	public Canvas canvas;
	public GameObject canvasPrefab;
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
		//Debug.LogError ("script called, deleting " + panelRef.name);
		//panelRef.SetActive (false);
		Destroy(panelRef.gameObject);
	}
	void InitiateVariables(){
		BaseFadeDistance=EndFadeDistance/(EndFadeDistance-BeginFadeDistance);
		FadeDivder=EndFadeDistance-BeginFadeDistance;
		if(canvas==null){
			GameObject OBJToFind=GameObject.FindGameObjectWithTag("CanvasEHBC");
			if(OBJToFind!=null)
				canvas=OBJToFind.GetComponent<Canvas>();
			if(canvas==null){
				GameObject CanvasObject=(GameObject)Instantiate(canvasPrefab,new Vector3(0,0,0),Quaternion.identity);
				canvas = CanvasObject.GetComponent<Canvas>();
				if(canvas==null){
					Debug.LogWarning("Major Error! Enemy Health bar Canvas cannot be found or created");
				}
			}
		}
		if(panelRef==null){
            panelRef = Instantiate(panelPrefab) as GameObject;
			panelRef.transform.SetParent(canvas.transform, false);
			HealthSlider = panelRef.GetComponentInChildren<Slider>();
			depthUIRef = panelRef.GetComponent<DepthUI>();
			if(canvas!=null)
				canvas.GetComponent<ScreenSpaceCanvasManager>().AddToCanvas(panelRef);
		}
	}
	void OnEnable(){
        //Debug.Log("EUI on enable called");
        if (panelRef != null)
            panelRef.SetActive(true);
        InitiateVariables();
	}
    void OnDisable()
    {
        if (panelRef != null)
            panelRef.SetActive(false);
    }


	void Update () 
	{		
		Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + panelOffset, transform.position.z);
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
		if (panelRef != null)
			panelRef.transform.position = new Vector3 (screenPos.x, screenPos.y, screenPos.z);
		else if(GetComponent<Enemy>().ThisUnitState!=Enemy.State.Dying)
			InitiateVariables ();
		if (depthUIRef != null) {
			float distance = (worldPos - Camera.main.transform.position).magnitude;
			depthUIRef.depth = -distance;

			float alpha = BaseFadeDistance - distance / FadeDivder;

			depthUIRef.SetAlpha (alpha);
		}
	}
}
