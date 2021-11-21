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

        background.texture = backCam;

        camAvailable = true;
    }

    private void Update()
    {
        if (!camAvailable)
            return;

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        if (orient == -90)
            background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
        else
            background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}
