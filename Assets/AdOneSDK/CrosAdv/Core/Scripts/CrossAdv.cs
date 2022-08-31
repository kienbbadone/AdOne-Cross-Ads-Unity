using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace AdOneSDK.CrossAdv
{
    public partial class CrossAdv : MonoBehaviour
    {
        public static List<CrossAdvRespondData> CrossAdvData;

        public static CrossAdvRespondData DefaultAdvData;

        private static bool fetchDone;
        private static CrossAdv Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(this);
                return;
            }
            Instance = this;
            if (Directory.Exists($"{Application.persistentDataPath}/AdOneCrossAdv/Image/") == false)
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/AdOneCrossAdv/Image/");
            }
            if (Directory.Exists($"{Application.persistentDataPath}/AdOneCrossAdv/Video/") == false)
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/AdOneCrossAdv/Video/");
            }

            DefaultAdvData = new CrossAdvRespondData()
            {
                id = -1,
                app_key = Application.identifier,
                platform = "default platform",
                title = Application.productName,
                description = "default desc",
                button_text = "Play",
                creative_video = new List<string>() { },
                creative_image = new List<string>() { }
            };

            StartCoroutine(FetchMediaFiles());
        }
        public static void ShowImage(CrossAdvImage targetImg)
        {
            Instance.StartCoroutine(Instance.RoutineShowImage(targetImg));
        }

        internal static void ShowVideo(CrossAdvVideo player)
        {
            Instance.StartCoroutine(Instance.RoutineShowVideo(player));
        }
        IEnumerator RoutineShowVideo(CrossAdvVideo targetPlayer)
        {
            while (fetchDone == false)
            {
                yield return null;
            }

            var adv = CrossAdvData[Random.Range(0, CrossAdvData.Count)];

            targetPlayer.player.Stop();
            switch (targetPlayer.source)
            {
                case VideoSource.Url:
                    {
                        targetPlayer.player.source = VideoSource.Url;
                        if(adv.creative_video.Count > 0)
                            targetPlayer.player.url = adv.creative_video[Random.Range(0, adv.creative_video.Count)];
                    }
                    break;
                case VideoSource.VideoClip:
                    {
                        targetPlayer.player.source = VideoSource.Url;
                        if (adv.PathVideoLocal.Count > 0)
                            targetPlayer.player.url = adv.PathVideoLocal[Random.Range(0, adv.PathVideoLocal.Count)];
                    }
                    break;
            }
            targetPlayer.player.Play();
        }

        IEnumerator RoutineShowImage(CrossAdvImage targetImg)
        {
            while (fetchDone == false)
            {
                yield return null;
            }
            var adv = CrossAdvData[Random.Range(0, CrossAdvData.Count)];
            if(adv.Sprites.Count > 0)
                targetImg.img_Target.sprite = adv.Sprites[Random.Range(0, adv.Sprites.Count)];
            targetImg.txt_Button.text = adv.button_text;
            targetImg.txt_Name.text = adv.title;
            string clickUrl =
#if UNITY_ANDROID
            $"https://play.google.com/store/apps/details?id={adv.app_key}";
#elif UNITY_IOS
            $"https://apps.apple.com/app/find-people/id{adv.app_key}";
#endif
            targetImg.btn_AdClick.onClick.AddListener(() =>
            {
                Application.OpenURL(clickUrl);
            });
        }

        IEnumerator FetchMediaFiles()
        {
            fetchDone = false;

            string bundleId = Application.identifier;
            string platform = "unknown";
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.WindowsEditor:
                    platform = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platform = "ios";
                    break;

            }
            string uri = $"https://api-cap.adone.net/api/v1/adv-cross?bundle={bundleId}&platform={platform}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        var respond = JsonUtility.FromJson<CrossAdvRespondModel>(webRequest.downloadHandler.text);
                        CrossAdvData = respond.data;
                        if (CrossAdvData == null || CrossAdvData.Count == 0)
                        {
                            fetchDone = true;
                            yield break;
                        }
                        for (int i = 0; i < CrossAdvData.Count; i++)
                        {
                            yield return StartCoroutine(FetchAdvMedia(CrossAdvData[i]));
                        }
                        break;
                }

                fetchDone = true;
            }
        }

        IEnumerator FetchAdvMedia(CrossAdvRespondData adv)
        {
            for (int i = 0; i < adv.creative_image.Count; i++)
            {
                string url = adv.creative_image[i];
                string fileName = url.Split('/').Last();
                using (var www = UnityWebRequestTexture.GetTexture(url))
                {
                    //Send Request and wait
                    yield return www.SendWebRequest();

                    switch (www.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                            Debug.Log("Error while Receiving: " + www.error);
                            break;
                        case UnityWebRequest.Result.Success:
                            {
                                Debug.Log("Success");

                                //Load Image
                                Texture2D texture2d = DownloadHandlerTexture.GetContent(www);
                                var sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), Vector2.zero);
                                adv.Sprites.Add(sprite);
                                string localPath = $"{Application.persistentDataPath}/AdOneCrossAdv/Image/{fileName}";
                                File.WriteAllBytes(localPath, texture2d.GetRawTextureData());
                                adv.PathImageLocal.Add(localPath);
                            }
                            break;
                    }
                }
            }

            for (int i = 0; i < adv.creative_video.Count; i++)
            {
                string url = adv.creative_video[i];
                string fileName = url.Split('/').Last();
                using (var www = UnityWebRequest.Get(url))
                {
                    //Send Request and wait
                    yield return www.SendWebRequest();

                    switch (www.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                            Debug.Log("Error while Receiving: " + www.error);
                            break;
                        case UnityWebRequest.Result.Success:
                            {
                                Debug.Log("Success");

                                string localPath = $"{Application.persistentDataPath}/AdOneCrossAdv/Video/{fileName}";
                                File.WriteAllBytes(localPath, www.downloadHandler.data);
                                adv.PathVideoLocal.Add(localPath);
                            }
                            break;
                    }
                }
            }
        }
    }
}
