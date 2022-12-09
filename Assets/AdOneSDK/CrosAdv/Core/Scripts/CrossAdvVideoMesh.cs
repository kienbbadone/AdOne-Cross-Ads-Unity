using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NaughtyAttributes;
using UnityEngine.UI;
using System;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(VideoPlayer))]
    public class CrossAdvVideoMesh : MonoBehaviour, ICrossAdPresenter
    {
        bool ShowSuccess;
        public VideoPlayer player { get; private set; }

        private void Awake()
        {
            player = GetComponent<VideoPlayer>();
            player.prepareCompleted += OnPrepareCompleted;
            player.loopPointReached += OnLoopPointReached;

            CrossAdv.OnShowAdsFailed.AddListener(OnShowAdsFailed);
            CrossAdv.OnShowAdsSuccess.AddListener(OnShowAdsSuccess);
        }

        private void OnLoopPointReached(VideoPlayer source)
        {
            CrossAdv.Instance.ShowVideo(this);
        }

        private void OnPrepareCompleted(VideoPlayer source)
        {
            //Debug.Log($"Video Length {source.length}");
            source.Play();
        }

        [Button]
        private void Start()
        {
            CrossAdv.Instance.ShowVideo(this);
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

                    CrossAdv.Instance.ShowVideo(this);
                }
            }
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
    }
}
