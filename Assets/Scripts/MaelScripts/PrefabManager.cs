using UnityEngine;

// Singleton class to manage prefabs
public class PrefabManager : MonoBehaviour
{
    //================================//
    public static PrefabManager Instance { get; private set; }

    //================================//
    [SerializeField] private GameObject         electricSpellPrefab;
    [SerializeField] private GameObject         waveSpellPrefab;

    //================================//
    public GameObject                           ElectricSpell   => electricSpellPrefab ?? throw new System.NullReferenceException("Electric spell prefab is not assigned!");
    public GameObject                           WaveSpell       => waveSpellPrefab ?? throw new System.NullReferenceException("Wave spell prefab is not assigned!");

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
}