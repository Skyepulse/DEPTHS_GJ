using UnityEngine;

public class Tunnel : MonoBehaviour
{
    private Transform open;
    private Transform closed;

    private bool isOpen = false;

    [SerializeField]
    private MapGenerator.Direction direction = MapGenerator.Direction.None;

    public MapGenerator.Direction Direction
    {
        get => direction;
        set
        {
            direction = value;
            transform.rotation = Quaternion.Euler(0, 0, (int)direction * 90);
        }
    }

    void OnValidate()
    {
        Direction = direction;
    }

    public void SetOpen(bool isOpen)
    {
        this.isOpen = isOpen;
        if (open == null || closed == null)
        {
            open = transform.Find("Open");
            closed = transform.Find("Closed");
        }

        open.gameObject.SetActive(isOpen);
        closed.gameObject.SetActive(!isOpen);
    }

    public bool IsOpen
    {
        get => isOpen;
        set => SetOpen(value);
    }


}
