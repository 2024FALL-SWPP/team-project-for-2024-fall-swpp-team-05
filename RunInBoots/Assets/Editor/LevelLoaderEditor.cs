using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelLoader))]
public class LevelDataLoaderEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        // �⺻ �ν����� UI�� ǥ��
        DrawDefaultInspector();

        // ��� ������Ʈ ��������
        LevelLoader loader = (LevelLoader)target;

        // "�ҷ�����" ��ư �߰�
        if (GUILayout.Button("Load Level"))
        {
            loader.LoadLevel();  // LoadLevelData �޼��� ȣ��
        }
    }
}
