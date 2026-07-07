using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ResultManager : MonoBehaviour
{
    // 先ほど作ったテキストUIを入れるための箱
    public TextMeshProUGUI scoreText;
    void Start()
    {
        // 1. GameSceneからスコアを受け取る（staticで引き継いだ場合）
        int finalScore = ScoreManager.score;
       
        scoreText.text = "SCORE   " + finalScore;
    }
}
