using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;


namespace Nlib.Editor.Util
{
	/// <summary>
	/// スクリプト自動生成util
	/// </summary>
	public class ScriptCreatorUtil
	{

		/// <summary>
		/// ファイルを作成する
		/// </summary>
		/// <param name="assetPath"></param>
		/// <param name="output"></param>
		/// <param name="prefsHashKey"></param>
		/// <param name="isRefresh"></param>
		/// <returns></returns>
		public static bool CreateFile(string assetPath, string output, string prefsHashKey, bool isRefresh)
		{
			if (!isRefresh)
			{
				if (File.Exists(assetPath) && EditorPrefs.GetInt(prefsHashKey) == output.GetHashCode())
					return false;

				if (EditorPrefs.GetInt(prefsHashKey) == output.GetHashCode())
					return false;
			}

			if (string.IsNullOrEmpty(output))
			{
				return false;
			}
			else
			{
				if (System.IO.File.ReadAllText(assetPath) == output)
				{
					EditorPrefs.SetInt(prefsHashKey, output.GetHashCode());
					return false;
				}
			}


			System.IO.File.WriteAllText(assetPath, output);
			EditorPrefs.SetInt(prefsHashKey, output.GetHashCode());


			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			//AssetPostprocessUTF8Encode.ConvertUtf8WithBom(assetPath);

			return true;
		}


		public static void WriteNormalMember<T>(System.Text.StringBuilder builder, T sourceClass, string[] ignoreMembers)
		{
			System.Reflection.FieldInfo[] fields = sourceClass.GetType().GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				if (IsIgnoreMember(ignoreMembers, field.Name)) continue;

				WriteMember(builder, sourceClass, field);

			}
		}




		public static void WriteMember<T>(System.Text.StringBuilder builder, T sourceClass, System.Reflection.FieldInfo field)
		{
			builder.AppendLine("");
			if (field.GetValue(sourceClass) is string)
			{
				WriteStringMember(builder, field.Name, (string)field.GetValue(sourceClass));
			}
			else if (field.GetValue(sourceClass) is Int32)
			{
				WriteIntMember(builder, field.Name, (int)field.GetValue(sourceClass));
			}
			else if (field.GetValue(sourceClass) is bool)
			{

				WriteBoolMember(builder, field.Name, (bool)field.GetValue(sourceClass));
			}
		}



		static bool IsIgnoreMember(string[] ignoreMembers, string memberName)
		{
			if (ignoreMembers.Length == 0) return false;
			foreach (string name in ignoreMembers)
				if (name == memberName) return true;
			return false;
		}



		#region write string
		public static void WriteStringMember(System.Text.StringBuilder builder, string memberName, string value)
		{
			builder.AppendLine("");
			WriteMemberDocSummaryString(builder, value);
			builder.Append("\t").AppendFormat(@"public const string {0} = ""{1}"";", memberName, value).AppendLine();
		}
		static void WriteMemberDocSummaryString(System.Text.StringBuilder builder, string value)
		{
			builder.Append("\t").AppendLine("/// <summary>");
			builder.Append("\t").AppendFormat("/// return \"{0}\"", value).AppendLine();
			builder.Append("\t").AppendLine("/// </summary>");
		}
		#endregion write string




		#region write int
		public static void WriteIntMember(System.Text.StringBuilder builder, string memberName, int value)
		{
			builder.AppendLine("");
			WriteMemberDocSummaryInt(builder, value.ToString());
			builder.Append("\t").AppendFormat(@"public const int {0} = {1};", memberName, value).AppendLine();
		}
		static void WriteMemberDocSummaryInt(System.Text.StringBuilder builder, string value)
		{
			builder.Append("\t").AppendLine("/// <summary>");
			builder.Append("\t").AppendFormat("/// return {0}", value).AppendLine();
			builder.Append("\t").AppendLine("/// </summary>");
		}
		#endregion write int


		#region write bool
		public static void WriteBoolMember(System.Text.StringBuilder builder, string memberName, bool value)
		{
			builder.AppendLine("");
			string strVal = (value) ? "true" : "false";
			WriteMemberDocSummaryInt(builder, strVal);
			builder.Append("\t").AppendFormat(@"public const bool {0} = {1};", memberName, strVal).AppendLine();
		}
		static void WriteMemberDocSummaryBool(System.Text.StringBuilder builder, string value)
		{
			builder.Append("\t").AppendLine("/// <summary>");
			builder.Append("\t").AppendFormat("/// return {0}", value).AppendLine();
			builder.Append("\t").AppendLine("/// </summary>");
		}
		#endregion write bool



	}
}