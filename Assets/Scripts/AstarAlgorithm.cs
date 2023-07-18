using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


//Grid feature for splitting up the the path in tiles and calculating the various values related to it
public class Grid<ObjectType>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs: EventArgs
    {
        public int x;
        public int y;
    }

    

    int height;
    int width;
    Vector3 origin;
    float cellsize;
    ObjectType[,] gridArray;

    public Grid(int height, int width, float cellsize, Vector3 origin, Func<Grid<ObjectType>, int, int, ObjectType> Object)
    {
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;
        this.origin = origin;

        gridArray = new ObjectType[width, height];

        for (int i = 0; i < gridArray.GetLength(0); i++)
            for (int j = 0; j < gridArray.GetLength(1); j++)
                gridArray[i, j] = Object(this, i, j);
    }

    public int GetWidth()
    {
        return gridArray.GetLength(0);
    }
    public int GetHeight()
    {
        return gridArray.GetLength(1);
    }

    private Vector3 GetWorldLocation(int x, int y, float cellsize)
    {
        return new Vector3(x, y) * cellsize + origin;
    }

    public void GetFromWorldPosition(Vector3 loc, out int x, out int y)
    {
        Vector3 newloc = loc - origin;
        x = Mathf.FloorToInt(newloc.x / cellsize);
        y = Mathf.FloorToInt(newloc.y / cellsize);
    }

    public void SetObject(int x, int y, ObjectType value)
    {
        gridArray[x, y] = value;
    }

    public ObjectType GetObject(int x, int y)
    {
        return gridArray[x, y];
    }

    public ObjectType GetObjectfromWorld(Vector3 loc)
    {
        int x;
        int y;
        GetFromWorldPosition(loc, out x, out y);
        return gridArray[x, y];
    }

    public float CellSize()
    {
        return cellsize;
    }
}

//Pathnode for individual nodes in th pathfinding algorithm
public class PathNode
{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gcost;
    public int hcost;
    public int fcost;

    public bool isWalkable;
    public PathNode FromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void Cost()
    {
        fcost = gcost + hcost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}

//A* algorithm

public class AstarAlgorithm
{
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    private const int straight = 10;
    private const int diagonal = 14;

    public static AstarAlgorithm Instance { get; private set; }
    public AstarAlgorithm(int width, int height)
    {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        grid.GetFromWorldPosition(start, out int x1, out int y1);
        grid.GetFromWorldPosition(end, out int x2, out int y2);

        List<PathNode> path = FindPath(x1, y1, x2, y2);
        if (path == null)
            return null;
        else
        {
            List<Vector3> result = new List<Vector3>();
            foreach (PathNode pathway in path)
                result.Add(new Vector2(pathway.x, pathway.y) * grid.CellSize() + Vector2.one * grid.CellSize() * 0.5f);
            return result;
        }
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetObject(startX, startY);
        PathNode endNode = grid.GetObject(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        for (int i = 0; i < grid.GetWidth(); i++)
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                PathNode node = grid.GetObject(i, j);
                node.gcost = int.MaxValue;
                node.Cost();
                node.FromNode = null;
            }

        startNode.gcost = 0;
        startNode.hcost = Distance(startNode, endNode);
        startNode.Cost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFcost(openList);
            if (currentNode == endNode)
                return CalculatePathTo(endNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbours(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                    continue;
                if(!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tempGcost = currentNode.gcost + Distance(currentNode, neighbourNode);

                if(tempGcost < neighbourNode.gcost)
                {
                    neighbourNode.FromNode = currentNode;
                    neighbourNode.gcost = tempGcost;
                    neighbourNode.hcost = Distance(neighbourNode, endNode);
                    neighbourNode.Cost();

                    if(!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        return null;
    }

    private List<PathNode> GetNeighbours(PathNode currentNode)
    {
        List<PathNode> neighbours = new List<PathNode>();

        if(currentNode.x - 1 >= 0)
        {
            neighbours.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y - 1 >= 0)
                neighbours.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight())
                neighbours.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if(currentNode.x + 1 < grid.GetWidth())
        {
            neighbours.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0)
                neighbours.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight())
                neighbours.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        if (currentNode.y - 1 >= 0)
            neighbours.Add(GetNode(currentNode.x, currentNode.y - 1));
        if (currentNode.y + 1 < grid.GetHeight())
            neighbours.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbours;
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetObject(x, y);
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    private List<PathNode> CalculatePathTo(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.FromNode != null)
        {
            path.Add(currentNode.FromNode);
            currentNode = currentNode.FromNode;
        }
        path.Reverse();
        return path;
    }

    private int Distance(PathNode a, PathNode b)
    {
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);
        int r = Mathf.Abs(x - y);
        return diagonal * Mathf.Min(x, y) + straight * r;
    }

    private PathNode GetLowestFcost(List<PathNode> list)
    {
        PathNode lowestFcost = list[0];
        for (int i = 1; i < list.Count; i++)
            if (list[i].fcost < lowestFcost.fcost)
                lowestFcost = list[i];

        return lowestFcost;
    }
}
