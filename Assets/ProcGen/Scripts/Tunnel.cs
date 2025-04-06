using UnityEngine;

public class Tunnel : MonoBehaviour
{

    public enum State
    {
        Open,
        Closed,
        Wall
    }

    private Transform open;
    private Transform closed;
    private Transform wall;

    private State state = State.Wall;

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

    public void SetState(State state)
    {
        this.state = state;
        if (open == null || closed == null || wall == null)
        {
            open = transform.Find("Open");
            closed = transform.Find("Closed");
            wall = transform.Find("Wall");
        }

        open.gameObject.SetActive(state == State.Open);
        closed.gameObject.SetActive(state == State.Closed);
        wall.gameObject.SetActive(state == State.Wall);
    }

    public State curState
    {
        get => state;
        set => SetState(value);
    }


}
