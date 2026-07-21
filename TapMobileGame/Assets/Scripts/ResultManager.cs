using UnityEngine;
using TMPro;

// リザルト画面でスコアを表示するスクリプト
public class ResultManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        // ScoreManager に静的（static）に保存されている最終スコアを取得
        int finalScore = ScoreManager.score;
       
        if (scoreText != null)
        {
            scoreText.text = "SCORE   " + finalScore;
        }
    }
}
