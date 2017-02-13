using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
[System.Serializable]
public class GameSettings : ScriptableObject {
	
	[Header("UI General")]
	public GameObject uiCanvas; 
	public GameObject uiController;

	[Header("UI Prefabs")]
	public GameObject selectStateButton;
	public GameObject digButton;
	public GameObject spawnGunnerUnitButton;
	public GameObject spawnFlamerUnitButton;
	public GameObject spawnGrenaderUnitButton;
	public GameObject restartLevelButton;
	public GameObject restartSpawnButton;
	public GameObject timelinePrefab;
	public GameObject enemyTimelineIcon;
	public GameObject endGameCanvas;
	public GameObject victoryCanvas;
	public GameObject pauseMenuCanvas;
	public GameObject commandPointText;
	public GameObject attackButton;
	public GameObject pointGainText;
	public GameObject pauseButton;
	public GameObject goToLevelButton;
	public GameObject mineSpawnButton;
	public GameObject bombSpawnButton;
	public GameObject waveTextObj;
	public GameObject centerOfScreenObj;

	[Header("Blocks")]
	public GameObject blockController;
	[Space(7)]
	public GameObject standardBlock;
	public GameObject destroyedBlock;
	public GameObject groundBlock;
	public GameObject nonInteractableStandardBlock;
	public GameObject nonWalkableBlock;
	public GameObject endGameBlock;
	[Space(7)]
	public GameObject standardBlockMud;
//	public GameObject destroyedBlockMud;
	public GameObject groundBlockMud;
	public GameObject nonInteractableStandardBlockMud;
	public GameObject nonWalkableBlockMud;
//	public GameObject endGameBlockMud;
	[Space(7)]
	public GameObject standardBlockIce;
//	public GameObject destroyedBlockIce;
	public GameObject groundBlockIce;
	public GameObject nonInteractableStandardBlockIce;
	public GameObject nonWalkableBlockIce;
//	public GameObject endGameBlockIce;
	public Color blockStartColorGrass;
	public Color blockStartColorMud;
	public Color blockStartColorIce;
	public Color blockSelectedColorGrass;
	public Color blockSelectedColorMud;
	public Color blockSelectedColorIce;
	public GameObject gridObj;
	public Color gridColorWalkable = Color.blue;
	public Color gridColorOccupied = Color.red;
	public Color gridColorStandard = Color.white;
	public Color blockDragColorCenterPositive;
	public Color blockDragColorAroundPositive;
	public Color blockDragColorCenterNegative;
	public Color blockDragColorAroundNegative;

	public GameObject arrowPrefab;
	[Space(7)]
	public float groundHeight = -0.65f;

	[Header("Units")]
	public GameObject unitController;
	public GameObject gunnerUnit;
	public GameObject flamerUnit;
	public GameObject grenaderUnit;
	public GameObject diggerUnit;
	public GameObject unitHealthUI;

	[Header("Enemies")]
	public GameObject enemySpawner;
	public GameObject enemyHealthUI;
	public GameObject grenaderEnemy;
	public GameObject flamethrowerEnemy;
	public GameObject gunnerEnemy;

	[Header("Shot Prefabs")]
	public GameObject grenade;
	public GameObject gunShot;
	public GameObject flameBullet;
	public GameObject enemyGrenade;
	public GameObject enemyGunShot;
	public GameObject enemyFlameBullet;

	[Header("Interactable Object Prefabs")]
	public GameObject friendlyBarracks;
	public GameObject enemyBarracks;
	public GameObject commander;

	[Header("Stats")]
	public GameObject gameStats;
	public float startCommandPoints = 50;
	public float blockCost = 10;
	public float gunnerCost = 10;
	public int maxGunners = 5;
	public float flamerCost = 10;
	public int maxFlamers = 2;
	public float grenaderCost = 10;
	public int maxGrenaders = 3;
	public float commandPointPickup = 25;
	public float mineCost = 30;
	public float bombCost = 50;

	[Header("CommandPointPrefabs")]
	public GameObject commandPointPrefab1;
	public string commandPointUIText = ""; 

	[Header("Obstacles")]
	public GameObject obstacleControls;
	public GameObject mine;
	public GameObject bomb;

	[Header ("Objectives")]
	public GameObject objectiveController;
	public Color flagTakeOverColor;

	[Header("Effects")]
	public GameObject diggingEffect;
	public GameObject diggingEffectIce;
	public GameObject grenadeEffect;
	public GameObject unitPlacementCircle;

	[Header ("Shaders")]
	public Material greyscale;
	public Material coloredSprite;

	[Header ("Audio")]
	public GameObject audioController;
	public GameObject audioSourceObj;
	public AudioClip ambientSound1;
	public AudioClip ambientSound2;
	public AudioClip endGameClip;
	public AudioClip winGameClip;
	public AudioClip winGameClip2;
	public AudioClip shotSound;
	public AudioClip flamerSound;
	public AudioClip grenadeHitSound;
}
