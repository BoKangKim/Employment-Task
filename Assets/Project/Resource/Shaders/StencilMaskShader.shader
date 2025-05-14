Shader "Custom/StencilMaskShader"
{
    SubShader
    {
        Tags {
            "Queue" = "Geometry-1"
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            ColorMask 0
            ZWrite Off
            ZTest Always
        }
    }
}