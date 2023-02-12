#ifndef ALTOS_CLOUD_SHADOWS_INCLUDED
#define ALTOS_CLOUD_SHADOWS_INCLUDED

#include "Math.hlsl"

Texture2D _CloudShadowHistoryTexture;
float4 _CloudShadowHistoryTexture_TexelSize;


float3 _MainCameraOrigin;
float4x4 _CloudShadow_WorldToShadowMatrix;
float3 _ShadowCasterCameraPosition;
float _CloudShadowStrength = 1.0;
float4 _CloudShadowOrthoParams;
SamplerState linear_clamp_sampler;
SamplerState point_clamp_sampler;

float GetOpticalDepth(float3 data, float depthEye)
{
	return min(data.b, data.g * max(0, depthEye - data.r));
}

float GetTransmittance(float v)
{
	return exp(-v);
}

float GetFalloff(float2 positionLS)
{
	float r = length(float2(0.5, 0.5) - positionLS.xy) * 2.0;
	return Remap(0.7, 1.0, 0.0, 1.0, saturate(r));
}

// This method gets the optical depth from the the cloud shadow texture.
// Then, we return the transmittance from the filtered optical depth.
float GetCloudShadowAttenuation(float3 positionWS)
{
	float3 positionLS = mul(_CloudShadow_WorldToShadowMatrix, float4(positionWS, 1.0)).xyz;
	
	// UNITY_REVERSED_Z gets defined as #define UNITY_REVERSED_Z 0 for OpenGL.
	// On D3D-like platforms, we need to flip the texture to sample it correctly.
	#if UNITY_REVERSED_Z == 1
	positionLS.y = 1.0 - positionLS.y;
	#endif
	
	float3 shadowData = float3(0, 0, 0);
	
	#define ONE_OVER_SIXTEEN 1.0 / 16.0
	
	for(float y = -1.5; y <= 1.5; y += 1.0)
	{
		for(float x = -1.5; x <= 1.5; x += 1.0)
		{
			shadowData += _CloudShadowHistoryTexture.SampleLevel(linear_clamp_sampler, positionLS.xy + _CloudShadowHistoryTexture_TexelSize.xy * float2(x,y), 0).rgb;
		}
	}
	shadowData *= ONE_OVER_SIXTEEN;
	
	
	float depthEye = positionLS.z * _CloudShadowOrthoParams.z * 2.0;
	float od = GetOpticalDepth(shadowData, depthEye);
	float transmittance = GetTransmittance(od);
	float shadowAttenuation = lerp(1.0 - _CloudShadowStrength, 1.0, transmittance);
	return lerp(shadowAttenuation, 1.0, GetFalloff(positionLS.xy)); 
}

void GetCloudShadowAttenuation_float(float3 positionWS, out float attenuation)
{
	attenuation = 1.0;
	#ifndef SHADERGRAPH_PREVIEW
		attenuation = GetCloudShadowAttenuation(positionWS);
	#endif
}

#endif
