// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/Custom/TerrainEngine/BillboardTree" {
    Properties{
        _MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
    }

    SubShader{
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TreeBillboard" }

        Pass {
            ColorMask rgb
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off Cull Off

            CGPROGRAM
            #pragma vertex billboard
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "CustomTreeLibrary.cginc"
            #include "CustomTerrainEngine.cginc"

            sampler2D _MainTex;
            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, input.uv);
                col.rgb *= input.color.rgb;
                //col.rgb *= UNITY_ACCESS_INSTANCED_PROP(UnityTerrain, _TreeInstanceColor);
                clip(col.a);
                UNITY_APPLY_FOG(input.fogCoord, col);
                return col;
            }
            ENDCG
        }

    }

    Fallback Off
}