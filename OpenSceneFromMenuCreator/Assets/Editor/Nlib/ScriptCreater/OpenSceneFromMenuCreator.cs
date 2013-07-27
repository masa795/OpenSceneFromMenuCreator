
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using Nlib.Editor.Util;


namespace MenuCreator
{
	/// <summary>
	/// メニューにシーンを開くクラスを自動生成する
	/// </summary>
	[InitializeOnLoad]
	public class OpenSceneFromMenuCreator : Editor
	{

		#region 設定
		/// <summary>シーンを開くスクリプトファイル保存場所 </summary>
		public const string SAVE_DIR = "Editor/OpenMyScenes";
		/// <summary>シーンを開くスクリプトファイル名 </summary>
		public const string SAVE_FILE_NAME = "OpenSceneFromMenu.cs";

		/// <summary>追加するメニュー場所 </summary>
		public const string ADD_MENU_PATH = "nLib/MyScenes/";
		/// <summary>hash </summary>
		private const string SCENENAME_HASH_KEY = "OpenSceneFromMenuCreator_Hash";

		/// <summary>
		/// 除外するパス
		/// 例）
		///		"Assets/NGUI/Examples/Scenes/"
		/// </summary>
		static string[] m_ignorePath = { "Assets/NGUI/Examples/Scenes/" };


		/// <summary>
		/// 対象にしたいパスを指定する
		/// 指定したフォルダ内のみを対象にする
		/// 例）
		///		"Assets/Scenes/"
		/// </summary>
		static string[] m_targetPath = {  };

		#endregion 設定


		/// <summary>
		/// 自動起動
		/// </summary>
		static OpenSceneFromMenuCreator()
		{
			if (EditorApplication.isPlaying || Application.isPlaying)
				return;
			if (InitOnLoad.IsClickedPlayButton() )
				return;

			BuildOpenSceneFromMenu(false);
		}




		[MenuItem(ADD_MENU_PATH + "メニューからシーンを開く【再作成】", false, 20)]
		public static void _OpenSceneFromMenuCreator()
		{
			if (EditorApplication.isPlaying || Application.isPlaying)
				return;
			if (InitOnLoad.IsClickedPlayButton())
				return;
			BuildOpenSceneFromMenu(true);
		}




		#region 書込

		static void BuildOpenSceneFromMenu(bool isRefresh)
		{

			if (!File.Exists(GetSaveFileFullPath()))
			{
				Directory.CreateDirectory(Application.dataPath + "/" + SAVE_DIR);
				System.IO.File.WriteAllText(GetSaveFileFullPath(), "");
			}


			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			builder = WriteSceneManagerClass(builder);
			if (builder == null)
			{
				Debug.Log("No Scene... ");
				return;
			}

			string text = builder.ToString().Replace(",}", "}");
			string assetPath = GetSaveFileFullPath();

			if (ScriptCreatorUtil.CreateFile(assetPath, text, SCENENAME_HASH_KEY, isRefresh))
			{
				Debug.Log("OpenSceneFromMenuCreator　Created " + SAVE_FILE_NAME);
			}


		}


		/// <summary>
		/// メニューからシーンを開く機能のスクリプトを生成する
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		static System.Text.StringBuilder WriteSceneManagerClass(System.Text.StringBuilder builder)
		{
			List<string> sceneList = GetCreateSceneList();
			if (sceneList.Count == 0) return null;
			
			builder.AppendLine("using UnityEngine;");
			builder.AppendLine("using UnityEditor;");
			builder.AppendLine("");
			builder.AppendLine("");
			builder.AppendLine("namespace MenuCreator {");

			builder.AppendLine("/// <summary>");
			builder.AppendLine("/// Do not directly edit");
			builder.AppendLine("/// This script is automatic create.");
			builder.AppendLine("/// </summary>");
			builder.AppendLine("public class OpenSceneFromMenu  : Editor ");
			builder.AppendLine("{");

			
			WriteSceneMenue(builder, sceneList);


			builder.AppendLine("}");
			builder.AppendLine("}");
			return builder;
		}



		/// <summary>
		/// メニュー　アイテムを作成する
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="sceneList"></param>
		static void WriteSceneMenue(System.Text.StringBuilder builder, List<string> sceneList)
		{
			sceneList.ToList().ForEach(sceneName =>
			{
				string replaceName = ReplaceForMenuName(sceneName);
				string replaceFunctionName = ReplaceForFunctionName(sceneName);

				builder.AppendLine("");
				builder.Append("\t").AppendLine("[MenuItem(\"" + ADD_MENU_PATH + replaceName + " \", false, 50)]");
				builder.Append("\t").AppendLine("public static void Open" + replaceFunctionName + "(){");

				builder.Append("\t").Append("\t").AppendLine("if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == true){");
				builder.Append("\t").Append("\t").Append("\t").AppendLine("EditorApplication.OpenScene(\"" + sceneName + "\");");
				builder.Append("\t").Append("\t").AppendLine("}");

				builder.Append("\t").AppendLine("}");
				builder.AppendLine("");
			});
		}

		#endregion 書込




		#region 対象シーン
		/// <summary>
		/// 生成するためのシーンリストを取得する
		/// </summary>
		/// <returns></returns>
		private static List<string> GetCreateSceneList()
		{
			List<string> sceneList = new List<string>();

			string[] allAssets = AssetDatabase.GetAllAssetPaths();
			foreach (string assetPath in allAssets)
			{
				if (Path.GetExtension(assetPath).ToLower() == ".unity")
				{
					if (!IsTargetItem(assetPath))
						continue;
					else if (IsIgnoreItem(assetPath))
						continue;

					//Debug.Log("assetPath=" + assetPath + "    GetExtension=" + Path.GetExtension(assetPath));
					sceneList.Add(assetPath);
				}
			}
			return sceneList;
		}


		/// <summary>
		/// 除外するシーンか
		/// </summary>
		/// <param name="assetPath"></param>
		/// <returns></returns>
		private static bool IsIgnoreItem(string assetPath)
		{
			if (m_ignorePath.Length == 0) return false;
			foreach (string path in m_ignorePath)
			{
				if (assetPath.Contains(path))
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// 指定されたパスか
		/// </summary>
		/// <param name="assetPath"></param>
		/// <returns></returns>
		private static bool IsTargetItem(string assetPath)
		{
			if (m_targetPath.Length == 0) return true;

			foreach (string path in m_targetPath)
			{
				if (assetPath.Contains(path))
				{
					return true;
				}

			}
			return false;
		}
		#endregion 対象シーン


		#region util


		/// <summary>
		/// 関数名に使えない文字を置換する
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		static string ReplaceForFunctionName(string name)
		{
			string[] invalidChar = new string[] { " ", "!", "\"", "#", "$", "%", "&", "\'", "(", ")", "-", "=", "^", "~", "¥", "|", "[", "{", "@", "`", "]", "}", ":", "*", ";", "+", "/", "?", ".", ">", ",", "<" };
			invalidChar.ToList().ForEach(s => name = name.Replace(s, string.Empty));
			return name;
		}


		/// <summary>
		/// メニューに表示できない文字を置換
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		static string ReplaceForMenuName(string name)
		{
			string[] invalidChar = new string[] { "/" };
			invalidChar.ToList().ForEach(s => name = name.Replace(s, "|"));
			invalidChar = new string[] { "&" };
			invalidChar.ToList().ForEach(s => name = name.Replace(s, ""));
			return name;
		}





		static string GetSaveFilePath()
		{
			return "Assets/" + SAVE_DIR + "/" + SAVE_FILE_NAME;
		}

		static string GetSaveFileFullPath()
		{
			return Application.dataPath + "/" + SAVE_DIR + "/" + SAVE_FILE_NAME;
		}



		static List<EditorBuildSettingsScene> GetEnableScenes()
		{
			return EditorBuildSettings.scenes.Where(scene => scene.enabled).ToList();
		}

		static string currentFolderPath
		{
			get
			{
				string currentFilePath = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
				string currentFolderPath = currentFilePath.Substring(0, currentFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(Path.DirectorySeparatorChar, '/');
				return "Assets" + currentFolderPath.Replace(Application.dataPath, string.Empty);
			}
		}
		#endregion util

	}
}