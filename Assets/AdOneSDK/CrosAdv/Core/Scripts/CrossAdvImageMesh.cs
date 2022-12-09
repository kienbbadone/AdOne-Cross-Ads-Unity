using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(Renderer))]
    public class CrossAdvImageMesh : MonoBehaviour, ICrossAdPresenter
    {
        bool ShowSuccess;
        Material mat;
        static readonly int materialTextureHash = Shader.PropertyToID("_MainTex");
        private void Awake()
        {
            mat = GetComponent<Renderer>().material;

            CrossAdv.OnShowAdsFailed.AddListener(OnShowAdsFailed);
            CrossAdv.OnShowAdsSuccess.AddListener(OnShowAdsSuccess);
        }

        private void OnShowAdsSuccess(ICrossAdPresenter arg0)
        {
            if (arg0 == this)
            {
                ShowSuccess = true;
            }
        }

        private void OnShowAdsFailed(ICrossAdPresenter arg0)
        {
            if (arg0 == this)
            {
            }
        }

        [Button]
        private void Start()
        {
            CrossAdv.Instance.ShowImage(this);
            StartCoroutine(TryToShow());
        }
        IEnumerator TryToShow()
        {
            float currentTryCooldown = 3f;
            int tryCount = 0;
            while (ShowSuccess == false)
            {
                yield return null;
                currentTryCooldown -= Time.deltaTime;
                if (currentTryCooldown < 0f)
                {
                    currentTryCooldown = 3f + tryCount * 3f;
                    tryCount++;

                    CrossAdv.Instance.ShowImage(this);
                }
            }
        }
        internal void SetTexture(Texture texture)
        {
            mat.SetTexture(materialTextureHash, texture);
        }
    }
}
