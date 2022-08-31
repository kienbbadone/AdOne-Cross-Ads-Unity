using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AdOneSDK.CrossAdv
{
    public class CrossAdvImage : MonoBehaviour
    {
        public Image img_Target;
        public Text txt_Button;
        public Text txt_Name;
        public Button btn_AdClick;

        private void Start()
        {
            CrossAdv.ShowImage(this);
        }
        float curcool = 3f;
        private void Update()
        {
            curcool -= Time.deltaTime;
            if (curcool <= 0f)
            {
                curcool = 3f;

                CrossAdv.ShowImage(this);
            }
        }
    }
}
