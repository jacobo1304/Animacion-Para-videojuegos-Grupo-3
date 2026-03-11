Shader "Custom/StealthVignette"
{
    Properties
    {
        _Intensity  ("Intensity",  Range(0, 1))   = 0
        _Color      ("Vignette Color", Color)      = (0, 0, 0, 1)
        _Radius     ("Inner Radius",  Range(0.1, 1.0)) = 0.35
        _Softness   ("Softness",      Range(0.01, 1.0)) = 0.45
    }

    SubShader
    {
        Tags
        {
            "Queue"          = "Overlay"
            "IgnoreProjector"= "True"
            "RenderType"     = "Transparent"
            "PreviewType"    = "Plane"
        }

        Cull     Off
        Lighting Off
        ZWrite   Off
        ZTest    [unity_GUIZTestMode]
        Blend    SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            float  _Intensity;
            fixed4 _Color;
            float  _Radius;
            float  _Softness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Center UV at (0,0), range [-0.5, 0.5]
                float2 uv = i.texcoord - 0.5;

                // Stretch horizontally to compensate for non-square screens
                uv.x *= _ScreenParams.x / _ScreenParams.y;

                float dist     = length(uv);
                float vignette = smoothstep(_Radius, _Radius + _Softness, dist);

                fixed4 col = _Color;
                col.a = vignette * _Intensity;
                return col;
            }
            ENDCG
        }
    }
}
