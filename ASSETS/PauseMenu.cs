using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool JuegoEnPausa = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(JuegoEnPausa)
            {
                Resume();
            }
            else
            {
                Pausa();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        JuegoEnPausa = false;
    }

    void Pausa()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        JuegoEnPausa = true;
    }
}
