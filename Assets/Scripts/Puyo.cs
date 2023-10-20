using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puyo : MonoBehaviour
{
    public bool Finish = false;

    public void Fall(int x, int y, int width, int height)
    {
        if (y + 1 < height && GameManager.Instance.Grid[x, y + 1] == null)
        {
            GameManager.Instance.Grid[x, y] = null;
            GameManager.Instance.Grid[x, y + 1] = this.gameObject;
            transform.position = new Vector3Int(x, -y - 1);
        }
        else if (Finish == false)
        {
            GameManager.Instance.DropPuyo(GameManager.Instance.Time);
            Finish = true;
        }
    }

    public IEnumerator Falling(float _time)
    {
        yield return new WaitForSeconds(_time);
        Fall((int)transform.position.x, -(int)transform.position.y, GameManager.Instance.SizeX, GameManager.Instance.SizeY);
        StartCoroutine(Falling(_time));
    }
}
