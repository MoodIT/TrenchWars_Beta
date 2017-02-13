using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class LevelChange : MonoBehaviour {

//	// Use this for initialization
	public string[] scenes;
	void Start () 
	{
//		private void Reset()
//		{
//			scenes = ReadNames();
		if(scenes.Length != 0)
		{
			for(int i = 0; i < scenes.Length; i++)
			{
				GameObject g = Instantiate(GameController.instance.gameSettings.goToLevelButton, GameController.instance.gameSettings.goToLevelButton.transform.position, Quaternion.identity) as GameObject;
				g.transform.SetParent(this.transform);
	//			g.transform.position = g.transform.position + new Vector3(i*10, 0, 0);
				g.GetComponentInChildren<Text>().text = scenes[i];

				g.GetComponent<RectTransform>().anchoredPosition = GameController.instance.gameSettings.goToLevelButton.GetComponent<RectTransform>().anchoredPosition;
				//			foreach(Block b in blockControls.selectedBlocks)
				//			{
				string str = scenes[i];
				g.GetComponent<Button>().onClick.AddListener(() => GameController.instance.uiControls.pauseButton.GetComponent<Pause>().PauseTime(false,false));
				g.GetComponent<Button>().onClick.AddListener(() => Application.LoadLevel(str));
				//			}
				RectTransform rT = g.GetComponent<RectTransform> ();
				RectTransform rTPrefab = GameController.instance.gameSettings.goToLevelButton.GetComponent<RectTransform> ();
				rT.sizeDelta = rTPrefab.sizeDelta;
				rT.anchoredPosition = rTPrefab.anchoredPosition + new Vector2(i* rTPrefab.sizeDelta.x + 10, 0);
				rT.localScale = rTPrefab.localScale;
			}
		}
//		}
	}

//	string[] ReadNames()
//	{
//		List<string> temp = new List<string>();
//		foreach(UnityEditor.EditorBuildSettingsScene s in UnityEditor.EditorBuildSettings.scenes)
//		{
//			string name = s.path.Substring(s.path.LastIndexOf('/')+1);
//			name = name.Substring(0,name.Length-6);
//			if(name != "MainMenu")
//			{
//				temp.Add(name);
//			}
//		}
//		return temp.ToArray();
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}

//	void 
}
