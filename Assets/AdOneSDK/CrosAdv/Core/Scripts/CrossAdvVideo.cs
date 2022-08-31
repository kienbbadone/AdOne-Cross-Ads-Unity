using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using NaughtyAttributes;
using UnityEngine.UI;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(VideoPlayer))]
    public class CrossAdvVideo : MonoBehaviour
    {
#if UNITY_EDITOR
        [OnValueChanged("OnChangeSource")]
#endif
        public VideoSource source;
        public Button btn_AdClick { get; private set; }
        public VideoPlayer player { get; private set;}
        private void Awake()
        {
            btn_AdClick = GetComponent<Button>();
            player = GetComponent<VideoPlayer>();
        }
        private void Start()
        {
            CrossAdv.Instance.ShowVideo(this);
        }

#if UNITY_EDITOR
        void OnChangeSource()
        {
            GetComponent<VideoPlayer>().source = source;
        }
#endif
    }
}
