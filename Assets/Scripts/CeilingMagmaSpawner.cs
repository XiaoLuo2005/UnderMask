using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingMagmaSpawner : MonoBehaviour
{
    public GameObject spikePrefab;

    [Header("线段参数")]
    public float width = 12f;      // 天花板长度（线段总宽度）
    public int spikeCount = 6;     // 同时发射数量

    [Header("发射节奏")]
    public float interval = 2f;

    void Start()
    {
        // 1秒后开始，每隔interval秒生成一次
        InvokeRepeating(nameof(SpawnLine), 1f, interval);
    }

    void SpawnLine()
    {
        // 循环生成指定数量的岩浆投射物
        for (int i = 0; i < spikeCount; i++)
        {
            // 核心修改：在[-width/2, width/2]范围内随机生成X坐标
            float randomX = Random.Range(-width / 2f, width / 2f);
            // 计算最终生成位置（基于当前物体的位置 + 随机X偏移）
            Vector3 randomPos = transform.position + new Vector3(randomX, 0, 0);

            // 生成投射物
            Instantiate(spikePrefab, randomPos, Quaternion.identity);
        }
    }

    void OnDrawGizmos()
    {
        // 绘制红色线段，方便在编辑器中查看生成范围
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position + Vector3.left * width / 2,
            transform.position + Vector3.right * width / 2
        );
    }
}