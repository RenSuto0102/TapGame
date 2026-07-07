using UnityEngine;
using UnityEngine.SceneManagement;

public class FancyBackgroundController : MonoBehaviour
{
    private Sprite circleSprite;
    private float spawnTimer = 0f;
    private float spawnInterval = 0.4f; // バブルの発生間隔

    // オブジェクトプール用の変数
    private const int MAX_BUBBLES = 30;
    private GameObject[] bubblePool;
    private int poolIndex = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // シーンロード時に、まだマネージャーが存在していなければ作成
        if (GameObject.Find("FancyBackgroundController") == null)
        {
            GameObject bg = new GameObject("FancyBackgroundController");
            bg.AddComponent<FancyBackgroundController>();
            DontDestroyOnLoad(bg);
        }
    }

    void Start()
    {
        circleSprite = CreateCircleSprite();
        
        // パフォーマンス向上のため、最初に最大数分のバブルを作って使い回す（オブジェクトプーリング）
        bubblePool = new GameObject[MAX_BUBBLES];
        for (int i = 0; i < MAX_BUBBLES; i++)
        {
            GameObject bubble = new GameObject("FancyBubble");
            bubble.transform.SetParent(this.transform); // 整理のために子オブジェクトにする
            SpriteRenderer sr = bubble.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;
            bubble.AddComponent<FancyBubble>();
            bubble.SetActive(false); // 初期状態は非表示
            bubblePool[i] = bubble;
        }
    }

    void Update()
    {
        // 1. メインカメラの背景色をなめらかなグラデーション（パステルレインボー）で変化させる
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            float hue = (Time.time * 0.02f) % 1f; // ゆっくり変化
            // 視認性を高めるため、彩度と明度を下げて少し落ち着いた色合いにする
            Camera.main.backgroundColor = Color.HSVToRGB(hue, 0.2f, 0.4f);
        }

        // 2. 背景バブルを定期的にスポーンさせる
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnBubble();
        }
    }

    private void SpawnBubble()
    {
        if (Camera.main == null) return;

        // プールから次に使うバブルを取り出す
        GameObject bubble = bubblePool[poolIndex];
        poolIndex = (poolIndex + 1) % MAX_BUBBLES;

        // カメラの表示領域（ワールド座標）を算出
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        float spawnX = Random.Range(-camWidth / 2f, camWidth / 2f);
        // 画面の少し下からスポーンさせる
        float spawnY = -Camera.main.orthographicSize - 1f;

        // 背景として描画させるため、Z座標は奥側（例: 10f）に配置
        bubble.transform.position = new Vector3(spawnX, spawnY, 10f);

        SpriteRenderer sr = bubble.GetComponent<SpriteRenderer>();
        
        // ランダムな色合い（パステルカラー、かなり薄い半透明）
        float hue = Random.Range(0f, 1f);
        Color color = Color.HSVToRGB(hue, 0.5f, 0.8f);
        color.a = Random.Range(0.05f, 0.15f); // 視認性を邪魔しないようかなり薄くする
        sr.color = color;

        // ランダムな大きさ
        float scale = Random.Range(0.2f, 0.8f);
        bubble.transform.localScale = new Vector3(scale, scale, 1f);

        // 浮遊の動きを制御するスクリプトをリセットしてアクティブにする
        FancyBubble behavior = bubble.GetComponent<FancyBubble>();
        behavior.speed = Random.Range(0.8f, 2.2f);
        behavior.amplitude = Random.Range(0.15f, 0.4f);
        behavior.frequency = Random.Range(0.8f, 2.0f);
        behavior.lifetime = Random.Range(6f, 12f);
        
        // 再生開始
        behavior.ResetBubble();
        bubble.SetActive(true);
    }

    // 実行時に動的にきれいな円のスプライトを生成する（テクスチャインポート不要）
    private Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];
        float center = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float maxDist = size / 2f;
                float alpha = Mathf.Clamp01(1f - (dist / maxDist));
                // 境界をなめらかにフェードアウトさせる
                alpha = Mathf.SmoothStep(0f, 1f, alpha);
                colors[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}

public class FancyBubble : MonoBehaviour
{
    public float speed = 1.5f;
    public float amplitude = 0.3f;
    public float frequency = 1.5f;
    public float lifetime = 8f;

    private float age = 0f;
    private float startX;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ResetBubble()
    {
        age = 0f;
        startX = transform.position.x;
    }

    void Update()
    {
        age += Time.deltaTime;
        if (age >= lifetime)
        {
            // 寿命が来たら非表示にする（オブジェクトプールに戻す）
            gameObject.SetActive(false);
            return;
        }

        // 上昇
        float nextY = transform.position.y + speed * Time.deltaTime;

        // 左右にゆらゆら揺らす
        float nextX = startX + Mathf.Sin(age * frequency) * amplitude;

        transform.position = new Vector3(nextX, nextY, transform.position.z);

        // 消え際にだんだんフェードアウト
        if (sr != null)
        {
            Color c = sr.color;
            // 寿命の残り割合に応じてアルファ値を減少
            c.a = Mathf.Lerp(c.a, 0f, age / lifetime);
            sr.color = c;
        }
    }
}
