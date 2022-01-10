using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    ScriptableRenderContext context;
    Camera camera;

    const string bufferName = "My Render Camera";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    public void Render(ScriptableRenderContext _context, Camera _camera)
    {
        context = _context;
        camera = _camera;

        Setup();
        DrawVisibleGeometry();
        Submit();
    }

    private void Setup()
    {
        buffer.BeginSample(bufferName);
        buffer.ClearRenderTarget(true, true, Color.clear);
        ExecuteBuffer();
        
        context.SetupCameraProperties(camera);
    }

    private void DrawVisibleGeometry()
    {
        //ScriptableCullingParameters cullingParameters;
        //if (!camera.TryGetCullingParameters(out cullingParameters))
        //{
        //    return;
        //}
        //CullingResults cullingResult = context.Cull(ref cullingParameters);

        //DrawingSettings drawingSettings = new DrawingSettings();
        //FilteringSettings filterSettings = new FilteringSettings();

        //context.DrawRenderers(cullingResult, ref drawingSettings, ref filterSettings);
        //context
        context.DrawSkybox(camera);
    }

    private void Submit()
    {

        buffer.EndSample(bufferName);
        ExecuteBuffer();
        context.Submit();
        
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
