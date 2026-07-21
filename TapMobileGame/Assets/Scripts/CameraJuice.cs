using UnityEngine;

// 画面（メインカメラ）を揺らす演出（スクリーンシェイク）を制御するスクリプト。
public class CameraJuice : MonoBehaviour
{
    private static CameraJuice instance;

    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    void Awake()
    {
        // 静的参照（シングルトン）に登録
        instance = this;
    }

    void Start()
    {
        // 揺らし始める前のデフォルトのカメラ位置を保存
        originalPos = transform.localPosition;
    }

    void Update()
    {
        // タイマーが有効な間、カメラの座標をズラす
        if (shakeTimer > 0)
        {
            // Random.insideUnitSphere（半径1の球体内のランダムな3Dベクトル）を使ってブレ位置を計算
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            
            // タイマーを減算
            shakeTimer -= Time.deltaTime;
            
            if (shakeTimer <= 0)
            {
                shakeTimer = 0f;
                // 終了時に正確に元の位置に戻して固定
                transform.localPosition = originalPos;
            }
        }
    }

    // 外部から静的メソッド経由でカメラシェイクを発動させる
    public static void Shake(float duration, float magnitude)
    {
        if (instance == null)
        {
            // メインカメラにこのスクリプトが貼り付けられていない場合、自動で検知してコンポーネントをアタッチ
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                instance = mainCam.gameObject.GetComponent<CameraJuice>();
                if (instance == null)
                {
                    instance = mainCam.gameObject.AddComponent<CameraJuice>();
                    instance.originalPos = mainCam.transform.localPosition;
                }
            }
        }

        if (instance != null)
        {
            // シェイクの時間と強さをセットして開始
            instance.shakeTimer = duration;
            instance.shakeMagnitude = magnitude;
        }
    }
}
