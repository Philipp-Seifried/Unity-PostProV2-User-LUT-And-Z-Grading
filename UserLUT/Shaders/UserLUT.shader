Shader "Hidden/Custom/UserLUT"
{
    HLSLINCLUDE

		#pragma multi_compile __ USE_BG_LUT
		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_UserLut, sampler_UserLut);
		float4 _UserLut_Params;
        #if USE_BG_LUT
            TEXTURE2D_SAMPLER2D(_BGLut, sampler_BGLut);
            float4 _BGLut_Params;
            float2 _BGLut_Blend;
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        #endif

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            color.rgb = saturate(color.rgb);
            half3 colorGraded;

			#if USE_BG_LUT
				half3 colorGradedBG;
				float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo));
				float bgBlend = saturate((depth - _BGLut_Blend.x) / _BGLut_Blend.y);
			#endif

			#if !UNITY_COLORSPACE_GAMMA
				half3 colorSRGB = LinearToSRGB(color.rgb);
				colorGraded = ApplyLut2D(TEXTURE2D_PARAM(_UserLut, sampler_UserLut), colorSRGB, _UserLut_Params.xyz);
				#if USE_BG_LUT
					colorGraded = lerp(colorGraded, ApplyLut2D(TEXTURE2D_PARAM(_BGLut, sampler_BGLut), colorSRGB, _BGLut_Params.xyz), bgBlend);
				#endif
				colorGraded = SRGBToLinear(colorGraded);
			#else 
				colorGraded = ApplyLut2D(TEXTURE2D_PARAM(_UserLut, sampler_UserLut), color.rgb, _UserLut_Params.xyz);
				#if USE_BG_LUT
					colorGraded = lerp(colorGraded, ApplyLut2D(TEXTURE2D_PARAM(_BGLut, sampler_BGLut), color.rgb, _BGLut_Params.xyz), bgBlend);
				#endif
			#endif

            color.rgb = lerp(color.rgb, colorGraded, _UserLut_Params.w);
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}