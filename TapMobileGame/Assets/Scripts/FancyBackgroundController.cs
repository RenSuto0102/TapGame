using UnityEngine;

// 背景のレインボーカラー変化と、浮遊する泡（バブル）のスポーンを管理するスクリプト。
// 大量の生成破棄を伴うため「オブジェクトプーリング」という手法でインスタンスを管理。
public class FancyBackgroundController : MonoBehaviour
{
    private Sprite circleSprite;
    private float spawnTimer = 0f;
    private float spawnInterval = 0.4f; // 通常時のバブル出現間隔

    // --- オブジェクトプーリング設定 ---
    // ランタイムでの頻繁な Instantiate と Destroy によるメモリ確保（ガベージコレクション負荷）を防ぐための仕組み。
    // 事前に設定した最大数まで生成してプールに格納し、アクティブ・非アクティブを切り替えて再利用する。
    private const int MAX_BUBBLES = 30;
    private GameObject[] bubblePool;
    private int poolIndex = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // シーンを跨いで背景演出が途切れないよう、DontDestroyOnLoadでオブジェクトを保持。
        if (GameObject.Find("FancyBackgroundController") == null)
        {
            GameObject bg = new GameObject("FancyBackgroundController");
            bg.AddComponent<FancyBackgroundController>();
            DontDestroyOnLoad(bg);
        }
    }

    void Start()
    {
        // プログラムから動的にアルファグラデーション付きの円スプライトを作成
        circleSprite = CreateCircleSprite();
        
        // オブジェクトプールを初期化し、非アクティブ状態で事前生成
        bubblePool = new GameObject[MAX_BUBBLES];
        for (int i = 0; i < MAX_BUBBLES; i++)
        {
            GameObject bubble = new GameObject("FancyBubble");
            bubble.transform.SetParent(this.transform); // 整理用に階層下に配置
            SpriteRenderer sr = bubble.AddComponent<SpriteRenderer>();
            sr.sprite = circleSprite;
            bubble.AddComponent<FancyBubble>();
            bubble.SetActive(false); // 初期状態は非アクティブ
            bubblePool[i] = bubble;
        }
    }

    void Update()
    {
        // 残り時間をチェックし、10秒未満になったらピンチ状態とする
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        bool isPinch = (timeManager != null && timeManager.timeLimit < 10f);

        // 1. 背景色の制御
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            
            if (isPinch)
            {
                // 残り時間わずか: 赤系統の色をベースに、PingPongで点滅（警告フラッシュ）させる
                float redHue = 0f; 
                Camera.main.backgroundColor = Color.HSVToRGB(redHue, 0.4f + Mathf.PingPong(Time.time * 2f, 0.2f), 0.5f);
            }
            else
            {
                // 通常時: レインボーカラーをゆっくりグラデーション変化させる
                float hue = (Time.time * 0.02f) % 1f; 
                Camera.main.backgroundColor = Color.HSVToRGB(hue, 0.2f, 0.4f);
            }
        }

        // 2. バブルのスポーン処理
        // ピンチ時は発生速度を3倍に上げて画面の密度を高める
        float currentSpawnInterval = isPinch ? spawnInterval * 0.3f : spawnInterval;
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnBubble(isPinch);
        }
    }

    // プールからオブジェクトを取り出して再利用する
    private void SpawnBubble(bool isPinch = false)
    {
        if (Camera.main == null) return;

        // プールから順番にオブジェクトを選択
        GameObject bubble = bubblePool[poolIndex];
        poolIndex = (poolIndex + 1) % MAX_BUBBLES;

        // カメラの画角から画面幅を割り出し、出現する横幅（X座標）を決定
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        float spawnX = Random.Range(-camWidth / 2f, camWidth / 2f);
        float spawnY = -Camera.main.orthographicSize - 1f; // 画面最下部よりも下

        // ゲームオブジェクトの手前を塞がないようにZ座標を奥(10f)に配置
        bubble.transform.position = new Vector3(spawnX, spawnY, 10f);

        SpriteRenderer sr = bubble.GetComponent<SpriteRenderer>();
        
        // パステル調のランダムな色（背景の邪魔をしないよう薄くする）
        float hue = Random.Range(0f, 1f);
        Color color = Color.HSVToRGB(hue, 0.5f, 0.8f);
        color.a = Random.Range(0.05f, 0.15f); 
        sr.color = color;

        // 大きさをランダム化
        float scale = Random.Range(0.2f, 0.8f);
        bubble.transform.localScale = new Vector3(scale, scale, 1f);

        // 各バブルの動作を制御するスクリプトを更新して再起動
        FancyBubble behavior = bubble.GetComponent<FancyBubble>();
        // ピンチ時は移動速度を3倍、揺れ幅と周波数を2倍にして激しい動きにする
        behavior.speed = Random.Range(0.8f, 2.2f) * (isPinch ? 3f : 1f);
        behavior.amplitude = Random.Range(0.15f, 0.4f) * (isPinch ? 2f : 1f);
        behavior.frequency = Random.Range(0.8f, 2.0f) * (isPinch ? 2f : 1f);
        behavior.lifetime = Random.Range(6f, 12f) / (isPinch ? 2f : 1f);
        
        behavior.ResetBubble();
        bubble.SetActive(true);
    }

    // 動的に円のスプライト用のテクスチャをメモリ上に構築する
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
                
                // 外側に向かってなめらかにフェードアウトするようアルファを滑らかに調整
                alpha = Mathf.SmoothStep(0f, 1f, alpha);
                colors[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}

// 泡のふわふわ浮遊アクションを制御する個別のクラス
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
        
        // 寿命を迎えたら非アクティブにする（プールへ返却）
        if (age >= lifetime)
        {
            gameObject.SetActive(false);
            return;
        }

        // Y軸方向の上昇運動
        float nextY = transform.position.y + speed * Time.deltaTime;

        // X軸方向へのSin波による反復運動（揺らゆらした動きをシミュレーション）
        float nextX = startX + Mathf.Sin(age * frequency) * amplitude;

        transform.position = new Vector3(nextX, nextY, transform.position.z);

        // 寿命に合わせて徐々にフェードアウトさせる
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(c.a, 0f, age / lifetime);
            sr.color = c;
        }
    }
}
