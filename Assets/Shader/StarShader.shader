Shader "Custom/PieceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"				//渲染顺序 AlphaTest = 2450
			"IgnoreProjector" = "Ture"			//无视投影器
			"RenderType" = "Transparent"		//类型：半透明
		}

		Pass
		{
			ZWrite Off							//关掉深度测试
			Blend SrcAlpha OneMinusSrcAlpha		//设置源颜色（该片元着色器产生的颜色）的混合因子设为ScrAlpha, 设置目标颜色（已经存在于颜色缓冲重的颜色）的混合因子设为OneMinusScrAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct a2v {
				float4 vertex:POSITION;
				float4 texcoord:TEXCOORD0;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				half2 uv:TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				float blurSize = 0.01f;
				float texelSize = 0.2f;

				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

				return o;
			}

			fixed4 frag(v2f i) :SV_Target
			{
				fixed4 uvColor = tex2D(_MainTex, i.uv);
				fixed4 color = uvColor * _Color;
			//	color.a = uvColor.a;
				return color;
			}

			ENDCG
		}
    }
    FallBack "Diffuse"
}
