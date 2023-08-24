using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapSize, minwidth, minheight, offset;

    [SerializeField] private GameObject Ground, Wall; //Remember the ground format is always 16:9
    [SerializeField] private GameObject roomtile, walltile;

    private int width, height, minroomcount;
    private List<BoundsInt> rooms;
    private Queue<BoundsInt> roomQ;
    private GameObject[,] groundlayout;
    private int[,] map;

    public bool test = true;

    // Start is called before the first frame update
    void Awake()
    {
        //the initial positions
        if (width == 0 && height == 0)
        {
            width = 16 * mapSize;
            height = 9 * mapSize;

        }
        offset = offset == 0 ? 1 : offset;

        //new map layout
        map = new int[width, height];
        groundlayout = new GameObject[width, height];

        //Set minimum width and height as 7
        if (minheight == 0 && minwidth == 0)
        {
            minheight = 4;
            minwidth = 4;
        }
        minroomcount = (width * height) / (4 * minwidth * minheight);
        //Debug.Log(minroomcount);
        PCG();
    }
    //---------------------------------------

    private void PCG()
    {
        BSP();
        CreateCorridors();
        showrooms();
    }

    //---------------------------------------

    private void showrooms()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (groundlayout[i, j] != null)
                    Destroy(groundlayout[i, j]);

        for (int i = 0; i < width; i++)
            for(int j = 0; j < height; j++)
            {
                if (map[i, j] != 0)
                {
                    groundlayout[i, j] = Instantiate(roomtile, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
                    groundlayout[i, j].transform.SetParent(Ground.transform);
                }
                else
                {
                    groundlayout[i, j] = Instantiate(walltile, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
                    groundlayout[i, j].transform.SetParent(Wall.transform);
                }
            }
    }
    //---------------------------------------

    private void BSP()
    {
        rooms = new List<BoundsInt>();
        roomQ = new Queue<BoundsInt>();
        var newroom = new BoundsInt();
        newroom.size = new Vector3Int(width - 1, height - 1, 0);
        newroom.position = new Vector3Int(0, 0, 0);
        roomQ.Enqueue(newroom);
        ResetMap();
        while (roomQ.Count > 0)
        {
            newroom = roomQ.Dequeue();

            if (rooms.Count >= minroomcount && Random.value > 0.95f)
                AddRooms(newroom);
            else if (newroom.size.x >= 2 * minwidth && newroom.size.y >= 2 * minheight)
            {
                if (Random.value > 0.5)
                    SplitHorizontally(newroom);
                else
                    SplitVertically(newroom);
            }
            else if (newroom.size.x >= 2 * minwidth)
                SplitHorizontally(newroom);
            else if (newroom.size.y >= 2 * minheight)
                SplitVertically(newroom);
            else
                if (Random.value < 0.75)
                AddRooms(newroom);
        }

        //Creating rooms in an array and then using said array to render the rooms
        //Debug.Log("Rooms: " + rooms.Count);
        foreach (var room in rooms)
            for (int i = 0; i < room.size.x; i++)
                for (int j = 0; j < room.size.y; j++)
                    map[room.position.x + i, room.position.y + j] = 1;
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
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                map[i, j] = 0;
    }
    //---------------------------------------

    private void AddRooms(BoundsInt thisroom)
    {
        thisroom.position = new Vector3Int(thisroom.position.x + offset, thisroom.position.y + offset, 0);
        thisroom.size = new Vector3Int(thisroom.size.x - offset, thisroom.size.y - offset, 0);
        rooms.Add(thisroom);
    }

    private void CreateCorridors()
    {
        List<Vector3Int> roomcenters = new List<Vector3Int>();
        foreach (var room in rooms)
            roomcenters.Add(Vector3Int.RoundToInt(room.center));
            //Debug.Log(room.center);

        //Debug.Log("Room centers: " + roomcenters.Count);

        var currentroom = roomcenters[Random.Range(0, roomcenters.Count)];
        roomcenters.Remove(currentroom);

        while (roomcenters.Count > 0)
        {

            var nearestroom = roomcenters[0];
            //Finds the nearest room to the selected one
            foreach (var roomcenter in roomcenters)
                if (Vector3.Distance(roomcenter, currentroom) <= Vector3.Distance(nearestroom, currentroom))
                    nearestroom = roomcenter;

            //Debug.Log("Current Room: " + currentroom);
            //Debug.Log("Nearest Room: " + nearestroom);

            roomcenters.Remove(nearestroom);

            if (currentroom.x != nearestroom.x)
            {
                int start, end;

                if(currentroom.x < nearestroom.x)
                {
                    start = currentroom.x;
                    end = nearestroom.x;
                }
                else
                {
                    start = nearestroom.x;
                    end = currentroom.x;
                }

                for (int i = start; i <= end; i++)
                {
                        map[i, currentroom.y] = 2;
                        map[i, nearestroom.y] = 2;
                }
            }
            if (currentroom.y != nearestroom.y)
            {
                int start, end;

                if (currentroom.y < nearestroom.y)
                {
                    start = currentroom.y;
                    end = nearestroom.y;
                }
                else
                {
                    start = nearestroom.y;
                    end = currentroom.y;
                }

                for (int i = start; i <= end; i++)
                {
                        map[currentroom.x, i] = 2;
                        map[nearestroom.x, i] = 2;
                }
            }
            if (Random.value > 0.75)
                roomcenters.Add(currentroom); ;

            currentroom = nearestroom;
        }
    }

    //---------------------------------------
    //Get rooms

    public List<BoundsInt> GetRooms()
    {
        return rooms;
    }

    public Vector3 GetMapSize()
    {
        return new Vector3(width, height, 0);
    }

    public int[,] GetMapArray()
    {
        return map;
    }
}
