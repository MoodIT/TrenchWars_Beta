using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Timeline : MonoBehaviour {

	private GameStats gameStats;
	private EnemySpawning enmSpawning;

	private Slider slider;
	public Text counterText;

	private float timeLineCounter;
	void Start () 
	{
		counterText = GameObject.Find("Counter").GetComponent<Text>();
		enmSpawning = GameController.instance.enemySpawner;
		gameStats = GameController.instance.gameStats;
		slider = GetComponent<Slider> ();
		slider.maxValue = enmSpawning.lengthInSeconds;
		foreach(Wave n in enmSpawning.waves)
		{
//			Debug.Log(n.spawnTime);
			GameObject eIcon = Instantiate(GameController.instance.gameSettings.enemyTimelineIcon, Vector3.zero, Quaternion.identity) as GameObject;
			eIcon.transform.SetParent(slider.gameObject.transform);

//			eIcon.transform.localPosition = slider.transform.localPosition;
			RectTransform rT = eIcon.GetComponent<RectTransform> ();
			RectTransform rTPrefab = GameController.instance.gameSettings.enemyTimelineIcon.GetComponent<RectTransform> ();
			rT.sizeDelta = rTPrefab.sizeDelta;
			rT.anchoredPosition = rTPrefab.anchoredPosition;
			rT.localScale = rTPrefab.localScale;

			float xPos = (slider.GetComponent<RectTransform>().rect.width / enmSpawning.lengthInSeconds) * n.time;
			eIcon.transform.localPosition = GameController.instance.gameSettings.enemyTimelineIcon.transform.localPosition  - new Vector3(slider.GetComponent<RectTransform>().rect.width/2, 0, 0) + new Vector3(xPos, 0,0);
		}
		timeLineCounter =enmSpawning.lengthInSeconds;

//		if(enmSpawning.defOrCap == EnemySpawning.DefOrCap.CAPTURE)
//		{
//
//		}
	}
	
	void Update () 
	{
//		foreach(Wave n in enmSpawning.waves)
//        {
//			if(n.time 
//		}
		if(timeLineCounter > 0 && enmSpawning.defOrCap == EnemySpawning.DefOrCap.CAPTURE)
		{
			timeLineCounter -= Time.deltaTime;
			counterText.text = "TIME:" +  timeLineCounter.ToString("F2");
		}
		else
		{
			counterText.text = "TIME:" +  gameStats.counter.ToString("F2");
		}
		slider.value = enmSpawning.counter;
	}

}
