Shader "Lowpolygunpack/ReflexSight/Color"
{
    Properties
    {

        _MainTex ("Reticle Texture", 2D) = "black" {}
        [HDR] _ReticleColor ("Reticle Color", Color) = (4,0,0,1)
		_Vertical ("Vertical", float) = 0
		_ReticleSize ("Reticle Size", Range(0.0001,1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "ForceNoShadowCasting"="True" }
        LOD 200
		Blend OneMinusDstColor One

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert

        #pragma target 3.5

        sampler2D _MainTex;

        struct Input
        {
            float4 screenPos;
			float4 reticleScreenPos;
			float4 reticleTppScreenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EyepieceColor;
        fixed4 _ReticleColor;
		int _ViewDir;
		float _Vertical;
		float _ReticleSize;

		float luminance(fixed3 rgb){
			return 0.298912 * rgb.r + 0.586611 * rgb.g + 0.114478 * rgb.b;
		}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float scaleZ = length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z));
				o.reticleScreenPos = ComputeScreenPos(UnityObjectToClipPos(float4(0, _Vertical/scaleZ, 10000/scaleZ, 1)));
				o.reticleTppScreenPos = ComputeScreenPos(UnityObjectToClipPos(float4(0, (_Vertical+10000)/scaleZ, 10000/scaleZ, 1)));
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float2 normReticleScreenPos = IN.reticleScreenPos.xy / (IN.reticleScreenPos.w + 0.0000001); 
			float2 normReticleTopScreenPos = IN.reticleTppScreenPos.xy / (IN.reticleTppScreenPos.w + 0.0000001); 
			float2 normScreenPos = IN.screenPos.xy / (IN.screenPos.w + 0.0000001);
			float aspect = _ScreenParams.x / _ScreenParams.y;

		#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
			aspect = aspect / scaleOffset.x / scaleOffset.y;
		#endif

			float2 up = (normReticleTopScreenPos - normReticleScreenPos);
			up.x *= aspect;
			float distance = length(up);
			float reticleCos =  -(up.x / distance);
			float reticleSin = up.y / distance;
			float2 reticleUV = (normScreenPos - normReticleScreenPos);
			reticleUV.x *= aspect;
			reticleUV = fixed2(reticleUV.x * reticleCos - reticleUV.y * reticleSin, reticleUV.x * reticleSin + reticleUV.y * reticleCos)/_ReticleSize + 0.5;
            float4 reticleCol = tex2D (_MainTex, reticleUV) * float4(_ReticleColor.rgb, 1);
			reticleCol *= step(0, reticleUV.x) * step(reticleUV.x, 1);
			reticleCol *= step(0, reticleUV.y) * step(reticleUV.y, 1);

			float z = (UNITY_NEAR_CLIP_VALUE < 0) ? -IN.reticleScreenPos.z : IN.reticleScreenPos.z;

            o.Albedo = _Color.rgb * step(0, z) + saturate(reticleCol.rgb * reticleCol.a + _EyepieceColor.a) * step(z, 0);
			o.Alpha = _Color.a * step(0, z) + saturate(luminance(reticleCol.rgb) * reticleCol.a + _EyepieceColor.a) * step(z, 0);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Emission = reticleCol.rgb * reticleCol.a * step(z, 0);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
