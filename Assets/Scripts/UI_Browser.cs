﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Browser : MonoBehaviour
{
    #region PauseScreen
    public void Continue()
    {
        Time.timeScale = 1;
        Destroy(this.gameObject);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region Main Menu
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Other Methods
    public void HideGameObject(GameObject target)
    {
        target.SetActive(false);
    }

    public void ShowGameObject(GameObject target)
    {
        target.SetActive(true);
    }

    public void InstanceGameObject(GameObject prefab)
    {
        Instantiate(prefab, this.gameObject.transform);
    }

    public void DestroyGameObject(GameObject target)
    {
        Destroy(target);
    }
    #endregion
}