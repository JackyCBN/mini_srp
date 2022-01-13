using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
public partial class CameraRenderer
{
    static Material errorMaterial;
    static ShaderTagId[] legacyShaderTagIds =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    partial void DrawGizmos();
    partial void DrawUnsupportedShaders();

    partial void PrepareForSceneWindow();
    
#if UNITY_EDITOR
    partial void DrawUnsupportedShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        DrawingSettings drawingSettings = new DrawingSettings(
            legacyShaderTagIds[0], new SortingSettings(camera)
            )
        {
            overrideMaterial = errorMaterial
        };

        for (int i = 1; i != legacyShaderTagIds.Length; ++i)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        FilteringSettings filterSettings = FilteringSettings.defaultValue;
        context.DrawRenderers(cullingResult, ref drawingSettings, ref filterSettings);
    }

    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }
    partial void PrepareForSceneWindow()
    {
        if(camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }


#endif
}
