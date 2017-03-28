using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScreenSpaceCanvasManager : MonoBehaviour 
{
	List<DepthUI> panels = new List<DepthUI>();

	void Awake () 
	{
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width,Screen.height);
		panels.Clear();
	}
	
	void Update () 
	{
		Sort();
	}
	
	public void AddToCanvas(GameObject objectToAdd)
	{
		panels.Add(objectToAdd.GetComponent<DepthUI>());
	}

	void Sort()
	{
		panels.Sort((x, y) => x.depth.CompareTo(y.depth));
		for (int i = 0; i < panels.Count; i++)
		{
			if(panels[i]!=null)
				panels[i].transform.SetSiblingIndex(i);
			else
				panels.Remove(panels[i]);
		}
	}
}
