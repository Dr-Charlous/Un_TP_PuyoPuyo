using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    public enum Color
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Pink,
        White
    }

    public Color color;
    public Sprite[] spritesStates;
    public bool Finish = false;
    public bool EndFinish = false;
    public int iteration = 0;
    public float TimerHigh = 0;

    public void Update()
    {
        CheckAsset((int)transform.position.x, -(int)transform.position.y, GameManager.Instance.SizeX, GameManager.Instance.SizeY);

#region Lose High
        if (transform.position.y >= -2)
        {
            TimerHigh += Time.deltaTime;

            if (TimerHigh > GameManager.Instance.PuyoFallSpeed + 0.15f)
            {
                GameManager.Instance.LoseUI.SetActive(true);
                GameManager.Instance.GameFinish = true;
                GameManager.Instance.StopAllCoroutines();
            }
        }
        else
        {
            TimerHigh = 0;
        }
#endregion
    }

    public void Move(int x, int x2, int y, int width)
    {
        if (x2 < width && GameManager.Instance.Grid[x2, y] == null)
        {
            GameManager.Instance.Grid[x, y] = null;
            GameManager.Instance.Grid[x2, y] = this.gameObject;
            transform.position = new Vector3Int(x2, -y);
        }
    }

    #region Fall
    public void Fall(int x, int y, int width, int height)
    {
        if (y + 1 < height && GameManager.Instance.Grid[x, y + 1] == null)
        {
            GameManager.Instance.Grid[x, y] = null;
            GameManager.Instance.Grid[x, y + 1] = this.gameObject;
            transform.position = new Vector3Int(x, -y - 1);
            Finish = false;
        }
        else if (Finish == false)
        {
            iteration = GameManager.Instance.ComboCheck(x, y, width, height, color, iteration);
            GameManager.Instance.Combo(iteration);
            iteration = 0;

            Finish = true;

            if (EndFinish == false && GameManager.Instance._timer <= 60)
            {
                GameManager.Instance.SpawnPuyo();
                EndFinish = true;
            }
        }
    }

    public IEnumerator Falling(float _time)
    {
        Fall((int)transform.position.x, -(int)transform.position.y, GameManager.Instance.SizeX, GameManager.Instance.SizeY);
        yield return new WaitForSeconds(_time);
        StartCoroutine(Falling(_time));
    }
    #endregion

    #region AssetUpdate
    public void CheckAsset(int x, int y, int width, int height)
    {
        int visu = 0;
        
        if (x - 1 > 0 && GameManager.Instance.Grid[x - 1, y] != null) 
        {
            if (GameManager.Instance.Grid[x - 1, y].GetComponent<Puyo>().color == color)
            {
                visu += 1;
            }
        }

        if (x + 1 < width && GameManager.Instance.Grid[x + 1, y] != null)
        {
            if (GameManager.Instance.Grid[x + 1, y].GetComponent<Puyo>().color == color)
            {
                visu += 4;
            }
        }

        if (y - 1 > 0 && GameManager.Instance.Grid[x, y - 1] != null)
        {
            if (GameManager.Instance.Grid[x, y - 1].GetComponent<Puyo>().color == color)
            {
                visu += 8;
            }
        }

        if (y + 1 < height && GameManager.Instance.Grid[x, y + 1] != null)
        {
            if (GameManager.Instance.Grid[x, y + 1].GetComponent<Puyo>().color == color)
            {
                visu += 2;
            }
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = spritesStates[visu];
    }
    #endregion
}
