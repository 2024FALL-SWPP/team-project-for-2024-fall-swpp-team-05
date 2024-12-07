using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class InverseMask : Image
{
    // 출처: https://www.youtube.com/watch?si=bbiK9ozN4T6_4-z8&v=XJJl19N2KFM&feature=youtu.be
    public override Material materialForRendering
    {
       get
       {
           Material material = new Material(base.materialForRendering);
          material.SetInt("_StencilComp", (int)6);
           return material;
       }
   }
}
