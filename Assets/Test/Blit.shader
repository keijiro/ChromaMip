Shader "Custom/Blit"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Cutoff("Cutoff", Float) = 0.5
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float _Cutoff;

    half4 frag(v2f_img i) : SV_Target 
    {
        float2 uv = i.uv;// + _MainTex_TexelSize.xy * 0.5;
        half4 c = tex2D(_MainTex, uv);
        clip(c.a - _Cutoff);
        return c;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma glsl
            ENDCG
        }
    } 
}
