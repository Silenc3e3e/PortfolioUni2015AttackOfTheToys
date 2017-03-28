using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Lever : MonoBehaviour {

	public List<GameObject> DoorsToOpen;
	[Range(0.0f,10f)]
	public float UseDistance=3.5f;

	public List<GameObject> ObjectsToSpawn;
    public List<Enemy> EnemiesToDefeatFirst;
    private int NoEnemies = 0;
    private bool Pulled=false;
    public List<GameObject> DoorsToClose;
	public List<GameObject> DoorsToOpenOnStart;
    public bool RestorePlayerHealth;

    public Animator LeverAnimator;

	private Transform PlayerTrans;
	public bool Activated=false;
	private int EnvironmentLayerMask;

    //Making the interact hand appear
    public Canvas canvas;
    public GameObject canvasPrefab;
    public GameObject panelRef;
    public GameObject panelPrefab;
    private DepthUI depthUIRef;
    private Button HandButton;
    //depth, alpha stuff
    public float panelOffset = 0.35f;
    private float BeginFadeDistance;
    private float BaseFadeDistance;
    private float FadeDivder;

    void Start(){
		PlayerTrans=GameObject.FindGameObjectWithTag("Player").transform;
		EnvironmentLayerMask=1<<LayerMask.NameToLayer("Environment");
		foreach (GameObject current in ObjectsToSpawn) {
            if(current!=null)
			    current.SetActive(false);
		}
		foreach(GameObject Current in DoorsToOpenOnStart){
            if (Current != null)
            {
                LowerObject ThisScript = Current.GetComponent<LowerObject>();
                if (ThisScript != null)
                {
                    ThisScript.LowerTheObject();
                }
            }
		}
        //Debug.Log("ETDF "+EnemiesToDefeatFirst.Count);
        foreach(Enemy current in EnemiesToDefeatFirst)
        {
            if (current != null)
            {
                NoEnemies++;
                current.PassThroughLever(this);
            }
        }
        InitiateHand();
    }


	#if UNITY_EDITOR
	public void OnDrawGizmos () {
		Gizmos.color=Color.red;
		foreach(GameObject current in ObjectsToSpawn){
			if(current!=null)
				Gizmos.DrawLine(transform.position,current.transform.position);
		}
		foreach(GameObject current in DoorsToOpen){
			if(current!=null)
				Gizmos.DrawLine(transform.position,current.transform.position);
		}
	}
	#endif


	void Update(){
        if (!Activated)
        {
            if (Vector3.Distance(transform.position, PlayerTrans.position) <= UseDistance && !Physics.Linecast(transform.position, PlayerTrans.position, EnvironmentLayerMask))
            {
                //Make interact button appear

                if (Input.GetButton("Interact"))
                {
                    Click();
                }
            }


            //visualizing the hand
            Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + panelOffset, transform.position.z);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            if (panelRef != null)
                panelRef.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
            else
                InitiateHand();
            if (depthUIRef != null)
            {
                float distance = Mathf.Infinity;
                distance = Vector3.Distance(transform.position, PlayerTrans.position);
                depthUIRef.depth = -distance;
                if (distance > 999999999999999f)
                    distance = 0f;
                float alpha = BaseFadeDistance - distance / FadeDivder;

                depthUIRef.SetAlpha(alpha);
            }
        }
        else if(!Pulled && NoEnemies<=0)
        {
            Pulled = true;
            LeverTask();
        }
    }
    public void ReduceNoEnemies()
    {
        NoEnemies--;
    }

    public void Click(bool ignoreSight = false)
    {
        if (!Activated && (ignoreSight ||(Vector3.Distance(transform.position, PlayerTrans.position) <= UseDistance && !Physics.Linecast(transform.position, PlayerTrans.position, EnvironmentLayerMask))))
        {
            //animation
            LeverAnimator.SetTrigger("Pull");

            Destroy(panelRef);
            Activated = true;
            //Debug.LogError("Interaction activated");
            if (NoEnemies <= 0)
                LeverTask();
        }
    }

    void LeverTask()
    {
        foreach (GameObject Current in DoorsToOpen)
        {
            if (Current != null)
            {
                LowerObject ThisScript = Current.GetComponent<LowerObject>();
                if (ThisScript != null)
                {
                    ThisScript.LowerTheObject();
                }
            }
        }
        foreach (GameObject Current in DoorsToClose)
        {
            if (Current != null)
            {
                LowerObject ThisScript = Current.GetComponent<LowerObject>();
                if (ThisScript != null)
                {
                    ThisScript.RaiseTheObject();
                }
            }
        }
        foreach (GameObject current in ObjectsToSpawn)
        {
            if (current != null)
                current.SetActive(true);
        }

        //restore player health
        if (RestorePlayerHealth)
        {
            PlayerControl PCScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
            if (PCScript != null)
            {
                PCScript.RestoreAllHealth();
            }
        }
    }

    void InitiateHand()
    {
        //showing fade
        BeginFadeDistance = UseDistance - UseDistance/3.5f;
        BaseFadeDistance = UseDistance / (UseDistance - BeginFadeDistance);
        FadeDivder = UseDistance - BeginFadeDistance;


        if (canvas == null)
        {
            GameObject OBJToFind = GameObject.FindGameObjectWithTag("CanvasEHBC");
            if (OBJToFind != null)
                canvas = OBJToFind.GetComponent<Canvas>();
            if (canvas == null)
            {
                GameObject CanvasObject = (GameObject)Instantiate(canvasPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                canvas = CanvasObject.GetComponent<Canvas>();
                if (canvas == null)
                {
                    Debug.LogWarning("Major Error! Enemy Health bar Canvas cannot be found or created");
                }
            }
        }
        if (panelRef == null)
        {
            if (panelRef == null)
                panelRef = Instantiate(panelPrefab) as GameObject;
            panelRef.transform.SetParent(canvas.transform, false);
            HandButton = panelRef.GetComponent<Button>();
            HandButton.onClick.AddListener(() => Click());
            depthUIRef = panelRef.GetComponent<DepthUI>();
            if (canvas != null)
                canvas.GetComponent<ScreenSpaceCanvasManager>().AddToCanvas(panelRef);
        }
    }
}
