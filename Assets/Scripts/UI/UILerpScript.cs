using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILerpScript : MonoBehaviour {

	public Button currentButton;

	private Vector3 startPosition;
	private Vector3 endPosition;

	public float yVal;
	private float alpha = 0.0f;
	public float speed;

	public string[] names;

	public void Start(){
		StartCoroutine (ScrollButtonRoutine ());
	}

	public IEnumerator ScrollButtonRoutine(){
		startPosition = currentButton.transform.position;
		endPosition = currentButton.transform.position;

		endPosition.y += yVal;

		while (alpha<1.0f) {
			transform.position = Vector3.Lerp (startPosition, endPosition, alpha);

			alpha+=Time.deltaTime*speed;
			yield return 0;
		}
	}

	public void showButtons(){
		//float start = 0.0f;
//		foreach (string current in names) {
//
//		}
	}
}
