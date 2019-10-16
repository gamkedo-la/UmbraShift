using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class WebGLBuildConfig
{
    static WebGLBuildConfig()
    {
        PlayerSettings.WebGL.memorySize = 512;
        PlayerSettings.WebGL.threadsSupport = false;
    }
}