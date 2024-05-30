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

        // ここに UIを描画する処理を記述
        ShowButtons(sceneView.position.size);

        Handles.EndGUI();
    }

    /// <summary>
    /// ボタンの描画関数
    /// </summary>
    private static void ShowButtons(Vector2 sceneSize)
    {
        var count = 10;
        var buttonSize = 40;

        foreach (var i in Enumerable.Range(0, count))
        {
            // 画面下部、水平、中央寄せをコントロールする Rect
            var rect = new Rect(
              sceneSize.x / 2 - buttonSize * count / 2 + buttonSize * i,
              sceneSize.y - buttonSize * 1.6f,
              buttonSize,
              buttonSize);

            if (GUI.Button(rect, i.ToString()))
            {
                Debug.Log("おされた");
            }
        }
    }
}