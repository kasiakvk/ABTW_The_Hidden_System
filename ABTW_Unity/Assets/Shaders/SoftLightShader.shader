Shader "ABTW/SoftLightShader"
{
    // ============================================================
    // ABTW Soft Light Shader
    // Style: painterly, soft, no harsh shadows, subtle transitions
    // Used for: Academy walls, floors, atmospheric surfaces
    // ============================================================

    Properties
    {
        _MainTex        ("Base Texture", 2D)           = "white" {}
        _Color          ("Base Color", Color)           = (1, 1, 1, 1)
        _EmissionColor  ("Emission Color", Color)       = (0, 0, 0, 0)
        _EmissionStrength ("Emission Strength", Range(0, 3)) = 0.0

        // Soft light controls
        _SoftnessFactor ("Softness Factor", Range(0, 1))    = 0.6
        _ShadowSoftness ("Shadow Softness", Range(0, 1))    = 0.8
        _AmbientBoost   ("Ambient Boost", Range(0, 1))      = 0.3

        // Perception / Glitch effect
        _GlitchIntensity ("Glitch Intensity", Range(0, 1))  = 0.0
        _GlitchSpeed     ("Glitch Speed", Range(0, 10))     = 3.0
        _GlitchColor     ("Glitch Color", Color)            = (0.4, 0.4, 1, 1)

        // Activation glow
        _ActivationGlow  ("Activation Glow", Range(0, 1))   = 0.0
        _ActivationColor ("Activation Color", Color)        = (1, 0.95, 0.7, 1)

        // Alpha
        _Alpha           ("Alpha", Range(0, 1))             = 1.0

        // Fog integration
        _FogBlend        ("Fog Blend", Range(0, 1))         = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue"      = "Transparent"
        }

        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 color    : COLOR;
                UNITY_FOG_COORDS(3)
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _Color;
            float4    _EmissionColor;
            float     _EmissionStrength;
            float     _SoftnessFactor;
            float     _ShadowSoftness;
            float     _AmbientBoost;
            float     _GlitchIntensity;
            float     _GlitchSpeed;
            float4    _GlitchColor;
            float     _ActivationGlow;
            float4    _ActivationColor;
            float     _Alpha;
            float     _FogBlend;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos         = UnityObjectToClipPos(v.vertex);
                o.uv          = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos    = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color       = v.color;
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            // ─── Soft Lambert Lighting ───────────────────────────────────
            float3 SoftLambert(float3 normal, float3 lightDir, float softness)
            {
                float NdotL = dot(normalize(normal), normalize(lightDir));
                // Wrap lighting — softer transition from light to shadow
                float wrapped = (NdotL + softness) / (1.0 + softness);
                return saturate(wrapped) * _LightColor0.rgb;
            }

            // ─── Glitch Effect ───────────────────────────────────────────
            float2 GlitchUV(float2 uv, float intensity, float speed)
            {
                if (intensity <= 0.01) return uv;

                float time = _Time.y * speed;
                float scanLine = floor(uv.y * 20.0) / 20.0;
                float noise = frac(sin(scanLine * 127.3 + time) * 43758.5);

                // Only glitch on some scan lines
                float glitchMask = step(0.85, noise) * intensity;
                uv.x += (frac(sin(time * 3.7 + scanLine) * 1000.0) - 0.5) * glitchMask * 0.05;

                return uv;
            }

            // ─── Activation Pulse ────────────────────────────────────────
            float ActivationPulse(float3 worldPos, float glow)
            {
                if (glow <= 0.01) return 0.0;
                float pulse = sin(_Time.y * 2.0) * 0.5 + 0.5;
                return glow * pulse * 0.4;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // ─── Base texture with glitch UV ─────────────────────────
                float2 glitchedUV = GlitchUV(i.uv, _GlitchIntensity, _GlitchSpeed);
                fixed4 texColor = tex2D(_MainTex, glitchedUV) * _Color * i.color;

                // ─── Soft lighting ───────────────────────────────────────
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir    = normalize(_WorldSpaceLightPos0.xyz);

                float3 softLight = SoftLambert(worldNormal, lightDir, _SoftnessFactor);

                // Ambient boost — ABTW never has pure black shadows
                float3 ambient = ShadeSH9(float4(worldNormal, 1.0));
                ambient = max(ambient, _AmbientBoost);

                float3 lighting = softLight + ambient;
                lighting = lerp(lighting, float3(1,1,1), _ShadowSoftness * 0.3);

                // ─── Apply lighting ──────────────────────────────────────
                float3 finalColor = texColor.rgb * lighting;

                // ─── Emission ────────────────────────────────────────────
                float3 emission = _EmissionColor.rgb * _EmissionStrength;
                finalColor += emission;

                // ─── Glitch color overlay ────────────────────────────────
                if (_GlitchIntensity > 0.01)
                {
                    float glitchFlicker = sin(_Time.y * _GlitchSpeed * 6.28) * 0.5 + 0.5;
                    float glitchMask = step(0.7, glitchFlicker) * _GlitchIntensity;
                    finalColor = lerp(finalColor, _GlitchColor.rgb, glitchMask * 0.4);
                }

                // ─── Activation glow ─────────────────────────────────────
                float activationPulse = ActivationPulse(i.worldPos, _ActivationGlow);
                finalColor = lerp(finalColor, _ActivationColor.rgb,
                    activationPulse + _ActivationGlow * 0.2);

                // ─── Alpha ───────────────────────────────────────────────
                float finalAlpha = texColor.a * _Alpha;

                fixed4 result = fixed4(finalColor, finalAlpha);

                // ─── Fog ─────────────────────────────────────────────────
                UNITY_APPLY_FOG(i.fogCoord, result);

                return result;
            }
            ENDCG
        }
    }

    // Fallback for older hardware
    FallBack "Diffuse"

    CustomEditor "ABTWSoftLightShaderGUI"
}