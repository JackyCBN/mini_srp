using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    ScriptableRenderContext context;
    Camera camera;
    public void Render(ScriptableRenderContext _context, Camera _camera)
    {
        context = _context;
        camera = _camera;

        ScriptableCullingParameters cullingParameters;
        if (!camera.TryGetCullingParameters(out cullingParameters))
        {
            return;
        }
        CullingResults cullingResult = context.Cull(ref cullingParameters);

        DrawingSettings drawingSettings = new DrawingSettings();
        FilteringSettings filterSettings = new FilteringSettings();

        context.DrawRenderers(cullingResult, ref drawingSettings, ref filterSettings);
        //context
        context.DrawSkybox(camera);
    }
}
