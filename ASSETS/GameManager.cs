using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool elJuegoTermino = false;

    public float reiniciarDelay;

    public void EndGame()
    {
        if(elJuegoTermino == false)
        {
            elJuegoTermino = true;
            Debug.Log("End Game");
            Invoke("Reiniciar", reiniciarDelay);
        }
    }

    void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

