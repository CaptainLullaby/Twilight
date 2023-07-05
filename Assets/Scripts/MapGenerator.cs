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

    public GameObject ground; //Remember the ground format is always 16:9

    public GameObject groundtile;
    public GameObject borderwall;

    private List<BoundsInt> rooms;
    private Queue<BoundsInt> roomQ;
    private GameObject[,] groundlayout;
    private int[,] map;

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

        //new map layout
        map = new int[x, y];
        groundlayout = new GameObject[x, y];

        //Set minimum width and height as 7
        if (minheight == 0 && minwidth == 0)
        {
            minheight = 7;
            minwidth = 7;
        }

        rooms = new List<BoundsInt>();
        roomQ = new Queue<BoundsInt>();
        BSP();
        showrooms();
        UpdateBorder();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResetMap();
            BSP();
            showrooms();
            UpdateBorder();
        }
    }
    //---------------------------------------

    private void showrooms()
    {
        createrooms();
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                if (groundlayout[i, j] != null)
                    Destroy(groundlayout[i, j]);

        for (int i = 0; i < x; i++)
            for(int j=0; j < y; j++)
            {
                if (map[i, j] == 1)
                    groundlayout[i, j] = Instantiate(groundtile, new Vector3(i - x / 2 + 0.5f, j - y / 2 + 0.5f, 0), Quaternion.identity);
                else
                    groundlayout[i, j] = Instantiate(borderwall, new Vector3(i - x / 2 + 0.5f, j - y / 2 + 0.5f, 0), Quaternion.identity);
            }
    }

    private void createrooms()
    {
        Debug.Log(rooms.Count);
        foreach (var room in rooms)
            for (int i = offset; i < room.size.x - offset + 1; i++)
                for (int j = offset; j < room.size.y - offset + 1; j++)
                    map[room.position.x + i, room.position.y + j] = 1;
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
            {
                if (map[i, j] != 1)
                    map[i, j] = 0;
            }                    
    }
    //---------------------------------------

    private void BSP()
    {
        var room = new BoundsInt();
        room.size = new Vector3Int(x - 1, y - 1, 0);
        room.position = new Vector3Int(0, 0, 0);
        roomQ.Enqueue(room);
        while (roomQ.Count > 0)
        {
            room = roomQ.Dequeue();
            if(room.size.x >= 2 * minwidth || room.size.y >= 2 * minheight) //&& Random.value < 0.95)
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
        //Debug.Log("Pass Horizontal:");
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
        //Debug.Log("Pass Vertical:");
        int ySplit = Random.Range(minheight, room.size.y - minheight);
        var room_top = new BoundsInt();
        room_top.size = new Vector3Int(room.size.x, ySplit, room.size.z);
        room_top.position = new Vector3Int(room.position.x, room.position.y, room.position.z);

        var room_bottom = new BoundsInt();
        room_bottom.size = new Vector3Int(room.size.x , room.size.y - ySplit, room.size.z);
        room_bottom.position = new Vector3Int(room.position.x, room.position.y + ySplit, room.position.z);

        roomQ.Enqueue(room_top);
        roomQ.Enqueue(room_bottom);
    }

    //---------------------------------------
    private void ResetMap()
    {
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                map[i, j] = 0;
    }
    //---------------------------------------

    private void UpdateBorder()
    {
        ground.transform.localScale = new Vector3(x, y, 1);
        GameObject borderwall_bottom = Instantiate(borderwall, new Vector3(-0.5f, -(float)(y + 1) / 2, 0), Quaternion.identity);
        borderwall_bottom.transform.localScale = new Vector3(x + 1, 1, 1);

        GameObject borderwall_left = Instantiate(borderwall, new Vector3(-(float)(x + 1) / 2, 0.5f, 0), Quaternion.identity);
        borderwall_left.transform.localScale = new Vector3(1, y + 1, 1);

        GameObject borderwall_top = Instantiate(borderwall, new Vector3(0.5f, (float)(y + 1) / 2, 0), Quaternion.identity);
        borderwall_top.transform.localScale = new Vector3(x + 1, 1, 1);

        GameObject borderwall_right = Instantiate(borderwall, new Vector3((float)(x + 1) / 2, -0.5f, 0), Quaternion.identity);
        borderwall_right.transform.localScale = new Vector3(1, y + 1, 1);
    }
}
