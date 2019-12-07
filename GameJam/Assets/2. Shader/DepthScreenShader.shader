Shader "Hidden/DepthScreen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Target Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 scrPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};
			
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.scrPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float depth = (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos))).r;
				depth = depth * step(0.56, depth);
				depth = 1 - depth;
                fixed stepVal = step(0.2, depth);
                if(depth == 1) return _Color;
                depth = step(0.1, depth);
                col = lerp(0, col, depth);
                
				return col;
			}
			ENDCG
		}
	}
}
