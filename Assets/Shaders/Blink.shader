Shader "Custom/Blink"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OpenValue ("Open Value", Range(0,1)) = 0
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
            float _OpenValue;

            fixed4 frag(v2f_img i) : SV_Target
            {
                // 0：全黑
                if (_OpenValue <= 0.01) return fixed4(0,0,0,0);
                // 1：完全清晰
                if (_OpenValue >= 0.99) return tex2D(_MainTex, i.uv);
                
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 中心化 UV [-1,1]
                float2 uv = i.uv * 2.0 - 1.0;
                
                // 椭圆半径：初始横向很大（左右超出），纵向极小（一条缝）
                // 结束横向和纵向都远大于屏幕（确保完全无遮罩）
                float minRx = 2.5;    // 初始横向半径（>1，左右超出屏幕）
                float maxRx = 3.0;    // 结束横向半径（远大于屏幕）
                float minRy = 0.05;   // 初始纵向极扁
                float maxRy = 3.0;    // 结束纵向半径
                
                float rx = lerp(minRx, maxRx, _OpenValue);
                float ry = lerp(minRy, maxRy, _OpenValue);
                
                // 椭圆距离场
                float ellipseDist = (uv.x * uv.x) / (rx * rx) + (uv.y * uv.y) / (ry * ry);
                
                // 动态模糊范围：开始模糊强（宽过渡带），结束模糊弱（几乎没有）
                float blurStrength = lerp(1.2, 0.0, _OpenValue);
                float inner = 0.4 - blurStrength * 0.3;   // 初始约0.04，结束约0.4
                float outer = 0.4 + blurStrength * 0.6;   // 初始约1.12，结束约0.4
                
                // 计算 alpha： ellipseDist 小于 inner 则全可见，大于 outer 则全黑，中间平滑
                float alpha = 1.0 - smoothstep(inner, outer, ellipseDist);
                
                // 混合黑色背景
                col.rgb = col.rgb * alpha;
                
                return col;
            }
            ENDCG
        }
    }
}