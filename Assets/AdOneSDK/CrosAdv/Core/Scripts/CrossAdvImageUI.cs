using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AdOneSDK.CrossAdv
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Button))]
    public class CrossAdvImageUI : MonoBehaviour, ICrossAdPresenter
    {
        bool ShowSuccess;
        public Image img_Target;
        public Text txt_Button;
        public Text txt_Name;
        public Button btn_AdClick { get; private set; }
        public CanvasGroup can_show { get; private set; }

        private void Awake()
        {
            btn_AdClick = GetComponent<Button>();
            can_show = GetComponent<CanvasGroup>();
            CrossAdv.OnShowAdsFailed.AddListener(OnShowAdsFailed);
            CrossAdv.OnShowAdsSuccess.AddListener(OnShowAdsSuccess);

            can_show.alpha = 0f;
            can_show.blocksRaycasts = false;
            can_show.interactable = false;
        }

        private void OnShowAdsSuccess(ICrossAdPresenter arg0)
        {
            if (arg0 == this)
            {
                can_show.alpha = 1f;
                can_show.blocksRaycasts = true;
                can_show.interactable = true;
                ShowSuccess = true;
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
        float curcool = 3f;
        private void Update()
        {
            curcool -= Time.deltaTime;
            if (curcool <= 0f)
            {
                curcool = 3f;

                CrossAdv.Instance.ShowImage(this);
            }
        }
    }
}
