using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(UserLUTRenderer), PostProcessEvent.AfterStack, "Custom/UserLUT")]
public sealed class UserLUT : PostProcessEffectSettings
{
    [DisplayName("LUT"), Tooltip("LDR Lookup Texture, 1024x32")]
    public TextureParameter lut = new TextureParameter { value = null, defaultState = TextureParameterDefault.Lut2D };
    [Range(0f, 1f), Tooltip("LUT blend")]
    public FloatParameter blend = new FloatParameter { value = 1.0f };
    [DisplayName("Use Background LUT"), Tooltip("Activate background LUT")]
    public BoolParameter useBackgroundLut = new BoolParameter{ value = false };
    [DisplayName("Background LUT"), Tooltip("Background Lookup Texture, 1024x32")]
    public TextureParameter backgroundLut = new TextureParameter { value = null, defaultState = TextureParameterDefault.Lut2D };
    [Tooltip("Distance at which blend to background starts")]
    public FloatParameter backgroundBlendStart = new FloatParameter { value = 50.0f };
    [Tooltip("Range of blending default LUT to background LUT (from start of blend)")]
    public FloatParameter backgroundBlendRange = new FloatParameter { value = 10.0f };

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if (lut.value == null || blend.value == 0)
        {
            return false;
        }
        return enabled.value;
    }
}

public sealed class UserLUTRenderer : PostProcessEffectRenderer<UserLUT>
{
    static class Uniforms {
        internal static readonly int _UserLut = Shader.PropertyToID("_UserLut");
        internal static readonly int _UserLut_Params = Shader.PropertyToID("_UserLut_Params");
        internal static readonly int _BGLut = Shader.PropertyToID("_BGLut");
        internal static readonly int _BGLut_Params = Shader.PropertyToID("_BGLut_Params");
        internal static readonly int _BGLut_Blend = Shader.PropertyToID("_BGLut_Blend");
    }

    public override DepthTextureMode GetCameraFlags()
    {
        if (settings.useBackgroundLut.value)
            return DepthTextureMode.Depth;
        return DepthTextureMode.None;
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/UserLUT"));
        sheet.ClearKeywords();
        Texture lut = settings.lut.value;

        Vector4 lutParams = new Vector4(1f / lut.width, 1f / lut.height, lut.height - 1f, settings.blend.value);
        sheet.properties.SetTexture(Uniforms._UserLut, lut);
        sheet.properties.SetVector(Uniforms._UserLut_Params, lutParams);
        if (settings.useBackgroundLut)
        {
            sheet.EnableKeyword("USE_BG_LUT");
            if (settings.backgroundLut.value != null)
            {
                lut = settings.backgroundLut.value;
            }
            lutParams = new Vector4(1f / lut.width, 1f / lut.height, lut.height - 1f, 0);
            sheet.properties.SetTexture(Uniforms._BGLut, lut);
            sheet.properties.SetVector(Uniforms._BGLut_Params, lutParams);
            sheet.properties.SetVector(Uniforms._BGLut_Blend, new Vector4(settings.backgroundBlendStart.value, settings.backgroundBlendRange.value, 0, 0));
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}