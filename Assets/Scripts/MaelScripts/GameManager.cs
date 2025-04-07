using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//================================//
public class GameManager : MonoBehaviour
{
    //================================//
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return _instance;
        }
    }

    [Serializable]
    public struct DungeonFloor
    {
        public int roomCount;
        public int difficultyLevel;
        public int maxRoomEnemies;
        public Color depthColor;
        public Vector3 SpawnPoint;
        [HideInInspector]
        public List<Enemy> enemies;
        public List<GameObject> enemyPrefabs;
    }

    //================================//
    // Settings
    [Header("Settings")]
    [SerializeField] private DungeonFloor[]     dungeonFloors = null;
    [SerializeField] private Material           fullScreenMatReference = null;
    
    [Header("Spell Canvas Image")]
    [SerializeField] private Image              spellCanvasImage;
    [SerializeField] private Sprite[]           SpellSprites;

    [Header("Scenes")]
    [SerializeField] private string            mainMenuScene = "Main Menu Scene";
    [SerializeField] private string            creditsScene = "CreditsScene";


    //================================//
    private PlayerController     _playerController   = null;
    public PlayerController      Player => _playerController;
    private int                 _currentFloor = 0;
    public int                  CurrentFloor => _currentFloor;
    private Canvas              _gameCanvas = null;

    //================================//
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);        
    }

    //================================//
    private void Start()
    {
        if (dungeonFloors == null || dungeonFloors.Length == 0)
        {
            Debug.LogError("Dungeon floors are not assigned or empty!");
            return;
        }

        _gameCanvas = GetComponent<Canvas>();
        if (_gameCanvas == null)
        {
            Debug.LogError("Game canvas not found please assign one");
            return;
        }

        CameraController camera = FindFirstObjectByType<CameraController>();
        if ( camera == null )
        {
            Debug.LogError("Camera Controller not found in the scene");
        }
        _gameCanvas.worldCamera = camera.GetComponent<Camera>();

        InitializeGame(0);
    }

    //================================//
    // METHODS
    public void OnFloorWin()
    {
        _currentFloor++;
        if (_currentFloor >= dungeonFloors.Length)
        {
            OnGameWin();
            return;
        }

        InitializeGame(_currentFloor);
    }

    //================================//
    public void OnGameWin()
    {
        Debug.Log("You win the game!");
        StartCoroutine(CreditsAsync());
    }

    //================================//
    public void InitializeGame(int floorLevel)
    {
        // Cleanup Enemies
        CleanupEnemies();

        // Assign background color
        fullScreenMatReference.SetColor("_ColorAdd", dungeonFloors[floorLevel].depthColor);

        // Create dungeon floor
        int numberOfRooms = dungeonFloors[floorLevel].roomCount;
        int difficultyLevel = dungeonFloors[floorLevel].difficultyLevel;

        // TODO
        MapGenerator.Instance.GenerateMap(numberOfRooms);
        dungeonFloors[floorLevel].SpawnPoint = MapGenerator.Instance.GetSpawnPoint();
  

        // Cleanup and Spawn Player
        SpawnPlayer();
    }

    //================================//
    public void SpawnPlayer()
    {
        // Destroy Character Controller if it exists
        PlayerController existingPlayer = FindFirstObjectByType<PlayerController>();
        if (existingPlayer != null)
        {
            Destroy(existingPlayer.gameObject);
        }

        // Create new Character Controller
        PlayerController player = Instantiate(PrefabManager.Instance.Player, dungeonFloors[_currentFloor].SpawnPoint, Quaternion.identity).GetComponent<PlayerController>();
        _playerController = player;

        // Search for main camera and attach the player
        CameraController camera = FindFirstObjectByType<CameraController>();
        if (camera != null)
        {
            camera.SetTarget(player.transform);
            camera.transform.position = new Vector3(dungeonFloors[_currentFloor].SpawnPoint.x, dungeonFloors[_currentFloor].SpawnPoint.y, camera.transform.position.z);
        }
        else
        {
            Debug.LogError("CameraController not found in the scene!");
        }
    }

    //================================//
    public void OnPlayerDeath()
    {
        Debug.Log("Player has died!");
        StartCoroutine(RespawnAfterDelay());
    }

    //================================//
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        InitializeGame(_currentFloor);
    }

    //================================//
    public void CleanupEnemies()
    {
        foreach( Enemy enemy in dungeonFloors[_currentFloor].enemies )
        {
            Destroy(enemy.gameObject);
        }
        dungeonFloors[_currentFloor].enemies.Clear();
    }

    //================================//
    public void ChangeSpell(int spell)
    {
        spellCanvasImage.sprite = SpellSprites[spell];
    }

    //================================//
    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        // Clean all don't destroy on load objects
        CleanupManagers();
    }

    //================================//
    public void GoToCredits()
    {
        SceneManager.LoadScene(creditsScene);
        CleanupManagers();
    }

    //================================//
    private IEnumerator CreditsAsync()
    {
        yield return new WaitForSeconds(3f);
        GoToCredits();
    }

    //================================//
    public static void Destroy()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    //================================//
    private void CleanupManagers()
    {
        PrefabManager.Destroy();
        MapGenerator.Destroy();
        GameManager.Destroy();
    }

    public void EnterRoom(int roomIndex)
    {
        Debug.Log("Entering room: " + roomIndex);
        // Close doors for the current room
        MapGenerator.Instance.CloseDoors(roomIndex);

        // Spawn enemies TODO
    }
}