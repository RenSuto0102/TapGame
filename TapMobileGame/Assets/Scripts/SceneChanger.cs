using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// シーン遷移を管理するスクリプト。UIのボタンイベント等から呼び出される。
public class SceneChanger : MonoBehaviour
{
    [Header("フェード演出用設定")]
    public Image fadeImage; // フェードアウト演出用の黒UIイメージ
    public float fadeSpeed = 1.5f;

    // タイトルシーンへ遷移
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    // ゲームシーンへ遷移
    public void GoToGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    // デバッグ用などにリザルトシーンへ直接遷移する処理
    public void GoToResultScene()
    {
        SceneManager.LoadScene("Result");
    }
}