using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // ゲーム開始時にスコアを0にリセット
        score = 0;
        
        // 最初から表示させる
        if (scoreText != null)
        {
            scoreText.text = "SCORE   " + score;
        }
    }

    public void AddScore(int point)
    {
        // スコアを加算
        score += point;
    }
    void Update()
    {
        //スコアを更新
        if (scoreText != null)
        {
            scoreText.text = "SCORE   " + score;
        }
    }
}
