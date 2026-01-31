using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {
        /* ===================== 引用 ===================== */

        [FoldoutGroup("Reference")]
        public Animator animator;

        [FoldoutGroup("Reference"), Tooltip("打开宝箱时显示的物体（在 Inspector 里拖）")]
        public GameObject revealObject;

        /* ===================== 设置 ===================== */

        [Header("触发设置")]
        public string playerTag = "Player";
        public bool openOnce = true;

        /* ===================== 运行时状态 ===================== */

        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
            }
        }
        private bool isOpened;

        /* ===================== 生命周期 ===================== */

        private void Reset()
        {
            // 防止忘记拖 Animator
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // 确保初始隐藏（防止忘记手动关）
            if (revealObject != null)
                revealObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (openOnce && IsOpened) return;

            Open();
        }

        /* ===================== 功能接口 ===================== */

        [FoldoutGroup("Runtime"), Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            IsOpened = true;

            if (revealObject != null)
                revealObject.SetActive(true);
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;

            // 如果你不希望关闭时再隐藏，可以删掉这一段
            if (revealObject != null)
                revealObject.SetActive(false);
        }
    }
}
