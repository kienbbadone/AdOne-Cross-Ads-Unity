using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NaughtyAttributes;
using UnityEngine.UI;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(VideoPlayer))]
    public class CrossAdvVideo : MonoBehaviour
    {
#if UNITY_EDITOR
        [OnValueChanged("OnChangeSource")]
#endif
        public VideoSource source;
        public Button btn_AdClick;
        public VideoPlayer player { get; private set;}
        private void Awake()
        {
            player = GetComponent<VideoPlayer>();
            //player.audioOutputMode = VideoAudioOutputMode.None;
            //player.aspectRatio = VideoAspectRatio.FitInside;
            //player.renderMode = VideoRenderMode.RenderTexture;
        }
        private void Start()
        {
            CrossAdv.ShowVideo(this);
        }

#if UNITY_EDITOR
        void OnChangeSource()
        {
            GetComponent<VideoPlayer>().source = source;
        }
#endif
    }
}
