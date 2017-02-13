using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class VictoryScreen : MonoBehaviour {
	public Text textDigits;
	private GameStats gStats;
	public Button nextLevelButton;
	void Start () 
	{
		gStats = GameController.instance.gameStats;
		textDigits.text = gStats.counter.ToString("F2");
		if(nextLevelButton != null)
			nextLevelButton.onClick.AddListener(() => GoNextLevel());
	}
	void GoNextLevel()
	{
//		int lvlInt = Application.lo
//		Application.levelCount
		if(Application.loadedLevel != Application.levelCount)
			Application.LoadLevel(Application.loadedLevel+1);
		else
			Application.LoadLevel(Application.loadedLevel);

	}

}
