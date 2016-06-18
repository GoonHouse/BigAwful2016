Shader "AlphaMask" {
Properties {
_alphaValue ("Alpha Value", Range (0, 1)) = 1
_MainTex ("Base (RGB)", 2D) = "white" {}
_AlphaTex ("Alpha mask (R)", 2D) = "white" {}

}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
LOD 100

ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha

Pass {  
    CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct appdata_t {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            half2 texcoord : TEXCOORD0;
        };

        sampler2D _MainTex;
        sampler2D _AlphaTex;
        float _alphaValue;

        float4 _MainTex_ST;

        v2f vert (appdata_t v)
        {
            v2f o;
            o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
            o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.texcoord);
            fixed4 col2 = tex2D(_AlphaTex, i.texcoord);

            //Uncomment for only alpha of mask
            float alpha = col2.r+_alphaValue;
            return fixed4(col.r, col.g, col.b,alpha);
             //return fixed4(col.r, col.g, col.b,_alphaValue);
        }
    ENDCG
}
}
}