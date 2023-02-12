#ifndef ALTOS_TEX_UTILS_INCLUDED
#define ALTOS_TEX_UTILS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Math.hlsl"

float _CLOUD_RENDER_SCALE;
Texture2D _BLUE_NOISE;
float4 _BLUE_NOISE_TexelSize;
Texture2D _Halton_23_Sequence;
float4 _Halton_23_Sequence_TexelSize;
SamplerState linear_clamp_sampler;
SamplerState linear_repeat_sampler;
SamplerState point_repeat_sampler;
SamplerState point_clamp_sampler;
int _FrameId;
Texture2D _DitheredDepthTexture;
float4 _RenderTextureDimensions;



void GetWSRayDirectionFromUV(half2 uv, out half3 rayDirection, out half viewLength)
{
	half3 viewVector = mul(unity_CameraInvProjection, float4(uv * 2 - 1, 0.0, -1)).xyz;
	viewVector = mul(unity_CameraToWorld, half4(viewVector, 0.0)).xyz;
	viewLength = length(viewVector);
	
	rayDirection = viewVector / viewLength;
}

bool IsUVInRange01(half2 UV)
{
	if (UV.x <= 0.0 || UV.x >= 1.0 || UV.y <= 0.0 || UV.y >= 1.0)
	{
		return false;
	}
	return true;
}

float2 GetTexCoordSize(float renderTextureScale)
{
	return 1.0 / (_ScreenParams.xy * renderTextureScale);
}

int GetPixelIndex(float2 uv, int2 size)
{
	return uv.y * size.y * size.x + uv.x * size.x;
}

int GetPixelIndexConstrained(float2 uv, int2 size, int2 domain)
{
	int indY = (uv.y * size.y) % domain.y;
	int indX = (uv.x * size.x) % domain.x;
	int ind = indY * domain.x + indX;
	return ind;
}

half2 GetHaltonFromTexture(int index)
{
	return _Halton_23_Sequence.Load(int3(index % _Halton_23_Sequence_TexelSize.z, 0, 0)).rg;
}

half Halton(int base, int index)
{
	float result = 0.0;
	float f = 1.0;
	while (index > 0)
	{
		f = f / float(base);
		result += f * float(index % base);
		index = index / base;
	}
	return result;
}

half2 Halton23(int index)
{
	return half2(Halton(2, index), Halton(3, index));
}

void ScreenToViewVector_half(half2 UV, out half3 viewVector)
{
#ifdef SHADERGRAPH_PREVIEW
	viewVector = half3(0.0, 0.0, 0.0);
#endif
	
	float3 viewDirectionTemp = mul(unity_CameraInvProjection, float4(UV * 2 - 1, 0.0, -1)).xyz;
	viewVector = mul(unity_CameraToWorld, float4(viewDirectionTemp, 1.0)).xyz;
}

float4 ObjectToClipPos(float3 pos)
{
	return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(pos, 1)));
}

// <- Note on Upsample Methodology ->
// 
// Sample the local box and accumulate samples with a similar depth
// Then replace the color value here with a random sample from the local depth region
//
// <- End Note ->
float _UPSCALE_SOURCE_RENDER_SCALE;
void CheckerboardUpsample_half(Texture2D Tex, half2 UV, out half3 Color, out half Alpha)
{
#ifdef SHADERGRAPH_PREVIEW
	Color = 0;
	Alpha = 0;
#endif
	
	float2 o[4] =
	{
		float2(0.0, 0.0),
		float2(1.0, 0.0),
		float2(1.0, 1.0),
		float2(0.0, 1.0)
	};
	
	
	half2 texCoord = GetTexCoordSize(_UPSCALE_SOURCE_RENDER_SCALE);
	
	float depthRaw = SampleSceneDepth(UV);
	float depth01 = Linear01Depth(depthRaw, _ZBufferParams);
	
	float4 col = float4(0,0,0,0);
	int validCounter = 0;
	float4 colors[4];
	
	float depthThreshold = 0.001;
	
	float sDepths[4];
	
	for (int g = 0; g < 4; g++)
	{
		
		float sDepthRaw = _DitheredDepthTexture.SampleLevel(point_clamp_sampler, (UV + o[g] * texCoord), 0).r;
		float sDepth01 = Linear01Depth(sDepthRaw, _ZBufferParams);
		sDepths[g] = sDepth01;
	}
	
	bool areAllNear = abs(sDepths[0] - depth01) < depthThreshold && abs(sDepths[1] - depth01) < depthThreshold && abs(sDepths[2] - depth01) < depthThreshold && abs(sDepths[3] - depth01) < depthThreshold;
	if(areAllNear)
	{
		col = Tex.SampleLevel(linear_clamp_sampler, UV, 0);   
	}
	else
	{
		int nearestDepthUVId = 0;
		float nearestDistance = abs(sDepths[0] - depth01);
		for(int h = 1; h < 4; h++)
		{
			if(abs(sDepths[h] - depth01) < nearestDistance)
				nearestDepthUVId = h;
		}
		
		col = Tex.SampleLevel(point_clamp_sampler, UV + o[nearestDepthUVId] * texCoord, 0);
	}
	
	Color = col.rgb;
	Alpha = col.a;
}


// <- Start Note ->
//
// For each 2x2 pixel block, I want to pick the farthest depth and assign it to all 4 pixels.
// 
// SampleSceneDepth returns a value in the range [1, 0], where 1 is the near plane and 0 is the far plane.
// To get the farthest sample, you want the lesser raw depth value.
// This may depend on the platform.
// If we start seeing bugs in this area, we can compare the Linear01Depths instead using Linear01Depth(rawDepth, _ZBufferParams) and match an index.
// For now, we aren't taking this approach because it can incur a performance cost.
//
// <- End Note ->
int _USE_DITHERED_DEPTH;

void DitherDepth_float(float2 UV, out float RawDepth)
{
	#ifdef SHADERGRAPH_PREVIEW
	RawDepth = 0;
	#endif
	
	
	if (_USE_DITHERED_DEPTH)
	{
		float2 o[4] =
		{
			float2(0.0, 0.0),
			float2(1.0, 0.0),
			float2(1.0, 1.0),
			float2(0.0, 1.0)
		};
		
		float2 s = GetTexCoordSize(1.0);
		float depthMaxRaw = 1.0;
		float depthMinRaw = 0.0;
		UV *= _ScreenParams.xy;
		UV *= 0.5;
		
		UV = floor(UV);
		UV *= 2.0;
		float2 PixelCoord = UV;
		UV /= _ScreenParams.xy;
		
		
		
		for (int i = 0; i < 4; i++)
		{
			float2 coord = UV + s * o[i] + s * 0.5;
			float depthSampleRaw = SampleSceneDepth(coord);;
			depthMaxRaw = min(depthSampleRaw, depthMaxRaw);
			depthMinRaw = max(depthSampleRaw, depthMinRaw);
		}
		
		if((PixelCoord.x + PixelCoord.y) % 2 == 0)
		{
			RawDepth = depthMaxRaw;	  
		}
		else
		{
			RawDepth = depthMinRaw;
		}
	}
	else
	{
		RawDepth = SampleSceneDepth(UV);
	}
}
#endif
