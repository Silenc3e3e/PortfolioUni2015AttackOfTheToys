using UnityEngine;
public class WeaponClass
{
	public int ID;
	public string Name;
	public float damage;
	public float KnockbackDistance;
	public float KnockbackDuration;
	public StoreObjectsInCollider AttackAreaScript;
	public GameObject AttackArea;
	[Range(0.0f,1.0f)]
	public float StrikeTime;
	public float AttackDuration;
	public float RangedProjectileSpeed;
	public float RangedProjectileDistance;
    public GameObject DisplayIcon;
    public MeshRenderer WeaponMesh;
    public SkinnedMeshRenderer WeaponMeshSkin;
}