using UnityEngine;
using System.Collections;

public class SnapToObject : MonoBehaviour {

    public Transform Target;
	
	// Update is called once per frame
	void Update () {
        if (Target == null)
            Target = GameObject.Find("Player").transform;
        transform.position = Target.position;
	}
}
