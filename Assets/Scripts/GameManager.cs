using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    //Grids Setup
    public int SizeX = 6;
    public int SizeY = 13;
    public GameObject[,] Grid;
    public bool[,] GridCombo;

    //Puyos
    public GameObject[] PuyoPrefab;
    public Transform PuyoParent;
    public float PuyoFallSpeed = 0.2f;
    public GameObject _currentPuyo;

    //Combos
    public int _comboCount;
    public float _timerCombo;
    public GameObject SunPrefab;
    public bool _isRunningCoroutine;

    //Tiles
    public Tilemap TileMap;
    public Tile GroundSprite;

    //Score
    public int _score;
    public TextMeshProUGUI ScoreText;

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

        ScoreText.text = "0";
        DropPuyo();
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

    public void DropPuyo()
    {
        int randomNumber = Random.Range(0, PuyoPrefab.Length);
        _currentPuyo = Instantiate(PuyoPrefab[randomNumber], new Vector3Int(3, 0), Quaternion.identity, PuyoParent);
        Grid[3, 0] = _currentPuyo;
        StartCoroutine(_currentPuyo.GetComponent<Puyo>().Falling(PuyoFallSpeed));
    }

    public int ComboCheck(int x, int y, int width, int height, Puyo.Color color, int iteration)
    {
        if (x < width && x >= 0 && y > 0 && y < height)
        {
            if (Grid[x, y] != null)
            {
                if (GridCombo[x, y] == false && color == Grid[x, y].GetComponent<Puyo>().color)
                {
                    GridCombo[x, y] = true;
                    iteration++;

                    iteration = ComboCheck(x + 1, y, width, height, color, iteration);
                    iteration = ComboCheck(x - 1, y, width, height, color, iteration);
                    iteration = ComboCheck(x, y + 1, width, height, color, iteration);
                    iteration = ComboCheck(x, y - 1, width, height, color, iteration);
                }
            }
        }
        return iteration;
    }

    public void Combo(int _iteration)
    {
        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                if (GridCombo[x, y] == true)
                {
                    if (_iteration >= 4)
                    {
                        Destroy(Grid[x, y]);
                        Grid[x, y] = null;
                        _comboCount++;
                    }

                    GridCombo[x, y] = false;
                }
            }
        }

        if (_iteration >= 4)
        {
            _comboCount += 1 - _iteration;
            _score += _iteration * 2;
            ScoreText.text = _score.ToString();

            if (_isRunningCoroutine == false)
            {
                StartCoroutine(ComboFeedBackTimer(_comboCount, _iteration));
            }
        }
    }

    IEnumerator ComboFeedBackTimer(int comboCountInit, int iterations)
    {
        _isRunningCoroutine = true;

        yield return new WaitForSeconds(PuyoFallSpeed * 2);

        if (comboCountInit != _comboCount)
        {
            GameObject sun = Instantiate(SunPrefab, new Vector3Int(-4, -(int)(SizeY / 2f)), Quaternion.identity);
            _score += iterations * (_comboCount - comboCountInit) / 4;
            ScoreText.text = _score.ToString();

            yield return new WaitForSeconds(PuyoFallSpeed * 4);
            Destroy(sun);
        }

        _isRunningCoroutine = false;
    }
}
