Shader "ABTW/PerceptionShader"
{
    // ABTW Perception Shader
    // Activates soft desaturation + subtle distortion when Perception Mode is active.
    // The world does not break — it becomes more visible.

    Properties
    {
        _MainTex        ("Albedo (RGB)", 2D)          = "white" {}
        _Color          ("Base Color", Color)          = (1,1,1,1)
        _EmissionColor  ("Emission Color", Color)      = (0,0,0,0)
        _EmissionTex    ("Emission Map", 2D)           = "black" {}

        // Perception properties
        _PerceptionStrength ("Perception Strength", Range(0,1)) = 0.0
        _DesaturationAmount ("Desaturation", Range(0,1))        = 0.0
        _DistortionAmount   ("Distortion", Range(0,0.05))       = 0.0
        _DistortionSpeed    ("Distortion Speed", Range(0,2))    = 0.5
        _GlowIntensity      ("Glow Intensity", Range(0,2))      = 0.0
        _GlowColor          ("Glow Color", Color)               = (0.6,0.7,1,1)

        // Soft light
        _SoftLightBlend ("Soft Light Blend", Range(0,1)) = 0.3
        _RimPower       ("Rim Power", Range(0.5,8))      = 3.0
        _RimColor       ("Rim Color", Color)             = (0.5,0.6,0.9,0.3)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _EmissionTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            float3 worldPos;
        };

        fixed4  _Color;
        fixed4  _EmissionColor;
        half    _PerceptionStrength;
        half    _DesaturationAmount;
        half    _DistortionAmount;
        half    _DistortionSpeed;
        half    _GlowIntensity;
        fixed4  _GlowColor;
        half    _SoftLightBlend;
        half    _RimPower;
        fixed4  _RimColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Distortion UV offset (subtle, perception-reactive)
            float2 uv = IN.uv_MainTex;
            float distort = sin(_Time.y * _DistortionSpeed + uv.y * 10.0) * _DistortionAmount * _PerceptionStrength;
            uv.x += distort;

            fixed4 c = tex2D(_MainTex, uv) * _Color;

            // Desaturation (world becomes more visible, not less)
            float lum = dot(c.rgb, float3(0.299, 0.587, 0.114));
            float desat = _DesaturationAmount * _PerceptionStrength;
            c.rgb = lerp(c.rgb, float3(lum, lum, lum), desat);

            // Soft light blend
            c.rgb = lerp(c.rgb, c.rgb * 1.2, _SoftLightBlend);

            o.Albedo = c.rgb;
            o.Alpha  = c.a;

            // Rim glow (perception awareness)
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
            fixed3 rimGlow = _RimColor.rgb * pow(rim, _RimPower) * _PerceptionStrength;

            // Emission: base + glow + rim
            fixed4 emitTex = tex2D(_EmissionTex, IN.uv_MainTex);
            fixed3 emission = _EmissionColor.rgb + emitTex.rgb;
            emission += _GlowColor.rgb * _GlowIntensity * _PerceptionStrength;
            emission += rimGlow;

            o.Emission   = emission;
            o.Smoothness = 0.3;
            o.Metallic   = 0.0;
        }
        ENDCG
    }

    FallBack "Diffuse"
}