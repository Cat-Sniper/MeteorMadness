Shader "Hidden/Custom/Haze"
{
	HLSLINCLUDE
	#include "../../PostProcessing/Shaders/StdLib.hlsl"
	
	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_GlobalDistortionTex, sampler_GlobalDistortionTex);
	float4 _MainTex_TexelSize;
	float _Magnitude;

	float4 Frag(varyingDefault i) : SV_Target {
		float2 mag = _Magnitude * _MainTex_TexelSize.xy;
		float2 distortion = SAMPLE_TEXTURE2D(_GlobalDistortionTex, sampler_GlobalDistortionTex, i.texcoord).xy * mag;
		float 4 color = SAMPLE_TEXTURE2D(_Maintex, sampler_MainTex, i.texcoord + distortion);
		return color;
	}

	ENDHLSL
 
    SubShader
    {
        // No culling or depth
        Cull Off
		ZWrite Off
		ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag

			ENDHLSL

        
        }
    }
}
