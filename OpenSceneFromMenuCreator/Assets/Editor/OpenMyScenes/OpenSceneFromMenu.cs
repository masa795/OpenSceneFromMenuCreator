using UnityEngine;
using UnityEditor;


namespace MenuCreator {
/// <summary>
/// Do not directly edit
/// This script is automatic create.
/// </summary>
public class OpenSceneFromMenu  : Editor 
{

	[MenuItem("nLib/MyScenes/Assets|test01.unity ", false, 50)]
	public static void OpenAssetstest01unity(){
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == true){
			EditorApplication.OpenScene("Assets/test01.unity");
		}
	}


	[MenuItem("nLib/MyScenes/Assets|test04.unity ", false, 50)]
	public static void OpenAssetstest04unity(){
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == true){
			EditorApplication.OpenScene("Assets/test04.unity");
		}
	}


	[MenuItem("nLib/MyScenes/Assets|test02.unity ", false, 50)]
	public static void OpenAssetstest02unity(){
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == true){
			EditorApplication.OpenScene("Assets/test02.unity");
		}
	}


	[MenuItem("nLib/MyScenes/Assets|test03.unity ", false, 50)]
	public static void OpenAssetstest03unity(){
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == true){
			EditorApplication.OpenScene("Assets/test03.unity");
		}
	}

}
}
