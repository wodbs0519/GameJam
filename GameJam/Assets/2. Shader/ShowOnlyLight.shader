Shader "Unlit/ShowOnlyLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
//		_GraTex("Gradation Texture", 2D) = "white" {}
//		_NormalTex("Normal Texture", 2D) = "white" {}
//
//		
//        _IsFlip("isFlip", Int) = 1
//		_BoundaryRange("Boundary Range", Range(0,4)) = 0.2
//		_CorrectValue("Correction Value", Float) = 0
//		_zIntensity("Z Intensity", Float) = 0
//		_Glow("Glow", Float) = 0
//		_Attenuation("Attenuation", Float) = 0

	}
	SubShader
	{
		Tags { 
		"RenderType"="Opaque" 
		}
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
		        fixed4 color = tex2D(_MainTex, i.uv);
		        clip(color.a - 0.001);
		        
				return 0;
			}
			ENDCG
		}
		
//		Pass
//		{
//		    
//		    Tags{
//		        "LightMode" = "ForwardAdd"
//		    }
//		
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#include "UnityCG.cginc"
//
//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float2 uv : TEXCOORD0;
//			};
//
//			struct v2f
//			{
//				float2 uv : TEXCOORD0;
//				float4 vertex : SV_POSITION;
//			    half3 lightColorAttenuated	: COLOR;	// Rim light color attenuated with distance
//                half4 lightDir : TEXCOORD1;				// Light direction
//			};
//
//			sampler2D _MainTex;
//			float4 _MainTex_ST;
//			
//            sampler2D _GraTex;
//            float4 _GraTex_ST;
//            
//            sampler2D _NormalTex;
//            float4 _NormalTex_ST;
//            
//            int _IsFlip;
//                   
//            float _BoundaryRange, _zIntensity, _Glow, _CorrectValue, _Attenuation;
//			
//			uniform float4 _LightColor0;
//			
//			v2f vert (appdata v)
//			{
//				v2f o;
//				o.vertex = UnityObjectToClipPos(v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//				o.lightDir.xyz = ObjSpaceLightDir(v.vertex);
//				o.lightDir.z = -o.lightDir.z;
//                o.lightDir.w = length(o.lightDir.xyz);
//                o.lightColorAttenuated = _LightColor0.rgb / pow(o.lightDir.w, _Attenuation);
//                o.lightDir = normalize(o.lightDir);
//				return o;
//			}
//			
//			fixed4 frag (v2f i) : SV_Target
//			{
//				fixed4 col = tex2D(_MainTex, i.uv);
//				clip(col.a - 0.01);
//				
//				float4 finalColor = 0;
//				
//				
//				//그라데이션 밸류 구하기
//				half4 GraTex = tex2D(_GraTex, i.uv);
//				// 역전되있어서 뒤집어줌
//				GraTex.r *= -1;
//				GraTex.r += 1;
//				
//				
//				//그라데이션 밸류 조절
//				float tempGratex = GraTex.r / 0.2;
//				tempGratex -= _BoundaryRange;
//				tempGratex = saturate(tempGratex);
//				
//				half3 NormalTex = UnpackNormal(tex2D(_NormalTex, i.uv));
//				
//				
//				NormalTex.x *= _IsFlip;
//				
//				NormalTex.y *= -1;
//				NormalTex.z *=-1;
//				
//				half3 normalVector = NormalTex;
//			    normalVector = normalize(normalVector);
//			    
//			    
//			    float dotZ = dot(i.lightDir.z, normalVector.z);
//			    
//			    dotZ *= sign(dotZ);
//			    dotZ = 1- dotZ;
//			    
//			    dotZ = pow(dotZ, 2);
//			    
//				float dotValue = dot(i.lightDir.xy, normalVector.xy);
//				
//				dotValue += _CorrectValue;
//				
//				dotValue = saturate(dotValue);
//				dotValue = pow(dotValue, 2);
//				
//				dotValue += dotZ * _zIntensity;
//				
//				float3 rimResult = dotValue * i.lightColorAttenuated;
//				
//				finalColor.rgb = rimResult * _Glow;
//				finalColor *= tempGratex;
//				
//				float clampVal = 0.7;
//				
//				finalColor.r = clamp(finalColor.r, 0, clampVal);
//				finalColor.g = clamp(finalColor.g, 0, clampVal);
//				finalColor.b = clamp(finalColor.b, 0, clampVal);
//
//				return finalColor;
//			}
//			ENDCG
//		}
		
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
	            clip(texureColor.a - 0.001);
                SHADOW_CASTER_FRAGMENT(i)
            }
		    
		    ENDCG
		}
	}
}
