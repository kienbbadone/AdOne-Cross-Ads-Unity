using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(Renderer))]
    public class CrossAdvImageMesh : MonoBehaviour, ICrossAdPresenter
    {
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
            }
        }

        private void OnShowAdsFailed(ICrossAdPresenter arg0)
        {
            if (arg0 == this)
            {
            }
        }

        private void Start()
        {
            CrossAdv.Instance.ShowImage(this);
        }

        internal void SetTexture(Texture texture)
        {
            mat.SetTexture(materialTextureHash, texture);
        }
    }
}
