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
    public bool[,] GridCombo;

    public GameObject[] PuyoPrefab;
    public Transform PuyoParent;

    public float Time = 10;

    public Tilemap TileMap;
    public Tile GroundSprite;
    public GameObject _currentPuyo;

    private void Start()
    {
        Grid = new GameObject[SizeX, SizeY];
        GridCombo = new bool[SizeX, SizeY];

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
        int randomNumber = Random.Range(0, PuyoPrefab.Length);
        _currentPuyo = Instantiate(PuyoPrefab[randomNumber], new Vector3Int(3, 0), Quaternion.identity, PuyoParent);
        Grid[3, 0] = _currentPuyo;
        StartCoroutine(_currentPuyo.GetComponent<Puyo>().Falling(0.2f));
    }

    public void ComboCheck(int x, int y, int width, int height, Puyo.Color color)
    {
        int _trueNumber = 0;

        if (x < width && x > 0 && y > 0 && y < height)
        {
            if (Grid[x, y] != null)
            {
                if (GridCombo[x, y] == false && color == Grid[x, y].GetComponent<Puyo>().color)
                {
                    GridCombo[x, y] = true;
                    _trueNumber++;

                    ComboCheck(x + 1, y, width, height, color);
                    ComboCheck(x - 1, y, width, height, color);
                    ComboCheck(x, y + 1, width, height, color);
                    ComboCheck(x, y - 1, width, height, color);
                }
            }
        }

        if (_trueNumber >= 3)
        {
            Combo();
        }
        else
        {
            foreach (var item in GridCombo)
            {
                item = false;
            }
        }
    }

    public void Combo()
    {
        int numbers = 0;

        foreach (var item in GridCombo)
        {
            if (item == true)
            {
                numbers++;
            }
        }

        if (numbers >= 4)
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (GridCombo[x, y] == true)
                    {
                        Destroy(Grid[x, y]);
                        Grid[x, y] = null;
                        GridCombo[x, y] = false;
                    }
                }
            }
        }
    }
}
