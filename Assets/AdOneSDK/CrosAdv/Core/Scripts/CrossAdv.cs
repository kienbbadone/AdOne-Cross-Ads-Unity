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
    public class CrossAdv : MonoBehaviour
    {
        public static List<string> urlImage = new List<string>();
        public static List<string> urlVideo = new List<string>();

        public static List<string> pathImageLocal = new List<string>();
        public static List<string> pathVideoLocal = new List<string>();

        public static List<Sprite> loadedSprites = new List<Sprite>();

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
        }
        void Start()
        {
            StartCoroutine(FetchMediaFiles());
        }

        public static void ShowImage(UnityEngine.UI.Image targetImg)
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

            targetPlayer.player.Stop();
            switch (targetPlayer.source)
            {
                case VideoSource.Url:
                    {
                        targetPlayer.player.source = VideoSource.Url;
                        targetPlayer.player.url = "https://cdn.onepad.com/video/Video4.mp4";// urlVideo[Random.Range(0, urlVideo.Count)];
                    }
                    break;
                case VideoSource.VideoClip:
                    {
                        targetPlayer.player.source = VideoSource.Url;
                        targetPlayer.player.url = $"{Application.persistentDataPath}/AdOneCrossAdv/Video/Video4.mp4";// pathVideoLocal[Random.Range(0, pathVideoLocal.Count)];
                    }
                    break;
            }
            targetPlayer.player.Play();
        }

        IEnumerator RoutineShowImage(Image targetImg)
        {
            while (fetchDone == false)
            {
                yield return null;
            }
            targetImg.sprite = loadedSprites[Random.Range(0, loadedSprites.Count)];
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
                        urlImage = respond.data.SelectMany(x => x.creative_image).ToList();
                        urlVideo = respond.data.SelectMany(x => x.creative_video).ToList();
                        for (int i = 0; i < urlImage.Count; i++)
                        {
                            yield return StartCoroutine(DownloadImage(urlImage[i]));
                        }
                        yield return StartCoroutine(DownloadVideo("https://cdn.onepad.com/video/Video4.mp4"));
                        //for (int i = 0; i < urlVideo.Count; i++)
                        //{
                        //    yield return StartCoroutine(DownloadVideo(urlVideo[i]));
                        //}
                        break;
                }

                fetchDone = true;
            }

            IEnumerator DownloadImage(string url)
            {
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
                                loadedSprites.Add(sprite);
                                string localPath = $"{Application.persistentDataPath}/AdOneCrossAdv/Image/{fileName}";
                                File.WriteAllBytes(localPath, texture2d.GetRawTextureData());
                                pathImageLocal.Add(localPath);
                            }
                            break;
                    }
                }
            }

            IEnumerator DownloadVideo(string url)
            {
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
                                pathVideoLocal.Add(localPath);
                            }
                            break;
                    }
                }
            }
        }
    }
}
