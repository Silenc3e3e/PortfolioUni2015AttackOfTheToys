using UnityEngine;
using System.Collections;

public class MeshHitBoxEnemy : Enemy {


	#region Variables

	//How long the attack itself (animation etc) goes for
	public float attackDuration;
	//At what point the during the attack (see above) where the damage is passed through
	public float collisionTime;
	//This is tied in with attackDuration. It goes down, while the animation plays
	private float attackTimer;
	

	//The mesh collider where the player should get hit
	public StoreObjectsInCollider AttackBox;

	private Coroutine CurrentAttackCoroutine;

	//knock back related
	public float knockbackDurationOnPlayer;
	public float knockbackDistanceOnPlayer;
	public float attackDamage;
    #endregion

    public override void Start()
    {
        UnitTypeIs(1);
        base.Start();
    }

    public override void WhileAttackingTest (float compensation)
	{
		//If we are not currently knocked down. go through with the attack
		if(!CSEnemyIsKB() && !publicattackAnimationRunning){
			//if the player is within attack range
			if (Vector3.Distance(transform.position,PlayerTransform.position) <= attackRange) {
				CurrentAttackCoroutine=StartCoroutine(AttackingCoRout());
				//CSEnemyResetattackGapTimer();
			} else{
				//If the player is not in attack range. go back to walk'in around
				Movement();
			}
		}
	}


	public override void InteruptAttack ()
	{
		if(CurrentAttackCoroutine!=null)
			StopCoroutine (CurrentAttackCoroutine);
		//the timer for gaps between attacks can start going down again
		publicattackAnimationRunning = false;
		//if at end of development, this line can be cut if base script is unchanged
		base.InteruptAttack ();
	}


	IEnumerator AttackingCoRout(){
		//CSEnemyAgent ().Stop ();
		#region Stage 1: attack incoming


		//Debug.LogError("Attack 1");
		//Make the unit face the player
		transform.rotation=Quaternion.LookRotation (PlayerTransform.position - transform.position);

		//animation
		if(EnemyAnimator!=null){
			EnemyAnimator.SetBool (HashRunningReturn(), false);
			EnemyAnimator.SetTrigger (HashAttackReturn());
		}


		//stop the timer for the delay inbetween attacks from going down.
		publicattackAnimationRunning = true;

		yield return new WaitForSeconds (collisionTime);
		#endregion


		#region Stage 2: Damage if conditions are met
//		Debug.LogError("Attack 2");
		//If the enemy is within hit range. Damage the player (see the Player's control script)
//		if (Vector3.Distance (transform.position, PlayerTransform.position) <= attackRange + 1) {
//			PlayerControl PlayerCT = PlayerTransform.gameObject.GetComponent<PlayerControl> ();
//			PlayerCT.Damage (1f);
//			PlayerCT.Knockback (IEknockbacktime, IEknockbackDistance, transform.position);
//		}
//		if(AttackBox.ReturnObjectList().Count!=0)
		foreach (GameObject currentObj in AttackBox.ReturnObjectList()) {
			if(currentObj.tag=="Player"){
				PublicPlayerPCScript.DamageAndAttack(attackDamage, knockbackDurationOnPlayer,knockbackDistanceOnPlayer,transform.position);
				break;
			}
		}
		//Reset the timer which puts a gap between unit attacks.
		CSEnemyResetattackGapTimer ();
		#endregion


		#region Stage 3: wait for remaining animation to play
		//Debug.LogError("Attack 3");
		//If the attackDuration is less than the point in time where the damage is dealt. wait for (see if true)
		if (attackDuration - collisionTime < 0)
			//true
			//wait for the duration of attackDuration
			yield return new WaitForSeconds (attackDuration);
		else
			//false
			//wait for the difference between attackDuration and collisionTime
			yield return new WaitForSeconds (attackDuration - collisionTime);
		#endregion


		#region Stage 4: resume chasing the player and the likes.
	//	Debug.LogError("Attack 4");
		//the timer for gaps between attacks can start going down again
		publicattackAnimationRunning = false;

		//reengage movement. Investigate changing this, to see if the enemy can just stand on the spot while waiting before reinitiating the attack
		Movement ();

		//animation
		if(EnemyAnimator!=null)
			EnemyAnimator.SetBool (HashRunningReturn(), true);
		//EnemyAnimator.SetBool (HashAttackReturn(), false);
		//Debug.LogError("Attack End");
		#endregion

	}
}
