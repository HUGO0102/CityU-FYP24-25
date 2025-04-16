#ifndef BEAUTIFY_BLUR_FX
#define BEAUTIFY_BLUR_FX

	// Copyright 2022 Kronnect - All Rights Reserved.

    #include "BeautifyCommon.hlsl"

    struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
        float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
    };
  
    VaryingsCross VertBlur(Attributes input) {
        VaryingsCross output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

        BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(output);

        return output;
    }


   float4 FragBlur (VaryingsCross input): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        //i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(input)

        #if defined(FIRST_BLUR_PASS)
        input.uv *= _InputScale.xy;
        uv1 *= _InputScale.xy;
        uv2 *= _InputScale.xy;
        uv3 *= _InputScale.xy;
        uv4 *= _InputScale.xy;
        #endif

        float4 pixel0 = SAMPLE_TEXTURE2D_X(_InputTex, sampler_LinearClamp, input.uv);
        float4 pixel1 = SAMPLE_TEXTURE2D_X(_InputTex, sampler_LinearClamp, uv1);
        float4 pixel2 = SAMPLE_TEXTURE2D_X(_InputTex, sampler_LinearClamp, uv2);
        float4 pixel3 = SAMPLE_TEXTURE2D_X(_InputTex, sampler_LinearClamp, uv3);
        float4 pixel4 = SAMPLE_TEXTURE2D_X(_InputTex, sampler_LinearClamp, uv4);

        const float w0 = 0.2270270270;
        const float w1 = 0.3162162162;
        const float w2 = 0.0702702703;

        return pixel0 * w0 + (pixel1 + pixel2) * w1 + (pixel3 + pixel4) * w2;
    }   



#endif // BEAUTIFY_BLUR_FX


