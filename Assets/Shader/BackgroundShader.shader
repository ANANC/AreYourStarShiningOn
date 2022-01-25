Shader "Custom/BackgroundShader"
{
	Properties
	{
		_BaseColor("BaseColor", Color) = (1,1,1,1)
		_AppendColor("AppendColor", Color) = (1,1,1,1)
		_NoiseTexture("NoiseTexture", 2D) = "white" {}

		_Enable_0("Enable_0",Float) = 1
		_Color_0("Color_0", Color) = (1,1,1,1)
		_Position_0("Position_0",Vector) = (0,0,0,0)
		_Radius_0("Radius_0",Float) = 1

		_Enable_1("Enable_1",Float) = 1
		_Color_1("Color_1", Color) = (1,1,1,1)
		_Position_1("Position_1",Vector) = (0,0,0,0)
		_Radius_1("Radius_1",Float) = 1

		_Enable_2("Enable_2",Float) = 1
		_Color_2("Color_2", Color) = (1,1,1,1)
		_Position_2("Position_2",Vector) = (0,0,0,0)
		_Radius_2("Radius_2",Float) = 1

		_Enable_3("Enable_3",Float) = 1
		_Color_3("Color_3", Color) = (1,1,1,1)
		_Position_3("Position_3",Vector) = (0,0,0,0)
		_Radius_3("Radius_3",Float) = 0

		_Enable_4("Enable_4",Float) = 1
		_Color_4("Color_4", Color) = (1,1,1,1)
		_Position_4("Position_4",Vector) = (0,0,0,0)
		_Radius_4("Radius_4",Float) = 1
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _BaseColor;
			fixed4 _AppendColor;
			sampler2D _NoiseTexture;

			float _Enable_0;
			fixed4 _Color_0;
			float4 _Position_0;
			float _Radius_0;

			float _Enable_1;
			fixed4 _Color_1;
			float4 _Position_1;
			float _Radius_1;

			float _Enable_2;
			fixed4 _Color_2;
			float4 _Position_2;
			float _Radius_2;

			float _Enable_3;
			fixed4 _Color_3;
			float4 _Position_3;
			float _Radius_3;
			
			float _Enable_4;
			fixed4 _Color_4;
			float4 _Position_4;
			float _Radius_4;

			struct a2v {
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			fixed4 DrawCircle(fixed2 screenUV, float4 position, float radius, float enable)
			{
				float stepValue = 0.4f;
				float radiusBreatheValue = 0.12f;
				float noiseSpeed = 0.02f;
				float randomMaxRance = 0.2f;

				//噪音图取样
				fixed4 noiseUV = fixed4(position.x + _SinTime.x, position.y + _CosTime.x, 0, 0) * noiseSpeed;
				fixed4 noiseColor = tex2D(_NoiseTexture, noiseUV);

				//超出距离
				float excessX = (step(noiseColor.x - randomMaxRance, 0) * -1 + 1) * (noiseColor.x - randomMaxRance);
				float excessY = (step(noiseColor.y - randomMaxRance, 0) * -1 + 1) * (noiseColor.y - randomMaxRance);
				//位置规范
				fixed4 randomPosition = fixed4(position.x + noiseColor.x - excessX, position.y + noiseColor.y - excessY, 0, 0);

				//uv<->随机位置 的距离
				float positionLength = length(screenUV - randomPosition);

				//呼吸半径
				float breathRadius = radius - radiusBreatheValue * radius * abs(_SinTime.w + noiseColor.b);

				//阴影
				fixed4 circleShader = step(positionLength, breathRadius);//得到圆
				circleShader *= smoothstep(breathRadius, breathRadius - stepValue, positionLength);//得到柔和边缘
				circleShader *= enable;//开关

				return circleShader;
			}


			fixed4 frag(v2f i) :SV_Target
			{
				fixed2 screenUV = i.uv;

				fixed4 _c0 = DrawCircle(screenUV,_Position_0, _Radius_0, _Enable_0);

				fixed4 _c1 = DrawCircle(screenUV, _Position_1, _Radius_1, _Enable_1);

				fixed4 _c2 = DrawCircle(screenUV, _Position_2, _Radius_2, _Enable_2);

				fixed4 _c3 = DrawCircle(screenUV, _Position_3, _Radius_3, _Enable_3);

				fixed4 _c4 = DrawCircle(screenUV, _Position_4, _Radius_4, _Enable_4);

				fixed4 circleShader = _c0 + _c1 + _c2 + _c3 + _c4;
				fixed4 circleColor = _c0 * _Color_0 + _c1 * _Color_1 + _c2 * _Color_2 + _c3 * _Color_3 + _c4 * _Color_4;

				fixed4 color = _BaseColor * (1 - circleShader) + circleColor + _AppendColor;

				return color;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
