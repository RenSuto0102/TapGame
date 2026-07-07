using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // 追加: UIを操作するため

public class TimeManager : MonoBehaviour
{
    //制限時間
    public float timeLimit = 30.0f;
    public TextMeshProUGUI timeText;

    [Header("演出設定")]
    public Image fadeImage; // フェードアウト用の黒い画像
    public float fadeSpeed = 1.5f; // フェードの速さ
    
    private bool isEnding = false; // 終了処理が始まったかどうかのフラグ

    void Update()
    {
        // 終了処理中は何もしない
        if (isEnding) return;

        // 残り時間を少しずつ減らす
        timeLimit = timeLimit - Time.deltaTime;

        // 画面のテキストを更新する
        if (timeText != null)
        {
            // 日本語の文字化けを避けるため英語にし、小数点第1位("F1")まで表示します
            timeText.text = "Time: " + timeLimit.ToString("F1"); 
        }

        // もし時間が0以下になったら
        if (timeLimit <= 0)
        {
            timeLimit = 0; // マイナスにならないように0で止める
            isEnding = true; // 終了処理開始フラグを立てる
            
            // フェードアウトしてからシーン切り替えを行う
            StartCoroutine(FadeAndChangeScene());
        }
    }

    IEnumerator FadeAndChangeScene()
    {
        // もしフェード用の画像がセットされていれば、徐々に暗くする
        if (fadeImage != null)
        {
            float alpha = 0;
            while (alpha < 1.0f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                // 画像の色を黒(0,0,0)にして、透明度(alpha)を徐々に上げていく
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null; // 1フレーム待つ
            }
            // 念のため1秒待つ（余韻）
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            // 画像がセットされていなくても、1秒だけ待ってから切り替える
            yield return new WaitForSeconds(1.0f);
        }

        // リザルトシーンへ移動
        SceneManager.LoadScene("Result");
    }
}
