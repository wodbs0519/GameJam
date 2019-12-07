Shader "Unlit/JaeyunParticleAdditive"
{
	Properties {
     _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
     _MainTex ("Particle Texture", 2D) = "white" {}
     _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
     _Glow("Glow", Float) = 0
 }

     
     // ---- Fragment program cards
     SubShader {
         Pass {
         
         Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent" }
         Blend SrcAlpha One
         AlphaTest Greater .01
         ColorMask RGB
         Cull Off Lighting Off ZWrite Off
         
            CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma fragmentoption ARB_precision_hint_fastest
             #pragma multi_compile_particles
 
             #include "UnityCG.cginc"
 
             sampler2D _MainTex;
             fixed4 _TintColor;
             float _Glow;
             struct appdata_t {
                 float4 vertex : POSITION;
                 fixed4 color : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f {
                 float4 vertex : POSITION;
                 fixed4 color : COLOR;
                 float2 texcoord : TEXCOORD0;
                 #ifdef SOFTPARTICLES_ON
                 float4 projPos : TEXCOORD1;
                 #endif
             };
             
             float4 _MainTex_ST;
 
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 #ifdef SOFTPARTICLES_ON
                 o.projPos = ComputeScreenPos (o.vertex);
                 COMPUTE_EYEDEPTH(o.projPos.z);
                 #endif
                 o.color = v.color;
                 o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                 return o;
             }
 
             sampler2D _CameraDepthTexture;
             float _InvFade;
             
             fixed4 frag (v2f i) : COLOR
             {
                 #ifdef SOFTPARTICLES_ON
                 float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
                 float partZ = i.projPos.z;
                 float fade = saturate (_InvFade * (sceneZ-partZ));
                 i.color.a *= fade;
                 #endif
                 
                 return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord) * _Glow;
             }
             ENDCG 
         }
         
     Pass
    {
        Tags { "Queue"="AlphaTest" "LightMode"="ShadowCaster"}
        ZWrite On 
        ZTest Less

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_shadowcaster
        #include "UnityCG.cginc"
        
        sampler2D _MainTex;
        
        struct appdata
        {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float2 uv : TEXCOORD0;
        };

        struct v2f {
            V2F_SHADOW_CASTER;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata v)
        {
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            // do stuff to worldPos.xyz
            v.vertex = mul(unity_WorldToObject, worldPos);
            v2f o;
            o.uv = v.uv;
            TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
            return o;
        }

        float4 frag(v2f i) : SV_Target
        {
            fixed4 color = tex2D(_MainTex, i.uv);
            clip(color.a - 0.01);
            SHADOW_CASTER_FRAGMENT(i)
        }
        ENDCG
    }
 }     
}
