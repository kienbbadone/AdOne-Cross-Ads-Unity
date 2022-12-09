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

        private void Start()
        {
            CrossAdv.Instance.ShowVideo(this);
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
    }
}
