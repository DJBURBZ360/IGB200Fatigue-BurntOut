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
        Item.ResetNumInstance();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        //OnLevelWasLoaded(sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    public void ReturnToMainMenu()
    {
        Item.ResetNumInstance();
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region Main Menu
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        SaveDataManagement.SaveState();
        Application.Quit();
    }

    #endregion

    #region Level Management
    public void LoadPreviousLevel()
    {
        Item.ResetNumInstance();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        //OnLevelWasLoaded(sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextLevel()
    {
        Item.ResetNumInstance();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        //OnLevelWasLoaded(sceneIndex);
        SceneManager.LoadScene(sceneIndex);
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

    public void InstanceGameObjectHere(GameObject prefab)
    {
        Instantiate(prefab, this.gameObject.transform);
    }

    public void InstanceGameObjectOutside(GameObject prefab)
    {
        Instantiate(prefab);
    }

    public void DestroyGameObject(GameObject target)
    {
        Destroy(target);
    }    

    public void ResetProgress()
    {
        SaveDataManagement.ResetProgress();
    }
    #endregion
}
