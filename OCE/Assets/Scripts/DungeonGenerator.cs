using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum RoomType 
    {
        free = -1,
        normal,
        boss, 
        treasure,
        shop,
    }

    [Header("Prefab")]
    [SerializeField] private GameObject room1Door;
    [SerializeField] private GameObject room2DoorC;
    [SerializeField] private GameObject room2DoorS;
    [SerializeField] private GameObject room3Door;
    [SerializeField] private GameObject room4Door;
    [SerializeField] private float roomScale;
    [SerializeField] private GameObject bossRoom;
    [SerializeField] private GameObject treasureRoom;
    [SerializeField] private GameObject shopRoom;

    // Materials falls benötigt werden

    private int[,] map;
    private const int mapWidth = 9;
    private const int mapHeight = 8;
    private const int minEndRooms = 2;
    private List<Vector2Int> endRoomsList;
    private Vector3 startPos;
    
    private void Awake()
    {
        startPos = new Vector3(mapWidth, 0.0f, mapHeight) * 0.5f * -1.0f * roomScale;
        map = GenerateGridMap(2, ref endRoomsList);
    }

    private int[,] GenerateGridMap(int level, ref List<Vector2Int> endRooms)
    {
        endRoomsList = new();
        int roomNumbers = (int)(Random.Range(0, 2) + 5 + level * 2.6f);

        int[,] tempMap = new int[mapWidth, mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tempMap[x, y] = (int)RoomType.free;
            }
        }

        int iteration = 0;
        int maxIteration = 30000;
        bool isValidate = false;

        do
        {
            // Regeln einbinden (LevelGeneration)
            int[,] generateMap = GenerateLevel(tempMap, roomNumbers, ref endRooms);

            // Level validieren
            isValidate = ValidateMap(generateMap, roomNumbers, endRooms, minEndRooms);

            // Ja = fertig
            if (isValidate)
                tempMap = generateMap;

            // nein = Wiederholung
            iteration++;
        }
        while (!isValidate && iteration < maxIteration);
        return tempMap;
    }

    private int[,] GenerateLevel(int[,] currentMap, int roomAmount, ref List<Vector2Int> endRooms)
    {
        int[,] tempMap = currentMap;
        int width = tempMap.GetLength(0);
        int height = tempMap.GetLength(1);

        Queue<Vector2Int> posToExpand = new();
        Vector2Int midPos = new Vector2Int(width / 2, height / 2);
        tempMap[midPos.x, midPos.y] = (int)RoomType.normal;
        posToExpand.Enqueue(midPos);

        int currentRooms = 1;

        while (posToExpand.Count > 0)
        {
            Vector2Int currentPos = posToExpand.Dequeue();
            Vector2Int[] posToCheck = new Vector2Int[]
                {
                    currentPos + Vector2Int.up,
                    currentPos + Vector2Int.right,
                    currentPos + Vector2Int.down,
                    currentPos + Vector2Int.left,
                };

            bool addedRoom = false;

            for (int i = 0; i < posToCheck.Length; i++)
            {
                Vector2Int toCheck = posToCheck[i];
                if (toCheck.x >= 0 && toCheck.x < width && toCheck.y >= 0 && toCheck.y < height)
                {
                    // 1. Sind alle Räume generiert? => ja => Exit
                    if (currentRooms >= roomAmount)
                        continue;

                    // 2. Bist du belegt => ja => Exit
                    if (tempMap[toCheck.x, toCheck.y] != (int)RoomType.free)
                        continue;

                    // 3. 50% Quit Chance
                    float rndPercent = UnityEngine.Random.Range(0.0f, 1.0f);
                    if (rndPercent <= 0.5f)
                        continue;

                    // 4. Hast du 2 oder mehr Nachbarn? => ja => Exit
                    int neighboursCount = GetNeighboursCount(tempMap, toCheck);
                    if (neighboursCount > 1)
                        continue;

                    currentMap[toCheck.x, toCheck.y] = (int)RoomType.normal;
                    posToExpand.Enqueue(toCheck);
                    roomAmount++;
                    addedRoom = true;
                }
            }
            if (!addedRoom)
                endRooms.Add(currentPos);
        }

        return tempMap;
    }

    private int GetNeighboursCount(int[,] map, Vector2Int pos)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int neighboursCount = 0;

        Vector2Int[] posToCheck = new Vector2Int[]
        {
            pos + Vector2Int.up,
            pos + Vector2Int.right,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
        };

        for (int i = 0; i < posToCheck.Length; i++)
        {
            Vector2Int toCheck = posToCheck[i];
            if (toCheck.x >= 0 && toCheck.x < width && toCheck.y >= 0 && toCheck.y < height)
            {
                if (map[toCheck.x, toCheck.y] != (int)RoomType.free)
                {
                    neighboursCount++;
                }
            }
        }

        return neighboursCount;
    }

    private bool ValidateMap(int[,] checkMap, int countOfRooms, List<Vector2Int> listOfEndrooms, int endroomCount)
    {
        // haben wir genug Endräume
        if (listOfEndrooms.Count < endroomCount)
            return false;

        // haben wir genug Räume
        int width = checkMap.GetLength(0);
        int height = checkMap.GetLength(1);
        int count = 0;

        foreach (int item in checkMap)
        {
            // ggf. redundant
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (checkMap[x, y] != (int)RoomType.free)
                    {
                        count++;
                    }
                }
            }
        }

        return count >= countOfRooms;
    }
}
