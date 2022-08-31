using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdOneSDK.CrossAdv
{
    public static class CrossAdvStartup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Startup()
        {
            GameObject adv = new GameObject("AdOneAdv");
            adv.AddComponent<CrossAdv>();
            GameObject.DontDestroyOnLoad(adv);
        }
    }
}
