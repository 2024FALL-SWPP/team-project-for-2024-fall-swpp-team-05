using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.Linq;

public class LevelLoader : MonoBehaviour
{
    public int stage;
    public int index;

    private string fileName;
    private string path;
    public TerrainData terrainData;

    private void Start()
    {

    }

    public void LoadLevel()
    {
        fileName = $"Stage_{stage}_{index}";
        path = Application.dataPath + "/Resources/TerrainData/" + fileName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            terrainData = JsonConvert.DeserializeObject<TerrainData>(json, settings);
            Debug.Log("Level data loaded from " + path);

            CreateObjectsFromData(); 
        }
        else
        {
            Debug.LogError("No terrain data fount at " + path);
        }
    }
    private List<Vector2Int> collectedPositions = new List<Vector2Int>();
    private void CreateObjectsFromData()
    {
        Transform parentTransform = this.transform;
        Vector3 minPosition = FindMinPosition();
        collectedPositions.Clear();

        foreach (var levelObjectData in terrainData.levelObjects)
        {
            string prefabName = levelObjectData.objectType;
            GameObject prefab = Resources.Load<GameObject>("LevelObject/" + prefabName);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab '{prefabName}'�� ã�� �� �����ϴ�.");
                continue;
            }

            Vector3 adjustedPosition = GridToWorldPosition(levelObjectData.gridPosition) - minPosition;
            GameObject instance = PoolManager.Instance.Pool(prefab, adjustedPosition, parentTransform);
            instance.transform.SetParent(transform);
            //Debug.Log($"Placing object '{prefabName}' at {adjustedPosition}");
            if (IsCollectingTarget(instance))
            {
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(levelObjectData.gridPosition.x), Mathf.RoundToInt(levelObjectData.gridPosition.y));
                collectedPositions.Add(gridPos);

                // Remove the box collider as it's handled in the merged object
                DestroyImmediate(instance.GetComponent<BoxCollider>());
                continue;
            }

            if (levelObjectData is PipeData pipeData)
            {
                Pipe pipeComponent = instance.GetComponent<Pipe>();
                if (pipeComponent != null)
                {
                    pipeComponent.pipeID = pipeData.pipeID;
                    pipeComponent.targetPipeID = pipeData.targetPipeID;
                    //pipeComponent.targetStage = pipeData.targetStage;
                    pipeComponent.targetIndex = pipeData.targetIndex;
                }
                else
                {
                    Debug.LogError("No Pipe Script in Pipe");
                }
            }
            else if (levelObjectData is CatnipData catnipData)
            {
                Catnip catnipComponent = instance.GetComponent<Catnip>();
                if (catnipComponent != null)
                {
                    catnipComponent.catnipID = catnipData.catnipID;
                }
            }
        }
        CreateMergedColliders();
    }
    private bool IsCollectingTarget(GameObject instance)
    {
        BoxCollider boxCollider = instance.GetComponent<BoxCollider>();
        if (boxCollider != null &&
            boxCollider.center == Vector3.zero &&
            boxCollider.size == Vector3.one)
        {
            // Check if it only contains MeshFilter, MeshRenderer, Transform
            Component[] components = instance.GetComponents<Component>();
            foreach (var component in components)
            {
                if (!(component is Transform || component is MeshFilter || component is MeshRenderer || component is BoxCollider))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    private void CreateMergedColliders()
    {
        // Sort all positions by y (row order), then by x (column order)
        collectedPositions.Sort((a, b) =>
        {
            int yCompare = a.y.CompareTo(b.y);
            return yCompare == 0 ? a.x.CompareTo(b.x) : yCompare;
        });

        List<(Vector2Int start, Vector2Int end)> mergedRows = new List<(Vector2Int, Vector2Int)>();
        int i = 0;
        while (i < collectedPositions.Count)
        {
            // Get the starting position for the current row
            Vector2Int start = collectedPositions[i];
            int rowY = start.y;
            // Find the end of the merged row
            Vector2Int end = start;
            while (i + 1 < collectedPositions.Count &&
                   collectedPositions[i + 1].y == rowY &&
                   collectedPositions[i + 1].x == collectedPositions[i].x + 1)
            {
                i++;
                end = collectedPositions[i];
            }
            mergedRows.Add((start, end));
            i++;
        }

        // Group rows by their x-range (start.x and end.x)
        var rowsByXRange = new Dictionary<(int startX, int endX), List<int>>();
        foreach (var (start, end) in mergedRows)
        {
            var xRange = (start.x, end.x);

            if (!rowsByXRange.TryGetValue(xRange, out List<int> yPositions))
            {
                yPositions = new List<int>();
                rowsByXRange[xRange] = yPositions;
            }
            yPositions.Add(start.y);
        }

        // For each x-range, sort y positions and merge vertically where possible
        List<(Vector2Int start, Vector2Int end)> mergedRectangles = new List<(Vector2Int, Vector2Int)>();
        foreach (var kvp in rowsByXRange)
        {
            var xRange = kvp.Key;
            var yPositions = kvp.Value;
            yPositions.Sort();
            int k = 0;
            while (k < yPositions.Count)
            {
                int startY = yPositions[k];
                int endY = startY;
                // Merge vertically adjacent rows with the same x-range
                while (k + 1 < yPositions.Count && yPositions[k + 1] == yPositions[k] + 1)
                {
                    k++;
                    endY = yPositions[k];
                }
                // Add the merged rectangle
                mergedRectangles.Add((
                    new Vector2Int(xRange.startX, startY),
                    new Vector2Int(xRange.endX, endY)
                ));
                k++;
            }
        }

        // HashSet for quick lookup of occupied positions
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>(collectedPositions);
        // Identify vertical edges (left and right) for each cube
        List<(Vector2Int start, Vector2Int end, string side)> verticalEdges = new List<(Vector2Int, Vector2Int, string)>();
        foreach (var position in collectedPositions)
        {
            int x = position.x;
            int y = position.y;
            // Check for left edge
            Vector2Int leftNeighbor = new Vector2Int(x - 1, y);
            if (!occupiedPositions.Contains(leftNeighbor))
            {
                verticalEdges.Add((position, position, "Left"));
            }
            // Check for right edge
            Vector2Int rightNeighbor = new Vector2Int(x + 1, y);
            if (!occupiedPositions.Contains(rightNeighbor))
            {
                verticalEdges.Add((position, position, "Right"));
            }
        }

        // Merge vertical edges along the y-axis where possible
        var edgesByXAndSide = new Dictionary<(int x, string side), List<int>>();
        foreach (var (start, end, side) in verticalEdges)
        {
            var key = (start.x, side);
            if (!edgesByXAndSide.TryGetValue(key, out List<int> yPositions))
            {
                yPositions = new List<int>();
                edgesByXAndSide[key] = yPositions;
            }
            yPositions.Add(start.y);
        }

        List<(Vector2Int start, Vector2Int end, string side)> mergedVerticalLines = new List<(Vector2Int, Vector2Int, string)>();
        // For each group, merge consecutive y positions
        foreach (var kvp in edgesByXAndSide)
        {
            var (x, side) = kvp.Key;
            var yPositions = kvp.Value;
            yPositions.Sort();
            int iEdge = 0;
            while (iEdge < yPositions.Count)
            {
                int startY = yPositions[iEdge];
                int endY = startY;
                // Merge vertically adjacent positions
                while (iEdge + 1 < yPositions.Count && yPositions[iEdge + 1] == yPositions[iEdge] + 1)
                {
                    iEdge++;
                    endY = yPositions[iEdge];
                }
                mergedVerticalLines.Add((
                    new Vector2Int(x, startY),
                    new Vector2Int(x, endY),
                    side
                ));
                iEdge++;
            }
        }

        // Create a single parent object to hold all colliders
        GameObject colliderParent = new GameObject("MergedCollidersParent");
        colliderParent.transform.SetParent(this.transform);

        // Add a BoxCollider for each merged rectangle
        float edgeWidth = 0.2f; // Very thin ramping collider width for edge
        foreach (var (start, end) in mergedRectangles)
        {
            GameObject colliderObject = new GameObject($"Collider_{start.x}_{start.y}");
            colliderObject.transform.SetParent(colliderParent.transform);
            colliderObject.layer = LayerMask.NameToLayer("Ground");
            BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();
            // Calculate the center and size of the collider
            float width = end.x - start.x + 1 -edgeWidth;
            float height = end.y - start.y + 1;
            Vector3 center = new Vector3(start.x + width / 2f - 0.5f+edgeWidth/2f, start.y + height / 2f - 0.5f, 0);
            Vector3 size = new Vector3(width, height, 1);
            boxCollider.center = center - colliderObject.transform.position;
            boxCollider.size = size;
        }

        // Add vertical colliders for merged vertical lines
        foreach (var (start, end, side) in mergedVerticalLines)
        {
            GameObject edgeColliderObject = new GameObject($"{side}Edge_{start.x}_{start.y}");
            edgeColliderObject.transform.SetParent(colliderParent.transform);
            edgeColliderObject.layer = LayerMask.NameToLayer("Ground");
            BoxCollider boxCollider = edgeColliderObject.AddComponent<BoxCollider>();
            // Calculate the center and size of the collider
            float height = end.y - start.y + 0.9f;
            float depth = 1f;
            float centerX = start.x + (side == "Left" ? -0.5f + edgeWidth / 2f : 0.5f - edgeWidth / 2f);
            float centerY = start.y + height / 2f - 0.5f;
            Vector3 center = new Vector3(centerX, centerY, 0);
            Vector3 size = new Vector3(edgeWidth, height, depth);
            boxCollider.center = center - edgeColliderObject.transform.position;
            boxCollider.size = size;
        }
    }


    private Vector3 FindMinPosition()
    {
        Vector3 minPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        foreach (var levelObjectData in terrainData.levelObjects)
        {
            Vector3 position = GridToWorldPosition(levelObjectData.gridPosition);
            minPosition = Vector3.Min(minPosition, position);
        }
        return minPosition;
    }

    private Vector3 GridToWorldPosition(SerializableVector2Int gridPosition)
    {
        // �׸��� ��ǥ�� ���� ��ǥ�� ��ȯ�ϴ� ����� �����մϴ�.
        // ���÷� 1:1 ������ ����� �� �ֽ��ϴ�.
        return new Vector3(gridPosition.x, gridPosition.y, 0);
    }
}
