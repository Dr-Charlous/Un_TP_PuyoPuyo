using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    #region singleton
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
    #endregion

    #region Variables
    [Header("Grids")]
    //Grids Setup
    public int SizeX = 6;
    public int SizeY = 13;
    public GameObject[,] Grid;
    public bool[,] GridCombo;

    [Header("Puyos ")]
    //Puyos
    public GameObject[] PuyoPrefab;
    public Transform PuyoParent;
    public float PuyoFallSpeed = 0.2f;
    private GameObject _currentPuyo;

    [Header("Combos")]
    //Combos
    public int _comboCount;
    public float _timerCombo;
    public GameObject SunPrefab;
    public bool _isRunningCoroutine;

    [Header("Tiles")]
    //Tiles
    [SerializeField] private Tilemap TileMap;
    [SerializeField] private Tile GroundSprite;

    [Header("Score")]
    //Score
    public int _score = 0;
    public int _scoreResult = 15;
    [SerializeField] private TextMeshProUGUI ScoreText;

    [Header("Timer")]
    //Timer
    public float _timer = 0;
    [SerializeField] private TextMeshProUGUI TimerText;

    [Header("Lose / Win")]
    //Lose Win
    public GameObject LoseUI;
    [SerializeField] private GameObject WinUI;
    public bool GameFinish = false;
    #endregion

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
        TimerText.text = "0";
        SpawnPuyo();
    }

    public void Update()
    {
        Puyo _puyo = _currentPuyo.GetComponent<Puyo>();
        Vector3 _puyoPos = _currentPuyo.transform.position;

        //Chrono / Win / Lose
        if (_timer <= 60 && GameFinish == false)
        {
            _timer += Time.deltaTime;
            TimerText.text = ((int)_timer).ToString();
        }
        
        if ((_timer > 60 && _score < _scoreResult) || GameFinish == true)
        {
            LoseUI.SetActive(true);
            StopAllCoroutines();
        }
        else if (_timer > 60 && _score >= _scoreResult)
        {
            WinUI.SetActive(true);
            StopAllCoroutines();
        }

        //Move puyo
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

    public void SpawnPuyo()
    {
        int randomNumber = Random.Range(0, PuyoPrefab.Length);
        _currentPuyo = Instantiate(PuyoPrefab[randomNumber], new Vector3Int(3, 0), Quaternion.identity, PuyoParent);
        Grid[3, 0] = _currentPuyo;
        StartCoroutine(_currentPuyo.GetComponent<Puyo>().Falling(PuyoFallSpeed));
    }

    #region Combo
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
    #endregion
}
