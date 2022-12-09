using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NaughtyAttributes;
using UnityEngine.UI;
using System;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(CanvasGroup))]
    public class CrossAdvVideo : MonoBehaviour, ICrossAdPresenter
    {
#if UNITY_EDITOR
        [OnValueChanged("OnChangeSource")]
#endif
        public VideoSource source;
        public Button btn_AdClick { get; private set; }
        public VideoPlayer player { get; private set; }
        public CanvasGroup can_show { get; private set; }

        private void Awake()
        {
            btn_AdClick = GetComponent<Button>();
            player = GetComponent<VideoPlayer>();
            player.prepareCompleted += OnPrepareCompleted;
            player.loopPointReached += OnLoopPointReached;

            can_show = GetComponent<CanvasGroup>();
            CrossAdv.OnShowAdsFailed.AddListener(OnShowAdsFailed);
            CrossAdv.OnShowAdsSuccess.AddListener(OnShowAdsSuccess);

            can_show.alpha = 0f;
            can_show.blocksRaycasts = false;
            can_show.interactable = false;
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
                can_show.alpha = 1f;
                can_show.blocksRaycasts = true;
                can_show.interactable = true;
            }
        }

        private void OnShowAdsFailed(ICrossAdPresenter arg0)
        {
            if (arg0 == this)
            {
                can_show.alpha = 0f;
                can_show.blocksRaycasts = false;
                can_show.interactable = false;
            }
        }
#if UNITY_EDITOR
        void OnChangeSource()
        {
            GetComponent<VideoPlayer>().source = source;
        }
#endif
    }
}
