﻿// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "AdOne/MT_Character NPC"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_Color ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		[MainTexture] _MainTex ("Albedo", 2D) = "white" {}
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		_RampThreshold ("Threshold", Range(0.01,1)) = 0.5
		_RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
		[TCP2Separator]

		[TCP2HeaderHelp(Emission)]
		[NoScaleOffset] _EmissionMap ("Emission Map", 2D) = "white" {}
		 [TCP2ColorNoAlpha] _EmissionColor ("Emission Color", Color) = (0,0,0,1)
		[TCP2Separator]
		
		// Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"	// needed for LightColor

		// Texture/Sampler abstraction
		#define TCP2_TEX2D_WITH_SAMPLER(tex)						UNITY_DECLARE_TEX2D(tex)
		#define TCP2_TEX2D_NO_SAMPLER(tex)							UNITY_DECLARE_TEX2D_NOSAMPLER(tex)
		#define TCP2_TEX2D_SAMPLE(tex, samplertex, coord)			UNITY_SAMPLE_TEX2D_SAMPLER(tex, samplertex, coord)
		#define TCP2_TEX2D_SAMPLE_LOD(tex, samplertex, coord, lod)	UNITY_SAMPLE_TEX2D_SAMPLER_LOD(tex, samplertex, coord, lod)

		// Shader Properties
		TCP2_TEX2D_WITH_SAMPLER(_MainTex);
		TCP2_TEX2D_WITH_SAMPLER(_EmissionMap);
		
		// Shader Properties
		float4 _MainTex_ST;
		fixed4 _Color;
		fixed4 _EmissionColor;
		float _RampThreshold;
		float _RampSmoothing;
		fixed4 _HColor;
		fixed4 _SColor;

		ENDCG

		// Main Surface Shader

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha nolightmap nofog nolppv
		#pragma target 3.0

		//================================================================
		// STRUCTS

		// Vertex input
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord0 : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		#if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
			half4 tangent : TANGENT;
		#endif
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct Input
		{
			float2 texcoord0;
		};

		//================================================================

		// Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Specular;
			half Gloss;
			half Alpha;

			Input input;

			// Shader Properties
			float __rampThreshold;
			float __rampSmoothing;
			float3 __highlightColor;
			float3 __shadowColor;
			float __ambientIntensity;
		};

		//================================================================
		// VERTEX FUNCTION

		void vertex_surface(inout appdata_tcp2 v, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			// Texture Coordinates
			output.texcoord0.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input input, inout SurfaceOutputCustom output)
		{
			// Shader Properties Sampling
			float4 __albedo = ( TCP2_TEX2D_SAMPLE(_MainTex, _MainTex, input.texcoord0.xy).rgba );
			float4 __mainColor = ( _Color.rgba );
			float __alpha = ( __albedo.a * __mainColor.a );
			float3 __emission = ( TCP2_TEX2D_SAMPLE(_EmissionMap, _EmissionMap, input.texcoord0.xy).rgb * _EmissionColor.rgb );
			output.__rampThreshold = ( _RampThreshold );
			output.__rampSmoothing = ( _RampSmoothing );
			output.__highlightColor = ( _HColor.rgb );
			output.__shadowColor = ( _SColor.rgb );
			output.__ambientIntensity = ( 1.0 );

			output.input = input;

			output.Albedo = __albedo.rgb;
			output.Alpha = __alpha;

			output.Albedo *= __mainColor.rgb;
			output.Emission += __emission;

		}

		//================================================================
		// LIGHTING FUNCTION

		inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, UnityGI gi)
		{

			half3 lightDir = gi.light.dir;
			#if defined(UNITY_PASS_FORWARDBASE)
				half3 lightColor = _LightColor0.rgb;
				half atten = surface.atten;
			#else
				// extract attenuation from point/spot lights
				half3 lightColor = _LightColor0.rgb;
				half atten = max(gi.light.color.r, max(gi.light.color.g, gi.light.color.b)) / max(_LightColor0.r, max(_LightColor0.g, _LightColor0.b));
			#endif

			half3 normal = normalize(surface.Normal);
			half ndl = dot(normal, lightDir);
			half3 ramp;
			
			#define		RAMP_THRESHOLD	surface.__rampThreshold
			#define		RAMP_SMOOTH		surface.__rampSmoothing
			ndl = saturate(ndl);
			ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH*0.5, RAMP_THRESHOLD + RAMP_SMOOTH*0.5, ndl);

			// Apply attenuation (shadowmaps & point/spot lights attenuation)
			ramp *= atten;

			// Highlight/Shadow Colors
			#if !defined(UNITY_PASS_FORWARDBASE)
				ramp = lerp(half3(0,0,0), surface.__highlightColor, ramp);
			#else
				ramp = lerp(surface.__shadowColor, surface.__highlightColor, ramp);
			#endif

			// Output color
			half4 color;
			color.rgb = surface.Albedo * lightColor.rgb * ramp;
			color.a = surface.Alpha;

			// Apply indirect lighting (ambient)
			half occlusion = 1;
			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				half3 ambient = gi.indirect.diffuse;
				ambient *= surface.Albedo * occlusion * surface.__ambientIntensity;

				color.rgb += ambient;
			#endif

			return color;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi)
		{
			half3 normal = surface.Normal;

			// GI without reflection probes
			gi = UnityGlobalIllumination(data, 1.0, normal); // occlusion is applied in the lighting function, if necessary

			surface.atten = data.atten; // transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb; // remove attenuation

		}

		ENDCG

	}

	Fallback "Diffuse"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2020.3.33f1";ver:"2.9.0";tmplt:"SG2_Template_Default";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","UNITY_2019_4","UNITY_2020_1","PASS_SILHOUETTE","SILHOUETTE_STENCIL","EMISSION"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0"];shaderProperties:list[,,,,,,,,sp(name:"Emission";imps:list[imp_mp_texture(uto:False;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:3;chan:"RGB";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";tpln_scale:1;uv_shaderproperty:__NULL__;uv_cmp:__NULL__;sep_sampler:__NULL__;prop:"_EmissionMap";md:"";gbv:False;custom:False;refs:"";pnlock:False;guid:"38f3a32f-64fa-4dc2-9fd4-8ece558293c1";op:Multiply;lbl:"Emission Map";gpu_inst:False;locked:False;impl_index:-1),imp_mp_color(def:RGBA(0, 0, 0, 1);hdr:False;cc:3;chan:"RGB";prop:"_EmissionColor";md:"";gbv:False;custom:False;refs:"";pnlock:False;guid:"a3e4a094-fe5b-4e43-a093-0feb16a7a9cf";op:Multiply;lbl:"Emission Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH a38188af4e623cdbd4f680663f78c347 */
