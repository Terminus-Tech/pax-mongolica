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

    public Vector2Int mapSize;
    public int mapBorder;
    public int roadWidth;
    Vector2Int roadArea;
    
    public float fwdCh;
    public float vrtCh;
    float upCh;
    public float branchCh;

    public List<Vector2Int> roadPoints = new List<Vector2Int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roadArea = new Vector2Int(mapSize.x / roadWidth, mapSize.y / roadWidth);
        DrawBorder();
        GenerateRoad();
    }

    void DrawBorder()
    {
        for (int x = -mapBorder; x <= mapSize.x + mapBorder; x++)
        {
            if (0 > x || x >= mapSize.x)
            {
                for (int y = -mapBorder; y <= mapSize.y + mapBorder; y++)
                {
                    boundary.SetTile(new Vector3Int(x, y, 0), tile);
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
        roadPoints.Add(new Vector2Int(0, UnityEngine.Random.Range(0, roadArea.y)));
        // Z-values: 0 = right, 1 = Up, 2 = down, 3 = left, 4 = right/up, 5 = right/down, 6 = left/up, 7 = left/down, 8 = up/down

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
                    CalcChances();
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

        void CalcChances()
        {
            float chance = Mathf.Clamp(( -(( curPos.x / roadArea.x ) * ( curPos.y / roadArea.y )) + 0.5f + (0.5f * (curPos.x / roadArea.x)) ), 0, 1);
            upCh = vrtCh * chance;
            Debug.Log((-(curPos.x / roadArea.x) + " * " + (curPos.y / roadArea.y) + " + 0.5f " + (0.5f * (curPos.x / roadArea.x)) + " | downCh: " + (vrtCh - upCh) + " | backCh: " + (1 - (fwdCh + vrtCh)));
        }
    }
}
