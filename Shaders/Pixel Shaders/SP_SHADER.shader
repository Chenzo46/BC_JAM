Shader"Custom/PixelPerfectSmoothing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue"="Overlay"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
float2 _MainTex_TexelSize;

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

float4 texturePointSmooth(sampler2D smp, float2 uv, float2 pixel_size)
{
                // Automatic texture sampling with mipmaps
    return tex2D(smp, uv);
}

float4 frag(v2f i) : SV_Target
{
    return texturePointSmooth(_MainTex, i.uv, _MainTex_TexelSize);
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
