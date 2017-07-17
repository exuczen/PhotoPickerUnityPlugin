using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniqAssets;

public class iOSPhotoAndCameraExample : MonoBehaviour
{
	public Image image;

	public void TakePhoto()
	{
		iOSPhotoAndCamera.TakePhoto(true, (texture, state) =>
		   {
			   image.gameObject.SetActive(true);

			   if (state == iOSPhotoAndCamera.State.kStateSuccess)
				   image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f);

			   image.preserveAspect = true;
		   });
	}

	public void SelectPhoto()
	{
		iOSPhotoAndCamera.SelectPhoto(true, (texture, state) =>
		   {
			   image.gameObject.SetActive(true);
			   image.preserveAspect = true;

			   if (state == iOSPhotoAndCamera.State.kStateSuccess)
				   image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f);

			   image.preserveAspect = true;
		   });
	}
}
