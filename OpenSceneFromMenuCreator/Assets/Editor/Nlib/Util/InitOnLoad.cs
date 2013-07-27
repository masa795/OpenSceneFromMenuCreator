using UnityEngine;
using UnityEditor;


namespace Nlib.Editor.Util
{
	[InitializeOnLoad]
	public class InitOnLoad
	{

		static int couner = 0;

		static InitOnLoad()
		{
			++couner;
			EditorApplication.playmodeStateChanged += ChangedPlaymodeState;
			EditorApplication.update += Update;


			//Log(couner + " InitOnLoad!");
		}


		public static bool IsClickedPlayButton()
		{
			return EditorPrefs.GetBool("editor_InitOnLoad", false);
		}



		static void ChangedPlaymodeState()
		{
			++couner;
			EditorPrefs.SetBool("editor_InitOnLoad", EditorApplication.isPlayingOrWillChangePlaymode);

			//Log(couner + " State Changed!");
		}


		static void Update()
		{
			++couner;
			//Log(couner + " Update!");
			EditorApplication.update -= Update;
		}


		static void Log(string called)
		{
			Debug.Log(called
					+ " \n Application.isPlaying=" + Application.isPlaying
					+ " \n EditorApplication.isPlaying=" + EditorApplication.isPlaying
					+ " \n Application.isCompiling=" + EditorApplication.isCompiling
					+ " \n Application.isPaused=" + EditorApplication.isPaused
					+ " \n EditorApplication.isPlayingOrWillChangePlaymode=" + EditorApplication.isPlayingOrWillChangePlaymode
					+ " \n editor_InitOnLoad=" + EditorPrefs.GetBool("editor_InitOnLoad", false)
					+ " \n Time.timeSinceLevelLoad=" + Time.timeSinceLevelLoad
					+ " \n EditorApplication.timeSinceStartup=" + EditorApplication.timeSinceStartup
					);

		}

	}
}
