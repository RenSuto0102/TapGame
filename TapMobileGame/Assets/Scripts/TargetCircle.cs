using UnityEngine;

public class TargetCircle : MonoBehaviour
{
    // 円が出現するランダムな範囲（カメラのサイズに合わせて後から調整可能です）
    [Header("出現範囲")]
    public float minX = -2.0f;
    public float maxX = 2.0f;
    public float minY = -4.0f;
    public float maxY = 4.0f;

    void Start()
    {
        // ゲーム開始時にもランダムな位置に配置する
        MoveToRandomPosition();
    }

    [Header("エフェクト")]
    public GameObject tapEffectPrefab; // インスペクターからパーティクルのPrefabをセットします

    // このオブジェクト（円）がタップ（クリック）されたときに呼ばれる関数
    void OnMouseDown()
    {
        //スコアマネージャーを探す
        ScoreManager manager = FindAnyObjectByType<ScoreManager>();

        //スコア増加の関数を呼ぶ
        if(manager != null)
        {
            manager.AddScore(10);
        }

        // エフェクトのプレハブが設定されていれば、現在の位置に生成する
        if (tapEffectPrefab != null)
        {
            Instantiate(tapEffectPrefab, transform.position, Quaternion.identity);
        }

        // 現在の円を消して新しい円を出す（同じオブジェクトを別のランダムな場所に瞬間移動させることで表現します）
        MoveToRandomPosition();
    }

    // ランダムな位置に移動させる処理
    void MoveToRandomPosition()
    {
        // 指定した範囲の中からランダムなX座標とY座標を決める
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        // 新しい位置へ移動
        transform.position = new Vector3(randomX, randomY, 0f);
    }
}
