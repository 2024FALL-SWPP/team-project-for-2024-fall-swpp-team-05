[Serializable]
public class LevelObjectData
{
    public string objectType;
    public SerializableVector2Int gridPosition;
}

[Serializable]
public class PipeData : LevelObjectData
{
    public int pipeID;
    public int targetPipeID;
    public int targetStage;
    public int targetIndex;

    public PipeData()
    {
        objectType = "Pipe";
    }
}

[Serializable]
public class StartPointData : LevelObjectData
{
    public StartPointData()
    {
        objectType = "StartPoint";
    }
}

[Serializable]
public class GoalPointData : LevelObjectData
{
    public int targetStage;
    public int targetIndex;

    public GoalPointData()
    {
        objectType = "GoalPoint";
    }
}

[Serializable]
public class CatnipData : LevelObjectData
{
    public int catnipID;

    public CatnipData()
    {
        objectType = "Catnip";
    }
}
