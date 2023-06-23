﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Custom Effects - https://docs.monogame.net/articles/content/custom_effects.html
// High-level shader language (HLSL) - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl
// Programming guide for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-pguide
// Reference for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-reference
// HLSL Semantics - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 DiffuseColor;

float Time = 0;

struct VertexShaderInput
{
	float4 Position : POSITION;
};

struct VertexShaderOutput
{	
	float4 position : SV_POSITION;
	float4 objectPosition : TEXCOORD0;
	float4 worldPos : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{	
	VertexShaderInput resultado = (VertexShaderInput)0;
	if(input.Position.x>10.0){
		resultado.Position.x = 10.0;
	}else{
		resultado.Position.x = input.Position.x;
	}
	resultado.Position.y = input.Position.y;
	resultado.Position.z = input.Position.z;
	resultado.Position.w = input.Position.w;

    // Clear the output
	VertexShaderOutput output = (VertexShaderOutput)0;
    // Model space to World space
    float4 worldPosition = mul(resultado.Position, World);
    // World space to View space
    float4 viewPosition = mul(worldPosition, View);	
	// View space to Projection space
    output.position = mul(viewPosition, Projection);
	output.objectPosition = input.Position;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{	

	float3 color = float3(1.0,0.0,0.0);

	return float4(color, 1.0);
    
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};