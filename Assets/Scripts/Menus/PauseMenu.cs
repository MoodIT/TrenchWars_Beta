using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PauseMenu : MonoBehaviour{
	public GameObject restartButton;
	public GameObject mainMenuButton;
	public GameObject resumeButton;
	public GameObject shadowToggle;
	private Toggle t;
	private UIControl uiCon;
	bool toggled;
	void Start () 
	{
		uiCon = GameController.instance.uiControls;
		restartButton.GetComponent<Button>().onClick.AddListener(() => Application.LoadLevel(Application.loadedLevel));
		restartButton.GetComponent<Button>().onClick.AddListener(() => GameController.instance.uiControls.pauseButton.GetComponent<Pause>().PauseTime(false, false));
		mainMenuButton.GetComponent<Button>().onClick.AddListener(() => GameController.instance.uiControls.pauseButton.GetComponent<Pause>().PauseTime(false, false));
		mainMenuButton.GetComponent<Button>().onClick.AddListener(() => Application.LoadLevel("MainMenu"));
		resumeButton.GetComponent<Button>().onClick.AddListener(() => GameController.instance.uiControls.pauseButton.GetComponent<Pause>().PauseTime(false, false));
				t = shadowToggle.GetComponent<Toggle>();
	}

	void Update()
	{
		if(t.isOn && !toggled)
		{
			ShadowToggle(true);
			toggled= true;
		}
		if(!t.isOn && toggled)
		{
			ShadowToggle(false);
			toggled= false;
		}

	}
	void ShadowToggle(bool onOff)
	{
		Debug.Log("GEU");
		if(onOff)
			QualitySettings.SetQualityLevel(6);
		else
			QualitySettings.SetQualityLevel(0);
	}
//	public void OnPointerClick (PointerEventData eventData) 
//	{
//		Debug.Log("GEU");
//		if(shadowToggle.GetComponent<Toggle>().isOn)
//			QualitySettings.SetQualityLevel(6);
//		else
//			QualitySettings.SetQualityLevel(0);
//	}
}
