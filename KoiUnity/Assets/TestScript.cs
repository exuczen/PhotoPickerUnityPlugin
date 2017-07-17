using UnityEngine;
using System.Collections;
using SimpleJSON;
using ExifLib;
using System;
using System.IO;
using UnityEngine.UI;
using UniqAssets;

public class TestScript : MonoBehaviour {

	//The result image
	public Image mImage;

	private string imagePath;

	private const string destImageName = "avatar.jpg";

	void Start()
    {
		//handle Image data returned from Plugin
		PhotoPicker photoPicker = PhotoPicker.Instance;
		photoPicker.SetCropSize(256, 256);
		photoPicker.SetCropFileName(destImageName);

		photoPicker.onImageReceived_android = OnImageReceived_android;
		photoPicker.onImageReceived_iOS = OnImageReceived_iOS;
	}


	void OnImageReceived_iOS(Texture2D texture, iOSPhotoAndCamera.State state) {
		mImage.gameObject.SetActive(true);
		mImage.preserveAspect = true;
		if (state == iOSPhotoAndCamera.State.kStateSuccess)
		{
			SetImageTexture(texture);
			//byte[] data = texture.GetRawTextureData();
			byte[] data = texture.EncodeToJPG();

			imagePath = Path.Combine(Application.persistentDataPath, destImageName);
			File.WriteAllBytes(imagePath, data);
		}
	}

	void OnImageReceived_android(ImageData data) {

		Debug.LogError("OnImageReceived");
		JpegInfo jpegInfo = ExifReader.ReadJpegFromBytes(data.data, "");
		Debug.LogError("jpegInfo.IsValid=" + jpegInfo.IsValid + " " + jpegInfo.Width + " " + jpegInfo.Height);
		Debug.LogError("jpegInfo.Orientation=" + jpegInfo.Orientation);

		Debug.LogError("width=" + data.width + " height=" + data.height);
		Debug.LogError("filePath=" + data.filePath);
		Debug.LogError("Application.persistentDataPath=" + Application.persistentDataPath);

		SetImageTexture(PhotoPicker.GetTexture2DFromImageData(data));
		imagePath = data.filePath;
		if (!File.Exists(imagePath)) {
			File.WriteAllBytes(imagePath, data.data);
		}
	}


	private void SetImageTexture(Texture2D texture)
	{
		Image image = mImage;
		Canvas canvas = image.GetComponentInParent<Canvas>();
		Vector2 canvasSize = canvas.pixelRect.size;
		Debug.Log("texture: " + " " + texture.width + " " + texture.height);
		float textureRatio = (float)texture.width / (float)texture.height;
		float screenRatio = (float)canvasSize.x / (float)canvasSize.y;
		float destWidth, destHeight;
		float canvasScaleFactor = canvas.scaleFactor;
		// destW/destH = texW/texH
		if (textureRatio > screenRatio)
		{
			destWidth = canvasSize.x / canvasScaleFactor;// * 0.9f;
			destHeight = destWidth / textureRatio;
		}
		else
		{
			destHeight = canvasSize.y / canvasScaleFactor;// * 0.9f;
			destWidth = destHeight * textureRatio;
		}

		Debug.LogError("SetImageTexture: " + destWidth + " " + destHeight + " " + textureRatio + " " + screenRatio + " " + canvasSize.x + " " + canvasSize.y);
		Rect imageRect = new Rect(0, 0, texture.width, texture.height);
		Destroy(image.sprite);
		image.sprite = Sprite.Create(texture, imageRect, new Vector2(0.5f, 0.5f));
		image.rectTransform.sizeDelta = new Vector2(destWidth, destHeight);
		mImage.preserveAspect = true;
	}

}


