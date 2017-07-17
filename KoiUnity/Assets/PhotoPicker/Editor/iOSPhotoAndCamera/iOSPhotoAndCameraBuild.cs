using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class iOSPhotoAndCameraBuild 
{
	private const string NSCameraUsageDescription = "NSCameraUsageDescription";
	private const string NSPhotoLibraryUsageDescription = "NSPhotoLibraryUsageDescription";

	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) 
	{
		if (target == BuildTarget.iOS)
		{
			string plistFilePath = pathToBuiltProject + Path.DirectorySeparatorChar + "Info.plist";
			try 
			{
				PlistDocument plist = new PlistDocument();
				plist.ReadFromFile(plistFilePath);

				iOSPhotoAndCameraSettings settings = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/iOSPhotoAndCamera/iOSPhotoAndCameraSettings.asset") as iOSPhotoAndCameraSettings;

				plist.root.values[NSCameraUsageDescription] = new PlistElementString(settings.CameraUsageDescription);
				plist.root.values[NSPhotoLibraryUsageDescription] = new PlistElementString(settings.PhotoLibraryUsageDescription);

				string plistPathNew = plistFilePath + ".new";
				plist.WriteToFile(plistPathNew);

				File.Delete(plistFilePath);
				File.Move(plistPathNew, plistFilePath);
			} 
			catch (Exception e) 
			{
				Debug.LogError("iOSPhotoAndCameraBuild.OnPostprocessBuild plist error: " + e.Message);
			}
		}
	}
}