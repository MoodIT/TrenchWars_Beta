using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine.UI;
[System.Serializable]
public class EnemySpawning : MonoBehaviour{

	public Dictionary<Enemy, float> enemies = new Dictionary<Enemy, float>();
//	[SerializeField]
	public List<Enemy> _enemyKeys = new List<Enemy> ();
	[SerializeField]
	public List <float> _values = new List<float>();
	[SerializeField]
	public List<Wave> waves = new List<Wave>();

	public List<GameObject> enemiesInGame = new List<GameObject> ();
	public BlockControl bControl;
	public float lengthInSeconds = 120;
	[HideInInspector]
	public float counter;
	public GameObject soldierEnemy;
	public bool loopingSpawn;

	public GameStats gameStats;

	[HideInInspector]
	public bool stopSpawn;

	public enum DefOrCap{CAPTURE, DEFEND};
	public DefOrCap defOrCap;
//	private List<Enemy.EnemyType> enemyTypes = new List<Enemy.EnemyType>(3);
	// Use this for initialization
	void Start () 
	{
		gameStats = GameController.instance.gameStats;
		bControl = GameController.instance.blockController.GetComponent<BlockControl>();
//		Enemy enm1 = new Enemy ("dude", Enemy.EnemyType.SOLDIER, new Block{width = 19, length = 5}, false, 10);
//		for (int i=0; i!= _enemyKeys.Count; i++) //Mathf.Min(_enemyKeys.Count,_values.Count)
//			enemies.Add(_enemyKeys[i],_values[i]);
//		enemies.Add (enm1, 5);
		StartCoroutine (CounterCheck ());
		StartCoroutine(StartText());
	}

	IEnumerator StartText()
	{
		yield return new WaitForSeconds(2);
//		GameObject 
//		
		GameObject cOSO = Instantiate(GameController.instance.gameSettings.centerOfScreenObj, Vector3.zero, Quaternion.identity) as GameObject;
		cOSO.transform.SetParent(GameController.instance.uiCanvas.transform);
		
		RectTransform rT = cOSO.GetComponent<RectTransform> ();
		RectTransform rTPrefab = GameController.instance.gameSettings.waveTextObj.GetComponent<RectTransform> ();
		rT.sizeDelta = rTPrefab.sizeDelta;
		rT.anchoredPosition = rTPrefab.anchoredPosition;
		rT.localScale = rTPrefab.localScale;
		if(defOrCap == DefOrCap.CAPTURE)
		{
			cOSO.GetComponent<Text>().text = "CAPTURE THE FLAG BEFORE TIME RUNS OUT!";
			yield return new WaitForSeconds(3.3f);
		}
		if(defOrCap == DefOrCap.DEFEND)
		{
			cOSO.GetComponent<Text>().text = "DEFEND AGAINST THE ENEMIES!";
			yield return new WaitForSeconds(2.5f);
		}
		//		iWG.transform.position = GameController.instance.gameSettings.waveTextObj.transform.position;

		cOSO.SetActive(false);
	}
	void Update () 
	{
		if(!stopSpawn)
			counter += Time.deltaTime;

		if(defOrCap == DefOrCap.CAPTURE && !gameStats.gameComplete)
		{
			if(counter >= lengthInSeconds)
			{
				gameStats.EndGame();
			}
		}
		if(defOrCap == DefOrCap.DEFEND && !gameStats.gameComplete)
		{
			if(enemiesInGame.Count == 0 && _enemyKeys[_enemyKeys.Count-1].spawnTime < counter)
			{
				gameStats.WinGame();
			}
		}

	}

	public void Reset()
	{
		counter = 0;
		foreach(Enemy e in _enemyKeys)
		{
			e.spawned = false;
		}
	}
	IEnumerator CounterCheck()
	{
		while(true)
		{
			yield return new WaitForSeconds (0.3f);
			
			foreach (Enemy item in _enemyKeys) 
			{
				if(item.spawnTime < counter && !item.spawned)
				{
					item.spawned = true;
					SpawnEnemy(item);

				}
			}

			foreach (Wave item in waves) 
			{
				if(item.time < counter && !item.spawned)
				{
					item.spawned = true;
//					SpawnEnemy(item);
					StartCoroutine(WaveIncoming());
					
				}
			}
		}
		
	}

	IEnumerator WaveIncoming()
	{
		GameObject iWG = Instantiate(GameController.instance.gameSettings.waveTextObj, Vector3.zero, Quaternion.identity) as GameObject;
		iWG.transform.SetParent(GameController.instance.uiCanvas.transform);

		RectTransform rT = iWG.GetComponent<RectTransform> ();
		RectTransform rTPrefab = GameController.instance.gameSettings.waveTextObj.GetComponent<RectTransform> ();
		rT.sizeDelta = rTPrefab.sizeDelta;
		rT.anchoredPosition = rTPrefab.anchoredPosition;
		rT.localScale = rTPrefab.localScale;
//		iWG.transform.position = GameController.instance.gameSettings.waveTextObj.transform.position;
		yield return new WaitForSeconds(1.5f);
		iWG.SetActive(false);
	}
	void SpawnEnemy(Enemy enm)
	{

		if(enm.spawnPos.length >= bControl.length)
		{
			enm.spawnPos.length = bControl.length-1;
		}
		GameObject spawnObj = null;
		if(enm.enemyType == Enemy.EnemyType.GUNNER)
		{
			spawnObj = GameController.instance.gameSettings.gunnerEnemy;
		}
		if(enm.enemyType == Enemy.EnemyType.FLAMETHROWER)
		{
			spawnObj = GameController.instance.gameSettings.flamethrowerEnemy;
		}
		if(enm.enemyType == Enemy.EnemyType.GRENADER)
		{
			spawnObj = GameController.instance.gameSettings.grenaderEnemy;
		}
//		GameObject enmObj = Instantiate (spawnObj, new Vector3 (bControl.width-2, 0, bControl.enemyBarracksBlockPos.length), Quaternion.identity) as GameObject;
		GameObject enmObj = Instantiate (spawnObj, new Vector3 ((int)enm.spawnPos.mWidth, 1, (int)enm.spawnPos.mLength), Quaternion.identity) as GameObject;
		enemiesInGame.Add(enmObj);
		EnemyStats enmSts = enmObj.GetComponent<EnemyStats> ();
		EnemyMovement enmMov = enmObj.GetComponent<EnemyMovement> ();
		enmMov.startBlockGoTo = new Block{ width = enm.spawnPos.width, length = enm.spawnPos.length};
//		enmMov.startBlock = new Block{ width = bControl.width-2, length = bControl.enemyBarracksBlockPos.length};
		enmMov.startBlock = new Block{ width = enm.spawnPos.mWidth, length = enm.spawnPos.mLength};
//		enmSts.health = enm.health;
	}
}

//[System.Serializable]
//public class SerializeMe : ScriptableObject
//{
//	[SerializeField]
//	private List<Enemy> m_Instances;
//	
//	public void OnEnable ()
//	{
////		m_Instances = GetComponent<EnemySpawning> ()._enemyKeys;
//		hideFlags = HideFlags.HideAndDontSave;
////		if (m_Instances == null)
////			m_Instances = new List<Enemy> ();
//	}
//	
//	public void OnGUI ()
//	{
//		foreach (var instance in m_Instances)
//			instance.OnGUI ();
//		
////		if (GUILayout.Button ("Add Simple"))
////			m_Instances.Add (new Enemy("lala", Enemy.EnemyType.SOLDIER, 
//	}
//}
