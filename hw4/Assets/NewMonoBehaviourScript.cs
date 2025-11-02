using System.Collections;

using System.Collections.Generic;

using UnityEngine;



// 建議將腳本名稱改為 DroneController

public class DroneController : MonoBehaviour

{

    // --- 移動相關變數 ---

    float speedX = 0; // 用於水平移動 (X軸)

    float speedY = 0; // 新增：用於垂直移動 (Y軸)

    Vector2 startPos;



    // 控制無人機是否停止移動 (撞到三角錐後設為 true)

    private bool isStopped = false;



    // 調整滑動靈敏度，值越大，移動越慢 (初始值提高以讓速度慢一點)

    public float swipeSensitivity = 1000.0f;



    // 用於整體微調移動速度的乘數

    public float moveSpeedFactor = 30.0f; // 數值越小，移動越慢



    // 減速因子，值越小，減速越快

    public float dampingFactor = 0.96f;



    // --- 音效相關變數 ---

    private AudioSource audioSource;



    [Header("音效設定")]

    // 請在 Unity Inspector 視窗中拖入您的音效檔案

    public AudioClip flyingSound;       // 飛行時的音效 (循環播放)

    public AudioClip coneHitSound;      // 碰到三角錐的音效

    public AudioClip conePassSound;     // 成功通過/沒碰到三角錐的音效



    void Start()

    {

        // 獲取 AudioSource 組件，如果沒有則新增一個

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)

        {

            audioSource = gameObject.AddComponent<AudioSource>();

        }



        // 播放飛行音效 (並設為循環播放)

        if (flyingSound != null)

        {

            audioSource.clip = flyingSound;

            audioSource.loop = true;

            audioSource.Play();

        }

    }



    void Update()

    {

        // 檢查無人機是否停止，如果已停止則不執行移動和滑動邏輯

        if (isStopped)

        {

            // 如果撞到後，飛行音效也應該停止

            if (audioSource.isPlaying && audioSource.clip == flyingSound)

            {

                audioSource.Stop();

            }

            return;

        }



        // --- 處理滑動輸入 ---

        if (Input.GetMouseButtonDown(0))

        {

            startPos = Input.mousePosition;

        }

        else if (Input.GetMouseButtonUp(0))

        {

            Vector2 endPos = Input.mousePosition;



            // 計算滑動長度

            float swipeLengthX = endPos.x - startPos.x;

            float swipeLengthY = endPos.y - startPos.y; // 新增垂直滑動



            // 設定 X 軸和 Y 軸速度

            // 速度計算：滑動長度 / 靈敏度

            this.speedX = swipeLengthX / swipeSensitivity;

            this.speedY = swipeLengthY / swipeSensitivity;

        }



        // --- 移動 ---

        // 結合 X (左右) 和 Y (上下) 軸移動

        // 乘以 Time.deltaTime 和 moveSpeedFactor 確保平滑且控制整體速度

        Vector3 moveDelta = new Vector3(this.speedX, this.speedY, 0) * moveSpeedFactor * Time.deltaTime;

        transform.Translate(moveDelta, Space.World);



        // --- 減速 (阻尼) ---

        this.speedX *= dampingFactor;

        this.speedY *= dampingFactor;



        // 如果速度接近於零，則完全歸零

        if (Mathf.Abs(this.speedX) < 0.005f) this.speedX = 0;

        if (Mathf.Abs(this.speedY) < 0.005f) this.speedY = 0;

    }



    // --- 碰撞/觸發器 處理 ---

    // 無人機必須有 Collider 和 Rigidbody (可設為 Is Kinematic)

    void OnTriggerEnter(Collider other)

    {

        // 1. 碰到三角錐

        // 假設您的三角錐 Tag 設定為 "Cone"

        if (other.CompareTag("Cone"))

        {

            // 播放碰撞音效

            if (coneHitSound != null)

            {

                audioSource.PlayOneShot(coneHitSound);

            }



            // 停止無人機移動

            isStopped = true;



            Debug.Log("無人機撞到三角錐！已停止移動。");

        }



        // 2. 沒碰到三角錐 (成功通過)

        // 假設您在三角錐後面設定了一個 Tag 為 "PassZone" 的隱形觸發器

        else if (other.CompareTag("PassZone"))

        {

            if (conePassSound != null)

            {

                audioSource.PlayOneShot(conePassSound);

            }

            Debug.Log("成功通過！");



            // 成功通過後，可以禁用這個通過區域，防止重複觸發

            other.enabled = false;

        }

    }

}