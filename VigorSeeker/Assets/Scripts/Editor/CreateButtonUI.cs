using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CreateButtonUi
{
    static CreateButtonUi()
    {
        SceneView.duringSceneGui += OnGui;
    }

    private static void OnGui(SceneView sceneView)
    {
        Handles.BeginGUI();

        // ������ UI��`�悷�鏈�����L�q
        ShowButtons(sceneView.position.size);

        Handles.EndGUI();
    }

    /// <summary>
    /// �{�^���̕`��֐�
    /// </summary>
    private static void ShowButtons(Vector2 sceneSize)
    {
        var count = 10;
        var buttonSize = 40;

        foreach (var i in Enumerable.Range(0, count))
        {
            // ��ʉ����A�����A�����񂹂��R���g���[������ Rect
            var rect = new Rect(
              sceneSize.x / 2 - buttonSize * count / 2 + buttonSize * i,
              sceneSize.y - buttonSize * 1.6f,
              buttonSize,
              buttonSize);

            if (GUI.Button(rect, i.ToString()))
            {
                Debug.Log("�����ꂽ");
            }
        }
    }
}