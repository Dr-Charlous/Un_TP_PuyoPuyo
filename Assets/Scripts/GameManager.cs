using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public int SizeX = 6;
    public int SizeY = 13;
    public GameObject[,] Grid;
    public GameObject PuyoPrefab;
    public Transform PuyoParent;
    public float Time = 10;

    public Tilemap TileMap;
    public Tile GroundSprite;

    private void Start()
    {
        Grid = new GameObject[SizeX, SizeY];

        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                TileMap.SetTile(new Vector3Int(x, -y), GroundSprite);
            }
        }

        DropPuyo(Time);
    }

    public void DropPuyo(float _time)
    {
        GameObject _object = Instantiate(PuyoPrefab, new Vector3Int(3, 0), Quaternion.identity, PuyoParent);
        Grid[3, 0] = _object;
        StartCoroutine(_object.GetComponent<Puyo>().Falling(1));
    }
}
