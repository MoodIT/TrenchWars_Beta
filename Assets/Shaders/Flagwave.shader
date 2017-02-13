// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

// Original shader by cboe - Mar, 23, 2009
// Enhanced to 3 axis movement by Seon - Jan, 21, 2010
// Added _WaveSpeed - Jan, 26, 2010
//
// Requirements: assumes you are using a subdivided plane created with X (width) * Z (height) where Y is flat.
// Requirements: assumes UV as: left X (U0) is attatched to pole, and Top Z (V1) is at top of pole.  
//
// Enjoy!
 
Shader "Selfmade/FlagWave"
{
 
Properties
{
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" { }
    _WaveSpeed ("Wave Speed", Range(0.0, 150.0)) = 50.0
}
 
SubShader
{
    Pass
    {
       CULL Off
       
      CGPROGRAM
      #pragma vertex vert 
      #pragma fragment frag 
      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
       
      float4 _Color;
      sampler2D _MainTex;
      float _WaveSpeed;
 
      // vertex input: position, normal
      struct appdata {
          float4 vertex : POSITION;
          float4 texcoord : TEXCOORD0;
      };
       
      struct v2f {
          float4 pos : POSITION;
          float2 uv: TEXCOORD0;
      };
       
      v2f vert (appdata v) {
          v2f o;
 
        float sinOff=v.vertex.x+v.vertex.y+v.vertex.z;
        float t=-_Time*_WaveSpeed;
        float fx=v.texcoord.x;
        float fy=v.texcoord.x*v.texcoord.y;
 
        v.vertex.x+=sin(t*1.45+sinOff)*fx*0.5;
//        v.vertex.y=sin(t*3.12+sinOff)*fx*0.5-fy*2;
//        v.vertex.z-=sin(t*2.2+sinOff)*fx*0.2;
   
          o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
          o.uv = v.texcoord;
 
         return o;
      }
       
      float4 frag (v2f i) : COLOR
      {
         half4 color = tex2D(_MainTex, i.uv) * _Color;
         return color;
      }
 
      ENDCG
 
      SetTexture [_MainTex] {combine texture}
    }
//    Pass {
//            Name "ShadowCaster"
//            Tags { "LightMode" = "ShadowCaster" }
//           
//            Fog {Mode Off}
//            ZWrite On ZTest Less Cull Off
//            Offset [_ShadowBias], [_ShadowBiasSlope]
//   
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma multi_compile SHADOWS_NATIVE SHADOWS_CUBE
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #include "UnityCG.cginc"
//            #include "TerrainEngine.cginc"
//           
//            struct v2f {
//                V2F_SHADOW_CASTER;
//            };
//           
//            struct appdata {
//                float4 vertex : POSITION;
//                float4 color : COLOR;
//            };
//            v2f vert( appdata v )
//            {
//                v2f o;
//                TerrainAnimateTree(v.vertex, v.color.w);
//                TRANSFER_SHADOW_CASTER(o)
//                return o;
//            }
//           
//            float4 frag( v2f i ) : COLOR
//            {
//                SHADOW_CASTER_FRAGMENT(i)
//            }
//            ENDCG  
//        }
//        Pass 
//         {
//             Name "ShadowCollector"
//             Tags { "LightMode" = "ShadowCollector" }
//        
//             Fog {Mode Off}
//             ZWrite On ZTest Less
//             
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma fragmentoption ARB_precision_hint_fastest
//             #pragma multi_compile_shadowcollector
// 
//             #define SHADOW_COLLECTOR_PASS
//             #include "UnityCG.cginc"
// 
//             uniform float _Scale;
//             
//             struct appdata 
//             {
//                 float4 vertex : POSITION;
//             };
// 
//             struct v2f 
//             {
//                 V2F_SHADOW_COLLECTOR;
//             };
// 
//             v2f vert (appdata v)
//             {
//                 v2f o;
//                 v.vertex.xyz *= _Scale;
//                 TRANSFER_SHADOW_COLLECTOR(o)
//                 return o;
//             }
// 
//             fixed4 frag (v2f i) : COLOR
//             {
//                 SHADOW_COLLECTOR_FRAGMENT(i)
//             }
//             ENDCG
// 
//         }
}
   Fallback "VertexLit"
}