#ifndef ALTOS_TEMPORAL_AA_INCLUDED
#define ALTOS_TEMPORAL_AA_INCLUDED

#define _DEBUG_MOTION_VECTORS 0

#include "TextureUtils.hlsl"

void TAA_float(Texture2D HistoricData, Texture2D NewFrameData, float2 UV, float BlendFactor, float2 MotionVector, out half4 MergedData, out half3 MergedDataRGB, out half MergedDataA)
{
	#ifdef SHADERGRAPH_PREVIEW
	MergedData = 0;
	MergedDataRGB = 0;
	MergedDataA = 0;
	#endif
	
	float2 texCoord = (1.0 / _ScreenParams.xy);
	
	float4 newFrame = NewFrameData.SampleLevel(point_clamp_sampler, UV, 0);
	float cDepth01 = Linear01Depth(SampleSceneDepth(UV), _ZBufferParams);
	
	float2 offsets[8] =
	{
		float2(0, -1),
		float2(0, 1),
		float2(1, 0),
		float2(-1, 0),
		float2(-1, -1),
		float2(1, -1),
		float2(-1, 1),
		float2(1, 1)
	};
	
	float2 HistUV = UV + MotionVector;
	bool isValidHistUV = IsUVInRange01(HistUV);
	
	
	if (isValidHistUV)
	{
		half4 HistSample = HistoricData.SampleLevel(point_clamp_sampler, HistUV, 0);
		
		half4 minResults[2] =
		{
			newFrame,
			newFrame
		};
		half4 maxResults[2] =
		{
			newFrame,
			newFrame
		};
		
		half4 v[8];
		
		for (int i = 0; i < 8; i++)
		{
			v[i] = NewFrameData.SampleLevel(point_clamp_sampler, UV + texCoord * offsets[i], 0);
		}
		
		// cross sample
		for (int j = 0; j < 4; j++)
		{
			minResults[0] = min(v[j], minResults[0]);
			maxResults[0] = max(v[j], maxResults[0]);
		}
		
		minResults[1] = minResults[0];
		maxResults[1] = maxResults[0];
		
		// box sample 
		// (leverages the results of the cross sample to avoid rework).
		for (int k = 4; k < 8; k++)
		{
			minResults[1] = min(v[k], minResults[1]);
			maxResults[1] = max(v[k], maxResults[1]);
		}
		
		// average the results of the cross and box samples
		half4 minResult, maxResult;
		minResult = (minResults[0] + minResults[1]) * 0.5;
		maxResult = (maxResults[0] + maxResults[1]) * 0.5;
		
		half4 clampedHist = clamp(HistSample, minResult, maxResult);
		newFrame = lerp(clampedHist, newFrame, BlendFactor);
		
	}
	
	#if _DEBUG_MOTION_VECTORS
	newFrame = half4(1.0, 1.0, 1.0, 0.0);
	
	if (isValidHistUV)
	{
		newFrame = half4((MotionVector).x, (MotionVector).y, 0, 0) * 10.0;
	}
	#endif
	
	MergedData = newFrame;
	MergedDataRGB = MergedData.rgb;
	MergedDataA = MergedData.a;
}
#endif