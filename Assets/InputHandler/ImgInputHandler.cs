using UnityEngine;

public class ImgInputHandler : InputHandler
{
    [SerializeField] ImageLoader imgLoader = default;

    void Start()
    {
        if (imgLoader == null)
        {
            Debug.LogError("Specify imgLoader");
        }
    }

    public void HandleImgLink(string url)
    {
        if (url != null && url != "")
        {
            Debug.Log("Entered url: " + url);
            savedString = url;
            imgLoader.UpdateImage(savedString);
        }
    }
}
