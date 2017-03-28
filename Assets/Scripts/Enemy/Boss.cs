using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Boss : MonoBehaviour {

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

    public Transform DeathCamPos;
    public Transform DeathCamTarg;


    [SerializeField] private int MeleeAttackCounter;
    [SerializeField] private int SpinAttackCounter;
    [SerializeField] private int RangedAttackCounter;
    [SerializeField] private int MeleeAttackTimeSinceLastUseCounter;
    [SerializeField] private int SpinAttackTimeSinceLastUseCounter;
    [SerializeField] private int RangedAttackTimeSinceLastUseCounter;
    private int CurrentStage=1;
	public string CurrentStagePassBack{
		get{ return CurrentStage.ToString();}
	}
	[Range (0.01f,1.0f)]
	public float[] CurrentStageHealthPercentageRequired;

    //Visualization of boss strengthening
    public ParticleSystem DeathFire;
    public float EmmisionToReach;
    public Transform[] ObjectsToAppearStage2;
    public ParticleSystem[] SystemsToActivateStage3;
    private bool ShouldGrowObjects = false;
    //private float GrowTimer = 1f;

	#region nav mesh movement
	public float[] moveSpeed4Stages;
	public Transform PlayerTransform;
    private PlayerControl PCScript;


	//public is temporary. please return to private
	public NavMeshAgent agent;
	#endregion
	
	#region Attacking
	//NOTE: see line 40-41. Attack leeway. related to function "WhileMovingTest"
	//The gap of time between COMPLETING each attack. This is the value that attackGapTimer is reset to each time
	public float[] attackGapsDuration4Stages;
	private float attackGapTimer= 0;
	//private EnemeyStrategisingScript MasterAI;
	
	//This is used to prevent the timer for attacking between attacks from going down when an attack animation is playing
	private bool attackAnimationRunning=false;
	
	//layer mask to see the player
	private int EnvironmentLayerMask;

	


	//melee attacks
	public float MeleeAttackRange;
	public int MeleeMaxNoAttackInARow;
	//how much how much the player has to be with this units attack range. 1= Only JUST needs to touch. 0= must be as close next to the player. related to function "WhileMovingTest"
	[Range (0.01f,1.0f)]public float[] MeleeAttackLeeWay4Stages;
	public float[] MeleeAttackDuration4Stages;
	[Range(0.0f,1f)]
	public float meleeCollisionTime;
	public StoreObjectsInCollider MeleeAttackBox;
	//Melee knock back related
	public float[] MeleeknockbackDurationOnPlayer4Stages;
	public float[] MeleeknockbackDistanceOnPlayer4Stages;
	public float[] MeleeAttackDamage4Stages;
	public float[] MeleeTurnRate4Stages;

	//Spin attacks
	public StoreObjectsInCollider SpinAttackBox;
	public int SpinMaxNoAttackInARow;
	public float SpinAttackRange;
	//No lee way for spin, as the boss can start the spin outside of the spin's attack range
	public float SpinTimeToBeginSpinning;
	public float[] SpinDuration3Stages;
	public float[] SpinknockbackDurationOnPlayer3Stages;
	public float[] SpinknockbackDistanceOnPlayer3Stages;
	public float[] SpinAttackDamage3Stages;
	public float[] SpinMovementSpeed3Stages;
	public float SpinPlayerHitExtraCoolDown=2f;

	//Ranged attacks
	public int RangedMaxNoAttackInARow;
	public float[] RangedAttackDamage2Stages;
	public float RangedAttackRange;
	[Range (0.01f,1.0f)]public float[] RangedAttackLeeWay2Stages;
	public float[] RangedKnockbackDuration2Stages;
	public float[] RangedKnockbackDistance2Stages;
	public float[] RangedTravelSpeed2Stages;
	public float[] RangedDurationTillProjectileFire2Stages;
	public float[] RangedTimeToReturnToNeutral2Stages;
	public GameObject RangedPrefabToSpawn;
	public Transform RangedSpawnPoint;
	#endregion
	
	//Used in conjunction with attacking. See update function. But also can be used with other functions in future
	private float previousTime;

   

    #region Unit state
    public enum State
	{
		Idle,
		MoveToTarget,
		Attack,
		Dying,
		Roar
	}
	private State currentState;
	public string CurrentStatePassBack{
		get{ return currentState.ToString();}
	}
	public void SwitchToAttackState(){
		currentState = State.Attack;
	}
	#endregion
	
	#region health
	public float maxHealth;
	public float currentHealth;
	public BossUI UIScriptRef;
	public Renderer MaterialRenderer;
	#endregion
	
	
	#region Animation Controller Thing
	public Animator EnemyAnimator;
	private bool publicattackAnimationRunning;

	//Hashes
	private int HashDeath;
	private int HashMoving;
	private int HashAttack;
	private int HashAttackTypeMSR;
	private int HashSpinning;
	private int HashRoar;
    #endregion

    #endregion

    //for death
    public GameObject PrefabLoading;

    //for spawning units
    public Spawner MasterSpawner;

    #region pass through. IE. function sets the variable
    public void CSEnemyResetattackGapTimer ()
	{
		attackGapTimer = attackGapsDuration4Stages[CurrentStage-1];
	}
	public void CSEnemyLeaveIdelState(){
		if (currentState == State.Idle) {
			Movement();
			//Debug.Log("Boss leave idle called!");
		}
	}
	#endregion
	
	#if UNITY_EDITOR

	private string GizmoDisplayText="Empty";

	void OnDrawGizmos() {
		//Melee
		Color HandleColour=Color.cyan;
		if(PlayerTransform!=null){
			if(Vector3.Distance(transform.position,PlayerTransform.position) <= MeleeAttackRange)
				HandleColour=Color.red;
		} else{
			PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		
		HandleColour.a=0.5f;
		Handles.color=HandleColour;
		Handles.DrawWireDisc(transform.position,transform.up,MeleeAttackRange);
		HandleColour=Color.blue;
		if(PlayerTransform!=null){
			if(Vector3.Distance(transform.position,PlayerTransform.position) 
			   <= MeleeAttackRange*
			   MeleeAttackLeeWay4Stages[0])
				HandleColour=Color.red;
		}
		foreach(float current in MeleeAttackLeeWay4Stages)
			Handles.DrawWireDisc(transform.position,transform.up,MeleeAttackRange*current);

		//Spin
		HandleColour=Color.green;
		if(PlayerTransform!=null){
			if(Vector3.Distance(transform.position,PlayerTransform.position) <= SpinAttackRange)
				HandleColour=Color.red;
		} else{
			PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		HandleColour.a=0.5f;
		Handles.color=HandleColour;
		Handles.DrawWireDisc(transform.position,transform.up,SpinAttackRange);

		//Ranged
		HandleColour=Color.black;
		if(PlayerTransform!=null){
			if(Vector3.Distance(transform.position,PlayerTransform.position) <= RangedAttackRange)
				HandleColour=Color.red;
		} else{
			PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		HandleColour.a=0.5f;
		Handles.color=HandleColour;
		Handles.DrawWireDisc(transform.position,transform.up,RangedAttackRange);
		HandleColour=Color.gray;
		if(PlayerTransform!=null){
			if(Vector3.Distance(transform.position,PlayerTransform.position) <= RangedAttackRange*RangedAttackLeeWay2Stages[0])
				HandleColour=Color.red;
		}
		foreach(float current in RangedAttackLeeWay2Stages)
			Handles.DrawWireDisc(transform.position,transform.up,RangedAttackRange*current);


		Handles.color=Color.green;
		Handles.Label(transform.position + Vector3.up*3f,GizmoDisplayText);
	}
	#endif

	
	public void Start(){
		agent = gameObject.GetComponent<NavMeshAgent> ();

	//Make visualizations of strength vanish
    foreach(Transform current in ObjectsToAppearStage2)
        {
            current.localScale = new Vector3(0f, 0f, 0f);
        }
    foreach(ParticleSystem current in SystemsToActivateStage3)
        {
            current.enableEmission = false;
        }


        DeathFire.emissionRate = 0f;

        //Animation
        if (EnemyAnimator != null) {
			HashDeath = Animator.StringToHash ("Death");
			HashMoving = Animator.StringToHash ("Moving");
			HashAttack = Animator.StringToHash ("Attack");
			HashAttackTypeMSR= Animator.StringToHash ("AttackTypeMSR");
			HashSpinning= Animator.StringToHash ("Spinning");
			HashRoar= Animator.StringToHash ("Roar");
		}
		
		
		currentHealth = maxHealth;
		if (!PlayerTransform)
			PlayerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		if(UIScriptRef==null)
			UIScriptRef=gameObject.GetComponent<BossUI>();

		agent.SetDestination (PlayerTransform.position);
		agent.speed = moveSpeed4Stages[CurrentStage-1];

		//temporary for testing
		CSEnemyLeaveIdelState ();
		//Idle();
		
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
	
	#region Functions that change "currentState". IE. called once to set the unit into motion to begin the approriate action/state of being
	private void Idle ()
	{
		//Debug.LogError ("IDLE");
		agent.Stop ();
		currentState = State.Idle;
		#if UNITY_EDITOR
		GizmoDisplayText="Idling";
		#endif
		//animation
		EnemyAnimator.SetBool (HashMoving, false);
	}
	
	public void Attacking ()
	{
		#if UNITY_EDITOR
		GizmoDisplayText="Attacking";
		#endif
		//Debug.LogError ("ATTACKING");
		currentState = State.Attack;
		agent.SetDestination(PlayerTransform.position);
		StartCoroutine(MeleeAttackCoroutine());
	}

    public void Movement(bool ignoreKB = false)
    {
        if (currentState != State.Dying)
        {
            currentState = State.MoveToTarget;
            agent.SetDestination(PlayerTransform.position);
            agent.Resume();
#if UNITY_EDITOR
            GizmoDisplayText = "Moving";
#endif
            EnemyAnimator.SetBool(HashMoving, true);
        }
	}


    private bool NextStage;
	public void Damage (float ReduceHealthBy, bool Animate = true, bool IgnoresKB = true, bool interuptsAttack =true)
	{
		currentHealth -= Mathf.Abs (ReduceHealthBy);
        //this needs to be changed. but close enough for now
        //float HealthLevelNeeded=CurrentStageHealthPercentageRequired[CurrentStage-1];
        if (currentHealth <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Death());
            currentState = State.Dying;
            UIScriptRef.Death();
        }
        else
            UIScriptRef.UpdateHealthBar(currentHealth / maxHealth);

        if (CurrentStageHealthPercentageRequired.Length >=CurrentStage) {
            if (currentHealth <= CurrentStageHealthPercentageRequired[CurrentStage - 1] * maxHealth)
            {
                NextStage = true;
            }
		}
	}

    private IEnumerator Roar()
    {
        if (currentState != State.Dying)
        {
            currentState = State.Roar;
            agent.Stop();
            EnemyAnimator.SetTrigger(HashRoar);
            EnemyAnimator.SetBool(HashMoving, false);
            transform.rotation = Quaternion.LookRotation(new Vector3(PlayerTransform.position.x, transform.position.y, PlayerTransform.position.z) - transform.position);
            yield return new WaitForSeconds(1.633333333333333f);
            if (PCScript == null)
                PCScript = PlayerTransform.gameObject.GetComponent<PlayerControl>();
            PCScript.DamageAndAttack(0, 1f, 5f, transform.position);
            PCScript.RestoreAllHealth();
            yield return new WaitForSeconds(2.899666666666667f);
            if (currentState != State.Dying)
                Movement();
        }
    }
	
	private IEnumerator Death(){
        //PlayerControl thisObjectsPlayerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        PlayerTransform.gameObject.SetActive(false);
        /*if(thisObjectsPlayerScript!=null)
            thisObjectsPlayerScript.MyState = PlayerControl.State.Dead;*/

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>().DeathCam(DeathCamPos, DeathCamTarg);
        foreach (ParticleSystem current in SystemsToActivateStage3)
        {
            current.emissionRate = 0f;
        }
        if (MasterSpawner != null)
            MasterSpawner.BossIsDead();

		#if UNITY_EDITOR
		GizmoDisplayText="Dying";
		#endif
		currentState=State.Dying;
        agent.Stop();
		if(EnemyAnimator!=null)
			EnemyAnimator.SetBool(HashDeath,true);
		yield return new WaitForSeconds(3f);
        agent.Stop();
        //particle flash thing?
        float timer = 3.5f;
        float previousTime = Time.time;
        while(timer>0)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale,.01f,Time.deltaTime);
            Time.fixedDeltaTime = Time.timeScale;
            timer -= Time.time - previousTime;
            previousTime = Time.time;
            yield return null;
        }
        //yield return new WaitForSeconds(1f);
        agent.Stop();
        GameObject spawnedPrefab = (GameObject)Instantiate(PrefabLoading, new Vector3(0, 0, 0), Quaternion.identity);
        LoadNextScene LNSScript = spawnedPrefab.GetComponent<LoadNextScene>();
        if (LNSScript != null)
            LNSScript.LoadLevel("MainMenu");
        else
        {
            Debug.LogWarning("Loading bar not found. Loading level Manually");
            Application.LoadLevel("MainMenu");
        }
    }
	#endregion
	
	
	#region Update function. And all functions called with it each frame
	private void Update ()
	{
		switch (currentState) {
		case State.Idle:
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

        if (ShouldGrowObjects)
        {
            foreach (Transform current in ObjectsToAppearStage2)
            {
                current.localScale = Vector3.Lerp(current.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime);
            }
            //Debug.Log("Remaining scale "+Vector3.Distance(ObjectsToAppearStage2[0].localScale, new Vector3(1f, 1f, 1f)));
            if (Vector3.Distance(ObjectsToAppearStage2[0].localScale,new Vector3(1f,1f,1f))<.001f)
            {
                ShouldGrowObjects = false;
                foreach (Transform current in ObjectsToAppearStage2)
                {
                    current.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }

        if (currentState == State.Dying)
            DeathFire.emissionRate = Mathf.Lerp(DeathFire.emissionRate,EmmisionToReach,Time.deltaTime/10f);

        //this MUST remain at BOTTOM of update function
        previousTime = Time.time;
    }
	
	public void Flashing(bool ShouldFlash){
		//FlashMaterial = ShouldFlash;
		if(ShouldFlash)
			MaterialRenderer.material.color = Color.red;
		else
			MaterialRenderer.material.color = Color.white;
		//		if (FlashMaterial) {
		//			IncreaseCWON=false;
		//			currentWhiteOnMaterial=1f;
		//		}
		
	}


	
	//Called every frame (see update function)
	private void WhileMovingTest ()
	{
		if (PlayerTransform.position != agent.destination) {
			agent.SetDestination (PlayerTransform.position);
		}
        //Debug.Log("Next Stage "+ NextStage);
        if (NextStage)
        {
            NextStage = false;
            CurrentStage++;
            if (MasterSpawner != null)
                MasterSpawner.IncreaseCurrentStage();
            agent.speed = moveSpeed4Stages[CurrentStage - 1];
            MeleeAttackCounter = 0;
            RangedAttackCounter = 0;
            SpinAttackCounter = 0;

            //making visual items on the boss reappear
            if (CurrentStage >= 2)
                ShouldGrowObjects = true;

            if (CurrentStage >= 3)
                foreach (ParticleSystem current in SystemsToActivateStage3)
                {
                    current.enableEmission = true;
                    GetComponent<PlayAudio>().ConnectedAudioPlay(0);
                }

            if (currentState != State.Dying)
                StartCoroutine(Roar());

        } else


        //I KNOW, I KNOW. Switch statement. But I wasn't sure how to do it when each case hase multiple conditions
        if (!Physics.Linecast(transform.position, PlayerTransform.position, EnvironmentLayerMask)) {
            //Melee test instead of in a seperate function

            bool SpinAttackNotNeeded = !(SpinAttackTimeSinceLastUseCounter > MeleeAttackCounter + RangedAttackCounter && SpinAttackCounter < SpinMaxNoAttackInARow);
            bool RangedAttackNotNeeded = !(RangedAttackTimeSinceLastUseCounter > MeleeAttackCounter + SpinAttackCounter && RangedAttackCounter <= RangedMaxNoAttackInARow);

            if (MeleeAttackCounter < MeleeMaxNoAttackInARow || CurrentStage == 1) {
                if (attackGapTimer <= 0) {
                    if (Vector3.Distance(transform.position, PlayerTransform.position) <= MeleeAttackRange) {
                        if (CurrentStage < 3 || (SpinAttackNotNeeded && RangedAttackNotNeeded)) {
                            RunMeleeAttack();
                        } else if (!SpinAttackNotNeeded) {
                            //Debug.Log("Spin 1 SANN = false. Current stage 3 or greater. RANN = "+RangedAttackNotNeeded);
                            RunSpinAttack();
                        } else if (!RangedAttackNotNeeded) {
                            RunRangedAttack();
                        } else {
                            //Debug.LogError("WARNING. BOSS FAILED TO RUN AN ATTACK: Melee test");
                        }
                    } else {
                        TestForSpin(RangedAttackNotNeeded, SpinAttackNotNeeded);
                    }
                } else {
                    IsWithinLeeWayRange(MeleeAttackRange * MeleeAttackLeeWay4Stages[CurrentStage - 1]);
                }
            } else {
                TestForSpin(RangedAttackNotNeeded, SpinAttackNotNeeded);
            }
        }
        else if (currentState != State.Dying)
        {
            Movement();
        }	
	}
	private void RunMeleeAttack(){
        //Debug.LogError("Melee attack");
        StartCoroutine(MeleeAttackCoroutine());

        MeleeAttackTimeSinceLastUseCounter = 0;
        RangedAttackTimeSinceLastUseCounter++;
        SpinAttackTimeSinceLastUseCounter++;

		MeleeAttackCounter++;
		RangedAttackCounter--;
		if(CurrentStage<3){
			SpinAttackCounter-=2;
		}else{
			SpinAttackCounter--;
		}

        if (SpinAttackCounter < 0)
            SpinAttackCounter = 0;
        if (RangedAttackCounter < 0)
            RangedAttackCounter = 0;
    }
	private void TestForSpin(bool RangedAttackNotNeeded, bool SpinAttackNotNeeded){
        bool MeleeAttackNotNeeded = !(MeleeAttackTimeSinceLastUseCounter > SpinAttackCounter + RangedAttackCounter && MeleeAttackCounter < MeleeMaxNoAttackInARow);
        if (SpinAttackCounter < SpinMaxNoAttackInARow && CurrentStage>=2){
			if(attackGapTimer <= 0){
				if(Vector3.Distance(transform.position,PlayerTransform.position)<=SpinAttackRange){
					if(CurrentStage<3 || (MeleeAttackNotNeeded && RangedAttackNotNeeded)){
                        //Debug.Log("Spin 2 MANN = "+MeleeAttackNotNeeded+" Current stage 3 or greater. RANN = " + RangedAttackNotNeeded);
                        RunSpinAttack();
					} else if(!MeleeAttackNotNeeded && currentState!=State.Dying){
						Movement();
					}else if(!RangedAttackNotNeeded){
						RunRangedAttack();
					}else{
						//Debug.LogError("WARNING. BOSS FAILED TO RUN AN ATTACK: Spin");
					}
				}else{
					TestForRanged(MeleeAttackNotNeeded, SpinAttackNotNeeded);
				}
			} else{
				IsWithinLeeWayRange(SpinAttackRange);
			}
		}else{
			TestForRanged(MeleeAttackNotNeeded, SpinAttackNotNeeded);
		}
	}
	private void RunSpinAttack(){
        //Debug.LogError("Spin attack");
        StartCoroutine(SpinAttackCoroutine());

        MeleeAttackTimeSinceLastUseCounter++;
        RangedAttackTimeSinceLastUseCounter++;
        SpinAttackTimeSinceLastUseCounter=0;

        SpinAttackCounter++;
		RangedAttackCounter--;
		if(CurrentStage<3){
			MeleeAttackCounter-=2;
		}else{
			MeleeAttackCounter--;
		}
        if (RangedAttackCounter < 0)
            RangedAttackCounter = 0;

        if (MeleeAttackCounter < 0)
            MeleeAttackCounter = 0;
    }
	private void TestForRanged(bool MeleeAttackNotNeeded, bool SpinAttackNotNeeded){
		if(RangedAttackCounter < RangedMaxNoAttackInARow && CurrentStage>=3){
			if(attackGapTimer <= 0){
                if (Vector3.Distance(transform.position,PlayerTransform.position)<=RangedAttackRange && MeleeAttackNotNeeded && SpinAttackNotNeeded){
					RunRangedAttack();
				}else if (currentState != State.Dying){
					Movement();
				}
			} else{
				if(RangedAttackLeeWay2Stages.Length>=CurrentStage-2)//May need to be -3 OR -4
					IsWithinLeeWayRange(RangedAttackRange*RangedAttackLeeWay2Stages[CurrentStage-3]);
				else
					IsWithinLeeWayRange(0f);
			}
		}
		else if (currentState != State.Dying){
			Movement();
		}
	}
	private void RunRangedAttack(){
        //Debug.LogError("Ranged attack");
		StartCoroutine(RangedAttackCoroutine());

        MeleeAttackTimeSinceLastUseCounter++;
        RangedAttackTimeSinceLastUseCounter=0;
        SpinAttackTimeSinceLastUseCounter++;

        RangedAttackCounter++;
		SpinAttackCounter--;
        if (SpinAttackCounter < 0)
            SpinAttackCounter = 0;
		MeleeAttackCounter--;
        if (MeleeAttackCounter < 0)
            MeleeAttackCounter = 0;
    }
	private void IsWithinLeeWayRange(float distanceCheck){
		if(Vector3.Distance(transform.position,PlayerTransform.position)<distanceCheck){
			agent.Stop();
            EnemyAnimator.SetBool(HashMoving, false);
		}else if (currentState != State.Dying){
			Movement();
		}
	}

	
	//called each update (see update function)
	public void WhileAttackingTest (float compensation)
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


	#region Attacks
	IEnumerator MeleeAttackCoroutine ()
	{
		currentState=State.Attack;
		agent.Stop ();
		#region Stage 1: attack incoming
		#if UNITY_EDITOR
		GizmoDisplayText="Attack Stage 1";
		#endif

		//Debug.LogError ("Attack 1");
		//Make the unit face the player
		transform.rotation = Quaternion.LookRotation (new Vector3(PlayerTransform.position.x,transform.position.y,PlayerTransform.position.z) - transform.position);
		
		//animation
		if (EnemyAnimator != null) {
            EnemyAnimator.SetInteger(HashAttackTypeMSR,0);
			EnemyAnimator.SetBool (HashMoving, false);
			EnemyAnimator.SetTrigger (HashAttack);
		}
		
		
		//stop the timer for the delay inbetween attacks from going down.
		publicattackAnimationRunning = true;



		//yield return new WaitForSeconds (meleeCollisionTime);
		float AttackDuration=MeleeAttackDuration4Stages[CurrentStage-1];
		float timer=meleeCollisionTime*AttackDuration;
		while(timer>0){
			timer-=Time.deltaTime;
			//Debug.Log("Player point is: "+Quaternion.LookRotation(PlayerTransform.position - transform.position)+". enemy is facing: "+transform.rotation);
			//Debug.Log("Rotating to " +Quaternion.LookRotation(new Vector3(PlayerTransform.position.x,transform.position.y,PlayerTransform.position.z) - transform.position));
			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation(new Vector3(PlayerTransform.position.x,transform.position.y,PlayerTransform.position.z) - transform.position),Time.deltaTime*MeleeTurnRate4Stages[CurrentStage-1]);

			yield return null;
		}
		#endregion
		
		
		#region Stage 2: Damage if conditions are met
		//		Debug.LogError("Attack 2");
		//If the enemy is within hit range. Damage the player (see the Player's control script)
		//		if (Vector3.Distance (transform.position, PlayerTransform.position) <= MeleeAttackRange + 1) {
		//			PlayerControl PlayerCT = PlayerTransform.gameObject.GetComponent<PlayerControl> ();
		//			PlayerCT.Damage (1f);
		//			PlayerCT.Knockback (IEknockbacktime, IEknockbackDistance, transform.position);
		//		}
		//		if(AttackBox.ReturnObjectList().Count!=0)
		foreach (GameObject currentObj in MeleeAttackBox.ReturnObjectList()) {
			if (currentObj.tag == "Player") {
				PlayerControl thisObjectsPlayerScript = currentObj.GetComponent<PlayerControl> ();
				thisObjectsPlayerScript.DamageAndAttack (MeleeAttackDamage4Stages[CurrentStage-1], MeleeknockbackDurationOnPlayer4Stages[CurrentStage-1], MeleeknockbackDistanceOnPlayer4Stages[CurrentStage-1], transform.position);
				break;
			}
		}

		#if UNITY_EDITOR
		GizmoDisplayText="Attack Stage 2";
		#endif

		//Reset the timer which puts a gap between unit attacks.
		CSEnemyResetattackGapTimer ();
		#endregion


		#region Stage 3: wait for remaining animation to play
		//Debug.LogError("Attack 3");
		//If the attackDuration is less than the point in time where the damage is dealt. wait for (see if true)
		if (AttackDuration - meleeCollisionTime < 0)
			//true
			//wait for the duration of attackDuration
			yield return new WaitForSeconds (AttackDuration);
		else
			//false
			//wait for the difference between attackDuration and collisionTime
			yield return new WaitForSeconds (AttackDuration - meleeCollisionTime);
		#if UNITY_EDITOR
		GizmoDisplayText="Attack Stage 3";
		#endif
		#endregion
		
		
		#region Stage 4: resume chasing the player and the likes.
		//	Debug.LogError("Attack 4");
		//the timer for gaps between attacks can start going down again
		publicattackAnimationRunning = false;


		//Resetting the timer for inbetween attack
		attackGapTimer=attackGapsDuration4Stages[CurrentStage-1];


		#if UNITY_EDITOR
		GizmoDisplayText="Attack Stage 4 End";
		#endif

		//reengage movement. Investigate changing this, to see if the enemy can just stand on the spot while waiting before reinitiating the attack
		Movement ();

		#endregion

	}

	IEnumerator SpinAttackCoroutine(){
		currentState=State.Attack;
		agent.Stop ();

		EnemyAnimator.SetInteger(HashAttackTypeMSR,1);
		EnemyAnimator.SetBool(HashSpinning,true);
        EnemyAnimator.SetBool(HashMoving, false);
		EnemyAnimator.SetTrigger (HashAttack);


		//stop the timer for the delay inbetween attacks from going down.
		publicattackAnimationRunning = true;

		yield return new WaitForSeconds(SpinTimeToBeginSpinning);

		agent.Resume();
		agent.speed=SpinMovementSpeed3Stages[CurrentStage-2];

		//float HoldSpinDuration=SpinDuration3Stages[CurrentStage-2];
		float RemainingSpinTime;
		if(SpinDuration3Stages.Length>=CurrentStage-1)
			RemainingSpinTime=SpinDuration3Stages[CurrentStage-2];
		else
			RemainingSpinTime=SpinDuration3Stages[0];
		float previoustime=Time.time;
		bool HasHitPlayer=false;
		float HitPlayerCoolDown=0f;
		while(RemainingSpinTime>0){
			float TimeDifference=Time.time-previoustime;
            previoustime = Time.time;
            RemainingSpinTime -=TimeDifference;

            //making sure to follow the player
            if (agent.destination != PlayerTransform.position)
                agent.destination = PlayerTransform.position;

			if(!HasHitPlayer){
				foreach (GameObject currentObj in SpinAttackBox.ReturnObjectList()) {
					if (currentObj.tag == "Player") {
						PlayerControl thisObjectsPlayerScript = currentObj.GetComponent<PlayerControl> ();
						thisObjectsPlayerScript.DamageAndAttack (SpinAttackDamage3Stages[CurrentStage-2], SpinknockbackDurationOnPlayer3Stages[CurrentStage-2], SpinknockbackDistanceOnPlayer3Stages[CurrentStage-2], transform.position);
						HasHitPlayer=true;
						HitPlayerCoolDown=SpinknockbackDurationOnPlayer3Stages[CurrentStage-2]+SpinPlayerHitExtraCoolDown;
                        //Debug.LogError("Hit player");
						break;
					}
				}
			}else{
				HitPlayerCoolDown-=TimeDifference;
				if(HitPlayerCoolDown<=0){
					HasHitPlayer=false;
				}
			}
           // Debug.Log("SPINNING SPINNING SPINNING");
			yield return null;
		}

        //leaving the spin (wind down animation)
		agent.Stop ();
		if (EnemyAnimator != null)
			EnemyAnimator.SetBool (HashSpinning, false);
		yield return new WaitForSeconds (1.467f);

		//end attack
		//returning to normal movement speed
		agent.speed=moveSpeed4Stages[CurrentStage-1];

		publicattackAnimationRunning = false;

		//Resetting the timer for inbetween attack
		attackGapTimer=attackGapsDuration4Stages[CurrentStage-1];

		//reengage movement. Investigate changing this, to see if the enemy can just stand on the spot while waiting before reinitiating the attack
		Movement ();
		
		//animation


	}
	IEnumerator RangedAttackCoroutine(){
		//stop the timer for the delay inbetween attacks from going down.
		publicattackAnimationRunning = true;

		currentState=State.Attack;
		agent.Stop ();

		//animation
		EnemyAnimator.SetInteger (HashAttackTypeMSR, 2);
		EnemyAnimator.SetTrigger (HashAttack);
        EnemyAnimator.SetBool(HashMoving, false);

		float timer=RangedDurationTillProjectileFire2Stages[CurrentStage-3];
		float previousTime=Time.time;
		while(timer>0){
			timer-=Time.time-previousTime;
			previousTime=Time.time;
			//Debug.Log("Player point is: "+Quaternion.LookRotation(PlayerTransform.position - transform.position)+". enemy is facing: "+transform.rotation);
			//Debug.Log("Rotating to " +Quaternion.LookRotation(new Vector3(PlayerTransform.position.x,transform.position.y,PlayerTransform.position.z) - transform.position));
			transform.rotation = Quaternion.LookRotation(new Vector3(PlayerTransform.position.x,transform.position.y,PlayerTransform.position.z) - transform.position);
			//The above line is not working at all it seems?
			yield return null;
		}

		//FIRE!
		GameObject spawnedPrefab = (GameObject)Instantiate(RangedPrefabToSpawn,RangedSpawnPoint.position,RangedSpawnPoint.rotation);
		ArrowScript currentPrefabAS=spawnedPrefab.GetComponent<ArrowScript>();
		if(currentPrefabAS!=null){
			currentPrefabAS.SetDamageAndKnockback(RangedAttackDamage2Stages[CurrentStage-3],RangedKnockbackDuration2Stages[CurrentStage-3],RangedKnockbackDistance2Stages[CurrentStage-3]);
			currentPrefabAS.ResetFlight(RangedTravelSpeed2Stages[CurrentStage-3],RangedAttackRange);
		}

		yield return new WaitForSeconds(RangedTimeToReturnToNeutral2Stages[CurrentStage-3]);

		//end attack
		publicattackAnimationRunning = false;
		
		//Resetting the timer for inbetween attack
		attackGapTimer=attackGapsDuration4Stages[CurrentStage-1];
		
		//reengage movement. Investigate changing this, to see if the enemy can just stand on the spot while waiting before reinitiating the attack
		Movement ();
	}
	#endregion
}