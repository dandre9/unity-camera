using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    bool camAvailable;
    WebCamTexture backCam;
    Texture defaultBackground;
    float scale = 1f;

    [SerializeField] RawImage background;
    [SerializeField] AspectRatioFitter fit;
    [SerializeField] Text infoText;

    private void Start()
    {
        defaultBackground = background.texture;

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            //TODO: Open photo gallery
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (backCam == null)
        {
            return;
        }

        backCam.Play();

        // infoText.text = background.rectTransform.localScale.x.ToString() + " / " + background.rectTransform.localScale.y.ToString();

        background.texture = backCam;

        // infoText.text += " -- " + background.rectTransform.localScale.x.ToString() + " / " + background.rectTransform.localScale.y.ToString();

        camAvailable = true;
    }

    private void Update()
    {
        if (!camAvailable)
            return;

        // float ratio = (float)Screen.width / (float)Screen.height;
        // fit.aspectRatio = ratio;

        // infoText.text = fit.aspectRatio.ToString();

        // float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(scale, scale, scale);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
    }

    public void ChangeScale(string plusOrMinus)
    {
        if (plusOrMinus == "plus")
            scale += 0.1f;
        else
            scale -= 0.1f;
    }
}
