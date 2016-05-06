using System;
using UnityEngine;
using System.Text.RegularExpressions;  //Get Regex
using KSP.UI.Screens;

namespace AnyRes
{

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class AnyRes : MonoBehaviour
	{

		public static Rect windowRect = new Rect(35, 99, 200, 190);

		public string xString = "1280";
		public string yString = "720";

		public int x = 1280;
		public int y = 720;

		public bool windowEnabled = false;
		public bool fullScreen = true;
		public bool reloadScene = false;

		private static ApplicationLauncherButton appLauncherButton;

		void Start() {

			//Thanks bananashavings http://forum.kerbalspaceprogram.com/index.php?/profile/156147-bananashavings/ - https://gist.github.com/bananashavings/e698f4359e1628b5d6ef
			//Also thanks to Crzyrndm for the fix to that code!
			if (appLauncherButton == null) {

				appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
					() => { windowEnabled = true; },
					() => { windowEnabled = false; },
					() => {},
					() => {},
					() => {},
					() => {},
					ApplicationLauncher.AppScenes.ALWAYS,
					(Texture)GameDatabase.Instance.GetTexture("AnyRes/textures/toolbar", false));
				
			}

			xString = GameSettings.SCREEN_RESOLUTION_WIDTH.ToString ();
			yString = GameSettings.SCREEN_RESOLUTION_HEIGHT.ToString ();
			fullScreen = GameSettings.FULLSCREEN;

			if (HighLogic.LoadedScene == GameScenes.SETTINGS) {

				windowEnabled = true;
				windowRect.x = 35;
				windowRect.y = 99;

			} else if (HighLogic.LoadedScene == GameScenes.EDITOR) {

				windowRect.x = 1008;
				windowRect.y = 489;

			}

		}

		public void OnDestroy ()
		{

			//Destroy the button in order to create a new one.  It's required with multiple scene handling, unfortunately.
			ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
			appLauncherButton = null;

		}

		void Update() {

			if ((GameSettings.MODIFIER_KEY.GetKey() ) && Input.GetKeyDown (KeyCode.Slash)) {

				windowEnabled = !windowEnabled;
				if (ApplicationLauncher.Ready) {

					if (windowEnabled) {

						appLauncherButton.SetTrue (true);

					} else {

						appLauncherButton.SetFalse (true);

					}

				}

			}

//			Meant for debugging
//			Debug.Log ("X: " + windowRect.x.ToString ());
//			Debug.Log ("Y: " + windowRect.y.ToString ());



		}

		void OnGUI() {

			GUI.skin = HighLogic.Skin;

			if (windowEnabled) {

				windowRect = GUI.Window (09271, windowRect, GUIActive, "AnyRes");

			}

		}

		void GUIActive(int windowID) {

			if (HighLogic.LoadedScene == GameScenes.SETTINGS) {

				GUI.BringWindowToFront (09271);

			}
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Width: ");
			xString = GUILayout.TextField (xString);
			xString = Regex.Replace (xString, @"[^0-9]", "");
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Height: ");
			yString = GUILayout.TextField (yString);
			yString = Regex.Replace (yString, @"[^0-9]", "");
			GUILayout.EndHorizontal ();
			fullScreen = GUILayout.Toggle (fullScreen, "Fullscreen");
			reloadScene = GUILayout.Toggle (reloadScene, "Reload scene");
			if (GUILayout.Button("Set Screen Resolution")) {

				if (xString != null && yString != null) {

					x = Convert.ToInt32(xString);
					y = Convert.ToInt32(yString);

					if (x > 0 && y > 0) {

						GameSettings.SCREEN_RESOLUTION_HEIGHT = y;
						GameSettings.SCREEN_RESOLUTION_WIDTH = x;
						GameSettings.FULLSCREEN = fullScreen;
						GameSettings.SaveSettings ();
						Screen.SetResolution(x, y, fullScreen);
						if (reloadScene) {

							if (HighLogic.LoadedScene != GameScenes.LOADING) {
								HighLogic.LoadScene (HighLogic.LoadedScene);
							} else {

								ScreenMessages.PostScreenMessage("You cannot reload the scene while loading the game!", 1);

							}

						}

					} else {

						ScreenMessages.PostScreenMessage("One or both of your values is too small.  Please enter a valid value.", 1, ScreenMessageStyle.UPPER_CENTER);

					}

				} else {

					ScreenMessages.PostScreenMessage("The values you have set are invalid.  Please set a valid value.", 1, ScreenMessageStyle.UPPER_CENTER);

				}

			}
			GUILayout.EndVertical();

			GUI.DragWindow ();

		}

	}
}

