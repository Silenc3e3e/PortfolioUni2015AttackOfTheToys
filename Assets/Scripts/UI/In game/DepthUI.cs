using UnityEngine;
using System.Collections;

public class DepthUI : MonoBehaviour 
{
	public float depth;

	private CanvasGroup canvasGroup;
	
	void Start()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void SetAlpha(float alpha)
	{
		if (canvasGroup == null) {
			Destroy (gameObject);
			return;
		}
		canvasGroup.alpha = alpha;
		
		if (alpha <= 0)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
		}
	}
}
