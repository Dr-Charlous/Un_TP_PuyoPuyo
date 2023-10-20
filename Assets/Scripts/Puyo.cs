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
    public bool Finish = false;
    public bool EndFinish = false;
    public int iteration = 0;

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

            if (EndFinish == false)
            {
                GameManager.Instance.DropPuyo();
                EndFinish = true;
            }
        }
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

    public IEnumerator Falling(float _time)
    {
        Fall((int)transform.position.x, -(int)transform.position.y, GameManager.Instance.SizeX, GameManager.Instance.SizeY);
        yield return new WaitForSeconds(_time);
        StartCoroutine(Falling(_time));
    }
}
