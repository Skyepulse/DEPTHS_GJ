using UnityEngine;

// Singleton class to manage prefabs
public class PrefabManager : MonoBehaviour
{
    //================================//
    public static PrefabManager Instance { get; private set; }

    //================================//
    [SerializeField] private GameObject         electricSpellPrefab;
    [SerializeField] private GameObject         waveSpellPrefab;
    [SerializeField] private GameObject         playerPrefab;
    [SerializeField] private GameObject         damageEffectPrefab;
    [SerializeField] private GameObject         enemydamageEffectPrefab;

    //================================//
    public GameObject                           ElectricSpell   => electricSpellPrefab ?? throw new System.NullReferenceException("Electric spell prefab is not assigned!");
    public GameObject                           WaveSpell       => waveSpellPrefab ?? throw new System.NullReferenceException("Wave spell prefab is not assigned!");
    public GameObject                           Player          => playerPrefab ?? throw new System.NullReferenceException("Player prefab is not assigned!");
    public GameObject                           DamageEffect     => damageEffectPrefab ?? throw new System.NullReferenceException("Damage effect prefab is not assigned!");
    public GameObject                           EnemyDamageEffect => enemydamageEffectPrefab ?? throw new System.NullReferenceException("Enemy damage effect prefab is not assigned!");
    
    //================================//
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //================================//
    private void Start()
    {
        if (electricSpellPrefab == null || waveSpellPrefab == null || playerPrefab == null)
        {
            Debug.LogError("One or more prefabs are not assigned in the PrefabManager!");
        }
    }
}