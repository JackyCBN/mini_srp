using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public partial class CameraRenderer
{

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    ScriptableRenderContext context;
    Camera camera;

    const string bufferName = "My Render Camera";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    partial void PrepareBuffer();

#if UNITY_EDITOR
    string SampleName { get; set; }

    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name;
        Profiler.EndSample();
    }
#else
    const string SampleName = bufferName;
#endif

    CullingResults cullingResult;
    public void Render(ScriptableRenderContext _context, Camera _camera)
    {
        context = _context;
        camera = _camera;

        PrepareBuffer();
        PrepareForSceneWindow();
        if (!Cull())
        {
            return;
        }

        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    private void Setup()
    {
        context.SetupCameraProperties(camera);
        var clearFlags = camera.clearFlags;
        buffer.ClearRenderTarget(
            clearFlags<= CameraClearFlags.Nothing,
            clearFlags == CameraClearFlags.Color,
            clearFlags == CameraClearFlags.Color ?
                camera.backgroundColor.linear : Color.clear);

        buffer.BeginSample(SampleName);        
        ExecuteBuffer();
    }

    private void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);

        context.DrawRenderers(cullingResult, ref drawingSettings, ref filterSettings);

        //context
        context.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filterSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResult, ref drawingSettings, ref filterSettings);

    }
    private bool Cull()
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResult = context.Cull(ref p);
            return true;
        }

        return false;
    }

    private void Submit()
    {

        buffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
        
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
