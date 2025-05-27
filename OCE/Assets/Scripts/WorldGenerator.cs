using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
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
    private List<Vector2Int> endRooms = new();
    private Vector3 startPos;
    
    private void Awake()
    {
        startPos = new Vector3(mapWidth, 0.0f, mapHeight) * 0.5f * -1.0f * roomScale;
    }

    private int[,] GenerateGridMap(int level, ref List<Vector2Int> endRoom)
    {
        endRoom = new();
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


            // Level validieren
            // Ja = fertig
            // nein = Wiederholung
        }
        while (!isValidate && iteration < maxIteration);
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

        while (posToExpand.Count > 0) {
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
                    if (Random.Range(0, 2) == 0)
                        continue;

                    // 4. Hast du 2 oder mehr Nachbarn? => ja => Exit
                }
            }
    }

}
