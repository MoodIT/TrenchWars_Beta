
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
class TimelineEditor : EditorWindow {
	[SerializeField]
	private GameObject enemySpawnObj; 
	[SerializeField]
	SerializedObject enemySpawnerComponent;
	SerializedObject enemySpawnerComponentPrefab;

	//enemyspawner properties

	SerializedProperty enemyKeys;
	SerializedProperty enemyWaves;
//	SerializedProperty enemyTimers;

	[SerializeField]
	private GameObject blockControlObj; 
	SerializedObject blockControlComponent;
	SerializedObject blockControlComponentPrefab;


	SerializedProperty mapLength;


	private bool spawnerCreated;

	GameObject eS;

	[SerializeField]
	private GameObject gameStatsObj;
	[SerializeField]
	SerializedObject gameStatsComponent;

	SerializedProperty lengthInSeconds;


	private float settingPos;

	private bool selectingUnit;
	private bool settingPlaceToSpawn;
	private float insetPos;


	[MenuItem ("Window/Timeline Editor")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(TimelineEditor));
	}

	void OnEnable()
	{
		enemySpawnObj =  AssetDatabase.LoadAssetAtPath("Assets/Prefabs/InstantiatedIntoScene/EnemySpawner.prefab", typeof(GameObject)) as GameObject;
//		enemySpawnerComponent = new SerializedObject (enemySpawnObj.GetComponent<EnemySpawning>()) as SerializedObject;

		enemySpawnerComponentPrefab = new SerializedObject (enemySpawnObj.GetComponent<EnemySpawning> ()) as SerializedObject;




		blockControlObj =  AssetDatabase.LoadAssetAtPath("Assets/Prefabs/InstantiatedIntoScene/BlocksController.prefab", typeof(GameObject)) as GameObject;

		if(GameObject.FindObjectOfType<BlockControl> ())
        {
            blockControlComponent = new SerializedObject (GameObject.FindObjectOfType<BlockControl> ().GetComponent<BlockControl>()) as SerializedObject;
            
        }
		else
		{
			blockControlComponent = new SerializedObject (blockControlObj.GetComponent<BlockControl>()) as SerializedObject;
            
        }



		mapLength = blockControlComponent.FindProperty ("length");

		if(GameObject.FindObjectOfType<EnemySpawning> ())
		{
			eS = GameObject.FindObjectOfType<EnemySpawning> ().gameObject;
			enemySpawnerComponent = new SerializedObject (eS.GetComponent<EnemySpawning>()) as SerializedObject;
			lengthInSeconds = enemySpawnerComponent.FindProperty ("lengthInSeconds");
			
			enemyKeys = enemySpawnerComponent.FindProperty ("_enemyKeys");
			enemyWaves = enemySpawnerComponent.FindProperty("waves");
			EditorUtility.SetDirty(eS);
			spawnerCreated = true;
		}
//		gameStatsObj =  AssetDatabase.LoadAssetAtPath("Assets/Prefabs/InstantiatedIntoScene/GameStats.prefab", typeof(GameObject)) as GameObject;
//		gameStatsComponent = new SerializedObject (gameStatsObj.GetComponent<GameStats>()) as SerializedObject;
//		lengthInSeconds = gameStatsComponent.FindProperty ("levelDuration");
//		Debug.Log (mapLength.floatValue);

//		enemyTimers = enemySpawnerComponent.FindProperty ("_values");
//		for (int i = 0; i < enemyKeys.arraySize; i++) 
//		{
//			SerializedProperty MyListRef = enemyKeys.GetArrayElementAtIndex (i);
//			SerializedProperty timer = MyListRef.FindPropertyRelative ("spawnTime");
////			SerializedProperty power = MyListRef.FindPropertyRelative ("power");
////			EditorGUI.PropertyField(new Rect(i*20, 20, 20,20), timer);
//			EditorGUILayout.PropertyField (timer);
//			//			EditorGUILayout.PropertyField (power);
//
////			float pos = Map(0,lengthInSeconds.floatValue,0,w.position.width - 40,e.mousePosition.x-20);
//		}
	}
	void OnGUI () {

		EditorWindow w = TimelineEditor.GetWindow <TimelineEditor>();

		Event e = Event.current;
//		if(EventType


		if(!Application.isPlaying && !spawnerCreated)
		{
			if(GUI.Button(new Rect(0,60,position.width, 20), "Create Spawner") )
			{
				spawnerCreated = true;
//				blockControlObj.GetComponent<BlockControl>().width = width;
//				blockControlObj.GetComponent<BlockControl>().length = length;
				eS = Instantiate(enemySpawnObj, Vector3.zero, Quaternion.identity) as GameObject;
				//			bC.GetComponent<BlockControl>().width = width;
				//			blockControlComponent = new SerializedObject (bC.GetComponent<BlockControl> ()) as SerializedObject;
				//			blockControlComponent.FindProperty("spawned").boolValue = true;
				
				//			bC.GetComponent<BlockControl>().spawned = true;
				enemySpawnerComponent = new SerializedObject (eS.GetComponent<EnemySpawning> ()) as SerializedObject;
				lengthInSeconds = enemySpawnerComponent.FindProperty ("lengthInSeconds");				
				enemyKeys = enemySpawnerComponent.FindProperty ("_enemyKeys");
				GameObject.Find("GameController").GetComponent<GameController>().enemySpawnerInstantiated = true;
				
				EditorUtility.SetDirty(eS);
			}
		}
		if(!Application.isPlaying && spawnerCreated)
		{
			enemySpawnerComponent.Update ();
		
		EditorGUILayout.LabelField ("0");
//		EditorGUI.LabelField (new Rect (w.position.width-75, 0, 75, 50), "length: " + lengthInSeconds.floatValue);
		EditorGUIUtility.labelWidth = 50;
		lengthInSeconds.floatValue = EditorGUI.FloatField (new Rect (w.position.width - 85, 0,85, 16), "Length: ", lengthInSeconds.floatValue);
		GUIStyle bgColorStyle = new GUIStyle();
		bgColorStyle.normal.background = MakeTex(600, 1, new Color(0.7f, 0.7f, 0.7f, 1f));
		GUILayout.BeginArea (new Rect (w.position.width/90, Mathf.Clamp(w.position.height/25,20, 25) , w.position.width/1.02f, w.position.height/2), bgColorStyle);
		for(int i = 0; i < lengthInSeconds.floatValue; i++)
		{
			if(lengthInSeconds.floatValue < 60)
			{
				if(i % 5 == 0)
				{
					Handles.DrawLine (new Vector2 (i *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0), new Vector2 (i*((w.position.width/1.02f) / lengthInSeconds.floatValue), w.position.height));
					EditorGUI.LabelField(new Rect(i *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0, 25,20), "" + i.ToString("F1"));
				}
				else if(i+1 == lengthInSeconds.floatValue)
				{
					Handles.DrawLine (new Vector2 ((i+0.99f) *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0), new Vector2 ((i+0.99f)*((w.position.width/1.02f) / lengthInSeconds.floatValue), w.position.height));
				}
			}
			else if(lengthInSeconds.floatValue > 60 && lengthInSeconds.floatValue < 240)
			{
				if(i % 10 == 0)
				{
					Handles.DrawLine (new Vector2 (i *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0), new Vector2 (i*((w.position.width/1.02f) / lengthInSeconds.floatValue), w.position.height));
					EditorGUI.LabelField(new Rect(i *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0, 25,20), "" + i.ToString("F1"));
				}
				else if(i+1 == lengthInSeconds.floatValue)
				{
					Handles.DrawLine (new Vector2 ((i+0.99f) *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0), new Vector2 ((i+0.99f)*((w.position.width/1.02f) / lengthInSeconds.floatValue), w.position.height));
				}
			}
			else if(lengthInSeconds.floatValue > 240)
			{
				if(i % 20 == 0)
				{
					Handles.DrawLine (new Vector2 (i *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0), new Vector2 (i*((w.position.width/1.02f) / lengthInSeconds.floatValue), w.position.height));
					EditorGUI.LabelField(new Rect(i *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0, 25,20), "" + i.ToString("F1"));
				}
				else if(i+1 == lengthInSeconds.floatValue)
				{
					Handles.DrawLine (new Vector2 ((i+0.99f) *((w.position.width/1.02f) / lengthInSeconds.floatValue), 0), new Vector2 ((i+0.99f)*((w.position.width/1.02f) / lengthInSeconds.floatValue), w.position.height));
				}
			}
		}

		for (int iW = 0; iW < enemyWaves.arraySize; iW++) 
        {
				SerializedProperty MyListRef = enemyWaves.GetArrayElementAtIndex (iW);

				SerializedProperty timer = MyListRef.FindPropertyRelative ("time");
				float pos = Map(0,w.position.width/1.02f,0,lengthInSeconds.floatValue,timer.floatValue);
				EditorGUI.LabelField(new Rect(pos,  20 +200, 85,20), "Wave ");
				Handles.DrawSolidDisc(new Vector2(pos,20), Vector3.one, 5);
				if(GUI.Button(new Rect(pos-10, 20 + 180, 20,20), "D"))
				{
                    enemyWaves.DeleteArrayElementAtIndex(iW);
                    
				}
		}
		for (int i = 0; i < enemyKeys.arraySize; i++) 
		{
			Handles.color = Color.gray;
			SerializedProperty MyListRef = enemyKeys.GetArrayElementAtIndex (i);
			//			SerializedProperty name = MyListRef.FindPropertyRelative ("name");
			//			SerializedProperty power = MyListRef.FindPropertyRelative ("power");
			SerializedProperty timer = MyListRef.FindPropertyRelative ("spawnTime");
			SerializedProperty type = MyListRef.FindPropertyRelative ("enemyType");
			SerializedProperty lane = MyListRef.FindPropertyRelative ("spawnPos").FindPropertyRelative("mLength");
			float pos = Map(0,w.position.width/1.02f,0,lengthInSeconds.floatValue,timer.floatValue);
			//			SerializedProperty MyGO = MyListRef.FindPropertyRelative ("AnGO");
			//			SerializedProperty MyArray = MyListRef.FindPropertyRelative ("AnIntArray");
			//			public string name;
			//			public enum EnemyType{SOLDIER, FLAMETHROWER};
			//			public EnemyType enemyType;
			//			public int power;
			//			public Block spawnPos;
			//			public bool spawned;
			//			public float health;
			
			// Display the property fields in two ways.
			
			//			Debug.Log(pos);
			bool goDown = false;
			//			for(int i2 = 0; i2 < enemyKeys.arraySize; i2++)
			//			{
			////				Debug.Log (enemyKeys.GetArrayElementAtIndex(i2).FindPropertyRelative("spawnTime").floatValue + " " + timer.floatValue);
			//				if(enemyKeys.GetArrayElementAtIndex(i2).FindPropertyRelative("spawnTime").floatValue < timer.floatValue + 5f &&  enemyKeys.GetArrayElementAtIndex(i2).FindPropertyRelative("spawnTime").floatValue > timer.floatValue - 5f )
			//				{
			//					if(i != i2)
			//					{
			//					goDown = true;
			////					Debug.Log (enemyKeys.GetArrayElementAtIndex(i2).FindPropertyRelative("spawnTime").floatValue + " " + timer.floatValue);
			//					}
			//				}
			//			}
			//			if(goDown)
			//			{
			//				EditorGUI.LabelField(new Rect(pos, 20 + 40, 185,20), "Type: " + (Enemy.EnemyType)type.enumValueIndex);
			//				EditorGUI.LabelField(new Rect(pos, 30 + 40, 95,20), "Spawn time: " + timer.floatValue.ToString("F0"));
			//				EditorGUI.LabelField(new Rect(pos, 40 + 40, 85,20), "Lane: " + (lane.intValue +1));
			//				Handles.DrawDottedLine(new Vector2(pos, 20), new Vector2(pos, 50), 1);
			//				Handles.DrawSolidDisc(new Vector2(pos,20), Vector3.one, 5);
			//			}
			//			else
			//			{
			int i2 = i;
			if(i2 > 7)
			{
				i2 = 0;
				//				i2+= i/2; 
			}
			EditorGUI.LabelField(new Rect(pos, i2*20 + 20, 185,20), "Type: " + (Enemy.EnemyType)type.enumValueIndex);
			EditorGUI.LabelField(new Rect(pos,  i2*20 +30, 95,20), "Spawn time: " + timer.floatValue.ToString("F0"));
			EditorGUI.LabelField(new Rect(pos,  i2*20 +40, 85,20), "Lane: " + (lane.intValue +1));
			Handles.DrawDottedLine(new Vector2(pos, i2*20), new Vector2(pos, 50), 1);
			Handles.DrawSolidDisc(new Vector2(pos,i2*20), Vector3.one, 5);
			if(GUI.Button(new Rect(pos-10, i2*20 + 20, 20,20), "D"))
			{
								enemyKeys.DeleteArrayElementAtIndex(i);

			}
			//			}
			//			Handles.draw
			//			EditorGUI.PropertyField(new Rect(pos, 20, 20,20), timer);
			//			EditorGUILayout.PropertyField (power);
			//			EditorGUILayout.PropertyField (spawnPos, true);
			//			EditorGUILayout.PropertyField (MyVect3);
			// The actual window code goes here
		}


		if(!selectingUnit && !settingPlaceToSpawn)
		{
			float pos = Map(0,lengthInSeconds.floatValue,0,w.position.width/1.02f,e.mousePosition.x);
			if (e.button == 0 ) //&& e.type == EventType.mouseDown
			{

				Handles.color = Color.gray;
				if(e.mousePosition.x > 0  && e.mousePosition.x < w.position.width/1.02f && e.mousePosition.y > 0 && e.mousePosition.y < w.position.height/2)
				{

	//				Debug.Log(Map(0,lengthInSeconds.floatValue,0,w.position.width - 40,e.mousePosition.x-20));

					Handles.DrawLine(new Vector2(e.mousePosition.x, 0), new Vector2(e.mousePosition.x, w.position.height));
	//				Handles.
					EditorGUI.LabelField(new Rect(e.mousePosition.x, w.position.height/2-20, 35,20), "" + pos);
					//				Debug.Log (Mathf.Lerp(0,lengthInSeconds.floatValue, Mathf.InverseLerp(0, 1000, e.mousePosition.x-20)));
					//				Debug.Log (e.mousePosition.x-20);
					//				Debug.Log((e.mousePosition.x -20) * ((w.position.width - 40)/ lengthInSeconds.floatValue)/ lengthInSeconds.floatValue); // / lengthInSeconds.floatValue
				}
			}
//			bool t = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
			if(e.button == 0 && e.type == EventType.mouseDown && e.type == EventType.MouseDown)
			{
				Repaint();
				settingPos = pos;
				insetPos = pos;
				selectingUnit = true;
				Handles.DrawLine(new Vector2(e.mousePosition.x, 0), new Vector2(e.mousePosition.x, w.position.height));
			}
		}

		else if(selectingUnit && !settingPlaceToSpawn)
		{
			Handles.color = Color.gray;
			float pos3 = Map(0,lengthInSeconds.floatValue,0,w.position.width/1.02f,e.mousePosition.x);
			float pos2 = Map(0,w.position.width/1.02f,0,lengthInSeconds.floatValue,settingPos);
			Handles.DrawLine(new Vector2(pos2, 0), new Vector2(pos2, w.position.height));
			if(GUI.Button(new Rect(pos2+30, w.position.height/15, 80,w.position.height/20), "GRENADER"))
			{
				int arS = enemyKeys.arraySize;
				enemyKeys.InsertArrayElementAtIndex(arS);
				SerializedProperty MyListRef = enemyKeys.GetArrayElementAtIndex (arS);
				SerializedProperty timer = MyListRef.FindPropertyRelative ("spawnTime");
				SerializedProperty type = MyListRef.FindPropertyRelative ("enemyType");
				type.enumValueIndex = (int)Enemy.EnemyType.GRENADER;
				timer.floatValue = insetPos;
				selectingUnit = false;
				settingPlaceToSpawn = true;
//				enemyKeys.GetArrayElementAtIndex(arS). = lengthInSeconds.floatValue;
			}
			if(GUI.Button(new Rect(pos2+30, w.position.height/9, 80,w.position.height/20), "FLAMER"))
			{					
				int arS = enemyKeys.arraySize;
				enemyKeys.InsertArrayElementAtIndex(arS);
				SerializedProperty MyListRef = enemyKeys.GetArrayElementAtIndex (arS);
				SerializedProperty timer = MyListRef.FindPropertyRelative ("spawnTime");
				SerializedProperty type = MyListRef.FindPropertyRelative ("enemyType");
				type.enumValueIndex = (int)Enemy.EnemyType.FLAMETHROWER;
				timer.floatValue = insetPos;
				selectingUnit = false;
				settingPlaceToSpawn = true;
			}
			if(GUI.Button(new Rect(pos2+30, w.position.height/6, 80,w.position.height/20), "GRUNNER"))
			{	
				int arS = enemyKeys.arraySize;
				enemyKeys.InsertArrayElementAtIndex(arS);
				SerializedProperty MyListRef = enemyKeys.GetArrayElementAtIndex (arS);
				SerializedProperty timer = MyListRef.FindPropertyRelative ("spawnTime");
				SerializedProperty type = MyListRef.FindPropertyRelative ("enemyType");
				type.enumValueIndex = (int)Enemy.EnemyType.GUNNER;
				timer.floatValue = insetPos;
				selectingUnit = false;
				settingPlaceToSpawn = true;
			}
			if(GUI.Button(new Rect(pos2+30, w.position.height/4, 80,w.position.height/20), "WAVE"))
            {
					int arS = enemyWaves.arraySize;
					enemyWaves.InsertArrayElementAtIndex(arS);
					SerializedProperty MyListRef = enemyWaves.GetArrayElementAtIndex (arS);
					SerializedProperty timer = MyListRef.FindPropertyRelative ("time");
					timer.floatValue = insetPos;
					selectingUnit = false;
					settingPlaceToSpawn = false;
			}
			
			if(e.button == 0 && e.type == EventType.mouseDown)
			{
				float pos = Map(0,lengthInSeconds.floatValue,0,w.position.width/1.02f,e.mousePosition.x);
				settingPos = pos;
				selectingUnit = false;
				Handles.DrawLine(new Vector2(e.mousePosition.x, 0), new Vector2(e.mousePosition.x, w.position.height));
			}
		}
		else if(settingPlaceToSpawn)
		{
			Handles.color = Color.gray;
			float pos2 = Map(0,w.position.width/1.02f,0,lengthInSeconds.floatValue,settingPos);
			Handles.DrawLine(new Vector2(pos2, 0), new Vector2(pos2, w.position.height));
			EditorGUI.LabelField(new Rect(pos2-30, w.position.height/6, 85,20), "Choose lane:");
			for(int li = 0; li < mapLength.intValue; li++)
			{
				if(GUI.Button(new Rect(pos2 + 100 + mapLength.intValue - li * 30, w.position.height/5, 20,20), "" + (li+1)))
				{
					SerializedProperty MyListRef = enemyKeys.GetArrayElementAtIndex (enemyKeys.arraySize-1);
					SerializedProperty pos = MyListRef.FindPropertyRelative ("spawnPos");
//					SerializedProperty type = MyListRef.FindPropertyRelative ("enemyType");
					SerializedProperty posL = pos.FindPropertyRelative("mLength");
					SerializedProperty posW = pos.FindPropertyRelative("mWidth");
						SerializedProperty healthhh = MyListRef.FindPropertyRelative("health");
					posL.intValue = li;
					posW.intValue = blockControlComponent.FindProperty("width").intValue -1;
						healthhh.floatValue = 10; //fix
					settingPlaceToSpawn = false;
				}
			}
		}

		GUILayout.EndArea ();
//		GUILayout.FlexibleSpace();
//		EditorGUILayout.Space ();
//		bool b = false;
//		EditorGUILayout.BeginToggleGroup ("GNUE",b);
		GUILayout.BeginArea (new Rect (20, w.position.height - w.position.height/2.3f, w.position.width - 40, (w.position.height-200)), bgColorStyle);
		GUILayout.Button ("GNEU");
		GUILayout.EndArea ();

//		enemySpawnerComponent.OnGUI ();
//		enemySpawnObj.OnGUI ();
			enemySpawnerComponent.ApplyModifiedProperties ();
		}

	}
	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width*height];
		
		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;
		
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		
		return result;
	}
	public float Map(float from, float to, float from2, float to2, float value){
		if(value <= from2){
			return from;
		}else if(value >= to2){
			return to;
		}else{
			return (to - from) * ((value - from2) / (to2 - from2)) + from;
		}
	}

	public void ShowUnitSpawnButtons()
	{

//		for(int i = 0; i < 
	}
}