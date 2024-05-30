using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Diagnostics;

[InitializeOnLoad]
public class NewBehaviourScript
{
    // ボタンの大きさ
    const float ButtonWidth = 120f;

    static NewBehaviourScript()
    {
        SceneView.onSceneGUIDelegate += (sceneView) =>
        {
            Handles.BeginGUI();
            if (GUILayout.Button("ボタンです", GUILayout.Width(ButtonWidth)))
            {
                //<--- ボタンが押されるとここが実行されます。--->//
                UnityEngine.Debug.Log("ボタンが押されました。");
            }
            Handles.EndGUI();
        };
    }
}
