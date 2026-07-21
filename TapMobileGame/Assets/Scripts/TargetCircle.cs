using UnityEngine;
using System.Collections;

// タップ対象のターゲットオブジェクト（円）を制御するスクリプト
public class TargetCircle : MonoBehaviour
{
    [Header("ランダム出現範囲の限界座標")]
    [SerializeField] private float minX = -2.0f;
    [SerializeField] private float maxX = 2.0f;
    [SerializeField] private float minY = -4.0f;
    [SerializeField] private float maxY = 4.0f;

    [Header("エフェクト設定")]
    [SerializeField] private GameObject tapEffectPrefab;

    private Coroutine popInCoroutine;

    void Start()
    {
        // 開始時にランダムな位置に配置
        MoveToRandomPosition();
    }

    // オブジェクトがタップ（クリック）されたときに呼ばれるコールバック
    void OnMouseDown()
    {
        // スコアマネージャーを探してスコアを加算
        ScoreManager manager = FindAnyObjectByType<ScoreManager>();
        if (manager != null)
        {
            manager.AddScore(10);
        }

        // タップされた位置にパーティクルエフェクトを生成
        if (tapEffectPrefab != null)
        {
            Instantiate(tapEffectPrefab, transform.position, Quaternion.identity);
        }

        // カメラを揺らす（時間:0.1秒、揺れの強さ:0.2）
        CameraJuice.Shake(0.1f, 0.2f);

        // オブジェクトを破壊せず、別の場所に瞬間移動させて使い回す
        MoveToRandomPosition();
    }

    // ランダムな位置に移動させ、出現アニメーションを開始する
    void MoveToRandomPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        transform.position = new Vector3(randomX, randomY, 0f);

        // 重複実行を避けるため、すでに実行中の出現コルーチンがあれば停止
        if (popInCoroutine != null)
        {
            StopCoroutine(popInCoroutine);
        }
        // 出現時バウンドアニメーション（コルーチン）を開始
        popInCoroutine = StartCoroutine(PopIn());
    }

    // 出現時にポンッと弾むようなイージングアニメーション（OutBack風）をコルーチンで制御
    private IEnumerator PopIn()
    {
        float t = 0;
        Vector3 targetScale = Vector3.one;
        transform.localScale = Vector3.zero;

        while (t < 1f)
        {
            // 時間の経過（Time.deltaTime）に合わせてアニメーションを進行
            t += Time.deltaTime * 5f; 
            
            // サイン波を加えて一時的に目標サイズ(1.0)を超える動き（バウンド）を作成
            float scale = Mathf.Sin(t * Mathf.PI) * 0.3f + t;
            transform.localScale = targetScale * scale;
            yield return null; // 1フレーム待機
        }
        transform.localScale = targetScale;
        popInCoroutine = null;
    }
}
