#ifndef ALTOS_CAMERA_MOTION_VECTORS_INCLUDED
#define ALTOS_CAMERA_MOTION_VECTORS_INCLUDED


float4x4 _ViewProjM;
float4x4 _PrevViewProjM;

float2 GetMotionVector(float3 positionOS)
{
	float4 positionCS = TransformObjectToHClip(positionOS);
	float4 projPos = positionCS * 0.5;
	projPos.xy = projPos.xy + projPos.w;
	
	half depth = 1.0;
	float3 viewPos = ComputeViewSpacePosition(projPos.xy, depth, unity_CameraInvProjection);
	float4 worldPos = float4(mul(unity_CameraToWorld, float4(viewPos, 1.0)).xyz, 1.0);

	float4 prevClipPos = mul(_PrevViewProjM, worldPos);
	float4 curClipPos = mul(_ViewProjM, worldPos);

	float2 prevPosCS = prevClipPos.xy / prevClipPos.w;
	float2 curPosCS = curClipPos.xy / curClipPos.w;
	
	float2 velocity = prevPosCS - curPosCS;
	
	velocity.xy *= 0.5;
	return velocity;
}

void GetCameraMotionVectors_float(float4 UV, float3 positionOS, out float2 velocity)
{
	velocity = 0;
	#ifndef SHADERGRAPH_PREVIEW
	velocity = GetMotionVector(positionOS);
	#endif
}
#endif