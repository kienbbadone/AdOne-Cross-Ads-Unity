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

        private void Start()
        {
            CrossAdv.ShowImage(this.img_Target);
        }
        float curcool = 2f;
        private void Update()
        {
            curcool -= Time.deltaTime;
            if (curcool <= 0f)
            {
                curcool = 2f;

                CrossAdv.ShowImage(this.img_Target);
            }
        }
    }
}
