using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace AnyRes.Util
{
	
	public class Presets : MonoBehaviour
	{
 
            

		public bool windowEnabled = false;
		public bool newEnabled = false;
		public bool loadEnabled = false;

		public Rect windowRect = new Rect(30, 30, 200, 100);
		public Rect newRect = new Rect(30, 30, 200, 230);
		public Rect loadRect = new Rect(30, 30, 200, 400);

		string newName = "Name";
		string newX = "1280";
		string newY = "720";
		bool newFullscreen = false;

		string[] files = Directory.GetFiles(KSPUtil.ApplicationRootPath.Replace("\\", "/") + "GameData/AnyRes/presets/", "*.cfg");
		string file = "";

		void Start () {

			Debug.Log ("Started");

		}

		void Update () {



		}

		void OnGUI () {
            
			GUI.skin = HighLogic.Skin;

			if (windowEnabled) {

				windowRect = GUI.Window (09272, windowRect, onWindow, "Presets");

			}

			if (newEnabled) {

				newRect = GUI.Window (09273, newRect, onNew, "New Preset");

			}

			if (loadEnabled) {

				loadRect = GUI.Window (09274, loadRect, onLoad, "Load Preset");

			}

		}

		void onWindow (int windowID) {

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("New")) {

				newEnabled = !newEnabled;
                loadEnabled = false;


            }
			if (GUILayout.Button ("Load")) {

				loadEnabled = !loadEnabled;
				files = Directory.GetFiles(KSPUtil.ApplicationRootPath.Replace("\\", "/") + "GameData/AnyRes/presets/", "*.cfg");
                newEnabled = false;
			}
			GUILayout.EndVertical ();

			GUI.DragWindow ();

		}

		void onNew(int windowID) {

			GUILayout.BeginVertical ();
			newName = GUILayout.TextField (newName);
			newX = GUILayout.TextField (newX);
			newX = Regex.Replace (newX, @"[^0-9]", "");
			newY = GUILayout.TextField (newY);
			newY = Regex.Replace (newY, @"[^0-9]", "");
			newFullscreen = GUILayout.Toggle (newFullscreen, "Fullscreen");
			if (GUILayout.Button ("Save")) {

				ConfigNode config = new ConfigNode (newName);
				config.AddValue ("name", newName);
				config.AddValue ("x", newX);
				config.AddValue ("y", newY);
				config.AddValue ("fullscreen", newFullscreen.ToString());
				config.Save (KSPUtil.ApplicationRootPath.Replace ("\\", "/") + "GameData/AnyRes/presets/" + newName + ".cfg");

				ScreenMessages.PostScreenMessage ("Preset saved.  You can change the preset later by using the same name in this editor.", 5, ScreenMessageStyle.UPPER_CENTER);

			}
			if (GUILayout.Button ("Cancel")) {
				
				newName = "Name";
				newX = "1280";
				newY = "720";
				newFullscreen = false;
				newEnabled = false;

			}
			GUILayout.EndVertical ();

			GUI.DragWindow ();

		}

		void onLoad (int windowID) {



			GUILayout.BeginScrollView (new Vector2 (0, 0));
			for (int i = files.Length - 1; i >= 0; --i)
			{
				
				file = files[i];

				ConfigNode config = ConfigNode.Load (file);
				if (GUILayout.Button(config.GetValue("name"))) {

					int xVal;
					int.TryParse(config.GetValue("x"), out xVal);
					int yVal;
					int.TryParse(config.GetValue("y"), out yVal);
					bool fullscreen;
					bool.TryParse (config.GetValue("fullscreen"), out fullscreen);
					GameSettings.SCREEN_RESOLUTION_HEIGHT = yVal;
					GameSettings.SCREEN_RESOLUTION_WIDTH = xVal;
					GameSettings.FULLSCREEN = fullscreen;
					GameSettings.SaveSettings ();
					Screen.SetResolution(xVal, yVal, fullscreen);
					Debug.Log ("[AnyRes] Set screen resolution from preset");

				}

			}
			GUILayout.EndScrollView ();

			GUI.DragWindow ();

		}

	}
}

