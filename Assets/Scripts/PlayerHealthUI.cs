using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image hpImage;

    [Header("血条图片（按血量从0到最大排序）")]
    public Sprite[] hpSprites;

    private int lastHP = -1;

    void Update()
    {
        if (playerHealth.currentHP != lastHP)
        {
            UpdateHPImage();
            lastHP = playerHealth.currentHP;
        }
    }

    void UpdateHPImage()
    {
        int hp = playerHealth.currentHP;

        if (hp < 0) hp = 0;
        if (hp >= hpSprites.Length) hp = hpSprites.Length - 1;

        hpImage.sprite = hpSprites[hp];
    }
}
