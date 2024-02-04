using SKCell;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonReference : SKMonoSingleton<CommonReference>
{
    public static string TAG_BLOCK = "Block";

    public static int LAYER_GROUND = 9;

    public static Camera mainCam;
    private void Start()
    {
        mainCam = Camera.main;
        Debug.Log(mainCam.name);
    }
}
