Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width (pixels)", Range(1, 20)) = 5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;   // Unity 自动提供像素尺寸
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 main = tex2D(_MainTex, IN.texcoord);
                float alpha = main.a;

                // 内部像素（不透明度 > 0.5），直接返回原图（保持抗锯齿）
                if (alpha > 0.5)
                {
                    return main * IN.color;
                }

                // 外部或半透明像素：搜索最近的不透明像素
                float2 texelSize = _MainTex_TexelSize.xy;
                const int MAX_WIDTH = 20;          // 与 Range 上限一致
                const int DIR_COUNT = 8;
                float2 dirs[DIR_COUNT] = {
                    float2(1,0), float2(0,1), float2(-1,0), float2(0,-1),
                    float2(0.707,0.707), float2(-0.707,0.707),
                    float2(0.707,-0.707), float2(-0.707,-0.707)
                };

                int nearestDist = -1;
                [loop]
                for (int r = 1; r <= MAX_WIDTH; r++)
                {
                    if (r > _OutlineWidth) break;
                    bool found = false;
                    for (int d = 0; d < DIR_COUNT; d++)
                    {
                        float2 offset = dirs[d] * r * texelSize;
                        float sampleAlpha = tex2Dlod(_MainTex, float4(IN.texcoord + offset, 0, 0)).a;
                        if (sampleAlpha > 0.5)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        nearestDist = r;
                        break;
                    }
                }

                if (nearestDist < 0)
                    return fixed4(0, 0, 0, 0);

                // 描边强度：距离边缘越近越强（1→0 渐变）
                float intensity = 1.0 - (nearestDist - 1.0) / _OutlineWidth;
                fixed4 outline = _OutlineColor;
                outline.a *= intensity;
                return outline;
            }
            ENDCG
        }
    }
}