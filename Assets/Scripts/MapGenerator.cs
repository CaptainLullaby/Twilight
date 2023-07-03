using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int x;
    public int y;
    public int minwidth;
    public int minheight;
    public int offset;
    public Vector3Int start;

    public GameObject ground; //Remember the ground format is always 16:9
    public GameObject borderwall_left;
    public GameObject borderwall_right;
    public GameObject borderwall_top;
    public GameObject borderwall_bottom;
    public GameObject groundtile;

    private List<BoundsInt> rooms;
    private Queue<BoundsInt> roomQ;
    private GameObject groundlayout;

    // Start is called before the first frame update
    void Awake()
    {
        //the initial positions
        if (x == 0 && y == 0)
        {
            x = (int)ground.transform.localScale.x;
            y = (int)ground.transform.localScale.y;

        }
        offset = offset == 0 ? 1 : offset;
        //Set start vector as a position
        if (start == Vector3Int.zero)
            start = new Vector3Int(0, 0, 0);

        //Set minimum width and height as 7
        if(minheight == 0 && minwidth == 0)
        {
            minheight = 7;
            minwidth = 7;
        }

        rooms = new List<BoundsInt>();
        roomQ = new Queue<BoundsInt>();
        BSP();
        //showrooms();
        createrooms();
        UpdateBorder();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            BSP();
            //showrooms();
            createrooms();
            UpdateBorder();
        }
    }
    //---------------------------------------

    private void showrooms()
    {
        Debug.Log(rooms.Count);
        foreach (var room in rooms)
        {
            Debug.Log("Room Size: \nX: " + room.size.x + "\t Y: " + room.size.y);
            Debug.Log("Room Position: \nX: " + room.position.x + "\t Y: " + room.position.y);
        }

    }

    private void createrooms()
    {
        if (ground.transform.childCount > 0)
            for (int c = ground.transform.childCount - 1; c >= 0; c--)
                Destroy(ground.transform.GetChild(c).gameObject);
        foreach (var room in rooms)
            for (int i = offset; i < room.size.x - offset; i++)
                for (int j = offset; j < room.size.y - offset; j++)
                {
                    groundlayout = Instantiate(groundtile, new Vector3(room.position.x + i, room.position.y + j, 0), Quaternion.identity);
                    groundlayout.transform.SetParent(ground.transform);
                }
    }
    //---------------------------------------

    private void BSP()
    {
        var room = new BoundsInt();
        room.size = new Vector3Int(x, y, 0);
        room.position = new Vector3Int(- x/2, - y/2, 0);
        roomQ.Enqueue(room);
        while (roomQ.Count > 0)
        {
            room = roomQ.Dequeue();
            if((room.size.x >= 2 * minwidth || room.size.y >= 2 * minheight) && Random.value < 0.95)
            {
                if (Random.value > 0.5 && room.size.x >= 2 * minwidth)
                    SplitHorizontally(room);
                else if (room.size.y >= 2 * minheight)
                    SplitVertically(room);
                else
                    continue;
            }
            else
                rooms.Add(room);
        }
    }

    private void SplitHorizontally(BoundsInt room)
    {
        Debug.Log("Pass Horizontal:");
        int xSplit = Random.Range(minwidth, room.size.x - minwidth);
        var room_left = new BoundsInt();
        room_left.size = new Vector3Int(xSplit, room.size.y, room.size.z);
        room_left.position = new Vector3Int(room.position.x, room.position.y, room.position.z);

        var room_right = new BoundsInt();
        room_right.size = new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z);
        room_right.position = new Vector3Int(room.position.x + xSplit, room.position.y, room.position.z);

        roomQ.Enqueue(room_left);
        roomQ.Enqueue(room_right);
        //roomQ.Enqueue(new Vector2Int(xSplit, room.y));
        //roomQ.Enqueue(new Vector2Int(room.x - xSplit, room.y));
    }

    private void SplitVertically(BoundsInt room)
    {
        Debug.Log("Pass Vertical:");
        int ySplit = Random.Range(minheight, room.size.y - minheight);
        var room_top = new BoundsInt();
        room_top.size = new Vector3Int(room.size.x, ySplit, room.size.z);
        room_top.position = new Vector3Int(room.position.x, room.position.y, room.position.z);

        var room_bottom = new BoundsInt();
        room_bottom.size = new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z);
        room_bottom.position = new Vector3Int(room.position.x, room.position.y + ySplit, room.position.z);

        roomQ.Enqueue(room_top);
        roomQ.Enqueue(room_bottom);
        //roomQ.Enqueue(new Vector2Int(room.x, ySplit));
        //roomQ.Enqueue(new Vector2Int(room.x, room.y - ySplit));
    }

    //---------------------------------------

    private void UpdateBorder()
    {
        ground.transform.localScale = new Vector3(x, y, 1);
        borderwall_bottom.transform.localScale = new Vector3(x + 1, 1, 1);
        borderwall_left.transform.localScale = new Vector3(1, y + 1, 1);
        borderwall_top.transform.localScale = new Vector3(x + 1, 1, 1);
        borderwall_right.transform.localScale = new Vector3(1, y + 1, 1);
    }
}
