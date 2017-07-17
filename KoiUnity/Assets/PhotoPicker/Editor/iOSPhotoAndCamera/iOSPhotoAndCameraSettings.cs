using UnityEngine;
using UnityEditor;

public class iOSPhotoAndCameraSettings : ScriptableObject 
{
	public string CameraUsageDescription = "Camera use";
	public string PhotoLibraryUsageDescription = "Photo library use";

	[MenuItem("Window/UniqAssets/iOS Photo And Camera Settings")]
	static void ShowSettings() 
	{
		Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/PhotoPicker/Editor/iOSPhotoAndCamera/iOSPhotoAndCameraSettings.asset");
	}
}