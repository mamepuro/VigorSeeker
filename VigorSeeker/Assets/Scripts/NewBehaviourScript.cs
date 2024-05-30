using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Diagnostics;

[InitializeOnLoad]
public class NewBehaviourScript
{
    // �{�^���̑傫��
    const float ButtonWidth = 120f;

    static NewBehaviourScript()
    {
        SceneView.onSceneGUIDelegate += (sceneView) =>
        {
            Handles.BeginGUI();
            if (GUILayout.Button("�{�^���ł�", GUILayout.Width(ButtonWidth)))
            {
                //<--- �{�^�����������Ƃ��������s����܂��B--->//
                UnityEngine.Debug.Log("�{�^����������܂����B");
            }
            Handles.EndGUI();
        };
    }
}
