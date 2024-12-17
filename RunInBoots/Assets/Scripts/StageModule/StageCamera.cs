using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Newtonsoft.Json;

public class StageCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    Dictionary<(int stage, int index), (int gridSizeX, int gridSizeY)> stageGridSizes =
    new Dictionary<(int, int), (int, int)>
    {
    };

    public void UpdateCameraTargetToPlayer(GameObject _player)
    {
        if (_player != null && _virtualCamera != null)
        {
            _virtualCamera.Follow = _player.transform;
            _virtualCamera.OnTargetObjectWarped(_player.transform, _player.transform.position - _virtualCamera.transform.position);
        }
        else
        {
            Debug.LogError("Player �Ǵ� Camera�� ã�� �� ����");
        }
    }

    public void StopFollowingPlayer()
    {
        if (_virtualCamera != null)
        {
            _virtualCamera.Follow = null;
        }
    }
    
    public void ZoomInPlayer()
    {
        Debug.Log("ZoomInPlayer");
        if (_virtualCamera != null)
        {
            _virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingVolume = null;
            _virtualCamera.m_Lens.FieldOfView = 15.0f;
        }
    }

    public void ZoomOutPlayer()
    {
        Debug.Log("ZoomOutPlayer");
        if (_virtualCamera != null)
        {
            _virtualCamera.m_Lens.FieldOfView = 30.0f;
        }
    }

    // load all grid sizes on stage construction
    public void LoadGridSize(int stage)
    {
        for (int index = 1; ; index++)
        {
            string fileName = $"Stage_{stage}_{index}";
            string path = $"TerrainData/{fileName}";
            var file = Resources.Load<TextAsset>(path);

            if (file == null)
                break;

            string json = file.text;
            TerrainData terrainData = JsonConvert.DeserializeObject<TerrainData>(json);

            stageGridSizes[(stage, index)] = (terrainData.gridSize.x, terrainData.gridSize.y);
        }
    }
    public void UpdateStageMapSize(int stage,int index)
    {
        int gridSizeX = 0;
        int gridSizeY = 0;
        if (stageGridSizes.TryGetValue((stage, index), out var gridSize))
        {
            gridSizeX = gridSize.gridSizeX;
            gridSizeY = gridSize.gridSizeY;
        }
        
        _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        var confiner = _virtualCamera.GetComponent<CinemachineConfiner>();
        if (confiner != null)
        {
            var colliderObject = new GameObject("Confiner");
            colliderObject.transform.position = Vector3.zero;
            colliderObject.layer = LayerMask.NameToLayer("Invincible");

            // calculate size of confiner based on width, height, fov, distance, and screen ratio
            float halfViewHeight = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance * Mathf.Tan(Mathf.Deg2Rad * _virtualCamera.m_Lens.FieldOfView*0.5f);
            float halfViewWidth = halfViewHeight * Screen.width/Screen.height;
            Debug.Log($"halfViewHeight: {halfViewHeight}, halfViewWidth: {halfViewWidth} {_virtualCamera.m_Lens.Aspect}");

            // Add a PolygonCollider2D to define the bounding shape
            var boxCollider = colliderObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3((gridSizeX - 2*halfViewWidth), gridSizeY - 2*halfViewHeight, 100);
            boxCollider.center = new Vector3(gridSizeX / 2 - 0.5f, gridSizeY / 2 - 0.5f, 0);

            confiner.m_BoundingVolume = boxCollider;

            Debug.Log("Confiner setup complete.");
        }
    }
}
