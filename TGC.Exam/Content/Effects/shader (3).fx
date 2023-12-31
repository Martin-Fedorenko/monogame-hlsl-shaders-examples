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

float Time;

texture Textura;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinate : TEXCOORD0;
    float4 worldPos : TEXCOORD1;
	
};


sampler2D textureSampler = sampler_state {
    Texture = (Textura);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	float radius = lerp(10, 100, sin(Time)*0.5+0.5);
    float3 sphere = length(input.Position.xyz)>radius ? normalize(input.Position.xyz) * radius : input.Position.xyz;
    float4 coordinates = float4(sphere, input.Position.w);
    
    // Clear the output
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(coordinates, World);
    // Model space to World space
    //float4 worldPosition = mul(float4(input.Position.x *= sin(Time), input.Position.y *= sin(Time), input.Position.z *= sin(Time), input.Position.w) *= sin(Time), World);
    output.worldPos = worldPosition;
    // World space to View space
    float4 viewPosition = mul(worldPosition, View);	
	// View space to Projection space
    output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate;


    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{	
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    float4 azul = float4(0.0,0.0,255.0,1.0);
    float4 color = lerp(azul, textureColor, sin(Time)*0.5+0.5);
    
    return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
