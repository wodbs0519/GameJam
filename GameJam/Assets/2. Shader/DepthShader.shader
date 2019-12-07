﻿Shader "Unlit/DepthShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
                float4 color    : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
                fixed4 color    : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				col *= i.color.a;
				clip(col.a - 0.01);
				return col;
			}
			ENDCG
		}
		
		Pass{
		    
		    Tags {"LightMode"="ShadowCaster"}
            
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f { 
                float2 uv : TEXCOORD0;
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata v)
            {
                v2f o;
		        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed4 texureColor = tex2D(_MainTex, i.uv);;
	            clip(texureColor.a - 0.01);
                SHADOW_CASTER_FRAGMENT(i)
            }
		    
		    ENDCG
		}
	}
}
