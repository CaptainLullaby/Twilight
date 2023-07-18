using System.Collections.Generic;
using UnityEngine;


public class Tree
{

}
public class MapGenerator : MonoBehaviour
{
    public int x;
    public int y;
    public int minwidth;
    public int minheight;
    public int offset;

    public GameObject Ground; //Remember the ground format is always 16:9
    public GameObject Wall; //Walls

    public GameObject groundtile;
    public GameObject borderwall;

    public GameObject Player;
    public GameObject enemyAI;

    private List<BoundsInt> rooms;
    private Queue<BoundsInt> roomQ;
    private GameObject[,] groundlayout;
    private int[,] map;
    private int minroomcount;
    private List<GameObject> enemiesAI;

    // Start is called before the first frame update
    void Awake()
    {
        //the initial positions
        if (x == 0 && y == 0)
        {
            x = 32;
            y = 18;

        }
        offset = offset == 0 ? 1 : offset;

        //new map layout
        map = new int[x, y];
        groundlayout = new GameObject[x, y];

        //Set minimum width and height as 7
        if (minheight == 0 && minwidth == 0)
        {
            minheight = 4;
            minwidth = 4;
        }
        minroomcount = (x * y) / (4 * minwidth * minheight);
        //Debug.Log(minroomcount);
        PCG();
        PlaceCharacters();
    }

    private void Update()
    {
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
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                if (groundlayout[i, j] != null)
                    Destroy(groundlayout[i, j]);

        for (int i = 0; i < x; i++)
            for(int j=0; j < y; j++)
            {
                if (map[i, j] == 1)
                {
                    groundlayout[i, j] = Instantiate(groundtile, new Vector3(i - x / 2 + 0.5f, j - y / 2 + 0.5f, 0), Quaternion.identity);
                    groundlayout[i, j].transform.SetParent(Ground.transform);
                }
                else
                {
                    groundlayout[i, j] = Instantiate(borderwall, new Vector3(i - x / 2 + 0.5f, j - y / 2 + 0.5f, 0), Quaternion.identity);
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
        newroom.size = new Vector3Int(x - 1, y - 1, 0);
        newroom.position = new Vector3Int(0, 0, 0);
        roomQ.Enqueue(newroom);
        ResetMap();
        while (roomQ.Count > 0)
        {
            newroom = roomQ.Dequeue();
            //old code
            /*if((newroom.size.x >= 2 * minwidth && newroom.size.y >= 2 * minheight) && Random.value < 0.95)
            {
                if (Random.value > 0.5 && newroom.size.x >= 2 * minwidth)
                    SplitHorizontally(newroom);
                else if (newroom.size.y >= 2 * minheight)
                    SplitVertically(newroom);
                else
                    continue;
            }
            else
                rooms.Add(newroom);*/

            //new code
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
        Debug.Log("Rooms: " + rooms.Count);
        foreach (var room in rooms)
            for (int i = 0; i < room.size.x; i++)
                for (int j = 0; j < room.size.y; j++)
                    map[room.position.x + i, room.position.y + j] = 1;
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
            {
                if (map[i, j] != 1)
                    map[i, j] = 0;
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

        Debug.Log("Room centers: " + roomcenters.Count);

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
            
            var startx = currentroom.x < nearestroom.x ? currentroom.x : nearestroom.x;
            var endx = currentroom.x > nearestroom.x ? currentroom.x : nearestroom.x;
            
            var starty = currentroom.y < nearestroom.y ? currentroom.y : nearestroom.y;
            var endy = currentroom.y > nearestroom.y ? currentroom.y : nearestroom.y;

            if (currentroom.x != nearestroom.x)
            {
                var start = currentroom.x < nearestroom.x ? currentroom.x : nearestroom.x;
                var end = currentroom.x > nearestroom.x ? currentroom.x : nearestroom.x;
                for (int i = start; i <= end; i++)
                    map[i, currentroom.y] = 1;
            }
            if (currentroom.y != nearestroom.y)
            {
                var start = currentroom.y < nearestroom.y ? currentroom.y : nearestroom.y;
                var end = currentroom.y > nearestroom.y ? currentroom.y : nearestroom.y;

                for (int i = start; i <= end; i++)
                    map[currentroom.x, i] = 1;
            }
            if (Random.value > 0.95)
                roomcenters.Add(currentroom); ;
            //float xDist = currentroom.x > nearestroom.x ? currentroom.x - nearestroom.x : nearestroom.x - currentroom.x;
            //float yDist = currentroom.y > nearestroom.y ? currentroom.y - nearestroom.y : nearestroom.y - currentroom.y;
            //Debug.Log("X: " + xDist + ": " + currentroom.x + " -> " + nearestroom.x + "\nY: " + yDist + ": " + currentroom.y + " -> " + nearestroom.y);
            
            currentroom = nearestroom;
        }
    }

    //---------------------------------------
    //Get rooms

    public List<BoundsInt> GetRooms()
    {
        return rooms;
    }

    public Vector2 GetMapSize()
    {
        return new Vector2(x, y);
    }

    //---------------------------------------
    private void PlaceCharacters()
    {
        enemiesAI = new List<GameObject>();
        int enemyCount = Random.Range(1, rooms.Count);
        int offsetsize;

        var playerroom = rooms[Random.Range(0, rooms.Count)];
        offsetsize = playerroom.size.x / 2 + offset;
        int i = Random.Range(playerroom.position.x + offsetsize, playerroom.size.x - offsetsize);
        int j = Random.Range(playerroom.position.y + offsetsize, playerroom.size.y - offsetsize);
        Instantiate(Player, new Vector3(i - x / 2 + 0.5f, j - y / 2 + 0.5f, 0f), Quaternion.identity);

        foreach (var room in rooms)
        {
            offsetsize = room.size.x / 2 + offset;
            i = Random.Range(room.position.x + offsetsize, room.size.x - offsetsize);
            j = Random.Range(room.position.y + offsetsize, room.size.y - offsetsize);
            if (enemyCount != 0)
            {
                GameObject enemy = Instantiate(enemyAI, new Vector3(i - x / 2 + 0.5f, j - y / 2 + 0.5f, 0f), Quaternion.identity);
                enemiesAI.Add(enemy);
                enemyCount--;
            }
            else
                continue;
        }
    }

    //
}
