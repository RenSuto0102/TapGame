using UnityEngine;
using TMPro;

// ゲームのスコア管理とUI更新を行うマネージャー
public class ScoreManager : MonoBehaviour
{
    // シーン遷移後もスコアデータを保持するため、静的（static）変数として宣言
    public static int score = 0;

    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        // ゲーム開始時にスコアをリセット
        score = 0;
        UpdateScoreUI();
    }

    // 的がタップされたときにスコアを加算するメソッド
    public void AddScore(int point)
    {
        score += point;
    }

    void Update()
    {
        // 毎フレームスコアUIの表示内容を更新
        UpdateScoreUI();
    }

    // スコアUIのテキストを更新する処理
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // 文字列と現在のスコアを連結して表示
            scoreText.text = "SCORE   " + score;
        }
    }
}
