using System;
using UnityEngine;
using System.Text.RegularExpressions;  //Get Regex
using KSP.UI.Screens;
using AnyRes.Util;

namespace AnyRes
{

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class AnyRes : MonoBehaviour
	{

		public static Rect anyresWinRect = new Rect(35, 99, 200, 190);

		public string xString = "1280";
		public string yString = "720";

		public int x = 1280;
		public int y = 720;

		public bool windowEnabled = false;
		public bool fullScreen = true;
		public bool reloadScene = false;

		private static ApplicationLauncherButton appLauncherButton = null;

		Presets presets;

        

		void Start() {

            if (HighLogic.LoadedScene == GameScenes.SETTINGS)
            {

                windowEnabled = true;
                anyresWinRect.x = 7;
                anyresWinRect.y = 231;

            }
            else if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {

                anyresWinRect.x = Screen.width - 272;
                anyresWinRect.y = Screen.height - 231;

            }

            Debug.Log ("[AnyRes] Loaded");

			presets = gameObject.AddComponent<Presets> () as Presets;

			
            //Debug.Log("[AnyRes] 1");
            
            xString = GameSettings.SCREEN_RESOLUTION_WIDTH.ToString ();
			yString = GameSettings.SCREEN_RESOLUTION_HEIGHT.ToString ();
			fullScreen = GameSettings.FULLSCREEN;
            //Debug.Log("[AnyRes] 2");

            
            //DontDestroyOnLoad(this);

        }

		public void OnDisable ()
		{

            //Destroy the button in order to create a new one.  It's required with multiple scene handling, unfortunately.
            if (appLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
                appLauncherButton = null;
                Debug.Log("[AnyRes] Remove application button");
            }

		}

		void Update() {

            //Thanks bananashavings http://forum.kerbalspaceprogram.com/index.php?/profile/156147-bananashavings/ - https://gist.github.com/bananashavings/e698f4359e1628b5d6ef
            //Also thanks to Crzyrndm for the fix to that code!
            //(HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.EDITOR)
            if (appLauncherButton == null && (HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.EDITOR))
            {

                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    () => { windowEnabled = true; },
                    () => { presets.newEnabled = false; presets.loadEnabled = false; presets.windowEnabled = false; windowEnabled = false; },
                    () => { },
                    () => { },
                    () => { },
                    () => { },
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.VAB,
                    (Texture)GameDatabase.Instance.GetTexture("AnyRes/textures/toolbar", false));

            }

            if ((GameSettings.MODIFIER_KEY.GetKey() ) && Input.GetKeyDown (KeyCode.Slash)) {

				windowEnabled = !windowEnabled;
				if (ApplicationLauncher.Ready) {

					if (appLauncherButton != null){
						if (windowEnabled) {

							appLauncherButton.SetTrue (true);

						} else {

							appLauncherButton.SetFalse (true);

						}
					}

				}

			}
            
            presets.windowRect.x = anyresWinRect.xMin + anyresWinRect.width;
            presets.windowRect.y = anyresWinRect.yMin;
            presets.loadRect.x = presets.windowRect.xMin + presets.windowRect.width;
            presets.loadRect.y = presets.windowRect.yMin;
            presets.newRect.x = presets.windowRect.xMin + presets.windowRect.width;
            presets.newRect.y = anyresWinRect.yMin;

            //			Meant for debugging
            //			Debug.Log ("X: " + anyresWinRect.x.ToString ());
            //			Debug.Log ("Y: " + anyresWinRect.y.ToString ());



        }

		void OnGUI() {

			GUI.skin = HighLogic.Skin;

			if (windowEnabled) {

				anyresWinRect = GUI.Window (09271, anyresWinRect, GUIActive, "AnyRes");

			}

		}

		void GUIActive(int windowID) {

			if (GUI.Button (new Rect (0, 0, 50, 25), "Presets")) {

				presets.windowEnabled = !presets.windowEnabled;
                if (!presets.windowEnabled)
                {
                    presets.loadEnabled = false;
                }
                else
                {
                    //presets.windowRect = new Rect(anyresWinRect.xMin + anyresWinRect.width, anyresWinRect.yMin, 200, 100);
                    presets.windowRect.x = anyresWinRect.xMin + anyresWinRect.width;
                    presets.windowRect.y = anyresWinRect.yMin;
                    //presets.loadRect = new Rect(presets.windowRect.xMin + presets.windowRect.width, anyresWinRect.yMin, 200, 400);
                    presets.loadRect.x = presets.windowRect.xMin + presets.windowRect.width;
                    presets.loadRect.y = presets.windowRect.yMin;
                    //presets.newRect = new Rect(presets.windowRect.xMin + presets.windowRect.width, anyresWinRect.yMin, 200, 230);
                    presets.newRect.x = presets.windowRect.xMin + presets.windowRect.width;
                    presets.newRect.y = anyresWinRect.yMin;

                }
            }

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
						Debug.Log ("[AnyRes] Set screen resolution");
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

