using SimpleJSON;
using System;
using System.Collections;
using System.IO;
using UniqAssets;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class ImageData
{
	public int width;
	public int height;
	public string filePath;
	public byte[] data;
}

public class PhotoPickerPlugin : MonoBehaviour
{
	[SerializeField]
	private Button openGalleryButton;

	[SerializeField]
	private Button takePhotoButton;

	[SerializeField]
	private Button cancelButton;

	private void Start()
	{
		PhotoPicker picker = PhotoPicker.Instance;

		picker.SetGameObjectName(gameObject.name);

		if (openGalleryButton != null)
		{
			openGalleryButton.onClick.AddListener(picker.OpenGallery);
		}
		if (takePhotoButton != null)
		{
			takePhotoButton.onClick.AddListener(picker.OpenGallery);
		}
		if (cancelButton != null)
		{
			cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });
		}
	}

	#region Android callback methods
	void OnPickDoneFromCamera(string jsonParams)
	{
		PhotoPicker.Instance.HandleImage_android(jsonParams);
	}

	void OnPickDoneFromGallery(string jsonParams)
	{
		PhotoPicker.Instance.HandleImage_android(jsonParams);
	}

	void OnCropDone(string jsonParams)
	{
		PhotoPicker.Instance.HandleImage_android(jsonParams);
	}
	#endregion

}

public class PhotoPicker
{
	private static PhotoPicker instance;
	public static PhotoPicker Instance { get { return (instance ?? (instance = new PhotoPicker())); } }
	public static PhotoPicker GetInstance()
	{
		if (instance == null)
		{
			instance = new PhotoPicker();
		}
		return instance;
	}

	#if (UNITY_ANDROID && !UNITY_EDITOR)
	AndroidJavaObject androidObject;
	AndroidJavaObject unityObject;
	#endif

	public Action<ImageData> onImageReceived_android;
	public Action<Texture2D, iOSPhotoAndCamera.State> onImageReceived_iOS;

	public static Texture2D GetTexture2DFromImageData(ImageData data) {
		Texture2D texture = new Texture2D(data.width, data.height, TextureFormat.BGRA32, false);
		texture.LoadImage(data.data);
		return texture;
	}

	private string GetFilePath(Uri uri, string uriPath)
	{
		if (!string.IsNullOrEmpty(uriPath) && File.Exists(uriPath))
		{
			return uriPath;
		}
		else if (uri != null)
		{
			if (File.Exists(uri.LocalPath))
			{
				return uri.LocalPath;
			}
			else if (File.Exists(uri.AbsolutePath))
			{
				return uri.AbsolutePath;
			}
		}
		return null;
	}

	private ImageData DeserializeImageData(string jsonParams, byte[] data) {

		Debug.LogError(jsonParams);
		JSONArray jsa = (JSONArray)JSON.Parse(jsonParams);
		JSONNode jsn = jsa[0];

		ImageData imageData = new ImageData();
		imageData.width = jsn["width"].AsInt;
		imageData.height = jsn["height"].AsInt;
		imageData.filePath = GetFilePath(new Uri(jsn["uri"]), jsn["uriPath"]);
		imageData.data = data;
		
		if (imageData.filePath == null)
		{
			imageData.filePath = Path.Combine(Application.persistentDataPath, "photoFromGalleryOrCamera.jpeg");
		}

		return imageData;
	}


	public void HandleImage_android(string param)
	{
		if (onImageReceived_android == null)
		{
			Debug.Log("You must assign imageDelegate first");
		}
		else
		{
			#if (UNITY_ANDROID && !UNITY_EDITOR)
			onImageReceived_android(DeserializeImageData(param, androidObject.Call<byte[]>("getPluginData")));
            androidObject.Call("cleanUp");
			#endif
		}
	}


	private PhotoPicker()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaClass androidClass = new AndroidJavaClass("com.pictorytale.messenger.android.photo.picker.Plugin");
        androidObject = androidClass.GetStatic<AndroidJavaObject>("instance");
		#endif
	}

	public void SetGameObjectName(string gameObjectName) {
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.CallStatic("setUnityObjectName", gameObjectName);
		#endif
	}

	public void SetCropSize(int width, int height) {
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.CallStatic("setCropSize", width, height);
		#endif
	}

	public void SetCopiedFileName(string name) {
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.CallStatic("setCopiedFileName", name);
		#endif
	}

	public void SetCropFileName(string name) {
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.CallStatic("setCropFileName", name);
		#endif
	}


	public void OpenCamera()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.Call("launchCamera", unityObject);
		#elif UNITY_IOS
		iOSPhotoAndCamera.TakePhoto(true, onImageReceived_iOS);
		#endif
	}

	public void OpenGallery()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		androidObject.Call("launchGallery", unityObject);
		#elif UNITY_IOS
		iOSPhotoAndCamera.SelectPhoto(true, onImageReceived_iOS);
		#endif
	}

}


