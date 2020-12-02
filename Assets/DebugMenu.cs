using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup debugMenuCanvasGroup = default;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleDebugMenu();
        }
        if (Input.touchCount > 3)
        {
            ToggleDebugMenu();
        }
    }

    private void ToggleDebugMenu()
    {
        var wasActive = debugMenuCanvasGroup.interactable;

        debugMenuCanvasGroup.interactable = !wasActive;
        debugMenuCanvasGroup.blocksRaycasts = !wasActive;
        debugMenuCanvasGroup.alpha = wasActive ? 0f : 1f;
    }

    public void DebugReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
