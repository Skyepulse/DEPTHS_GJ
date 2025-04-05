using UnityEngine;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public struct Door
    {
        public Vector3Int position;
        public MapGenerator.Direction direction;
        public Room room;
    }

    public Door[] doors;

    public GameObject GetPrefab()
    {
        return gameObject;
    }

    public void OnDrawGizmosSelected()
    {
        foreach (Door door in doors)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + (Vector3)door.position + Vector3.one * 0.5f, 0.1f);

            Gizmos.color = Color.green;
            Vector3 direction = door.direction switch
            {
                MapGenerator.Direction.North => Vector3.up,
                MapGenerator.Direction.East => Vector3.right,
                MapGenerator.Direction.South => Vector3.down,
                MapGenerator.Direction.West => Vector3.left,
                _ => Vector3.zero,
            };
            Gizmos.DrawLine(transform.position + (Vector3)door.position + Vector3.one * 0.5f, transform.position + (Vector3)door.position + Vector3.one * 0.5f + direction);
        }
    }

}
