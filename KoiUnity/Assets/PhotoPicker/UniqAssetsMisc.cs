#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UniqAssets 
{
	[InitializeOnLoad]
	public class UniqAssetsMisc 
	{
		//static readonly string EP_UNIQASSETS_IOSPAC_USAGE_TIME = "EP_UNIQASSETS_IOSPAC_USAGE_TIME";
		//static readonly string EP_UNIQASSETS_IOSPAC_ASKED_FOR_RATING = "EP_UNIQASSETS_IOSPAC_ASKED_FOR_RATING";
		//static readonly string ASSET_STORE_URL = "http://u3d.as/Pag";

		static UniqAssetsMisc() 
		{
			//if (string.IsNullOrEmpty (EditorPrefs.GetString (EP_UNIQASSETS_IOSPAC_USAGE_TIME, string.Empty))) 
			//{
			//	EditorPrefs.SetString(EP_UNIQASSETS_IOSPAC_USAGE_TIME, DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"));
			//	return;
			//}
			//DateTime usageTime = DateTime.ParseExact(EditorPrefs.GetString (EP_UNIQASSETS_IOSPAC_USAGE_TIME), "yyyy-MM-dd HH:mm:ss", null);
			//if ((DateTime.Now - usageTime).Days > 2 && EditorPrefs.GetBool(EP_UNIQASSETS_IOSPAC_ASKED_FOR_RATING, false) == false) 
			//{
			//	EditorPrefs.SetBool(EP_UNIQASSETS_IOSPAC_ASKED_FOR_RATING, true);
			//	bool rateMe = EditorUtility.DisplayDialog("You like 'iOS Photo And Camera'?", "Please rate it on the Unity Asset Store. That would help me a lot :)", "Rate now!", "No, thanks!");
			//	if (rateMe) 
			//	{
			//		Application.OpenURL(ASSET_STORE_URL);
			//	}
			//}
		}
	}
}
#endif
