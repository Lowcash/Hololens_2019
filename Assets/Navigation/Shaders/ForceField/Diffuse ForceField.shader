﻿Shader "Custom/Diffuse ForceField"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_NoiseTex("NoiseTexture", 2D) = "white" {}
		_DistortStrength("DistortStrength", Range(0,1)) = 0.2
		_DistortTimeFactor("DistortTimeFactor", Range(0,1)) = 0.2
		_RimStrength("RimStrength",Range(0, 1000)) = 2
		_IntersectPower("IntersectPower", Range(0, 3)) = 2
		_RimOffset("RimOffset", Range(0, 1)) = 0
		_RimOffsetTwo("RimOffsetTwo", Range(0, 1)) = 0
	}

	SubShader
	{
		ZWrite On
		Cull Front
		Blend SrcAlpha OneMinusSrcAlpha

		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		GrabPass
		{
			"_GrabTempTex"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

				struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 grabPos : TEXCOORD2;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD3;
			};

			sampler2D _GrabTempTex;
			float4 _GrabTempTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _DistortStrength;
			float _DistortTimeFactor;
			float _RimStrength;
			float _IntersectPower;
			float _RimOffset;
			float _RimOffsetTwo;

			sampler2D _CameraDepthTexture;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.grabPos = ComputeGrabScreenPos(o.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);

				o.screenPos = ComputeScreenPos(o.vertex);

				COMPUTE_EYEDEPTH(o.screenPos.z);

				o.normal = UnityObjectToWorldNormal(v.normal);

				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}

			fixed4 _Color;


			/*fixed4 frag(v2f i) : SV_Target
			{
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
				float partZ = i.screenPos.z;

				float diff = sceneZ - partZ;
				float intersect = (1 - diff) * _IntersectPower;

				float3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, i.vertex)));
				float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * _RimStrength;
				float glow = max(intersect, rim);

				float4 offset = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
				i.grabPos.xy -= offset.xy * _DistortStrength;
			
				fixed4 textureColor = tex2Dproj(_GrabTempTex, i.grabPos);
			
				float4 color = _Color * glow + textureColor;
				color.a *= _Color.a;

				return color;
			}*/

			fixed4 frag(v2f i) : SV_Target
			{
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
				float partZ = i.screenPos.z;

				float diff = sceneZ - partZ;
				float intersect = (1 - diff) * _IntersectPower;

				float3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, i.vertex)));
				float rim1 = 1 - abs(dot(i.normal, normalize(i.viewDir)) + _RimOffset) * _RimStrength;
				float rim2 = 1 - abs(dot(i.normal, normalize(i.viewDir)) + _RimOffsetTwo) * _RimStrength;
				float glow = max(intersect, rim1);
				float glow2 = max(intersect, rim2);

				float4 offset = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
				i.grabPos.xy -= offset.xy * _DistortStrength;

				fixed4 textureColor = tex2Dproj(_GrabTempTex, i.grabPos);

				float4 color = (_Color * glow) + (_Color * glow2) + textureColor;
				color.a *= _Color.a;

				return color;
			}

			ENDCG
		}
	}
}