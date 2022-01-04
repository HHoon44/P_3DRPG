
//http://wiki.unity3d.com/index.php/TransparentShadowCaster

Shader "Toon/ToonTerrain"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
		_Mask("Mixing Mask (RGBA)", 2D) = "red" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "red" {}
	}

		Category
	{
		Lighting On
		Cull Back
		Fog { Color[_AddFog] }
		Subshader
		{
			CGPROGRAM
			#pragma surface surf ToonRamp

			sampler2D _Ramp;


			inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
			{
				#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
				#endif

				half d = dot(s.Normal, lightDir) * 0.5 + 0.5;

				half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

				half4 c;



				c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten / 2);
				c.a = 0;
				return c;
			}

			sampler2D _Splat0;
			sampler2D _Splat1;

			sampler2D _Mask;
			float4 _Color;

			struct Input
			{
				float4 pos : SV_POSITION;
				float2 uv_Splat0;
				float2 uv_Splat1;

				float2 uv_Mask;
				float2 uv_Base;


				float4 diffuse : COLOR;
			};


			void surf(Input IN, inout SurfaceOutput o)
			{

				// get the first three layer colors
				half4 color1 = tex2D(_Splat0, IN.uv_Splat0);
				half4 color2 = tex2D(_Splat1, IN.uv_Splat1);


				// get the mixing mask texture
				half4 mask = tex2D(_Mask, IN.uv_Mask.xy);
				// mix the three layers

				half4 color = color1 * mask.r + color2 * mask.g;

				o.Albedo = color.rgb;
				o.Alpha = color.a;
			}

			ENDCG

			Blend one one

			CGPROGRAM
			#pragma surface surf ToonRamp

			sampler2D _Ramp;

			inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
			{
				#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
				#endif

				half d = dot(s.Normal, lightDir) * 0.5 + 0.5;

				half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

				half4 c;

				c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten / 2);
				c.a = 0;
				return c;
			}


			sampler2D _Splat2;
			sampler2D _Splat3;
			sampler2D _Mask;
			float4 _Color;

			struct Input
			{
				float4 pos : SV_POSITION;

				float2 uv_Splat2;
				float2 uv_Splat3;
				float2 uv_Mask;
				float2 uv_Base;


				float4 diffuse : COLOR;
			};



			void surf(Input IN, inout SurfaceOutput o)
			{

				// get the first three layer colors

				half4 color3 = tex2D(_Splat2, IN.uv_Splat2);
				half4 color4 = tex2D(_Splat3, IN.uv_Splat3);

				// get the mixing mask texture
				half4 mask = tex2D(_Mask, IN.uv_Mask.xy);


				// mix the three layers
				half4 color = color3 * mask.b + color4 * mask.a;

				o.Albedo = color.rgb / 2;
				o.Alpha = color.a;
			}

			ENDCG
		}
	}

		// ------------------------------------------------------------------
		// Radeon 7000 / 9000
				SubShader
			{
				Pass
				{
					Material
					{
						Diffuse[_Color]
						Ambient[_Color]
					}
					Lighting On
					SetTexture[_MainTexture]
					{
						Combine texture * primary DOUBLE, texture * primary
					}
				}
			}
}