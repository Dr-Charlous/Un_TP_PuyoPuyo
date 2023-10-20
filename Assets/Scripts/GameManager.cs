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
    public GameObject _currentPuyo;

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

    public void Update()
    {
        Puyo _puyo = _currentPuyo.GetComponent<Puyo>();
        Vector3 _puyoPos = _currentPuyo.transform.position;

        if (_puyo.Finish == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && _puyoPos.x > 0)
            {
                _puyo.Move((int)_puyoPos.x, (int)_puyoPos.x - 1, -(int)_puyoPos.y, SizeX);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && _puyoPos.x < GameManager.Instance.SizeX)
            {
                _puyo.Move((int)_puyoPos.x, (int)_puyoPos.x + 1, -(int)_puyoPos.y, SizeX);
            }
        }
    }

    public void DropPuyo(float _time)
    {
        _currentPuyo = Instantiate(PuyoPrefab, new Vector3Int(3, 0), Quaternion.identity, PuyoParent);
        Grid[3, 0] = _currentPuyo;
        StartCoroutine(_currentPuyo.GetComponent<Puyo>().Falling(1));
    }
}
