using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CommandPointsControl : MonoBehaviour {

	private GameStats gStats;
	private BlockControl bControl;
	private UIControl uICon;
	private UnitController uCon;
	private SelectBlocks sBlocks;
	public float cooldown = 5f;

	[HideInInspector]
	public Vector3 curVel;

	public AnimationCurve aCurve;
	void Start () 
	{
		uCon = GameController.instance.unitControls;
		uICon = GameController.instance.uiControls;
		bControl = GameController.instance.blockController.GetComponent<BlockControl> ();
		gStats = GetComponent<GameStats> ();
		sBlocks = GameController.instance.blockController.GetComponent<SelectBlocks> ();
		StartCoroutine (SpawnCommandPoint ());
		UpdateCommandPoints ();
	}
	
//	void Update () 
//	{
//	
//	}

	IEnumerator SpawnCommandPoint()
	{
		int widthRnd = Random.Range(0, bControl.width-2);
		int lengthRnd = Random.Range(0,bControl.length); //Debug.Log (17 - lengthRnd);
		Instantiate (GameController.instance.gameSettings.commandPointPrefab1,new Vector3(widthRnd, 9.5f-(lengthRnd), lengthRnd) , Quaternion.identity); //new Vector3 (bControl.interactableBlockObjs1D [Random.Range (0, bControl.interactableBlockStates1D.Length)].transform.position.x, 17, bControl.interactableBlockObjs1D [Random.Range (0, bControl.interactableBlockStates1D.Length)].transform.position.z)
		yield return new WaitForSeconds(cooldown);
		StartCoroutine (SpawnCommandPoint ());
	}

	public void UpdateCommandPoints()
	{
		GameController.instance.uiControls.commandPointTextC.text = "" + gStats.commandPoints;


//		sBlocks.digCostText = GameController.instance.uiControls.digButton.transform.Find("Cost").GetComponent<Text>();
//		
//		sBlocks.digCostText.text = "Cost: " + sBlocks.expectedCost;



		if(gStats.commandPoints < GameController.instance.gameSettings.gunnerCost || GameController.instance.gameSettings.maxGunners == uCon.gunnerAmt)
//			uICon.spawnGunnerObj.GetComponent<Image>().color = Color.red;
			uICon.spawnGunnerObj.GetComponent<Image>().material = GameController.instance.gameSettings.greyscale;
		else
//			uICon.spawnGunnerObj.GetComponent<Image>().color = Color.white;
			uICon.spawnGunnerObj.GetComponent<Image>().material = GameController.instance.gameSettings.coloredSprite;

		if(gStats.commandPoints < GameController.instance.gameSettings.flamerCost || GameController.instance.gameSettings.maxFlamers == uCon.flamerAmt)
			uICon.spawnFlamerObj.GetComponent<Image>().material = GameController.instance.gameSettings.greyscale;
		else
			uICon.spawnFlamerObj.GetComponent<Image>().material = GameController.instance.gameSettings.coloredSprite;

		if(gStats.commandPoints < GameController.instance.gameSettings.grenaderCost || GameController.instance.gameSettings.maxGrenaders == uCon.grenaderAmt)
			uICon.spawnGrenaderObj.GetComponent<Image>().material = GameController.instance.gameSettings.greyscale;
		else
			uICon.spawnGrenaderObj.GetComponent<Image>().material = GameController.instance.gameSettings.coloredSprite;

		if (gStats.commandPoints < sBlocks.expectedCost)
			uICon.digButton.GetComponent<Image>().material = GameController.instance.gameSettings.greyscale;
		else
			uICon.digButton.GetComponent<Image>().material = GameController.instance.gameSettings.coloredSprite;
	}

	public void PointsFlyUp(Vector3 fromPos, float points)
	{
		StartCoroutine(MoveIcon(InstPointImage(), Camera.main.WorldToScreenPoint (fromPos), points));
	}

	IEnumerator MoveIcon(GameObject g, Vector3 fromPos, float points)
	{
		float mTime = 0;
		bool onOff = true;
		g.transform.position = fromPos;
		g.GetComponent<Text>().text = ""+points;
		Vector3 sScale = g.transform.localScale;
//		Vector3 startPos = fromPos;
		while(onOff)
		{
			if(mTime < 0.5f)
			{
				mTime += Time.deltaTime;
				g.transform.position = Vector3.Lerp(fromPos, GameController.instance.uiControls.commandPointTextC.transform.position,aCurve.Evaluate(mTime/0.5f));
				g.transform.localScale = Vector3.Lerp(sScale, new Vector3(0.2f,0.2f,0.2f),aCurve.Evaluate(mTime/0.5f));
//				g.transform.position = Vector3.SmoothDamp(g.transform.position, GameController.instance.uiControls.commandPointTextC.transform.position, ref curVel, 1);
			}
			else
			{
				HitPointsEffect(g, points);
				yield break;
			}
			yield return null;
		}
	}
	void HitPointsEffect(GameObject g, float points)
	{
		gStats.commandPoints += points;
		UpdateCommandPoints();
		Destroy (g);
	}
	GameObject InstPointImage()
	{
		GameObject g = Instantiate(GameController.instance.gameSettings.pointGainText, Vector3.zero, Quaternion.identity) as GameObject;
		g.transform.SetParent(GameController.instance.uiCanvas.transform);
		return g;
	}
}
