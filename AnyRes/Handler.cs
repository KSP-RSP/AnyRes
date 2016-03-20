using System;
using UnityEngine;
using System.Text.RegularExpressions;  //Get Regex

namespace AnyRes
{

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class AnyRes : MonoBehaviour
	{

		public static Rect windowRect = new Rect(35, 99, 200, 160);

		public string xString = "1280";
		public string yString = "720";

		public int x = 1280;
		public int y = 720;

		public bool enabled = false;
		public bool fullScreen = true;

		private static ApplicationLauncherButton appLauncherButton;

		void Start() {

			//Thanks bananashavings http://forum.kerbalspaceprogram.com/index.php?/profile/156147-bananashavings/ - https://gist.github.com/bananashavings/e698f4359e1628b5d6ef
			//Also thanks to Crzyrndm for the fix to that code!
			if (ApplicationLauncher.Ready && appLauncherButton == null) {

				appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
					() => { toggleGUI(true); },
					() => { toggleGUI(false); },
					() => {},
					() => {},
					() => {},
					() => {},
					ApplicationLauncher.AppScenes.SPACECENTER,
					(Texture)GameDatabase.Instance.GetTexture("AnyRes/textures/toolbar", false));
				
			}

			xString = GameSettings.SCREEN_RESOLUTION_WIDTH.ToString ();
			yString = GameSettings.SCREEN_RESOLUTION_HEIGHT.ToString ();
			fullScreen = GameSettings.FULLSCREEN;

			if (HighLogic.LoadedScene == GameScenes.SETTINGS) {

				enabled = true;

			}

		}

		void toggleGUI(bool state) {

			enabled = state;

		}

		void Update() {

			if ((GameSettings.MODIFIER_KEY.GetKey() ) && Input.GetKeyDown (KeyCode.A)) {

				enabled = !enabled;

			}

			if (ApplicationLauncher.Ready) {

				if (enabled) {

					appLauncherButton.SetTrue (true);

				} else {

					appLauncherButton.SetFalse (true);

				}

			}

//			Meant for debugging
//			Debug.Log ("X: " + windowRect.x.ToString ());
//			Debug.Log ("Y: " + windowRect.y.ToString ());

		}

		void OnGUI() {

			GUI.skin = HighLogic.Skin;

			if (enabled) {

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

