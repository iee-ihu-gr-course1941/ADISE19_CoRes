using UnityEditor;

class WebGLBuilder
{
    static void build ()
    {
        string[] scenes = { "Assets\\Scenes\\Login.unity",
        					"Assets\\Scenes\\Game.unity" };
        
        string pathToDeploy = "Build";  
        
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
    }
}