Shader "Hidden/Shader/Beautify"
{
    Properties {
    	_FrostTex ("Frost RGBA", 2D) = "white" {}
	    _FrostNormals ("Frost Normals RGBA", 2D) = "bump" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    ENDHLSL


    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass // 0
        {
            Name "Beautify Uber Pass"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragBeautify
                #pragma multi_compile_local _ BEAUTIFY_VIGNETTING_MASK
                #pragma multi_compile_local _ BEAUTIFY_LUT BEAUTIFY_LUT3D
                #pragma multi_compile_local _ BEAUTIFY_FROZEN
                #pragma multi_compile_local _ BEAUTIFY_DEPTH_OF_FIELD
                #pragma multi_compile_local _ BEAUTIFY_TONEMAP_ACES
                #include "BeautifyCore.hlsl"
            ENDHLSL
        }

        Pass // 1
        {
            Name "Copy Exact"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragCopyExact
                #include "BeautifyCommon.hlsl"
            ENDHLSL
        }

        Pass // 2
        {
            Name "DoF CoC"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragCoC
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 3
        {
            Name "DoF Blur Horizontally"
            HLSLPROGRAM
                #pragma vertex VertBlur
                #pragma fragment FragBlurCoC
                #define BEAUTIFY_BLUR_HORIZ
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 4
        {
            Name "DoF Blur Vertically"
            HLSLPROGRAM
                #pragma vertex VertBlur
                #pragma fragment FragBlurCoC
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 5
        {
            Name "DoF Blur"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragBlur
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 6
        {
            Name "DoF Blur No Bokeh"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragBlurNoBokeh
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 7
        {
            Name "DoF Bokeh Threshold"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragThreshold
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 8
        {
            Name "Copy Additive"
            Blend One One
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragCopyBokeh
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 9
        {
            Name "DoF Blur Bokeh"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragBlurSeparateBokeh
                #include "BeautifyDoF.hlsl"
            ENDHLSL
        }

        Pass // 10
        {
            Name "Compare"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragCompare
                #include "BeautifyCore.hlsl"
            ENDHLSL
        }

        Pass // 11
        {
            Name "Blur Horizontal"
            HLSLPROGRAM
                #pragma vertex VertBlur
                #pragma fragment FragBlur
                #define BEAUTIFY_BLUR_HORIZ
                #include "BeautifyBlur.hlsl"
            ENDHLSL
        }

        Pass // 12
        {
            Name "Blur Vertical"
            HLSLPROGRAM
                #pragma vertex VertBlur
                #pragma fragment FragBlur
                #include "BeautifyBlur.hlsl"
            ENDHLSL
        }

        Pass // 13
        {
            Name "Blur Horizontal First Pass"
            HLSLPROGRAM
                #pragma vertex VertBlur
                #pragma fragment FragBlur
                #define BEAUTIFY_BLUR_HORIZ
                #define FIRST_BLUR_PASS
                #include "BeautifyBlur.hlsl"
            ENDHLSL
        }

        Pass // 14
        {
            Name "Copy"
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragCopy
                #include "BeautifyCommon.hlsl"
            ENDHLSL
        }
    }
    Fallback Off
}
