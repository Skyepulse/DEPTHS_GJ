using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

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
        public Transform SpawnPoint;
        public List<Enemy> enemies;
    }

    //================================//
    // Settings
    [Header("Settings")]
    [SerializeField] private DungeonFloor[] dungeonFloors = null;


    //================================//
    private PlayerController     _playerController   = null;
    public PlayerController      Player => _playerController;
    private int                 _currentFloor = 0;
    public int                  CurrentFloor => _currentFloor;

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
    }

    //================================//
    public void InitializeGame(int floorLevel)
    {
        // Cleanup Enemies
        CleanupEnemies();

        // Create dungeon floor
        int numberOfRooms = dungeonFloors[floorLevel].roomCount;
        int difficultyLevel = dungeonFloors[floorLevel].difficultyLevel;

        // TODO

  

        // Cleanup and Spawn Player
        SpawnPlayer();
    }

    //================================//
    public void SpawnPlayer()
    {
        // Destroy Character Controller if it exists
        PlayerController existingPlayer = FindFirstObjectOfType<PlayerController>();
        if (existingPlayer != null)
        {
            Destroy(existingPlayer.gameObject);
        }

        // Create new Character Controller
        PlayerController player = Instantiate(PrefabManager.Instance.Player, dungeonFloors[_currentFloor].SpawnPoint.position, Quaternion.identity).GetComponent<PlayerController>();
        _playerController = player;

        // Search for main camera and attach the player
        CameraController camera = FindFirstObjectOfType<CameraController>();
        if (camera != null)
        {
            camera.SetTarget(player.transform);
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
}