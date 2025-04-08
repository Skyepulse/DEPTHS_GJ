using UnityEngine;

public class Eggs : Enemy
{
    [SerializeField] private float hatchTime = 4f;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private GameObject hatchEffect;
    private float timer;

    void Start()
    {
        timer = hatchTime;
    }

    override public void Awake()
    {
        base.Awake();
        if (minionPrefab == null)
        {
            Debug.LogError("Minion prefab is not assigned in the inspector.");
        }
    }

    override public void Update()
    {
        base.Update();
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Hatch();
            Destroy(gameObject);

        }
    }

    private void Hatch()
    {
        Debug.Log("Hatching!");
        Instantiate(minionPrefab, transform.position, Quaternion.identity);
        if (hatchEffect != null)
        {
            Instantiate(hatchEffect, transform.position, Quaternion.identity);
        }
    }




}
