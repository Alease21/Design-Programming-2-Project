using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    //NICAM LIU


    [SerializeField] private SceneField _newGame;

    private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();



    public void StartGameButton()
    {
        scenesToLoad.Add(SceneManager.LoadSceneAsync(_newGame, LoadSceneMode.Additive));

    }
    public void LoadGameButton()
    {
        //load game
    }

    public void QuitGame()
    {
            Application.Quit();
    }

}
