using UnityEngine;

public class ArrowScript : MonoBehaviour {
	
	[Range(1f,50f)]public float moveSpeed=10f;
	[Range(1f,50f)]public float maxDistance=10f;
	private float timer;
	private float previousTime;

	private float myDamage = 1f;
	private float myKnockBackDuration=0f;
	private float myknockBackDistance=0f;

    public bool PauseBeforeDestroy=false;
    public float TimeDelayBeforeDestroy=1f;
    private bool DestroyTimeDown=false;

	public void SetDamageAndKnockback(float damage, float knockBackDuration, float knockBackDistance){
		myDamage = damage;
		myknockBackDistance = knockBackDistance;
		myKnockBackDuration = knockBackDuration;
	}

	void Start () {
		transform.rotation=Quaternion.LookRotation (GameObject.FindGameObjectWithTag ("Player").transform.position - transform.position);
		timer=maxDistance/moveSpeed;
		previousTime = Time.time;
	}

	void Update () {
		transform.position += transform.forward*Time.deltaTime*moveSpeed;
		timer -= Time.time - previousTime;
        if (timer <= 0)
            DestorySelf();

        if (DestroyTimeDown)
            TimeDelayBeforeDestroy -= Time.time - previousTime;
        if (TimeDelayBeforeDestroy <= 0)
            DestroyObject(gameObject);
        previousTime = Time.time;
	}
	void OnTriggerEnter(Collider other) {
        if (other.tag == "StaticEnvironment") {
            DestorySelf();
        }
        else if (other.tag == "Player") {
            PlayerControl otherPT = other.GetComponent<PlayerControl>();
            if (otherPT != null) {
                if (myKnockBackDuration > 0)
                    otherPT.DamageAndAttack(myDamage, myknockBackDistance, myKnockBackDuration, transform.position);
                else
                    otherPT.Damage(myDamage);
            }
            
        }
	}
	public void ResetFlight(float speed, float distance){
		moveSpeed=speed;
		maxDistance=distance;
		timer=distance/speed;
        Debug.Log("Timer = "+timer + "Speed "+speed + "Distance "+distance);
	}

    private void DestorySelf()
    {
        if (!PauseBeforeDestroy)
            Destroy(gameObject);
        else
        {
            DestroyTimeDown = true;
            GetComponent<BoxCollider>().enabled = false;
            GetComponentInChildren<ParticleSystem>().enableEmission=false;
        }
    }
}
