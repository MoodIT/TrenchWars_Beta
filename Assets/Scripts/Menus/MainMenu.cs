using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class MainMenu : MonoBehaviour {
	public Button playGameButton;
//	public Button level1Button;
//	public Button level2Button;
//	public Button level3Button;
	public List<string> levels;
	public ScrollRect scrollRect;
	public GameObject levelButtonPref;
	// Use this for initialization
	void Start () {
		playGameButton.onClick.AddListener(() => Play());
		if(levels.Count > 0)
		{
			levels.Reverse();
//			float tempSizeY = scrollRect.content.GetComponent<RectTransform>().sizeDelta.y;
//			scrollRect.content.GetComponent<RectTransform>().anchoredPosition = new Vector3(scrollRect.content.GetComponent<RectTransform>().anchoredPosition.x, 
			scrollRect.content.GetComponent<RectTransform>().sizeDelta = new Vector3(scrollRect.content.GetComponent<RectTransform>().sizeDelta.x, 40 * levels.Count);
			for(int i = 0; i < levels.Count; i++)
			{
				GameObject lB = Instantiate(levelButtonPref, Vector3.zero, Quaternion.identity) as GameObject;
				lB.name = "LButton" + i;
				Button tempButton = lB.GetComponent<Button>();
				int tempInt = i;
				tempButton.onClick.AddListener(() => LoadCLevel(levels[tempInt]));
//				lB.GetComponent<Button>().onClick.AddListener(() => LoadCLevel(levels[i]));
//				Debug.Log (i + " " + levels[i]);
				lB.GetComponentInChildren<Text>().text = "LEVEL " + levels[i].ToUpper();
				lB.transform.SetParent(scrollRect.content.transform);
				RectTransform rT = lB.GetComponent<RectTransform> ();
				RectTransform rTPrefab = levelButtonPref.GetComponent<RectTransform> ();
				rT.sizeDelta = rTPrefab.sizeDelta;
				rT.anchoredPosition = new Vector3(rTPrefab.anchoredPosition.x,  rTPrefab.anchoredPosition.y + (i*40));
				rT.localScale = rTPrefab.localScale;
				rT.localRotation = rTPrefab.localRotation;
//				lB.transform.position = levelButtonPref.transform.position;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadCLevel(string lvl)
	{
//		Debug.Log (lvl);
		Application.LoadLevel(lvl);
	}

	void Play()
	{
		Application.LoadLevel("1");
	}
}
