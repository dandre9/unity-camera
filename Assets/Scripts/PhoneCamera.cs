using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    bool camAvailable;
    bool isTouching = false;
    WebCamTexture backCam;
    Texture defaultBackground;

    [SerializeField] RawImage background;
    [SerializeField] AspectRatioFitter fit;
    [SerializeField] Text debugText;

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

        if (Input.touchCount > 0)
        {
            if (!isTouching)
            {
                isTouching = true;
                Touch touch = Input.GetTouch(0);

                if (touch.position.y / Screen.height > 0.1f)
                    backCam.autoFocusPoint = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);

                debugText.text = backCam.autoFocusPoint.ToString();
            }
        }
        else
        {
            isTouching = false;
        }
    }

    public void TakePhotoClick()
    {
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()  // Start this Coroutine on some button click
    {

        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo = rotateTexture(photo, true);
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToJPG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes("/storage/emulated/0/DCIM/Camera/photo" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg", bytes);
    }

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
