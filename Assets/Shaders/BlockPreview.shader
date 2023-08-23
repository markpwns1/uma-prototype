Shader "Custom/Block Preview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Placement ("Placement", Range(0,1)) = 0
        _Valid ("Valid", Range(0,1)) = 0
        _Alpha ("Alpha", Range(0,1)) = 1
        _ValidColour ("Valid Colour", Color) = (1,1,1,1)
        _InvalidColour ("Invalid Colour", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Geometry" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard noshadow alpha:fade


        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float _Placement;
        float _Valid;
        float _Alpha;
        fixed3 _ValidColour;
        fixed3 _InvalidColour;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base texture colour
            fixed4 texCol = tex2D (_MainTex, IN.uv_MainTex);
            // Green or red depending on placement validity
            float3 placementCol = lerp(_InvalidColour, _ValidColour, _Valid);
            // If in placement mode, lerp between the two colours
            fixed3 c = lerp(texCol, placementCol, min(0.5, _Placement));
            o.Emission = c.rgb * _Placement;
            o.Albedo = c.rgb;
            o.Alpha = lerp(texCol.a, _Alpha, _Placement);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
