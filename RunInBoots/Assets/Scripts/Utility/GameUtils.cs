using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils
{
    public static Pipe FindPipeByID(int id)
    {
        Pipe[] pipes = GameObject.FindObjectsOfType<Pipe>();
        foreach (Pipe pipe in pipes)
        {
            if (pipe.pipeID == id)
            {
                return pipe;
            }
        }
        return null;
    }
}
