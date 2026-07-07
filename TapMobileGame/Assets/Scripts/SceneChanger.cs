using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [Header("演出設定")]
    public Image fadeImage; // フェードアウト用の黒い画像
    public float fadeSpeed = 1.5f; // フェードの速さ
    public void GoToTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
    public void GoToGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    public void GoToResultScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
    }
}