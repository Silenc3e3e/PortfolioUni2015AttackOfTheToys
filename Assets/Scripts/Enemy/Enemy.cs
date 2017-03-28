using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : MonoBehaviour {
	
	/*Important notes:
	 * Made all variables lower case. and all functions upper case.
	 * exceptions: enum State{Idle, MoveToTarget, Attack, Cast, Hit, Dying, Death}
	 * 
	 * changed AttackSpeed to attackDuration
	 * 
	 * moved the following variables into "MeleeEnemy" script:
	 * attackDuration
	 * collisionTime
	 * attackTimer
	 * attackVisualization
	 * beginAttackVisualization
	 * 
	 * 
	 * added regions everywhere
	 * Recomendation: Tools-->Options-->Text Editor-->General-->Code Folding. Enable ALL 3 check boxes
	 * 
	 * changed the following functions from public to private:
	 * Start
	 * Idle
	 * Attacking
	 * 		Sub-note: Movement() is still public, as "MeleeEnemy" script calls said function
	 * Update
	 * WhileMovingTest
	 * WhileIdleTest
	 */
	
	
	#region Variables
	
	#region movement
	public float moveSpeed=5f;
	public Transform PlayerTransform;
	public float engageRange;
	public float attackRange;
	//how much how much the player has to be with this units attack range. 1= Only JUST needs to touch. 0= must be as close next to the player. related to function "WhileMovingTest"
	[Range (0.01f,1.0f)]public float attackLeeWay;
	
	private NavMeshAgent agent;

    //for the while moving test
    private bool SetRunningAnimFalse = false;
    private float TimerSRAF = 0;
    public float TimeDelayBeforeStopRunningAnim = .05f;
    #endregion

    #region Attacking
    //NOTE: see line 40-41. Attack leeway. related to function "WhileMovingTest"
    //The gap of time between COMPLETING each attack. This is the value that attackGapTimer is reset to each time
    public float attackGapsDuration;
	private float attackGapTimer= 0;
	//private EnemeyStrategisingScript MasterAI;
	
	//This is used to prevent the timer for attacking between attacks from going down when an attack animation is playing
	private bool attackAnimationRunning=false;

	//layer mask to see the player
	private int EnvironmentLayerMask;
	#endregion
	
	//Used in conjunction with attacking. See update function. But also can be used with other functions in future. also used for the check timer
	private float previousTime;

	
	#region Unit state
	public enum State
	{
		Idle,
		MoveToTarget,
		Attack,
		Hit,
		Dying,
		Knockback
	}
	public State currentState=State.Idle;
	public void SwitchToAttackState(){
		currentState = State.Attack;
	}
	public State ThisUnitState {
		get{ return currentState;}
	}
    #endregion

    #region health
    public GameObject HitVisual;
    public float maxHealth;
	public float currentHealth;
	public EnemyUI EUIScript;
	//public Renderer MaterialRenderer;
    #endregion


    #region Knockback variables
    public GameObject DizzyOBJ;
	private bool isKB = false;
	private float IEknockbacktime;
	private float IEknockbackDistance;
	public float MaxKnockbackFlyingBackDuration;
	private Vector3 IEknockbackplayer;
	private Coroutine KnockbackCoroutine;
	private bool AnimateKB=true;
	#endregion


	//used for the check timer to grab player's movement speed
	private PlayerControl MainPlayersControl;

	
	public GameObject DeathPoof;


	#region Animation Controller Thing
	public Animator EnemyAnimator;
	private int HashRunning;
	private int HashAttack;
	private int HashKnockback;
	private int HashDead;
	private int HashKnockbackMultiplier;
    private int HashKnockedOut;
    public int HashKnockedOutGet { get { return HashKnockedOut; } }
	#endregion
	
	#endregion
	
	
	
	
	#region Variables pass back/through
	
	#region pass back. IE. Returns a variable
	public NavMeshAgent CSEnemyAgent ()
	{
		return agent;
	}
	public bool CSEnemyIsKB ()
	{
		return isKB;
	}
	public float previousTimeReturn(){
		return previousTime;
	}
	public int HashRunningReturn(){
		return HashRunning;
	}
	public int HashAttackReturn(){
		return HashAttack;
	}
	#endregion

	public bool publicattackAnimationRunning{
		get{
			return attackAnimationRunning;
		}
		set{
			attackAnimationRunning=value;
		}
	}
	public PlayerControl PublicPlayerPCScript{
		get{
			return MainPlayersControl;
		}
	}
	#region pass through. IE. function sets the variable
	public void CSEnemyResetattackGapTimer ()
	{
		attackGapTimer = attackGapsDuration;
	}
	public void CSEnemyLeaveIdelState(){
		if (currentState == State.Idle) {
			Movement();
		}
	}
	#endregion



	#endregion

	#if UNITY_EDITOR
	
	public virtual void OnDrawGizmos() {

		//distance to player
		float PlayerDistance=Mathf.Infinity;
		if(PlayerTransform==null){
			PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
			if(PlayerTransform!=null)
				PlayerDistance=Vector3.Distance(transform.position,PlayerTransform.position);
		}

		//attack range
		Color HandleColour=Color.cyan;
		HandleColour.a=0.15f;
		if(PlayerTransform!=null){
			if(PlayerDistance <= attackRange){
				HandleColour=Color.red;
				HandleColour.a=0.75f;
			}
		}

		Handles.color=HandleColour;
//		Handles.color.a=0.0f;
		Handles.DrawWireDisc(transform.position,transform.up,attackRange);

		//distance to close to player
		if(PlayerTransform!=null){
			if(PlayerDistance <= attackRange*attackLeeWay){
				HandleColour=Color.red;
				HandleColour.a=0.75f;
			}
			else{
				HandleColour=Color.blue;
				HandleColour.a=0.15f;
			}
		}
		Handles.color=HandleColour;
		Handles.DrawWireDisc(transform.position,transform.up,attackRange*attackLeeWay);


		//distance for initial engagment
		if(PlayerTransform!=null){
			if(PlayerDistance <= engageRange){
				HandleColour=Color.yellow;
				HandleColour.a=0.75f;
			}else{
				HandleColour=Color.green;
				HandleColour.a=0.15f;
			}
		}

		Handles.color=HandleColour;
		Handles.DrawWireDisc(transform.position,transform.up,engageRange);
	}
    void OnDrawGizmosSelected()
    {
        if (!PlayerTransform)
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

	#endif



	private void notifyAlliesOfPlayer(){
		//Debug.LogError (name+" NAOP called");
		//GameObject CurrentNotifiedPrefab = (GameObject)Instantiate(NotifiedPrefab,transform.position,transform.rotation);
		//CurrentNotifiedPrefab.transform.Rotate(270,0,0);
		//CurrentNotifiedPrefab.transform.SetParent (transform);
		GameObject[] AllBadies = GameObject.FindGameObjectsWithTag ("Enemy");
		//List<GameObject> AllBadiesToBeNotified=new List<GameObject>();

		//go through every bad guy unit in map
		foreach (GameObject Current in AllBadies) {
//			if(Current.name=="EnemyMelee"){
//				Debug.LogError("Up to ally "+Current.name);
//				Debug.LogError(Vector3.Distance (transform.position, Current.transform.position)+" units away");
//			}
//			if (Vector3.Distance (transform.position, Current.transform.position) <= engageRange + 1) {
//				RaycastHit hit;
//				if (Physics.Linecast (transform.position, Current.transform.position, out hit)) {
//					Current.GetComponent<Enemy>().CSEnemyLeaveIdelState();
//				}
//			}

			float distanceToTarget=Vector3.Distance (transform.position, Current.transform.position);
			//is this unit within flat distance
			if (distanceToTarget <= engageRange + 1) {
//				Debug.LogError(Current.name + " in range");

//				Debug.DrawLine(transform.position, Current.transform.position);
				//Ray castedRay=(transform.position, Current.transform.position);
//				RaycastHit[] NAOPhits;
//				NAOPhits=Physics.RaycastAll (transform.position, Current.transform.position-transform.position, distanceToTarget);//maybe need distanceToTarget+0.1f????
//				// is unit within sight
//				//Debug.LogError(name + " in sight of "+hits.collider.name);
//
//				bool CanSeeUnit=true;
//				foreach(RaycastHit ray in NAOPhits){
//					//Debug.LogError("Ray item. name is "+ray.collider.name);
//					if (ray.collider.tag == "StaticEnvironment") {
//						//Debug.LogError("failed");
//						CanSeeUnit=false;
//						break;
//					}
//				}
				if(!Physics.Linecast(transform.position,Current.transform.position,1<< LayerMask.NameToLayer("Environment"))){
					Current.GetComponent<Enemy>().CSEnemyLeaveIdelState();
					//AllBadiesToBeNotified.Add(Current);
				}
			}
		}
		//CurrentNotifiedPrefab.GetComponentInChildren<ExclamationMarkAnimatorScript> ().AlliesToNotify (AllBadiesToBeNotified);
	}

    private Lever LeverToNotifyOfDeath;
    public void PassThroughLever(Lever pass)
    {
        LeverToNotifyOfDeath = pass;
    }

    private bool MoveNotIdleOnStart = false;
    private Spawner SpawnerToNotify;
    public void NotifyDeath(Spawner SpawnToNotify, bool attackOnSpawn=true)
    {
        SpawnerToNotify = SpawnToNotify;
        MoveNotIdleOnStart = attackOnSpawn;
    }
	public virtual void Start(){
        //turn off dizzy
        if (DizzyOBJ != null)
            DizzyOBJ.SetActive(false);

        //Animation
        HashRunning = Animator.StringToHash ("Running");
		HashAttack = Animator.StringToHash ("Attacking");
		HashKnockback=Animator.StringToHash ("Knockback");
		HashDead=Animator.StringToHash ("Dead");
		HashKnockbackMultiplier=Animator.StringToHash("KnockbackMultiplier");
        HashKnockedOut = Animator.StringToHash("KnockedOut");


        currentHealth = maxHealth;
		if (PlayerTransform==null)
			PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

		MainPlayersControl=PlayerTransform.gameObject.GetComponent<PlayerControl>();

		agent = GetComponent<NavMeshAgent> ();
		agent.SetDestination (PlayerTransform.position);
		agent.speed = moveSpeed;
		EUIScript=GetComponent<EnemyUI>();
        if (!MoveNotIdleOnStart)
            Idle();
        else
            Movement();

		//was being used to create master AI. But that has been put on hold. commented out of tidieness's sake
//		GameObject MasterAiSearch = GameObject.FindGameObjectWithTag ("EnemyUnitsMasterAI");
//		if (MasterAiSearch != null)
//			MasterAI=MasterAiSearch.GetComponent<EnemeyStrategisingScript> ();
		
		//for getting the time between frames
		previousTime=Time.time;

		//Grabbing the unit's color to pingpong between
		//colorEnd=MaterialRenderer.material.color;

		EnvironmentLayerMask=1<<LayerMask.NameToLayer("Environment");
	}
	
	public IEnumerator KnockBackCoRout ()
	{
		#region Stage 1: Initializing variables (that help prevent other tasks initiating. IE. Attacking, moving etc)
		isKB = true;
		float timer;
		bool extraWait = false;
		InteruptAttack();

		//animation
		if(EnemyAnimator!=null){
			if(AnimateKB){
				EnemyAnimator.SetTrigger (HashKnockback);
			}
			EnemyAnimator.SetBool (HashRunning, false);
		}
		#endregion

		#region Stage 2: Calculating amount of time (considering the distance) this unit should fly back for
		float DistanceDivider;
		if (IEknockbacktime > MaxKnockbackFlyingBackDuration) {
			timer = MaxKnockbackFlyingBackDuration;
			DistanceDivider = MaxKnockbackFlyingBackDuration;
			extraWait = true;
		} else {
			timer = IEknockbacktime;
			DistanceDivider = IEknockbacktime;
		}
		#endregion

		#region Stage 3: Knocking the unit back
		float previousTime = Time.time;

		while (timer>0) {
			float timeDifference=(Time.time - previousTime);
			if(timer>0f)
				transform.position = Vector3.MoveTowards (transform.position, IEknockbackplayer,timeDifference* (- IEknockbackDistance / DistanceDivider));
			timer-=timeDifference;
			previousTime = Time.time;
			yield return null;
		}

		#endregion

		#region Stage 4: Extra "dazed" wait
		if (extraWait) {
			yield return new WaitForSeconds (IEknockbacktime - MaxKnockbackFlyingBackDuration);
		}
        #endregion

        EndKnockback();
    }
    public virtual void EndKnockback()
    {
        #region Stage 5: Resuming functionality
        if (DizzyOBJ != null)
            DizzyOBJ.SetActive(false);
        Movement(true);
        isKB = false;
        //EnemyAnimator.SetBool (HashKnockback, false);
        if (EnemyAnimator != null)
            EnemyAnimator.SetBool(HashRunning, true);
        #endregion
    }

    #region Functions that change "currentState". IE. called once to set the unit into motion to begin the approriate action/state of being
    private void Idle ()

	{
		//Debug.LogError ("IDLE");
		agent.Stop ();
		currentState = State.Idle;
        if (EnemyAnimator != null)
            EnemyAnimator.SetBool(HashRunning,false);
	}
	
	public virtual void Attacking ()
	{
		//Debug.LogError ("ATTACKING");
		currentState = State.Attack;
		agent.Stop ();
        if (EnemyAnimator != null)
            EnemyAnimator.SetBool(HashRunning, false);
    }
	public virtual void InteruptAttack(){

	}

	public void Movement (bool ignoreKB=false)
	{
        //Debug.LogError ("MOVEMENT");
        if (DizzyOBJ != null)
            DizzyOBJ.SetActive(false);
        if (!isKB||ignoreKB) {
			currentState = State.MoveToTarget;
            if(agent==null)
                agent = GetComponent<NavMeshAgent>();
            agent.Resume ();
            if(PlayerTransform==null)
                PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            agent.SetDestination (PlayerTransform.position);
            if (EnemyAnimator != null)
            {
                SetRunningAnimFalse = false;
                EnemyAnimator.SetBool(HashRunning, true);
            }
        }
	}
	
	
	public virtual void knockback (float knockbackDuration, float knockbackDistance, Vector3 PointToTravelAwayFrom, bool ChainKB = true, bool interuptsAttack =true, bool Animate=true)
	{
		//if this knockback interupts attack
		if (interuptsAttack) {
			InteruptAttack ();
		}

		//remember. bool IsKB should be = true when a Knockback is in process
		if (!isKB || ChainKB) {
			if(KnockbackCoroutine!=null)
				StopCoroutine(KnockbackCoroutine);

//			try{
//				if(agent!=null)
			agent.Stop ();
            if (EnemyAnimator != null)
                EnemyAnimator.SetBool(HashRunning, false);
            //			}
            //			finally{
            //				Debug.LogWarning("Failed to find agent on "+name + ", of tag " + tag);
            //			}
            currentState = State.Knockback;
            if (DizzyOBJ != null)
                DizzyOBJ.SetActive(true);
            IEknockbacktime = knockbackDuration;
			IEknockbackplayer = PointToTravelAwayFrom;
			IEknockbackDistance = knockbackDistance;
			if(EnemyAnimator!=null)
				EnemyAnimator.SetFloat(HashKnockbackMultiplier,2.4f/knockbackDuration);
			AnimateKB=Animate;
			KnockbackCoroutine=StartCoroutine (KnockBackCoRout ());
		}
	}
	
	public void Damage (float ReduceHealthBy, bool Animate = true, bool IgnoresKB = true, bool interuptsAttack =true)
	{
		if (!isKB || IgnoresKB) {
			currentHealth -= Mathf.Abs (ReduceHealthBy);
            Instantiate(HitVisual, transform.position, Quaternion.identity);
            if (currentState==State.Idle)
				notifyAlliesOfPlayer();
			if(EUIScript!=null)
				EUIScript.UpdateHealthBar(currentHealth/maxHealth);
		}
		if (currentHealth <= 0) {
			StartCoroutine(Death());
		}

		if (interuptsAttack) {
			InteruptAttack ();
		}

	}

    private int UnitType=0;
    public void UnitTypeIs(int type)
    {
        UnitType = type;
    }

    public void SpawnerDeath()
    {
        StartCoroutine(Death());
    }

	private IEnumerator Death(){
        if (DizzyOBJ != null)
            DizzyOBJ.SetActive(true);
        if (EUIScript!=null)
			EUIScript.Death();
		currentState=State.Dying;
		if(EnemyAnimator!=null)
			EnemyAnimator.SetBool(HashDead,true);
		yield return new WaitForSeconds(.7f);
		Instantiate(DeathPoof,transform.position,Quaternion.identity * Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f)));
		yield return new WaitForSeconds(.1f);

        //for doors
        if (LeverToNotifyOfDeath != null)
            LeverToNotifyOfDeath.ReduceNoEnemies();

        //For continuous spawner
        if (SpawnerToNotify!=null)
            SpawnerToNotify.NotifyUnitDestroyed(gameObject, UnitType);
        Destroy(gameObject);
	}
	#endregion
	
	
	#region Update function. And all functions called with it each frame
	private float NextTestTimer=0f;
	private void Update ()
	{
		switch (currentState) {
		case State.Idle:
			WhileIdleTest ();
			break;
		case State.MoveToTarget:
			WhileMovingTest ();
			break;
		case State.Attack:
			if (attackGapTimer <= 0)
				WhileAttackingTest (attackGapTimer);
			break;
		}
		
		//timing down the cool down for the attack
		if (!attackAnimationRunning && attackGapTimer > 0) {
			attackGapTimer -= (Time.time - previousTime);
		}
		
		//this MUST remain at BOTTOM of update function
		previousTime = Time.time;

		//Make this enemy flash
		//MaterialToFlash.color = Color.white;
//		if (FlashMaterial) {
//			float lerp = Mathf.PingPong (Time.time, 0.5f) / 0.5f;
//			//foreach(Renderer current in MaterialRenderer)
//			MaterialRenderer.material.color = Color.Lerp (Color.red, colorEnd, lerp);
//			Debug.Log("Running flash for " +name);
//		}
	}
	//private float currentWhiteOnMaterial;
	//private bool IncreaseCWON;
//	private bool FlashMaterial;

//	private Color colorEnd;

	/*public void Flashing(bool ShouldFlash){
		//FlashMaterial = ShouldFlash;
		if(MaterialRenderer!=null){
			if(ShouldFlash)
				MaterialRenderer.material.color = Color.red;
			else
				MaterialRenderer.material.color = Color.white;
		}
//		if (FlashMaterial) {
//			IncreaseCWON=false;
//			currentWhiteOnMaterial=1f;
//		}

	}*/

    //Called every frame (see update function)


    
	private void WhileMovingTest ()
	{
        if (SetRunningAnimFalse)
        {
            TimerSRAF -= Time.time - previousTime;
            if (TimerSRAF <= 0)
                if (EnemyAnimator != null)
                {
                    EnemyAnimator.SetBool(HashRunning, false);
                    SetRunningAnimFalse = false;
                }

        }
		if (PlayerTransform.position != agent.destination) {
			//Debug.Log ("recalculating");
			agent.SetDestination (PlayerTransform.position);
		}
		if(NextTestTimer<=0){
			if (agent.remainingDistance <= attackRange) {
				if(!Physics.Linecast(transform.position,PlayerTransform.position,EnvironmentLayerMask)){
					if(agent.remainingDistance <= attackRange*attackLeeWay){
                        SetRunningAnimFalse = true;
                        TimerSRAF = TimeDelayBeforeStopRunningAnim;
						agent.Stop ();
                    }
                    else
                    {
                        if (EnemyAnimator != null)
                        {
                            SetRunningAnimFalse = false;
                            EnemyAnimator.SetBool(HashRunning, true);
                        }
                        agent.Resume();
                    }
					if (attackGapTimer <= 0){
                        TimerSRAF = TimeDelayBeforeStopRunningAnim;
                        SetRunningAnimFalse = true;
                        agent.Stop ();
						Attacking ();
					}
					else
                        NextTestTimerRecalculate();
                }
                else
                    NextTestTimerRecalculate();
                //Debug.LogError ("Launching coroutine from MTT");
            }
			else{
                if (EnemyAnimator != null)
                {
                    EnemyAnimator.SetBool(HashRunning, true);
                }
                SetRunningAnimFalse = false;
                agent.Resume ();

                NextTestTimerRecalculate();
			}
		}
		else
			NextTestTimer-= (Time.time - previousTime);
		
	}

    private void NextTestTimerRecalculate()
    {
        float RangeToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
        if (RangeToPlayer > attackLeeWay * attackRange)
        {
            float remainingStraightDistance = RangeToPlayer - attackLeeWay * attackRange;//when player in ideal attack range

            NextTestTimer = remainingStraightDistance / (moveSpeed + MainPlayersControl.walkSpeed);//complicated sum
            if (NextTestTimer > attackGapTimer)//IS the enemy ready to attack before in ideal range?
            {
                remainingStraightDistance = RangeToPlayer - attackRange;//when player in attack range

                NextTestTimer = remainingStraightDistance / (moveSpeed + MainPlayersControl.walkSpeed);//complicated sum
                if (NextTestTimer < attackGapTimer)//IS the enemy within max attack range before attack is ready?
                    NextTestTimer = attackGapTimer;//when attack is in range (somewhere between when the enemy can attack, and the ideal attack range
            }
        }
        else
        {
            float DistanceToCover =attackRange*attackLeeWay-RangeToPlayer;
            NextTestTimer =DistanceToCover/ MainPlayersControl.walkSpeed;
        }
        if (NextTestTimer < 0)
        {
            NextTestTimer = 0;
        }
    }
	
	//Called every frame (see update function)
	private void WhileIdleTest ()
	{
		
		//play idle animation here
		float distanceToTarget=Vector3.Distance (transform.position, PlayerTransform.position);
		if (distanceToTarget <= engageRange) {//there used to +1 to engageRange. I couldn't remember why, so I removed it

			if(!Physics.Linecast(transform.position,PlayerTransform.position,1<< LayerMask.NameToLayer("Environment"))){
				notifyAlliesOfPlayer();
				Movement ();
			}
			//}
		}
	}
	
	//called each update (see update function)
	public virtual void WhileAttackingTest (float compensation)
	{
		//NOTE: (below)
		/* attackGapTimer-=time.delta;
		 * attackGapTimer=0.03
		 * attackGapTimer= -0.005
		 * attackGapTimer= -5
		 * if attackGapTimer<=0// WILL NEED TO COMPENSATE FOR attackGapTimer OVER SHOOT INTO NEGATIVES. E.G. if it becomes -0.0something
		 * && attack routine NOT running bool <-- test this first
		 * 			{ being the enumeration
		 * 			}
		 * 
		 * 
		 * enum routine 
		 */
	}
	#endregion
	
	
}