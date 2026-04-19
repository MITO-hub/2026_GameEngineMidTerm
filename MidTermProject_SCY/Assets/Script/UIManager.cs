using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void GameStartButtonAction()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void GameEixtButtonAction()
    {
        Application.Quit();
        Debug.Log("게임 종료 버튼이 클릭되었습니다.");
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
