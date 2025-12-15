using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGen : MonoBehaviour
{
    public Tilemap road;
    public Tilemap ground;
    public Tilemap boundary;
    public Tilemap clutter;

    public Tile tile;
    public RuleTile roadTile;

    public List<Tile> clutterTiles = new List<Tile>();

    public Vector2Int mapSize;
    public int mapBorder;
    public int roadWidth;
    Vector2Int roadArea;
    
    public float fwdCh;
    public float vrtCh;
    public float upCh;
    public float branchCh;

    public List<GameObject> encounterPrefabs;
    public int minEncounters;
    public int maxEncounters;

    Vector2Int startPos;
    List<Vector2Int> roadPoints = new List<Vector2Int>();

    List<Vector3Int> encounterPositions = new List<Vector3Int>();

    public void GenerateMap(GameObject player, GameObject mainCamera, GameObject encounters)
    {
        roadArea = new Vector2Int(mapSize.x / roadWidth, mapSize.y / roadWidth);
        startPos = new Vector2Int(0, UnityEngine.Random.Range(0, roadArea.y));
        roadPoints.Add(startPos);

        DrawBorder();
        GenerateRoad();
        GenerateEncounters(encounters);
        FillMap();

        // Position player, camera, and goal
        player.GetComponent<Transform>().position = new Vector3(0, startPos.y * roadWidth, 0);
        mainCamera.GetComponent<Transform>().position = new Vector3(5, startPos.y * roadWidth, -10);
        encounters.transform.GetChild(0).transform.position = new Vector3(mapSize.x, mapSize.y / 2 + 0.5f, 0);
    }

    void DrawBorder()
    {
        for (int x = -mapBorder; x <= mapSize.x + mapBorder; x++)
        {
            if (0 > x)
            {
                for (int y = -mapBorder; y <= mapSize.y + mapBorder; y++)
                {
                    if (roadPoints[0].y * roadWidth > y || y >= roadPoints[0].y * roadWidth + roadWidth)
                    {
                        boundary.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                    else
                    {
                        boundary.SetTile(new Vector3Int(x, y, 0), roadTile);
                    }
                }
            }
            else if (x >= mapSize.x)
            {
                for (int y = -mapBorder; y <= mapSize.y + mapBorder; y++)
                {
                    if (mapSize.y / 2 > y || y >= mapSize.y / 2 + roadWidth)
                    {
                        boundary.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                    else
                    {
                        boundary.SetTile(new Vector3Int(x, y, 0), roadTile);
                    }
                }
            }
            else
            {
                for (int y = -mapBorder; y <= mapSize.y + mapBorder; y++)
                {
                    if (0 > y || y >= mapSize.y)
                    {
                        boundary.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
            }
        }
    }

    void GenerateRoad()
    {
        Vector2Int curPos;

        bool generating = true;
        int iterations = 0;

        for (int i = 0; i < roadPoints.ToArray().Length && i < 10; i++)
        {
            curPos = roadPoints[i];
            DrawRoad(curPos);
            generating = true;

            while (generating)
            {
                if (curPos.x < roadArea.x - 1)
                {
                    //CalcChances();
                    float randVal = UnityEngine.Random.value;
                    // Choose a direction to go
                    if (randVal <= fwdCh && curPos.x < roadArea.x)
                    {
                        curPos.x++; // Right
                    }
                    else if (randVal <= fwdCh + upCh && curPos.y < roadArea.y - 1)
                    {
                        curPos.y++; // Up
                    }
                    else if (randVal <= fwdCh + vrtCh && curPos.y > 0)
                    {
                        curPos.y--; // Down
                    }
                    else if (curPos.x >= 1)
                    {
                        curPos.x--; // Left
                    }
                    

                    randVal = UnityEngine.Random.value;
                    if (randVal <= branchCh)
                    {
                        roadPoints.Add(new Vector2Int(curPos.x, curPos.y));
                    }
                }
                else if (curPos.y != roadArea.y / 2)
                {
                    if (curPos.y > roadArea.y / 2)
                    {
                        curPos.y--; // Down
                    }
                    else
                    {
                        curPos.y++; // Up                            
                    }
                }
                else
                {
                    Debug.Log("Road branch generation completed. | End x: " + curPos.x + " | End y: " + curPos.y);
                    generating = false;
                }

                roadPoints[i] = (curPos);
                DrawRoad(curPos);

                iterations++;
                if (iterations > 1000)
                {
                    Debug.Log("Road generation aborted: too many iterations");
                    generating = false;
                }
            }

        }

        void DrawRoad(Vector2Int pos)
        {
            for (int x = 0; x < roadWidth; x++)
            {
                for (int y = 0; y < roadWidth; y++)
                {
                    road.SetTile(new Vector3Int(pos.x * roadWidth + x, pos.y * roadWidth + y, 0), roadTile);
                }
            }

        }

        //void CalcChances()
        //{
        //    float chance = Mathf.Clamp((-((curPos.x / roadArea.x) * (curPos.y / roadArea.y)) + 0.5f + (0.5f * (curPos.x / roadArea.x))), 0, 1);
        //    upCh = vrtCh * chance;
        //}
    }

    void FillMap()
    {
        Vector3Int mapPos = Vector3Int.zero;
        for (mapPos.x = 0; mapPos.x < mapSize.x; mapPos.x++)
        {
            for (mapPos.y = 0; mapPos.y < mapSize.y; mapPos.y++)
            {
                if (road.GetTile(mapPos) == null)
                {
                    ground.SetTile(mapPos, tile);

                    //randomly place clutter on non-road tiles
                    if (UnityEngine.Random.value < 0.05f && (road.GetTile(mapPos) == null))
                    {
                        clutter.SetTile(mapPos, clutterTiles[UnityEngine.Random.Range(0, clutterTiles.Count - 1)]);
                    }
                }

                
            }
        }
    }

    void GenerateEncounters(GameObject encounters)
    {
        int numberOfEncounters = UnityEngine.Random.Range(minEncounters, maxEncounters);
        // Position encounters
        for (int i = 0; i < numberOfEncounters; i++)
        {
            
            
            Vector3Int encounterPos;
            bool placed = false;
            int iterations = 0;
            
            while (!placed && iterations < 1000)
            {
                encounterPos = new Vector3Int(UnityEngine.Random.Range(1, mapSize.x - 1), UnityEngine.Random.Range(1, mapSize.y - 1), 0);
                if (road.GetTile(encounterPos) != null && !encounterPositions.Contains(encounterPos))
                {
                    GameObject encounter = Instantiate(encounterPrefabs[UnityEngine.Random.Range(0, encounterPrefabs.ToArray().Length)], encounterPos, Quaternion.identity, encounters.transform);
                    encounter.GetComponent<Encounter>().gameController = GameObject.Find("GameController");
                    encounterPositions.Add(encounterPos);
                    placed = true;
                }
                iterations++;
            }
            if (iterations >= 1000)
            {
                Debug.Log("Encounter placement aborted: too many iterations");
            }

        }
    }
}
