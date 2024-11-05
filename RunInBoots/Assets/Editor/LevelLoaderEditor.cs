using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelLoader))]
public class LevelDataLoaderEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 UI를 표시
        DrawDefaultInspector();

        // 대상 컴포넌트 가져오기
        LevelLoader loader = (LevelLoader)target;

        // "불러오기" 버튼 추가
        if (GUILayout.Button("Load Level"))
        {
            loader.LoadLevel();  // LoadLevelData 메서드 호출
        }
    }
}
