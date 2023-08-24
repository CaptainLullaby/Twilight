using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    [SerializeField] MapGenerator map;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<GameObject> enemyTypes;
    [SerializeField] List<GameObject> Objects;
    [SerializeField] GameObject Itmes;
    [SerializeField] GameObject Enemies;


    public bool placeObjects = false;
    public bool SpawnMore = false;

    private List<BoundsInt> rooms;
    private int[,] mapArr;

    private void Start()
    {
        rooms = map.GetRooms();
        mapArr = map.GetMapArray();
        PlaceObjects();
        PlaceCharas();
    }

    private void PlaceObjects()
    {
        if (placeObjects == false)
            return;

        foreach(var room in rooms)
        {
            for (int i = 0; i < room.size.x; i++)
                for (int j = 0; j < room.size.y; j++)
                    if (mapArr[room.position.x + i, room.position.y + j] == 1 && Random.value > 0.50)
                    {
                        GameObject item = Instantiate(Objects[Random.Range(0, Objects.Count)], new Vector2(room.position.x + i + 0.5f, room.position.y + j + 0.5f), Quaternion.identity);
                        item.transform.SetParent(Itmes.transform);
                    }
        }

    }

    private void PlaceCharas()
    {
        int numEnemies = Random.Range(rooms.Count / 2, rooms.Count);
        var Rooms = rooms;
        var player = Instantiate(playerPrefab, Rooms[0].center, Quaternion.identity);
        player.GetComponent<Health>().InitHealth(25);
        player.transform.SetParent(this.gameObject.transform);
        Rooms.Remove(Rooms[0]);
        foreach (var room in Rooms)
        {
            if (Random.value > 0.85)
                continue;

            var enemy = Instantiate(enemyTypes[Random.Range(0, enemyTypes.Count)], room.center, Quaternion.identity);
            enemy.transform.SetParent(Enemies.transform);
            numEnemies--;
            if (numEnemies == 0)
                break;
        }
                    
    }


    public void Spawner(Transform target)
    {
        var player = GameObject.FindGameObjectWithTag("PlayerChar");
        int numEnemies = Random.Range(3, rooms.Count / 2);
        var spawnroom = rooms[0];
        foreach (var room in rooms)
            if (Vector2.Distance(player.transform.position, room.center) > Vector2.Distance(player.transform.position, spawnroom.center))
                spawnroom = room;

        while(numEnemies > 0)
        {
            var enemy = Instantiate(enemyTypes[Random.Range(0, enemyTypes.Count)], spawnroom.center, Quaternion.identity);
            enemy.transform.SetParent(this.gameObject.transform.parent);

            if (enemy.GetComponent<AIBrute>())
                enemy.GetComponent<AIBrute>().SetTarget(target.transform);
            if (enemy.GetComponent<AISpotter>())
                enemy.GetComponent<AISpotter>().SetTarget(target.transform);
            if (enemy.GetComponent<AISoldier>())
                enemy.GetComponent<AISoldier>().SetTarget(target.transform);

            enemy.transform.SetParent(Enemies.transform);

            numEnemies--;
        }

    }
}
