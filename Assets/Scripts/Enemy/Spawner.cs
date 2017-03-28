using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    private List<GameObject> MeleeUnits = new List<GameObject>();
    private List<GameObject> RangedUnits = new List<GameObject>();
    private List<GameObject> SpinUnits = new List<GameObject>();

	public GameObject MeleeUnit;
	public GameObject RangedUnit;
	public GameObject SpinUnit;

    public Transform[] SpawnLocations;
    
    public int CurrentStage = 0;

    public float[] TimerLengthBetweenSpawns;
    [Range(1,20)]
	public int ThresholdCount=2;
    private int NoMeleeUnitsSpawned;
	public  int[] NoRangedUnitsToSpawn;
	private  int NoRangedUnitsSpawned;
	public int[] NoMeleeUnitsToSpawn;
    public int[] NoSpinUnitsToSpawn;
    private int NoSpinUnitsSpawned;
    private float Timer;
    public bool NeverZeroUnits;
    public bool ChangeAfterTime = false;
    public float Intervals;
    public int maxStages;
    private float IntervalTimer;
    private bool SpawnerIsActive = true;

    void Start()
    {
        IntervalTimer = Intervals;
    }

    public void IncreaseCurrentStage()
    {
        if (SpawnerIsActive)
        {
            CurrentStage++;
        }
    }

    public void BossIsDead()
    {
        SpawnerIsActive = false;
        Debug.Log("Boss is Dead () called");
        foreach (GameObject current in MeleeUnits)
        {
            if (current != null)
            {
                Debug.Log("Melee unit found");
                current.GetComponent<Enemy>().SpawnerDeath();
            }
        }
        foreach (GameObject current in RangedUnits)
        {
            if (current != null)
            {
                Debug.Log("ranged unit found");
                current.GetComponent<Enemy>().SpawnerDeath();
            }
        }
        foreach (GameObject current in SpinUnits)
        {
            if (current != null)
            {
                Debug.Log("spin unit found");
                current.GetComponent<Enemy>().SpawnerDeath();
            }
        }
    }

    public void NotifyUnitDestroyed(GameObject DestroyedOBJ, int type)
    {
        if (SpawnerIsActive)
        {
            if (NoMeleeUnitsSpawned >= NoMeleeUnitsToSpawn[CurrentStage] && NoRangedUnitsSpawned >= NoRangedUnitsToSpawn[CurrentStage] && NoSpinUnitsSpawned >= NoSpinUnitsToSpawn[CurrentStage])
                Timer = TimerLengthBetweenSpawns[CurrentStage];

            if (type == 1)
            {
                MeleeUnits.Remove(DestroyedOBJ);
                NoMeleeUnitsSpawned--;
            }
            else if (type == 2)
            {
                RangedUnits.Remove(DestroyedOBJ);
                NoRangedUnitsSpawned--;
            }
            else if (type == 3)
            {
                SpinUnits.Remove(DestroyedOBJ);
                NoSpinUnitsSpawned--;
            }
            else
            {
                Debug.LogWarning("Major error: NUD sent int type <>1,2,3. Ref Spawner script");
            }
        }
    }

    private float previousTime;
	private bool MeleeNotAtMax;
	private bool RangedNotAtMax;
	private bool SpinNotAtMax;
	void Update()
    {
        if (SpawnerIsActive)
        {
            //Debug.Log(SpawnLocations.Length);
            MeleeNotAtMax = NoMeleeUnitsSpawned < NoMeleeUnitsToSpawn[CurrentStage];
            RangedNotAtMax = NoRangedUnitsSpawned < NoRangedUnitsToSpawn[CurrentStage];
            SpinNotAtMax = NoSpinUnitsSpawned < NoSpinUnitsToSpawn[CurrentStage];
            if (((Timer > 0) && (MeleeNotAtMax || RangedNotAtMax || SpinNotAtMax)) || (NeverZeroUnits && MeleeNotAtMax && RangedNotAtMax && SpinNotAtMax))
            {
                Timer -= Time.time - previousTime;
            }
            else
            {
                bool UseMelee = (MeleeNotAtMax && (NoMeleeUnitsSpawned < NoRangedUnitsSpawned + ThresholdCount && NoMeleeUnitsSpawned < NoSpinUnitsSpawned + ThresholdCount));
                bool UseSpin = (SpinNotAtMax && (NoSpinUnitsSpawned < NoRangedUnitsSpawned + ThresholdCount && NoSpinUnitsSpawned < NoMeleeUnitsSpawned + ThresholdCount));
                bool UseRanged = (RangedNotAtMax && (NoRangedUnitsSpawned < NoMeleeUnitsSpawned + ThresholdCount && NoRangedUnitsSpawned < NoSpinUnitsSpawned + ThresholdCount));
                Debug.Log(UseMelee + "  " + UseSpin + "  " + UseRanged);
                RandomSpawner(UseMelee, UseSpin, UseRanged);
            }

            if (ChangeAfterTime)
            {
                if (IntervalTimer > 0)
                    IntervalTimer -= Time.time - previousTime;
                else if (CurrentStage + 1 < maxStages)
                {
                    CurrentStage++;
                    IntervalTimer = Intervals;
                }
                else
                {
                    ChangeAfterTime = false;
                }
            }

            previousTime = Time.time;
        }
    }

	private void RandomSpawner(bool MeleeSpawn, bool SpinSpawn, bool RangedSpawn)
	{
        if (SpawnerIsActive)
        {
            bool allfalse = !MeleeSpawn && !SpinSpawn && !RangedSpawn;
            List<int> Units = new List<int>();
            if (MeleeSpawn || (MeleeNotAtMax && allfalse))
                Units.Add(1);
            if (SpinSpawn || (SpinNotAtMax && allfalse))
                Units.Add(2);
            if (RangedSpawn || (SpinNotAtMax && allfalse))
                Units.Add(3);

            Debug.Log(Units.Count);
            if (Units.Count > 0)
            {
                int unitToSpawn = Units[Random.Range(0, Units.Count)];
                if (unitToSpawn == 1)
                {
                    SpawnMelee();
                }
                else if (unitToSpawn == 2)
                {
                    SpawnSpin();
                }
                else
                {
                    SpawnRanged();
                }
                Timer = TimerLengthBetweenSpawns[CurrentStage];
            }
        }
	}

	private void SpawnMelee(){
		NoMeleeUnitsSpawned++;
		GameObject spawned = (GameObject)Instantiate(MeleeUnit, SpawnLocations[Random.Range(0, SpawnLocations.Length)].position,transform.rotation);
		spawned.GetComponent<Enemy>().NotifyDeath(this);
        Timer = TimerLengthBetweenSpawns[CurrentStage];
        MeleeUnits.Add(spawned);
	}
	private void SpawnSpin(){
		NoSpinUnitsSpawned++;
		GameObject spawned = (GameObject)Instantiate(SpinUnit,SpawnLocations[Random.Range(0,SpawnLocations.Length)].position,transform.rotation);
		spawned.GetComponent<Enemy>().NotifyDeath(this);
        Timer = TimerLengthBetweenSpawns[CurrentStage];
        RangedUnits.Add(spawned);
    }
	private void SpawnRanged(){
		NoRangedUnitsSpawned++;
		GameObject spawned = (GameObject)Instantiate(RangedUnit, SpawnLocations[Random.Range(0, SpawnLocations.Length)].position,transform.rotation);
		spawned.GetComponent<Enemy>().NotifyDeath(this);
        Timer = TimerLengthBetweenSpawns[CurrentStage];
        SpinUnits.Add(spawned);
    }

}

