using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageLoader : MonoBehaviour
{
    Image image;
    public string url = "";

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void UpdateImage(string newUrl)
    {
        StartCoroutine(LoadUrlImageCo(newUrl));
    }

    IEnumerator LoadUrlImageCo(string newUrl)
    {
        if (newUrl == "")
        {
            yield break;
        }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(newUrl);
        var asyncOperation = www.SendWebRequest();
        float progress;
        while (!www.isDone)
        {
            progress = asyncOperation.progress;
            yield return null;
        }
        progress = 1f;


        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("TESTING");
            Debug.LogError(www.error);
            url = null;
        }

        var texture = DownloadHandlerTexture.GetContent(www);

        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        url = newUrl;
    }
}
