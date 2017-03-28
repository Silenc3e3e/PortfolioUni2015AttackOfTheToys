using UnityEngine;
using System.Collections.Generic;

public class OnDestroyHolla : MonoBehaviour {

    private Spawner MyMaster;
	private int Type;

	public void Reference(Spawner SendReference, int ThisType)
    {
        MyMaster = SendReference;
		Type=ThisType;
    }

    void OnDestroy()
    {
		MyMaster.NotifyUnitDestroyed(gameObject, Type);
    }
}
