using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    public void LoadScene(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDoJogo()
    {
        Application.Quit();
        Debug.Log("Quitando app");
    }
}
