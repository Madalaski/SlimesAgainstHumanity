// Normal Mapping for a Triplanar Shader - Ben Golus 2017
// GPU Gems 3 example shader

// Uses the GPU Gems 3 style normal map blend. Fastest method giving plausible results.

Shader "Triplanar/GPU Gems 3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            // flip UVs horizontally to correct for back side projection
            #define TRIPLANAR_CORRECT_PROJECTED_U

            // offset UVs to prevent obvious mirroring
            // #define TRIPLANAR_UV_OFFSET

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BumpMap;

            fixed4 _LightColor0;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // calculate triplanar blend
                half3 triblend = saturate(pow(i.worldNormal, 4));
                triblend /= max(dot(triblend, half3(1,1,1)), 0.0001);

                // preview blend
                // return fixed4(triblend.xyz, 1);

                // calculate triplanar uvs
                // applying texture scale and offset values ala TRANSFORM_TEX macro
                float2 uvX = i.worldPos.zy * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvY = i.worldPos.xz * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvZ = i.worldPos.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                // offset UVs to prevent obvious mirroring
            #if defined(TRIPLANAR_UV_OFFSET)
                uvY += 0.33;
                uvZ += 0.67;
            #endif

                // minor optimization of sign(). prevents return value of 0
                half3 axisSign = i.worldNormal < 0 ? -1 : 1;
                
                // flip UVs horizontally to correct for back side projection
            #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
                uvX.x *= axisSign.x;
                uvY.x *= axisSign.y;
                uvZ.x *= -axisSign.z;
            #endif

                // albedo textures
                fixed4 col = tex2D(_MainTex, i.uv);

                // tangent space normal maps
                half3 tnormalX = UnpackNormal(tex2D(_BumpMap, uvX));
                half3 tnormalY = UnpackNormal(tex2D(_BumpMap, uvY));
                half3 tnormalZ = UnpackNormal(tex2D(_BumpMap, uvZ));

                // flip normal maps' x axis to account for flipped UVs
            #if defined(TRIPLANAR_CORRECT_PROJECTED_U)
                tnormalX.x *= axisSign.x;
                tnormalY.x *= axisSign.y;
                tnormalZ.x *= -axisSign.z;
            #endif

                // swizzle tangent normal map to match world normals
                half3 normalX = half3(0.0, tnormalX.yx);
                half3 normalY = half3(tnormalY.x, 0.0, tnormalY.y);
                half3 normalZ = half3(tnormalZ.xy, 0.0);

                // blend normals and add to world normal
                half3 worldNormal = normalize(
                    normalX.xyz * triblend.x +
                    normalY.xyz * triblend.y +
                    normalZ.xyz * triblend.z +
                    i.worldNormal
                    );

                // preview world normals
                // return fixed4(worldNormal * 0.5 + 0.5, 1);

                // calculate lighting
                half ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 ambient = ShadeSH9(half4(worldNormal, 1));
                half3 lighting = _LightColor0.rgb * ndotl + ambient;

                // preview directional lighting
                // return fixed4(ndotl.xxx, 1);

                return fixed4(col.rgb * lighting, 1);
            }
            ENDCG
        }
    }
}
