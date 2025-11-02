using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // 如果這行在 Console 出現，表示三角錐上沒有 Audio Source
            Debug.LogError("三角錐上缺少 AudioSource 元件！請確保已添加。");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ----------------------------------------------------
        // 檢查重點就在這一行：
        // 確保這裡的 "Drone" 與您無人機上的 Tag (標籤) 完全一致
        // ----------------------------------------------------
        if (collision.gameObject.CompareTag("Drone")) // <--- 檢查這裡的 "Drone"
        {
            // 如果是無人機，就播放音效
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
                // 如果這行在 Console 出現，表示碰撞偵測成功
                Debug.Log("無人機撞到三角錐了，播放音效！");
            }
        }
    }
}