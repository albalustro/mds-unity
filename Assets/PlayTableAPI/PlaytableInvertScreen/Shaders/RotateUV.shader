// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/RotateUVs" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Rotation ("Rotation", float) = 0.0
		_X ("X", float) = 0.5
        _Y ("Y",  float) = 0.5
		_Scale ("Scale", float) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		//ZWrite Off

		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				struct vertexInput {
					float4 texcoord : TEXCOORD0;
					float4 vertex : POSITION;
				};
				
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv_MainTex : TEXCOORD0;
				};
			
				float4 _MainTex_ST;
				float _Rotation;
				float _X;
				float _Y;
				float _Scale;
			
				v2f vert(vertexInput v) {
					v2f o;

					float4x4 modelMatrix = unity_ObjectToWorld;
					float4x4 modelMatrixInverse = unity_WorldToObject;  
					
					float2 texc = v.texcoord.xy;
					texc.x = texc.x - _X;
					texc.y = texc.y - _Y;

					float RotTime = _Rotation;
					float s = sin ( RotTime );
					float c = cos ( RotTime );
 
					float2x2 rotationMatrix = float2x2( c, -s, s, c);
                       
					rotationMatrix = rotationMatrix * 0.5;
					rotationMatrix = rotationMatrix + 0.5;
					rotationMatrix = rotationMatrix * 2 -1;
					//rotationMatrix = rotationMatrix * (2 - _Scale);

					texc = mul(texc * _Scale, rotationMatrix );
					texc.x = texc.x + _X;
					texc.y = texc.y + _Y;
					
					o.uv_MainTex = texc;

					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}
			
				sampler2D _MainTex;
			
				float4 frag(v2f IN) : COLOR {
					half4 c = tex2D (_MainTex, float2(IN.uv_MainTex));
					return c;
				}
			ENDCG
		}
	}
}