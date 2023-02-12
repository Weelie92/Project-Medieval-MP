#ifndef ALTOS_MATH_INCLUDED
#define ALTOS_MATH_INCLUDED

static float _INF = 1e20;
static float _INV_INF = 1.0/1e20;

static half4 _LinearFalloff = half4(0.53, 0.27, 0.13, 0.07);
static half4 _SqrtFalloff = half4(0.39, 0.28, 0.20, 0.14);
static half4 _Pow2Falloff = half4(0.75, 0.19, 0.05, 0.01);
static half4 _ExpFalloff = half4(0.40, 0.24, 0.19, 0.17);


half CalculateHorizon(half3 position, half3 direction, half planetRadius)
{
	half h = max(position.y, 0);
	half r = planetRadius;
	half a = r + h;
	half b = r / a;
	half c = acos(b);
	half angle = direction.y * 1.571;
	half d = angle - c;
	
	return smoothstep(-radians(_INV_INF), radians(_INV_INF), d);
}


half InverseLerp(half a, half b, half v)
{
	return (v - a) / (b - a);
}

half RemapUnclamped(half iMin, half iMax, half oMin, half oMax, half v)
{
	half t = InverseLerp(iMin, iMax, v);
	return lerp(oMin, oMax, t);
}

half Remap(half iMin, half iMax, half oMin, half oMax, half v)
{
	v = clamp(v, iMin, iMax);
	return RemapUnclamped(iMin, iMax, oMin, oMax, v);
}

half Remap01(half iMin, half iMax, half v)
{
	return saturate(Remap(iMin, iMax, 0.0, 1.0, v));
}

half EaseIn(half a)
{
	return a * a;
}

half EaseOut(half a)
{
	return 1 - EaseIn(1 - a);
}

half EaseInOut(half a)
{
	return lerp(EaseIn(a), EaseOut(a), a);
}


float rand3dTo1d(float3 vec, float3 dotDir = float3(12.9898, 78.233, 154.681))
{
	float random = dot(sin(vec.xyz), dotDir);
	random = frac(sin(random) * 143758.5453);
	return random;
}

float2 rand3dto2d(float3 vec, float3 seed = 4141)
{
	return float2(
		rand3dTo1d(vec + seed),
		rand3dTo1d(vec + seed, float3(67.416, 44.529, 46.749))
	);
}

float rand2dTo1d(float2 vec, float2 dotDir = float2(12.9898, 78.233))
{
	float random = dot(sin(vec.xy), dotDir);
	random = frac(sin(random) * 143758.5453);
	return random;
}

float2 rand2dTo2d(float2 vec, float2 seed = 4605)
{
	return float2(
		rand2dTo1d(vec + seed),
		rand2dTo1d(vec + seed, float2(39.346, 11.135))
	);
}


half2 GetDir(half x, half y)
{
	return rand2dTo2d(float2(x, y)) * 2.0 - 1.0;
}

half GetPerlinNoise(half2 position)
{
	half2 lowerLeft = GetDir(floor(position.x), floor(position.y));
	half2 lowerRight = GetDir(ceil(position.x), floor(position.y));
	half2 upperLeft = GetDir(floor(position.x), ceil(position.y));
	half2 upperRight = GetDir(ceil(position.x), ceil(position.y));
	
	half2 f = frac(position);
	
	lowerLeft = dot(lowerLeft, f);
	lowerRight = dot(lowerRight, f - half2(1.0, 0.0));
	upperLeft = dot(upperLeft, f - half2(0.0, 1.0));
	upperRight = dot(upperRight, f - half2(1.0, 1.0));
	
	half2 t = half2(EaseInOut(f.x), EaseInOut(f.y));
	half lowerMix = lerp(lowerLeft.x, lowerRight.x, t.x);
	half upperMix = lerp(upperLeft.x, upperRight.x, t.x);
	return lerp(lowerMix, upperMix, t.y);
}

half GetLayeredPerlinNoise(int octaves, half2 position, half gain, half lacunarity)
{
	half value = 0.0;
	half amp = 1.0;
	half frequency = 1.0;
	half c = 0.0;
	
	for (int i = 1; i <= octaves; i++)
	{
		value += GetPerlinNoise(position * frequency) * amp;
		c += amp;
		amp *= gain;
		frequency *= lacunarity;
	}
	value /= c;
	return saturate(value + 0.5);
}


void GradientPerlinNoise_half(half2 position, out half value)
{
	value = GetPerlinNoise(position);
}

#endif
