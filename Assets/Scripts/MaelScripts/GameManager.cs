using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
        public int enemyCount;
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
    [SerializeField] private string             mainMenuScene = "Main Menu Scene";
    [SerializeField] private string             creditsScene = "CreditsScene";

    [Header("Tutorial")]
    [SerializeField] private GameObject         tutorial1 = null;
    [SerializeField] private GameObject         tutorial2 = null;
    [SerializeField] private GameObject         tutorial3 = null;
    [SerializeField] private GameObject         tutorial4 = null;
    [SerializeField] private GameObject         tutorial5 = null;
    private int                                 _currentTutorial = 0;
    private float                               _tutorialTime = 2f;
    private float                               _finalTutorialTimeMax = 5f;
    private float                               _tutorialTimer = 0f;
    public int GetCurrentTutorial()
    {
        if (_tutorialTimer > 0f)
        {
            return -1;
        }
        return _currentTutorial;
    }


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

        fullScreenMatReference.SetFloat("_Active", 1f);      

        Enemy.onEnemyDeath += EnemyDied;
    }

    //================================//
    private void EnemyDied(Enemy enemy)
    {
        if (enemy == null) return;
        if (_currentFloor < 0 || _currentFloor >= dungeonFloors.Length) return;
        dungeonFloors[_currentFloor].enemies.Remove(enemy);
        dungeonFloors[_currentFloor].enemyCount--;
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

        NextTutorial();
        InitializeGame(0);
    }

    //================================//
    // METHODS
    public void OnFloorWin()
    {
        CleanupEnemies();
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

    bool _isDoorsClosed = false;

    //================================//
    void Update()
    {
        if (_tutorialTimer > 0f)
        {
            _tutorialTimer -= Time.deltaTime;
            if (_tutorialTimer <= 0f)
            {
                _tutorialTimer = 0f;
                if (_currentTutorial == 5) NextTutorial();
            }
        }

        // If doors are closed and no enemies left, open doors
        if (_isDoorsClosed && dungeonFloors[_currentFloor].enemies.Count == 0)
        {
            MapGenerator.Instance.OpenDoors(lastRoomIndex);
            if (_playerController != null)
                _playerController.PlayCloseDoor();
            _isDoorsClosed = false;
        }
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
        yield return new WaitForSeconds(2f);
        InitializeGame(_currentFloor);
    }

    //================================//
    public void CleanupEnemies()
    {
        if (_currentFloor < 0 || _currentFloor >= dungeonFloors.Length)
        {
            Debug.LogError("Current floor index is out of bounds!");
            return;
        }

        foreach( Enemy enemy in dungeonFloors[_currentFloor].enemies )
        {
            if (enemy == null) continue;
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

            Enemy.onEnemyDeath -= _instance.EnemyDied;
        }
    }

    //================================//
    private void CleanupManagers()
    {
        PrefabManager.Destroy();
        MapGenerator.Destroy();
        GameManager.Destroy();
    }

    //================================//
    public void NextTutorial()
    {
        TMP_Text text = null;
        switch (_currentTutorial)
        {
            case 1:
                text = tutorial1.GetComponentInChildren<TMP_Text>();
                text.color = Color.green;
                break;
            case 2:
                text = tutorial2.GetComponentInChildren<TMP_Text>();
                text.color = Color.green;
                break;
            case 3:
                text = tutorial3.GetComponentInChildren<TMP_Text>();
                text.color = Color.green;
                break;
            case 4:
                text = tutorial4.GetComponentInChildren<TMP_Text>();
                text.color = Color.green;
                break;
        }

        if (_currentTutorial < 5)
        {
            _currentTutorial++;
            StartCoroutine(ShowTutorial(_currentTutorial));
            if(_currentTutorial < 4 )_tutorialTimer = _tutorialTime;
            else _tutorialTimer = _finalTutorialTimeMax;
        }
        else
        {
            tutorial1.SetActive(false);
            tutorial2.SetActive(false);
            tutorial3.SetActive(false);
            tutorial4.SetActive(false);
            tutorial5.SetActive(false);
        }
    }

    //================================//
    private IEnumerator ShowTutorial(int i, float time = 2f)
    {
        //Wait 2s
        yield return new WaitForSeconds(time);
        switch (i)
        {
            case 1:
                tutorial1.SetActive(true);
                break;
            case 2:
                tutorial1.SetActive(false);
                tutorial2.SetActive(true);
                break;
            case 3:
                tutorial2.SetActive(false);
                tutorial3.SetActive(true);
                break;
            case 4:
                tutorial3.SetActive(false);
                tutorial4.SetActive(true);
                break;
            case 5:
                tutorial4.SetActive(false);
                tutorial5.SetActive(true);
                break;
        }
    }

    private int lastRoomIndex = -1;

    public void SetCurrentRoom(int roomIndex)
    {
        if (roomIndex != lastRoomIndex)
        {
            lastRoomIndex = roomIndex;
            EnterRoom(roomIndex);
        }
    }

    //================================//
    public void SpawnEnemies()
    {
        if (lastRoomIndex > 0 && lastRoomIndex < dungeonFloors[_currentFloor].roomCount - 1)
        {
            Room room = MapGenerator.Instance.GetRoomNode(lastRoomIndex).room.GetComponentInChildren<Room>();
            if (room != null)
            {
                // num of enemies to spawn between 1 and maxRoomEnemies
                int numEnemies = UnityEngine.Random.Range(1, dungeonFloors[_currentFloor].maxRoomEnemies + 1);

                for( int i = 0; i < numEnemies; i++ )
                {
                    GameObject randomEnemyPrefab = dungeonFloors[_currentFloor].enemyPrefabs[UnityEngine.Random.Range(0, dungeonFloors[_currentFloor].enemyPrefabs.Count)];
                    Transform spawnPosition = room.GetRandomSpawnPoint();
                    if (spawnPosition != null && randomEnemyPrefab != null)
                    {
                        GameObject enemy = Instantiate(randomEnemyPrefab, spawnPosition.position, Quaternion.identity);
                        Enemy enemyComponent = enemy.GetComponent<Enemy>();
                        if (enemyComponent != null)
                        {
                            dungeonFloors[_currentFloor].enemies.Add(enemyComponent);
                            dungeonFloors[_currentFloor].enemyCount++;
                        }
                        else
                        {
                            Debug.LogError("Enemy prefab does not have an Enemy component!");
                        }
                    }
                    else
                    {
                        Debug.LogError("No spawn point found in the room, or enemy is null!");
                    }
                }
            }
        }
    }

    //================================//
    public void EnterRoom(int roomIndex)
    {
        Debug.Log("Entering room: " + roomIndex);
        // Close doors for the current room
        if(roomIndex > 0) {
            MapGenerator.Instance.CloseDoors(roomIndex);
            if (_playerController != null)
                _playerController.PlayCloseDoor();
            _isDoorsClosed = true;
        }

        // Spawn enemies TODO
        SpawnEnemies();
    }
}