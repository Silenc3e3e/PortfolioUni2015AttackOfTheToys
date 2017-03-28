using UnityEngine;
using System.Collections;

public class LowerObject : MonoBehaviour {

	public float AmountToChangeHieghtBy=5f;
	public Transform VisualObjectMovement;
    public float LowerDuration = 1f;

	private bool Lower = false;
	private bool Raise = false;
	private float DistanceCurrentlyLowered;
	private float DistanceCurrentlyRaised;
	private BoxCollider SelfCollider;
	private NavMeshObstacle SelfNavMeshObstacle;

    public GameObject SpawnOnOpen;
    public Transform spawnPoint;

    public Spawner NotifyOnOpen;

	public void LowerTheObject(){
		Lower=true;
	}
	public void RaiseTheObject(){
		Raise=true;
		SelfCollider.enabled=true;
		SelfNavMeshObstacle.enabled=true;
	}
	void Start(){
		SelfCollider=GetComponent<BoxCollider>();
		SelfNavMeshObstacle=GetComponent<NavMeshObstacle>();

	}

	void Update(){
		if(Lower){
			if(DistanceCurrentlyLowered<AmountToChangeHieghtBy){
				VisualObjectMovement.position= new Vector3(VisualObjectMovement.position.x,VisualObjectMovement.position.y-Time.deltaTime/LowerDuration,VisualObjectMovement.position.z);
				DistanceCurrentlyLowered+=Time.deltaTime / LowerDuration;
			}else{
				SelfCollider.enabled=false;
				SelfNavMeshObstacle.enabled=false;
				Lower=false;
                if (SpawnOnOpen != null)
                {
                    Transform pointtospawnAt=spawnPoint;
                    if (spawnPoint == null)
                        pointtospawnAt = transform;
                    Instantiate(SpawnOnOpen, pointtospawnAt.position, pointtospawnAt.rotation);
                }
                if(NotifyOnOpen!=null)
                    NotifyOnOpen.BossIsDead();
            }
		} else if(Raise){
			if(DistanceCurrentlyRaised<AmountToChangeHieghtBy){
				VisualObjectMovement.position= new Vector3(VisualObjectMovement.position.x,VisualObjectMovement.position.y+Time.deltaTime,VisualObjectMovement.position.z);
				DistanceCurrentlyRaised+=Time.deltaTime;
			}else{
				Raise=false;
			}
		}
	}

}
