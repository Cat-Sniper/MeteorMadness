using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(HazeRenderer), PostProcessEvent.BeforeStack, "Custom/Haze")]
public class Haze : PostProcessEffectSettings {

    [Range(0f, 1.0f), Tooltip("The magnitude in texels of distortion fx.")]
    public FloatParameter Magnitude = new FloatParameter { value = 1.0f };

    [Range(0, 4), Tooltip("The down-scale factor to apply to the generated texture")]
    public IntParameter DownScaleFactor = new IntParameter { value = 0 };

    [Tooltip("Displays the Distortion effects in debug view.")]
    public BoolParameter DebugView = new BoolParameter { value = false };
}

public class HazeRenderer : PostProcessEffectRenderer<Haze> {

    private int _globalHazeTexID;
    private Shader _hazeShader;

    public override DepthTextureMode GetCameraFlags() {
        return DepthTextureMode.Depth;
    }

    public override void Init() {
        
        _globalHazeTexID = Shader.PropertyToID("_GlobalHazeTex");
        _hazeShader = Shader.Find("Hidden/Custom/HazeEffect");

        base.Init();
    }

    public override void Render(PostProcessRenderContext context) {

        var sheet = context.propertySheets.Get(_hazeShader);
        sheet.properties.SetFloat("_Magnitude", settings.Magnitude);

        if (!settings.DebugView) {
            context.command.GetTemporaryRT(_globalHazeTexID,
                context.camera.pixelWidth >> settings.DownScaleFactor,
                context.camera.pixelHeight >> settings.DownScaleFactor,
                0, FilterMode.Bilinear, RenderTextureFormat.RGFloat);
            context.command.SetRenderTarget(_globalHazeTexID);
            context.command.ClearRenderTarget(false, true, Color.clear);
        }

        HazeManager.Instance.PopulateCommandBuffer(context.command);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}