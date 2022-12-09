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

        private static bool fetchDone;
        public static CrossAdv Instance;
        private int showImageCount, showVideoCount;
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

            StartCoroutine(FetchMediaFiles());
        }

        public static UnityEngine.Events.UnityEvent<ICrossAdPresenter> OnShowAdsFailed = new UnityEngine.Events.UnityEvent<ICrossAdPresenter>();
        public static UnityEngine.Events.UnityEvent<ICrossAdPresenter> OnShowAdsSuccess = new UnityEngine.Events.UnityEvent<ICrossAdPresenter>();//impression
        public static UnityEngine.Events.UnityEvent<ICrossAdPresenter> OnAdClicked = new UnityEngine.Events.UnityEvent<ICrossAdPresenter>();//ad has been clicked

        private void OnDestroy()
        {
            OnShowAdsFailed.RemoveAllListeners();
            OnShowAdsSuccess.RemoveAllListeners();
            OnAdClicked.RemoveAllListeners();
        }

        public void ShowImage(CrossAdvImageUI targetImg)
        {
            StartCoroutine(RoutineShowImage(targetImg));
        }

        internal void ShowVideo(CrossAdvVideoUI player)
        {
            StartCoroutine(RoutineShowVideo(player));
        }

        internal void ShowVideo(CrossAdvVideoMesh player)
        {
            StartCoroutine(RoutineShowVideo(player));
        }

        IEnumerator RoutineShowVideo(CrossAdvVideoMesh targetPlayer)
        {
            while (fetchDone == false)
            {
                yield return null;
            }

            if (CrossAdvData == null || CrossAdvData.Count == 0)
            {
                OnShowAdsFailed.Invoke(targetPlayer);
                yield break;
            }
            var adv = CrossAdvData[showVideoCount % CrossAdvData.Count];
            showVideoCount++;

            if (adv.PathVideoLocal.Count > 0)
            {
                targetPlayer.player.Stop();
                targetPlayer.player.url = adv.PathVideoLocal[Random.Range(0, adv.PathVideoLocal.Count)];
                targetPlayer.player.Prepare();
            }
            else
                yield break;

            StartCoroutine(RecordImpression(adv.imp_url));
            OnShowAdsSuccess.Invoke(targetPlayer);
        }
        internal void ShowImage(CrossAdvImageMesh targetImg)
        {

            StartCoroutine(RoutineShowImage(targetImg));
        }

        IEnumerator RoutineShowImage(CrossAdvImageMesh targetImg)
        {
            while (fetchDone == false)
            {
                yield return null;
            }

            if (CrossAdvData == null || CrossAdvData.Count == 0)
            {
                OnShowAdsFailed.Invoke(targetImg);
                yield break;
            }

            var adv = CrossAdvData[showImageCount % CrossAdvData.Count];
            showImageCount++;
            if (adv.Textures.Count == 0)
                yield break;

            targetImg.SetTexture(adv.Textures[Random.Range(0, adv.Textures.Count)]);

            OnShowAdsSuccess.Invoke(targetImg);
        }

        IEnumerator RoutineShowVideo(CrossAdvVideoUI targetPlayer)
        {
            while (fetchDone == false)
            {
                yield return null;
            }

            if (CrossAdvData == null || CrossAdvData.Count == 0)
            {
                OnShowAdsFailed.Invoke(targetPlayer);
                yield break;
            }
            var adv = CrossAdvData[showVideoCount % CrossAdvData.Count];
            showVideoCount++;
            string clickUrl = adv.click_url;

            targetPlayer.btn_AdClick.onClick.RemoveAllListeners();
            targetPlayer.btn_AdClick.onClick.AddListener(() =>
            {
                Debug.Log(clickUrl);
                Application.OpenURL(clickUrl);
            });

            if (adv.PathVideoLocal.Count > 0)
            {
                targetPlayer.player.Stop();                
                targetPlayer.player.url = adv.PathVideoLocal[Random.Range(0, adv.PathVideoLocal.Count)];
                targetPlayer.player.Prepare();
            }             
            else
                yield break;

            StartCoroutine(RecordImpression(adv.imp_url));
            OnShowAdsSuccess.Invoke(targetPlayer);
        }

        IEnumerator RoutineShowImage(CrossAdvImageUI targetImg)
        {
            while (fetchDone == false)
            {
                yield return null;
            }

            if (CrossAdvData == null || CrossAdvData.Count == 0)
            {
                OnShowAdsFailed.Invoke(targetImg);
                yield break;
            }

            var adv = CrossAdvData[showImageCount % CrossAdvData.Count];
            showImageCount++;
            if (adv.Sprites.Count == 0)
                yield break;

            targetImg.img_Target.sprite = adv.Sprites[Random.Range(0, adv.Sprites.Count)];
            targetImg.txt_Button.text = adv.button_text;
            targetImg.txt_Name.text = adv.title;
            string clickUrl = adv.click_url;
            targetImg.btn_AdClick.onClick.RemoveAllListeners();
            targetImg.btn_AdClick.onClick.AddListener(() =>
            {
                Debug.Log(clickUrl);
                Application.OpenURL(clickUrl);
            });

            StartCoroutine(RecordImpression(adv.imp_url));
            OnShowAdsSuccess.Invoke(targetImg);
        }

        IEnumerator RecordImpression(string uri)
        {
            //Debug.Log($"[AdOne] Recording impression {uri}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        break;
                    case UnityWebRequest.Result.Success:
                        {
                            //Debug.Log($"[AdOne] {webRequest.downloadHandler.text}");
                        }
                        break;
                }
            }
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
            Debug.Log($"[AdOne] Cross Ads Fetching url: {uri}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        break;
                    case UnityWebRequest.Result.Success:
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
                        case UnityWebRequest.Result.ProtocolError:
                            Debug.Log("Error while Receiving: " + www.error);
                            break;
                        case UnityWebRequest.Result.Success:
                            {
                                //Load Image
                                Texture2D texture2d = DownloadHandlerTexture.GetContent(www);
                                adv.Textures.Add(texture2d);
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
                        case UnityWebRequest.Result.ProtocolError:
                            Debug.Log("Error while Receiving: " + www.error);
                            break;
                        case UnityWebRequest.Result.Success:
                            {
                                //Debug.Log("Success");

                                string localPath = $"{Application.persistentDataPath}/AdOneCrossAdv/Video/{fileName}";
                                File.WriteAllBytes(localPath, www.downloadHandler.data);
                                adv.PathVideoLocal.Add(localPath);
                                adv.UrlVideoValid.Add(url);
                            }
                            break;
                    }
                }
            }
        }
    }
}
