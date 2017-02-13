using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
public class LevelEditor : EditorWindow {

	[SerializeField]
	private GameObject blockControlObj; 
	[SerializeField]
	SerializedObject blockControlComponent;

	[SerializeField]
	SerializedObject blockControlComponentOnPrefab;


	private GameObject destroyedBlock;

	private bool selectedBlock;
	private bool gridDisabled;
	private int width;
	private int length;
	private bool blockSpawned;

	Texture2D prefabObjectPreview;
//	[SerializeField]
	Texture2D[] blockPrefabsPreviews;
	Texture2D[,] blockPrefabPreviews2D;
	GameObject bC;
//	[SerializeField]
	public SerializedProperty blockToAdd;
	public SerializedProperty blockPrefabs;
	public SerializedProperty blockStates;
	private int selectedWidth;
	private int selectedLength;

//	[SerializeField]
	public BlockControl.BlockType blockType;
//	[SerializeField]
//	public enum BlockType{GRASS, MUD, ICE};
//	public BlockType bType;
//	SerializedProperty blockType;
	SerializedProperty arr;


	private int[] types = new int[]{1,2,4};
	private string[] typeNames = new string[]{"Destroyed", "Standard" ,"Blocking"};

	[SerializeField]

//	List<GameObject> blockPrefabs = new List<GameObject> ();

	GUIContent content = new GUIContent();
	[MenuItem ("Window/Level Editor")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(LevelEditor));
	}

	void OnEnable()
	{ 
		blockControlObj = AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/InstantiatedIntoScene/BlocksController.prefab", typeof(GameObject)) as GameObject;
		width = blockControlObj.GetComponent<BlockControl> ().width;
		length = blockControlObj.GetComponent<BlockControl> ().length;

//		Debug.Log (blockControlComponentOnPrefab);
		blockControlComponentOnPrefab = new SerializedObject (blockControlObj.GetComponent<BlockControl> ()) as SerializedObject;

//		Debug.Log (blockControlComponentOnPrefab);

		destroyedBlock = AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/Blocks/DestroyedBlock.prefab", typeof(GameObject)) as GameObject;
		if(GameObject.FindObjectOfType<BlockControl> ())
		{
			bC = GameObject.FindObjectOfType<BlockControl> ().gameObject;
			blockControlComponent = new SerializedObject (bC.GetComponent<BlockControl> ()) as SerializedObject;
			EditorUtility.SetDirty(bC);
			blockSpawned = true;
		}

		if(blockSpawned && !GameObject.FindObjectOfType<BlockControl> ())
		{
			blockSpawned = false;
			GameObject.Find("GameController").GetComponent<GameController>().blockControllerInstantiated = false;
            
        }
		EditorUtility.SetDirty (blockControlObj);

		blockToAdd = blockControlComponentOnPrefab.FindProperty ("editorBlockAdd");
		blockPrefabs = blockControlComponentOnPrefab.FindProperty ("editorBlockPrefabs");
		blockStates = blockControlComponentOnPrefab.FindProperty ("editorBlockStates");
//		blockPrefabs =  Resources.LoadAll ("Blocks", typeof(Object));
//		Debug.Log (AssetDatabase.LoadAllAssetsAtPath ("Assets/Prefabs/Blocks").Length);
//		blockPrefabsPreviews = new Texture2D[blockPrefabs.Length]; 
//		for(int i = 0; i< blockPrefabs.Length; i++)
//		{
//			blockPrefabsPreviews[i] = AssetPreview.GetAssetPreview(blockPrefabs[i]);
//		}
		

		Object prefabObject = AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/Blocks/StandardBlock.prefab", typeof(Object));
		prefabObjectPreview = AssetPreview.GetAssetPreview(prefabObject);



	}

	void OnGUI()
	{
//		blockControlComponentOnPrefab.Update ();
		Repaint ();
//		enemySpawnerComponent.Update ();
//		if(blockSpawned)
//			blockControlComponent.Update ();
		if(bC != null && bC.GetComponent<BlockControl>().interactableGrids1D[0] != null)
		{
//			Debug.Log("UI");
			if(bC.GetComponent<BlockControl>().interactableGrids1D[12].GetComponent<Renderer>().enabled && !Application.isPlaying)
			{

				for(int i = 0; i < bC.GetComponent<BlockControl>().interactableGrids1D.Length; i++)
				{
					bC.GetComponent<BlockControl>().interactableGrids1D[i].GetComponent<Renderer>().enabled = false;
				}
			}
		}
		if(blockSpawned && !GameObject.FindObjectOfType<BlockControl> ())
		{
			blockSpawned = false;
			GameObject.Find("GameController").GetComponent<GameController>().blockControllerInstantiated = false;
            
        }
//		EditorUtility.SetDirty(bC);
//		EditorGUILayout.FloatField(blockControlComponent.FindProperty("width"));
//		width = EditorGUILayout.IntField("Length", width);
		width = EditorGUI.IntSlider (new Rect (0, 0, position.width, 20), "Length", width, 10, 25);
		length = EditorGUI.IntSlider (new Rect (0, 30, position.width, 20), "Width", length, 5, 15);
//		width = EditorGUILayout.IntSlider ( width, 10, 25); 
//		blockControlComponent.FindProperty("width").intValue = EditorGUILayout.IntField("width", blockControlComponent.FindProperty("width").intValue);
		if (blockPrefabs != null && blockSpawned && !Application.isPlaying) { 
			for(int i = 0; i < blockPrefabs.arraySize; i++)
			{
//							GUI.Button (new Rect (i*185, 160, 180, 150), content); //blockPrefabsPreviews[i]

//				Debug.Log (blockPrefabs.GetArrayElementAtIndex(3).objectReferenceValue.name);
//				content.image.
				GUIStyle bgColorStyle = new GUIStyle();
				bgColorStyle.normal.background = MakeTex(600, 1, new Color(0.7f, 0.7f, 0.7f, 1f));
				bgColorStyle.imagePosition = ImagePosition.ImageAbove;
//				content.text = ""; 
				content.image = AssetPreview.GetAssetPreview(blockPrefabs.GetArrayElementAtIndex(i).objectReferenceValue);
//				content.image.hideFlags = HideFlags.DontSave;
				Rect rect = new Rect(i*165, 350, 150, 150);
//				for(int h = 0; h < 3; ++h)
//				{
//					for(int j = 0; j < 2; ++j)
//					{
//						rect = new Rect(h*165, j*350, 150, 150);
//					}
//				}
//				GUILayout.BeginArea(rect,bgColorStyle);
//				AssetPreview.GetAssetPreview
				if(GUI.Button(new Rect (rect.position.x + (rect.width/2 - 60), rect.position.y-120, 120, 120), content) && selectedBlock)
				{
					ReplaceBlock(blockStates.GetArrayElementAtIndex(i).intValue, (GameObject)blockPrefabs.GetArrayElementAtIndex(i).objectReferenceValue);
				}//rect.position.x + (rect.width/2 - 60)
				EditorGUI.LabelField(new Rect (rect.position.x + (rect.width/2 - 50), rect.position.y, 100,20),"" +  blockPrefabs.GetArrayElementAtIndex(i).objectReferenceValue.name);

				if(GUI.Button(new Rect (rect.position.x + (rect.width/2 - 60), rect.position.y+20, 90, 15), "Delete"))
				{
					if(blockPrefabs.GetArrayElementAtIndex(i).objectReferenceValue != null)
					{
						blockPrefabs.DeleteArrayElementAtIndex(i); 
						blockStates.DeleteArrayElementAtIndex(i);
					}
//					if(blockStates.GetArrayElementAtIndex(i).intValue != 0)
//					{
//
//					}
					blockPrefabs.DeleteArrayElementAtIndex(i);
//					blockStates.DeleteArrayElementAtIndex(i);

					blockControlComponentOnPrefab.ApplyModifiedProperties();

				}
//				blockControlComponentOnPrefab.Update();
				arr = blockControlComponent.FindProperty("interactableBlockStates1D");
				SerializedProperty widthInt = blockControlComponentOnPrefab.FindProperty("width");
				SerializedProperty bState = arr.GetArrayElementAtIndex(selectedLength * widthInt.intValue + selectedWidth);
				if (blockStates.arraySize > i)
				{
					blockStates.GetArrayElementAtIndex(i).intValue = EditorGUI.IntField(new Rect (rect.position.x + rect.width - 40, rect.position.y+20, 15,15), blockStates.GetArrayElementAtIndex(i).intValue);
				}
//				EditorGUI.IntPopup(new Rect (rect.position.x + (rect.width/2 - 50), rect.position.y, 100,20), "Type: ", typeNames, types);
//				GUI.Button (new Rect (i*185, 260, 180, 150),content);//
//				Debug.Log (blockPrefabs.arraySize);
//				GUILayout.EndArea();
				blockControlComponentOnPrefab.ApplyModifiedProperties();

			}
		}
//			Debug.Log (blockPrefabs.Length);
////			GUI.skin.button.alignment= TextAnchor.MiddleCenter;
//////			content.image.width = 100;
////			content.text = blockPrefabs[i].name.Substring(0, 10);
////			content.image =  blockPrefabsPreviews[i];
////			GUI.Button (new Rect (i*185, 160, 180, 150), content); //blockPrefabsPreviews[i]
////
//		}
//		for(int i = 0; i < 5; i++)
//		{
//			for(int i2 = 0; i2 < 5; i2++)
//			{
//				
//			}
//		}
//		(GameObject)EditorGUI.ObjectField (new Rect (185, 160, 180, 150), blockToAdd, typeof(GameObject));
		if (blockSpawned && !Application.isPlaying) 
		{ 
			EditorGUIUtility.labelWidth = 85;
//		blockToAdd = (GameObject)EditorGUI.ObjectField(new Rect (20, 160, 250, 15), "Block To Add: ", blockToAdd, typeof(GameObject), true);
			EditorGUI.PropertyField (new Rect (20, 160, 250, 15), blockToAdd);
		
			if (GUI.Button (new Rect (20, 190, 70, 30), "Add Block") && blockToAdd != null) {
//			blockPrefabs.Add(blockToAdd);
//			if(blockPrefabs != null)
//			{
//				blockPrefabs.arraySize = blockPrefabs.arraySize +1;
//				blockControlComponent.FindProperty ("editorBlockPrefabs").arraySize ++;
//				blockPrefabs = new GameObject[blockPrefabs.arraySize +1];
//			}
//			else
//			{

//				blockPrefabs = new GameObject[1];
//			}
//			EditorUtility.SetDirty(blockPrefabs);
//			blockControlComponent.Update();
//			blockControlComponentOnPrefab.ApplyModifiedProperties();
				blockPrefabs.InsertArrayElementAtIndex (blockPrefabs.arraySize);
				blockStates.InsertArrayElementAtIndex (blockStates.arraySize);
				blockStates.GetArrayElementAtIndex (blockPrefabs.arraySize - 1).intValue = 1;
				blockPrefabs.GetArrayElementAtIndex (blockPrefabs.arraySize - 1).objectReferenceValue = blockToAdd.objectReferenceValue;
//			blockPrefabs.GetArrayElementAtIndex(blockPrefabs.arraySize).objectReferenceValue = ;
				blockControlComponentOnPrefab.ApplyModifiedProperties ();
//			blockToAdd = new GameObject;
//			blockControlComponent.FindProperty ("editorBlockPrefabs").GetArrayElementAtIndex(blockPrefabs.arraySize).objectReferenceValue = blockToAdd.objectReferenceValue;
			}
		}

		if(!Application.isPlaying)
		{
			if(GUI.Button(new Rect(0,60,position.width, 20), "Spawn") )
			{
				blockSpawned = true;
	//			EditorUtility.SetDirty(bC);
				blockControlObj.GetComponent<BlockControl>().width = width;
				blockControlObj.GetComponent<BlockControl>().length = length;
				blockControlObj.GetComponent<BlockControl>().blockType = blockType;

				bC = Instantiate(blockControlObj, Vector3.zero, Quaternion.identity) as GameObject;
	//			bC.GetComponent<BlockControl>().width = width;
				blockControlComponent = new SerializedObject (bC.GetComponent<BlockControl> ()) as SerializedObject;
				blockControlComponent.FindProperty("spawned").boolValue = true;

	//			bC.GetComponent<BlockControl>().spawned = true;
				GameObject.Find("GameController").GetComponent<GameController>().blockControllerInstantiated = true;

				EditorUtility.SetDirty(bC);
	//			EditorApplication.MarkSceneDirty();
				//instantier block controller
				//brug shared materials
				//lav seperat function evt?
	//			blockControlObj.GetComponent<BlockControl>().BuildBlocks();
			}
			if(GUI.Button(new Rect(0,90,position.width, 20), "Remove"))
			{
				DestroyImmediate(bC.gameObject);
				blockSpawned = false;
				GameObject.Find("GameController").GetComponent<GameController>().blockControllerInstantiated = false;
	//			EditorUtility.SetDirty(bC);
			}

			blockType = (BlockControl.BlockType)EditorGUI.EnumPopup(new Rect(0,120,200, 20), blockType);
			if(Selection.activeGameObject != null && blockSpawned)
			{

				SelectionChecks();

			}
		}
//		else
//		{
//			selectedBlock = false;
//		}

//		if(blockSpawned)
//			blockControlComponent.ApplyModifiedProperties ();
	}
	void ReplaceBlock(int replaceState, GameObject replaceBlock)
	{

//				Vector3 pos = bC.GetComponent<BlockControl>().interactableBlockObjs[selectedWidth,selectedLength].transform.position;
		Vector3 pos = bC.GetComponent<BlockControl>().interactableBlockObjs1D[(selectedLength*bC.GetComponent<BlockControl>().width + selectedWidth)].transform.position;
		//							DestroyImmediate (bC.GetComponent<BlockControl>().interactableBlockObjs[i,i2].gameObject);
		DestroyImmediate (bC.GetComponent<BlockControl>().interactableBlockObjs1D[(selectedLength*bC.GetComponent<BlockControl>().width + selectedWidth)].gameObject);
		//
		bC.GetComponent<BlockControl>().interactableBlockObjs1D[(selectedLength*bC.GetComponent<BlockControl>().width + selectedWidth)] = Instantiate(replaceBlock, pos, Quaternion.identity) as GameObject;
		bC.GetComponent<BlockControl>().interactableBlockObjs1D[(selectedLength*bC.GetComponent<BlockControl>().width + selectedWidth)].transform.SetParent(bC.GetComponent<BlockControl>().interactableHolder.transform);
		bC.GetComponent<BlockControl>().interactableBlockStates1D[(selectedLength*bC.GetComponent<BlockControl>().width + selectedWidth)] = replaceState;
		bC.GetComponent<BlockControl> ().interactableBlocks1D [(selectedLength * bC.GetComponent<BlockControl> ().width + selectedWidth)].obj = bC.GetComponent<BlockControl> ().interactableBlockObjs1D [(selectedLength * bC.GetComponent<BlockControl> ().width + selectedWidth)];
		//						}
	}
	private void SelectionChecks()
	{
		try
		{
			for(int i = 0; i < bC.GetComponent<BlockControl>().width; i++)
			{
				for(int i2 = 0; i2 < bC.GetComponent<BlockControl>().length; i2++) 
				{
					if(bC.GetComponent<BlockControl>().interactableBlockObjs1D[(i2*bC.GetComponent<BlockControl>().width + i)] == Selection.activeGameObject )
					{
						selectedLength = i2;
						selectedWidth = i;
						selectedBlock = true;
						return;
//						if(GUI.Button(new Rect(0,120,position.width, 20),"Make into destroyed"))  
//						{
////							Vector3 pos = bC.GetComponent<BlockControl>().interactableBlockObjs[i,i2].transform.position;
//							Vector3 pos = bC.GetComponent<BlockControl>().interactableBlockObjs1D[(i2*bC.GetComponent<BlockControl>().width + i)].transform.position;
////							DestroyImmediate (bC.GetComponent<BlockControl>().interactableBlockObjs[i,i2].gameObject);
//							DestroyImmediate (bC.GetComponent<BlockControl>().interactableBlockObjs1D[(i2*bC.GetComponent<BlockControl>().width + i)].gameObject);
//
//							bC.GetComponent<BlockControl>().interactableBlockObjs1D[(i2*bC.GetComponent<BlockControl>().width + i)] = Instantiate(destroyedBlock, pos, Quaternion.identity) as GameObject;
//							bC.GetComponent<BlockControl>().interactableBlockObjs1D[(i2*bC.GetComponent<BlockControl>().width + i)].transform.SetParent(bC.GetComponent<BlockControl>().interactableHolder.transform);
//							bC.GetComponent<BlockControl>().interactableBlockStates1D[(i2*bC.GetComponent<BlockControl>().width + i)] = 0;
//						}

					}
					else
					{
						selectedBlock = false;
						selectedLength = 0;
						selectedWidth = 0;
					}
				}
			}
		}
		catch (System.Exception e)
		{
			Debug.Log("BGUEI");
//			print(e.ToString());
		}
//		if(bC == null)
//			throw new System.Exception("GENGUIO");
//		if (!bC.GetComponent<BlockControl>())
//			throw new System.Exception("GENGUIO");// ("No Block controller");
//		else
//		{
//			Debug.Log (bC.GetComponent<BlockControl>().interactableBlockObjs);
//			for(int i = 0; i < bC.GetComponent<BlockControl>().interactableBlockObjs.GetLength(0); i++)
//			{
//				for(int i2 = 0; i2 < bC.GetComponent<BlockControl>().interactableBlockObjs.GetLength(1); i2++)
//				{
//					if(bC.GetComponent<BlockControl>().interactableBlockObjs[i,i2] = Selection.activeGameObject)
//					{
//						GUILayout.Button("BGUEIG");
//					}
//				}
//			}
//		}
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
//	void ToggleGrid(bool on)
//	{
//		for(int i = 0; i < bC.GetComponent<BlockControl>().interactableGrids.GetLength(0); i++)
//		{
//			for(int i2 = 0; i2 < bC.GetComponent<BlockControl>().interactableGrids.GetLength(1); i2++)
//			{
//
//			}
//		}
////		bC.GetComponent<BlockControl>().interactableGrids
//	}

}
