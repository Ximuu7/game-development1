Shader "Custom/ScreenShake"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OffsetX ("Offset X", Range(-0.1, 0.1)) = 0
        _OffsetY ("Offset Y", Range(-0.1, 0.1)) = 0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _OffsetX;
            float _OffsetY;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x += _OffsetX;
                uv.y += _OffsetY;
                // 边缘处理：边缘像素重复（可选，也可以设为 clamp）
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}