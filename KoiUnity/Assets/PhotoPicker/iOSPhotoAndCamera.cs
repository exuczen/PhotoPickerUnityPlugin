using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

namespace UniqAssets 
{
	public class iOSPhotoAndCamera : MonoBehaviour 
	{
		public enum State
		{
			kStateSuccess,
			kStateUserCancelled,
			kStateFileNotFound
		}

		private static System.Action<Texture2D, iOSPhotoAndCamera.State> _callback;
		private static iOSPhotoAndCamera _instance;
	
		void Start()
		{
			DontDestroyOnLoad (gameObject);
		}

		/// <summary>Let the user take a photo on an iOS device (prints a warning when called on any other platform).</summary>
		/// <param name="allowEditing">Allows the user to edit the taken photo.</param>
		/// <param name="callback">Called when user has taken photo or when user cancelled.</param>
		/// <example> 
		/// This sample shows how to call the TakePhoto method.
		/// <code>
		/// using UniqAssets;
		/// 
		/// class TestClass : MonoBehaviour
		/// {
		///     void Start() 
		/// 	{
		/// 		iOSPhotoAndCamera.TakePhoto(true, (texture, state) => 
		/// 		{
		/// 			if (state == iOSPhotoAndCamera.State.kStateSuccess)
		/// 			{
		/// 				myRawImageComponent.texture = texture; 
		/// 			}
		/// 		});
		/// 	}
		/// }
		/// </code>
		/// </example>
		public static void TakePhoto(bool allowEditing, System.Action<Texture2D, iOSPhotoAndCamera.State> callback)
		{
			ChoosePhoto (false, allowEditing, callback);
		}

		/// <summary>Let the user choose a photo from library on an iOS device (prints a warning when called on any other platform).</summary>
		/// <param name="allowEditing">Allows the user to edit the chosen photo.</param>
		/// <param name="callback">Called when user has chosen photo or when user cancelled.</param>
		/// <example> 
		/// This sample shows how to call the SelectPhoto method.
		/// <code>
		/// using UniqAssets;
		/// 
		/// class TestClass : MonoBehaviour
		/// {
		///     void Start() 
		/// 	{
		/// 		iOSPhotoAndCamera.SelectPhoto(true, (texture, state) => 
		/// 		{
		/// 			if (state == iOSPhotoAndCamera.State.kStateSuccess)
		/// 			{
		/// 				myRawImageComponent.texture = texture; 
		/// 			}
		/// 		});
		/// 	}
		/// }
		/// </code>
		/// </example>
		public static void SelectPhoto(bool allowEditing, System.Action<Texture2D, iOSPhotoAndCamera.State> callback)
		{
			ChoosePhoto (true, allowEditing, callback);
		}

		private static iOSPhotoAndCamera _Init() 
		{
			if (_instance == null) 
			{
				GameObject go = new GameObject ("[iOSPhotoAndCamera]");
				go.hideFlags = HideFlags.HideAndDontSave;
				_instance = go.AddComponent<iOSPhotoAndCamera> ();
			}
			return _instance;
		}

		[DllImport ("__Internal")]
		private static extern void _ChoosePhoto (bool fromLibrary, bool allowEditing);

		private static void ChoosePhoto(bool fromLibrary, bool allowEditing, System.Action<Texture2D, iOSPhotoAndCamera.State> callback) 
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer) 
			{
				Debug.LogWarning ("Cannot access camera or photo library on this platform!");
				return;
			}

			if (_callback != null) 
			{
				Debug.LogWarning ("Media picking already in progress!");
				return;
			}

			_Init ();
			_callback = callback;
			_ChoosePhoto (fromLibrary, allowEditing);
		}

		private void _DidFinishPickingMedia (string message)
		{
			if (_callback != null) 
			{
				Texture2D tex = null;
				if (File.Exists (message)) 
				{
					tex = new Texture2D (2, 2);
					tex.LoadImage (File.ReadAllBytes (message));
					File.Delete (message);
					_callback (tex, iOSPhotoAndCamera.State.kStateSuccess);
				} else 
				{
					_callback (tex, iOSPhotoAndCamera.State.kStateFileNotFound);
				}
				_callback = null;
			}
		}

		private void _DidCancel(string message)
		{
			if (_callback != null) 
			{
				_callback (null, iOSPhotoAndCamera.State.kStateUserCancelled);
				_callback = null;
			}
		}
	}
}

