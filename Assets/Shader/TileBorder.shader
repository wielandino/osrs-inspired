Shader "Custom/TileBorder"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0, 1, 0, 1)  // Deine grüne Farbe
        _BorderColor ("Border Color", Color) = (0.5, 0.5, 0.5, 1)
        _BorderWidth ("Border Width", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        sampler2D _MainTex;
        float4 _Color;
        float4 _BorderColor;
        float _BorderWidth;
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;
            
            // Prüfe ob wir am Rand sind
            if (uv.x < _BorderWidth || uv.x > 1.0 - _BorderWidth ||
                uv.y < _BorderWidth || uv.y > 1.0 - _BorderWidth)
            {
                o.Albedo = _BorderColor.rgb;
            }
            else
            {
                // Multipliziere Textur mit Farbe (wie Unity Standard Shader)
                o.Albedo = tex2D(_MainTex, uv).rgb * _Color.rgb;
            }
        }
        ENDCG
    }
}