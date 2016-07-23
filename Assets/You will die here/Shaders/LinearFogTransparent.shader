Shader "Custom/LinearFogTransparent" {
  Properties {
  	_alphaValue ("Alpha Value", Range (-1, 1)) = 0
	_ColorT ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _AltTex ("Base (RGB)", 2D) = "white" {}
    // _AltValue ("Alt Value", Range (0, 1)) = 0
  }
  SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "False" }
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Back
    ZTest Less
    Offset -1, 10
    LOD 200

    CGPROGRAM
    #pragma surface surf Lambert finalcolor:mycolor vertex:myvert alpha:fade
    #pragma multi_compile_fog

    sampler2D _MainTex;
    sampler2D _AltTex;
    fixed4 _ColorT;
	float4 _Origin;
    uniform half4 unity_FogStart;
    uniform half4 unity_FogEnd;
    fixed Alpha;
    fixed _alphaValue;
    fixed _AltValue;

    struct Input {
      float2 uv_MainTex;
      half fog;
    };

    void myvert (inout appdata_full v, out Input data) {
      UNITY_INITIALIZE_OUTPUT(Input,data);
      float3 worldPos = mul (_Object2World, v.vertex).xyz;
      float dist = distance(worldPos.xyz, _Origin.xyz);
      float diff = unity_FogEnd.x - unity_FogStart.x;
      float invDiff = 1.0f / diff;
      dist = clamp (dist, 0.0, unity_FogEnd.x);
      data.fog = clamp ((unity_FogEnd.x-dist) * invDiff, 0.0, 1.0);
    }

    void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
      #ifdef UNITY_PASS_FORWARDADD
      UNITY_APPLY_FOG_COLOR(IN.fog, color, float4(0,0,0,0));
      #else
      UNITY_APPLY_FOG_COLOR(IN.fog, color, unity_FogColor);
      #endif
    }

    void surf (Input IN, inout SurfaceOutput o) {
      fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
      fixed4 texTwoCol = tex2D(_AltTex, IN.uv_MainTex);
      fixed4 c = lerp(mainCol, texTwoCol, _AltValue) * _ColorT;

      //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _ColorT;
      o.Albedo = c.rgb;
      o.Alpha = clamp(c.a-(IN.fog*_alphaValue),0,1);
    }

    ENDCG

  } 
  FallBack "Diffuse"
}