using UnityEngine;

public class MoveAndAttackEnemy : Enemy {
	
	
	#region Variables
	
	//How long the attack itself (animation etc) goes for
	public float attackDuration;
	private float attackDurationTimer;
	//At what point the during the attack (see above) where the damage is passed through
	public float attackIntervals;
	private float attackIntervalTimer;
	//This is tied in with attackDuration. It goes down, while the animation plays
	private float attackTimer;
	//How fast this unit attacks while moving
	public float attackMoveSpeed;
    public float AttackWindUpDuration=.6f;
    private float attackWindUpDurationTimer;
    public ParticleSystem ParticleSwish1;
    public ParticleSystem ParticleSwish2;

    


    //The mesh collider where the player should get hit
    public StoreObjectsInCollider AttackBox;

	//knock back related
	public float knockbackDurationOnPlayer;
	public float knockbackDistanceOnPlayer;
	public float attackDamage;

    //	private Coroutine CurrentAttackCoroutine;


    #endregion
    public override void Start()
    {
        ParticleSwish1.emissionRate=0;
        ParticleSwish2.emissionRate = 0;
        UnitTypeIs(3);
        base.Start();
    }

    public override void Attacking ()
	{
		//change to attacking animation here
		attackIntervalTimer = 0f;
		publicattackAnimationRunning = true;
		SwitchToAttackState ();
		CSEnemyAgent ().speed = attackMoveSpeed;
		attackDurationTimer = attackDuration;
        attackWindUpDurationTimer = AttackWindUpDuration;
        CSEnemyAgent ().Resume();
        EnemyAnimator.SetBool(HashAttackReturn(),true);

        ParticleSwish1.emissionRate = 100;
        ParticleSwish2.emissionRate = 100;
    }
	
	public override void WhileAttackingTest (float compensation)
	{
		
		float timeDifference = (Time.time - previousTimeReturn());
        if (attackWindUpDurationTimer >= 0)
            attackWindUpDurationTimer -= timeDifference;
        else
        {
            attackDurationTimer -= timeDifference;
            if (attackIntervalTimer > 0f)
                attackIntervalTimer -= timeDifference;
            else
            {
                foreach (GameObject currentObj in AttackBox.ReturnObjectList())
                {
                    if (currentObj.tag == "Player")
                    {
                        if (!PublicPlayerPCScript.IsBeingKnockedBack())
                        {
                            PublicPlayerPCScript.DamageAndAttack(attackDamage, knockbackDurationOnPlayer, knockbackDistanceOnPlayer, transform.position);
                            attackIntervalTimer = attackIntervals;

                            break;
                        }
                    }
                }
            }
        }

		if (PlayerTransform.position != CSEnemyAgent().destination) {
			//Debug.Log ("recalculating");
			CSEnemyAgent().SetDestination (PlayerTransform.position);
		}
//		if (CSEnemyAgent().remainingDistance <= attackRange) {
//			CSEnemyAgent().Stop ();
//			
//			//Debug.LogError ("Launching coroutine from MTT");
//		}
//		else
//			CSEnemyAgent().Resume ();

		if (attackDurationTimer <= 0f) {
			publicattackAnimationRunning = false;
			CSEnemyResetattackGapTimer ();
			CSEnemyAgent().speed=moveSpeed;
			Movement ();
            EnemyAnimator.SetBool(HashAttackReturn(), false);
            ParticleSwish1.emissionRate = 0;
            ParticleSwish2.emissionRate = 0;
        }

		//If we are not currently knocked down. go through with the attack
//		if(!CSEnemyIsKB() && !publicattackAnimationRunning){
//			//if the player is within attack range
//			if (Vector3.Distance(transform.position,PlayerTransform.position) <= attackRange+1.5f) {
//				/*CurrentAttackCoroutine=*/StartCoroutine(AttackingCoRout());
//				//CSEnemyResetattackGapTimer();
//			} else{
//				//If the player is not in attack range. go back to walk'in around
//				Movement();
//			}
//		}
	}

    public override void knockback(float knockbackDuration, float knockbackDistance, Vector3 PointToTravelAwayFrom, bool ChainKB = true, bool interuptsAttack = true, bool Animate = true)
    {
        EnemyAnimator.SetBool(HashKnockedOutGet, true);
        base.knockback(knockbackDuration, knockbackDistance, PointToTravelAwayFrom, ChainKB, interuptsAttack, Animate);
    }
    public override void EndKnockback()
    {
        EnemyAnimator.SetBool(HashKnockedOutGet, false);
        base.EndKnockback();
    }



}
