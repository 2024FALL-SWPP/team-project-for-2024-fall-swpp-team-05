using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PipeUtils
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
