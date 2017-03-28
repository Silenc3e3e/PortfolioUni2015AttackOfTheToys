using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerControl))]
public class SwitchWeapons : MonoBehaviour {

	public PlayerControl PCScript;

	public WeaponClass[] Weapons;
	public GameObject HitBoxSword;
	public GameObject HitBoxSpear;
    public GameObject RangedArrows;
    public GameObject IconSword;
    public GameObject IconSpear;
    public GameObject IconBow;
    public MeshRenderer MeshSword;
    public MeshRenderer MeshSpear;
    public SkinnedMeshRenderer MeshBow;

    private int CurrentIDDisplaying;
	private GameObject CurrentUIWepShowing;
    private MeshRenderer CurrentMeshShowing;
    private SkinnedMeshRenderer CurrentSkinMeshShowing;

    public void Start(){
        if (GameObject.FindGameObjectWithTag("CanvasHUD") != null)
        {
            Button WepSwitchButton = GameObject.Find("WeaponSwitch").GetComponent<Button>();
            WepSwitchButton.onClick.AddListener(() => ChangeIDDisplayingBy(1));
        }
        else
        {
            Debug.LogError("MARJOR ERROR: Canvas Pause-Menu required. Ref: SwitchWeapons");
        }

       if(IconSword==null)
		    IconSword = GameObject.Find("Icon:Sword");
        if (IconSpear == null)
            IconSpear = GameObject.Find("Icon:Spear");
        if (IconBow == null)
            IconBow = GameObject.Find("Icon:Bow");
			

		WeaponClass Sword = new WeaponClass();
		Sword.ID = 1;
		Sword.damage = 13f;//13f. change when finished debugging
		Sword.KnockbackDistance=5f;
		Sword.KnockbackDuration = 2f;
		Sword.Name="Sword";
		Sword.AttackAreaScript = HitBoxSword.GetComponent<StoreObjectsInCollider>();
		Sword.AttackArea=HitBoxSword;
		Sword.StrikeTime = 0.75f;
		Sword.AttackDuration=.8333333f;
        Sword.DisplayIcon = IconSword;
        Sword.WeaponMesh = MeshSword;
		WeaponClass Spear= new WeaponClass();
		Spear.ID = 2;
		Spear.damage = 8f;
		Spear.KnockbackDistance=1.5f;
		Spear.KnockbackDuration = 3f;
		Spear.Name="Spear";
		Spear.AttackAreaScript = HitBoxSpear.GetComponent<StoreObjectsInCollider>();
		Spear.AttackArea=HitBoxSpear;
		Spear.StrikeTime = 0.583f;
		Spear.AttackDuration= 0.92909f;
        Spear.DisplayIcon = IconSpear;
        Spear.WeaponMesh = MeshSpear;
		WeaponClass Bow= new WeaponClass();
		Bow.ID = 3;
		Bow.damage = 10f;
		Bow.KnockbackDistance=3f;
		Bow.KnockbackDuration = .75f;
		Bow.Name="Bow";
		Bow.AttackAreaScript = null;
		Bow.AttackArea= RangedArrows;
		Bow.StrikeTime = 0.89f;//585
		Bow.AttackDuration= 1.04467f;
		Bow.RangedProjectileSpeed=10f;
		Bow.RangedProjectileDistance=15f;
        Bow.DisplayIcon = IconBow;
        Bow.WeaponMeshSkin = MeshBow;
		Weapons = new WeaponClass[]{Sword,Spear,Bow};

		for(int i=1;i<=Weapons.Length;i++){
			SwitchWeapon (i);
		}
		SwitchWeapon (1);
	}

	public void ChangeIDDisplayingBy(int Up){
        if (PCScript.MyState != PlayerControl.State.Action)
        {
            CurrentIDDisplaying += Up;
            //dealing with CIDD going beyond weapon list range.
            if (CurrentIDDisplaying < 1)
            {
                if (Weapons != null)
                    CurrentIDDisplaying = Weapons.Length;
                else
                    Debug.LogError("ERROR: Missing Weapons list!");
            }
            else if (CurrentIDDisplaying > Weapons.Length)
                CurrentIDDisplaying = 1;
            SwitchWeapon(CurrentIDDisplaying);
        }
	}

	public void SwitchWeapon(int WepID){
        if (PCScript.MyState != PlayerControl.State.Action)
        {
            foreach (WeaponClass current in Weapons)
            {
                if (current.ID == WepID)
                {
                    //				Debug.LogError(current.AttackArea.name);
                    PCScript.switchWeapons(current);
                    //change displaying icon
                    if (CurrentUIWepShowing != null)
                        CurrentUIWepShowing.SetActive(false);
                    CurrentUIWepShowing = current.DisplayIcon;
                    if (CurrentUIWepShowing != null)
                        CurrentUIWepShowing.SetActive(true);

                    //change display weapon mesh
                    if (CurrentMeshShowing != null)
                        CurrentMeshShowing.enabled = false;
                    if (CurrentSkinMeshShowing != null)
                        CurrentSkinMeshShowing.enabled = false;
                    CurrentMeshShowing = current.WeaponMesh;
                    CurrentSkinMeshShowing = current.WeaponMeshSkin;
                    if (CurrentMeshShowing != null)
                        CurrentMeshShowing.enabled = true;
                    if (CurrentSkinMeshShowing != null)
                        CurrentSkinMeshShowing.enabled = true;

                    break;
                }
            }
        }
	}
	void Update(){
		if(Input.GetButtonDown("ChangeWeapon") && PCScript.MyState != PlayerControl.State.Action)
        {
			//Debug.Log("CW current input is "+Input.GetAxisRaw("ChangeWeapon"));
			if(Input.GetAxisRaw("ChangeWeapon")>.5f)
				ChangeIDDisplayingBy(1);
			else if(Input.GetAxisRaw("ChangeWeapon")<-.5f)
				ChangeIDDisplayingBy(-1);
		}
	}

//	public void AddWeapon(){
//		WeaponClass newWep= new WeaponClass();
//		newWep.ID = Weapons.Count + 1;
//		Weapons.Add (newWep);
//	}
//	public void RemoveWeapon(WeaponClass WepToRemove){
//		//comment more and include Matt more!!!!!!
//		//Matt, get in here!
//
//
//		Weapons.Remove (WepToRemove);
//	}

}
