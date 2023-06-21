using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int x;
    public int y;
    public int minwidth;
    public int minheight;
    //public int offset;
    public Vector3Int start;

    public GameObject ground; //Remember the ground format is always 16:9
    public GameObject borderwall_left;
    public GameObject borderwall_right;
    public GameObject borderwall_top;
    public GameObject borderwall_bottom;
    public GameObject groundtile;

    private List<BoundsInt> rooms;
    private Queue<BoundsInt> roomQ;
    private int[,] maplayout;

    // Start is called before the first frame update
    void Awake()
    {
        //the initial positions
        if (x == 0 && y == 0)
        {
            x = (int)ground.transform.localScale.x;
            y = (int)ground.transform.localScale.y;

        }

        //Set start vector as a position
        if (start == Vector3Int.zero)
            start = new Vector3Int(0, 0, 0);

        //Set minimum width and height as 7
        if(minheight == 0 && minwidth == 0)
        {
            minheight = 7;
            minwidth = 7;
        }

        //Map initialized as 0's
        maplayout = new int[x, y];
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                maplayout[i, j] = 0;

        rooms = new List<BoundsInt>();
        roomQ = new Queue<BoundsInt>();
        BSP();
        showrooms();
        UpdateBorder();
    }

    private void showrooms()
    {
        Debug.Log(rooms.Count);
        foreach (var room in rooms)
            Debug.Log("Room: X: " + room.x + "\t Y: " + room.y);
    }

    private void BSP()
    {
        var room = new BoundsInt();
        room.size = new Vector3Int(x, y, 0);
        room.position = new Vector3Int();
        roomQ.Enqueue(room);
        while (roomQ.Count > 0)
        {
            room = roomQ.Dequeue();
            if((room.x >= 2 * minwidth || room.y >= 2 * minheight) && Random.value < 0.95)
            {
                if (Random.value > 0.5 && room.x >= 2 * minwidth)
                    SplitHorizontally(room);
                else if (room.y >= 2 * minheight)
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
        int xSplit = Random.Range(minwidth, room.x - minwidth);
        //roomQ.Enqueue(new Vector2Int(xSplit, room.y));
        //roomQ.Enqueue(new Vector2Int(room.x - xSplit, room.y));
    }

    private void SplitVertically(BoundsInt room)
    {
        Debug.Log("Pass Vertical:");
        int ySplit = Random.Range(minheight, room.y - minheight);
        //roomQ.Enqueue(new Vector2Int(room.x, ySplit));
        //roomQ.Enqueue(new Vector2Int(room.x, room.y - ySplit));
    }

    private void UpdateBorder()
    {
        ground.transform.localScale = new Vector3(x, y, 1);
        borderwall_bottom.transform.localScale = new Vector3(x + 1, 1, 1);
        borderwall_left.transform.localScale = new Vector3(1, y + 1, 1);
        borderwall_top.transform.localScale = new Vector3(x + 1, 1, 1);
        borderwall_right.transform.localScale = new Vector3(1, y + 1, 1);
    }
}
