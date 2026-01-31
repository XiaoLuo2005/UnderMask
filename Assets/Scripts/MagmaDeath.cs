using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaDeath : MonoBehaviour
{
    [Header("Audio (音效)")]
    public AudioSource magmaSource;

    public int damage = 6;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        //// 命中玩家
        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("岩浆攻击到玩家");
            magmaSource.Play();
            var hp = col.collider.GetComponentInParent<PlayerHealth>();

            if (hp != null)
                hp.TakeDamage(damage);
        }
    }
}
