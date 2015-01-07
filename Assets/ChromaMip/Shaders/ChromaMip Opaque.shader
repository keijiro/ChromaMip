Shader "ChromaMip/Opaque"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half3 YCbCrtoRGB(half y, half cb, half cr)
    {
        return half3(
            y                 + 1.402    * cr,
            y - 0.344136 * cb - 0.714136 * cr,
            y + 1.772    * cb
        );
    }

    half4 frag(v2f_img i) : SV_Target 
    {
        float2 uv = i.uv;

#if 0
        half y  = tex2Dlod(_MainTex, float4(uv, 0, 0)).a;
        half cb = tex2Dlod(_MainTex, float4(uv, 0, 2)).a - 0.5;
        half cr = tex2Dlod(_MainTex, float4(uv, 0, 1)).a - 0.5;
#else
        half y  = tex2Dbias(_MainTex, float4(uv, 0, 0)).a;
        half cb = tex2Dbias(_MainTex, float4(uv, 0, 2)).a - 0.5;
        half cr = tex2Dbias(_MainTex, float4(uv, 0, 1)).a - 0.5;
#endif

        return half4(YCbCrtoRGB(y, cb, cr), 1);
    }

    ENDCG

    SubShader
    {
        Tags { "Queue"="Geometry" }
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
