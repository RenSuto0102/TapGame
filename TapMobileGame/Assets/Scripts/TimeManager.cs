using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

// 制限時間の計算、UIのテキスト表示、ゲーム終了時のフェードアウト処理を行うマネージャー
public class TimeManager : MonoBehaviour
{
    [Header("タイマー設定")]
    public float timeLimit = 30.0f; // 制限時間（秒）

    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("フェードアウト演出用設定")]
    [SerializeField] private Image fadeImage; // フェードアウト用の黒UIパネル
    [SerializeField] private float fadeSpeed = 1.5f; // フェードアウト速度
    
    private bool isEnding = false; // 終了処理の二重発生を防ぐためのフラグ

    void Update()
    {
        if (isEnding) return;

        // 毎フレーム残り時間を減算。フレームレートの影響を受けないよう Time.deltaTime を使用。
        timeLimit -= Time.deltaTime;

        UpdateTimerUI();

        // 時間切れ判定
        if (timeLimit <= 0)
        {
            timeLimit = 0;
            isEnding = true;
            
            // コルーチンを使ってフェードアウトとシーン遷移処理を開始
            StartCoroutine(FadeAndChangeScene());
        }
    }

    // タイマーテキストUIの表示更新
    private void UpdateTimerUI()
    {
        if (timeText != null)
        {
            // 小数点第1桁("F1")まで表示
            timeText.text = "Time: " + timeLimit.ToString("F1"); 
        }
    }

    // 非同期で黒フェードアウトを動かした後にリザルトシーンへ遷移するコルーチン
    private IEnumerator FadeAndChangeScene()
    {
        if (fadeImage != null)
        {
            float alpha = 0;
            // アルファ値を徐々に増やして画面を暗くしていく
            while (alpha < 1.0f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null; // 1フレーム待機
            }
            yield return new WaitForSeconds(0.5f); // 遷移前の余韻時間
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }

        // リザルトシーンへ切り替え
        SceneManager.LoadScene("Result");
    }
}
