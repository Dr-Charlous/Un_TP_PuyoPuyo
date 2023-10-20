using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerLvl : MonoBehaviour
{
    private Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    public void ButtonRetry()
    {
        SceneManager.LoadScene(scene.name);
    }

    public void ButtonNextLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
