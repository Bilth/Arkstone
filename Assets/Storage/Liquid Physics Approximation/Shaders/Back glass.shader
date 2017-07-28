// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:1,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:True,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3730,x:34228,y:33060,varname:node_3730,prsc:2|diff-5535-OUT,spec-6931-OUT,gloss-2432-OUT,emission-6156-OUT,alpha-4248-OUT;n:type:ShaderForge.SFN_Fresnel,id:8614,x:32814,y:33033,varname:node_8614,prsc:2|EXP-2165-OUT;n:type:ShaderForge.SFN_Color,id:9482,x:32814,y:32886,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_9482,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:2165,x:32499,y:33060,ptovrint:False,ptlb:FresnelOpacity,ptin:_FresnelOpacity,varname:node_2165,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:5;n:type:ShaderForge.SFN_Tex2d,id:5333,x:32499,y:33161,ptovrint:False,ptlb:MainPattern,ptin:_MainPattern,varname:node_5333,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-5235-OUT;n:type:ShaderForge.SFN_Multiply,id:5235,x:32343,y:33161,varname:node_5235,prsc:2|A-5141-UVOUT,B-6517-OUT;n:type:ShaderForge.SFN_TexCoord,id:5141,x:32159,y:33092,varname:node_5141,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:2140,x:33132,y:32962,varname:node_2140,prsc:2,v1:0;n:type:ShaderForge.SFN_OneMinus,id:9402,x:32656,y:33161,varname:node_9402,prsc:2|IN-5333-R;n:type:ShaderForge.SFN_Multiply,id:174,x:33135,y:33452,varname:node_174,prsc:2|A-9482-RGB,B-9402-OUT;n:type:ShaderForge.SFN_Blend,id:6503,x:33446,y:33505,varname:node_6503,prsc:2,blmd:6,clmp:False|SRC-4915-OUT,DST-8009-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6517,x:32159,y:33262,ptovrint:False,ptlb:MainPatternTiling,ptin:_MainPatternTiling,varname:node_6517,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:8009,x:33290,y:33505,varname:node_8009,prsc:2|A-174-OUT,B-1989-OUT;n:type:ShaderForge.SFN_Tex2d,id:5741,x:32973,y:33498,ptovrint:False,ptlb:TranslucencyPattern,ptin:_TranslucencyPattern,varname:node_5741,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5162-OUT;n:type:ShaderForge.SFN_Time,id:8187,x:32021,y:33488,varname:node_8187,prsc:2;n:type:ShaderForge.SFN_Vector1,id:6302,x:32973,y:33661,varname:node_6302,prsc:2,v1:0.999;n:type:ShaderForge.SFN_Step,id:1989,x:33135,y:33579,varname:node_1989,prsc:2|A-5741-R,B-6302-OUT;n:type:ShaderForge.SFN_Add,id:6457,x:32656,y:33440,varname:node_6457,prsc:2|A-1683-R,B-6861-OUT;n:type:ShaderForge.SFN_Multiply,id:6861,x:32343,y:33438,varname:node_6861,prsc:2|A-5399-OUT,B-8187-TDB;n:type:ShaderForge.SFN_Append,id:5162,x:32814,y:33498,varname:node_5162,prsc:2|A-6457-OUT,B-5106-OUT;n:type:ShaderForge.SFN_Multiply,id:9572,x:32343,y:33565,varname:node_9572,prsc:2|A-8187-TDB,B-7705-OUT;n:type:ShaderForge.SFN_Add,id:5106,x:32656,y:33567,varname:node_5106,prsc:2|A-1683-G,B-9572-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1683,x:32499,y:33487,varname:node_1683,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8463-OUT;n:type:ShaderForge.SFN_Multiply,id:8463,x:32343,y:33294,varname:node_8463,prsc:2|A-5141-UVOUT,B-1349-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1349,x:32159,y:33351,ptovrint:False,ptlb:TranslucencyPatternTiling,ptin:_TranslucencyPatternTiling,varname:node_1349,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:3689,x:33603,y:33505,ptovrint:False,ptlb:AddTranslucencyPattern,ptin:_AddTranslucencyPattern,varname:node_3689,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-4915-OUT,B-6503-OUT;n:type:ShaderForge.SFN_Blend,id:4915,x:32973,y:32962,varname:node_4915,prsc:2,blmd:1,clmp:False|SRC-9482-RGB,DST-8614-OUT;n:type:ShaderForge.SFN_Add,id:5305,x:33603,y:33340,varname:node_5305,prsc:2|A-8614-OUT,B-729-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:729,x:33446,y:33340,ptovrint:False,ptlb:AddedFresnel,ptin:_AddedFresnel,varname:node_729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2140-OUT,B-9402-OUT;n:type:ShaderForge.SFN_Slider,id:7607,x:32657,y:32741,ptovrint:False,ptlb:Reflectivity,ptin:_Reflectivity,varname:node_7607,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:0.5,max:1;n:type:ShaderForge.SFN_OneMinus,id:1623,x:32973,y:32741,varname:node_1623,prsc:2|IN-7607-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5399,x:32159,y:33472,ptovrint:False,ptlb:TranslucencyPatternSpeedX,ptin:_TranslucencyPatternSpeedX,varname:node_5399,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7705,x:32159,y:33599,ptovrint:False,ptlb:TranslucencyPatternSpeedY,ptin:_TranslucencyPatternSpeedY,varname:node_7705,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4248,x:33759,y:33340,varname:node_4248,prsc:2|A-5305-OUT,B-3659-OUT;n:type:ShaderForge.SFN_Multiply,id:6156,x:33759,y:33505,varname:node_6156,prsc:2|A-3689-OUT,B-3659-OUT;n:type:ShaderForge.SFN_Multiply,id:5535,x:33759,y:32946,varname:node_5535,prsc:2|A-9482-RGB,B-3659-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3659,x:33759,y:32858,ptovrint:False,ptlb:Trasparency,ptin:_Trasparency,varname:node_3659,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector1,id:4140,x:33446,y:33214,varname:node_4140,prsc:2,v1:100;n:type:ShaderForge.SFN_Multiply,id:6931,x:33601,y:33119,varname:node_6931,prsc:2|A-9814-OUT,B-4140-OUT;n:type:ShaderForge.SFN_Subtract,id:8569,x:33288,y:33057,varname:node_8569,prsc:2|A-697-OUT,B-3659-OUT;n:type:ShaderForge.SFN_Vector1,id:697,x:33132,y:33057,varname:node_697,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:2432,x:33759,y:33119,varname:node_2432,prsc:2|A-6931-OUT,B-1623-OUT,T-3659-OUT;n:type:ShaderForge.SFN_If,id:9814,x:33446,y:33057,varname:node_9814,prsc:2|A-3659-OUT,B-2140-OUT,GT-2140-OUT,EQ-8569-OUT,LT-8569-OUT;proporder:9482-3659-2165-729-5333-6517-3689-5741-1349-7607-5399-7705;pass:END;sub:END;*/

Shader "Liquid Physics Approximation/Back glass" {
    Properties {
        _Color ("Color", Color) = (1,0,0,1)
        _Trasparency ("Trasparency", Float ) = 1
        _FresnelOpacity ("FresnelOpacity", Range(0, 5)) = 3
        [MaterialToggle] _AddedFresnel ("AddedFresnel", Float ) = 0
        _MainPattern ("MainPattern", 2D) = "black" {}
        _MainPatternTiling ("MainPatternTiling", Float ) = 1
        [MaterialToggle] _AddTranslucencyPattern ("AddTranslucencyPattern", Float ) = 1
        _TranslucencyPattern ("TranslucencyPattern", 2D) = "white" {}
        _TranslucencyPatternTiling ("TranslucencyPatternTiling", Float ) = 1
        _Reflectivity ("Reflectivity", Range(0.1, 1)) = 0.5
        _TranslucencyPatternSpeedX ("TranslucencyPatternSpeedX", Float ) = 0
        _TranslucencyPatternSpeedY ("TranslucencyPatternSpeedY", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _FresnelOpacity;
            uniform sampler2D _MainPattern; uniform float4 _MainPattern_ST;
            uniform float _MainPatternTiling;
            uniform sampler2D _TranslucencyPattern; uniform float4 _TranslucencyPattern_ST;
            uniform float _TranslucencyPatternTiling;
            uniform fixed _AddTranslucencyPattern;
            uniform fixed _AddedFresnel;
            uniform float _Reflectivity;
            uniform float _TranslucencyPatternSpeedX;
            uniform float _TranslucencyPatternSpeedY;
            uniform float _Trasparency;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                UNITY_FOG_COORDS(7)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD8;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float node_2140 = 0.0;
                float node_9814_if_leA = step(_Trasparency,node_2140);
                float node_9814_if_leB = step(node_2140,_Trasparency);
                float node_8569 = (1.0-_Trasparency);
                float node_6931 = (lerp((node_9814_if_leA*node_8569)+(node_9814_if_leB*node_2140),node_8569,node_9814_if_leA*node_9814_if_leB)*100.0);
                float gloss = 1.0 - lerp(node_6931,(1.0 - _Reflectivity),_Trasparency); // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = node_6931;
                float specularMonochrome;
                float3 diffuseColor = (_Color.rgb*_Trasparency); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz)*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_8614 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelOpacity);
                float3 node_4915 = (_Color.rgb*node_8614);
                float2 node_5235 = (i.uv0*_MainPatternTiling);
                float4 _MainPattern_var = tex2D(_MainPattern,TRANSFORM_TEX(node_5235, _MainPattern));
                float node_9402 = (1.0 - _MainPattern_var.r);
                float2 node_1683 = (i.uv0*_TranslucencyPatternTiling).rg;
                float4 node_8187 = _Time + _TimeEditor;
                float2 node_5162 = float2((node_1683.r+(_TranslucencyPatternSpeedX*node_8187.b)),(node_1683.g+(node_8187.b*_TranslucencyPatternSpeedY)));
                float4 _TranslucencyPattern_var = tex2D(_TranslucencyPattern,TRANSFORM_TEX(node_5162, _TranslucencyPattern));
                float3 emissive = (lerp( node_4915, (1.0-(1.0-node_4915)*(1.0-((_Color.rgb*node_9402)*step(_TranslucencyPattern_var.r,0.999)))), _AddTranslucencyPattern )*_Trasparency);
/// Final Color:
                float3 finalColor = diffuse * ((node_8614+lerp( node_2140, node_9402, _AddedFresnel ))*_Trasparency) + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,((node_8614+lerp( node_2140, node_9402, _AddedFresnel ))*_Trasparency));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _FresnelOpacity;
            uniform sampler2D _MainPattern; uniform float4 _MainPattern_ST;
            uniform float _MainPatternTiling;
            uniform sampler2D _TranslucencyPattern; uniform float4 _TranslucencyPattern_ST;
            uniform float _TranslucencyPatternTiling;
            uniform fixed _AddTranslucencyPattern;
            uniform fixed _AddedFresnel;
            uniform float _Reflectivity;
            uniform float _TranslucencyPatternSpeedX;
            uniform float _TranslucencyPatternSpeedY;
            uniform float _Trasparency;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float node_2140 = 0.0;
                float node_9814_if_leA = step(_Trasparency,node_2140);
                float node_9814_if_leB = step(node_2140,_Trasparency);
                float node_8569 = (1.0-_Trasparency);
                float node_6931 = (lerp((node_9814_if_leA*node_8569)+(node_9814_if_leB*node_2140),node_8569,node_9814_if_leA*node_9814_if_leB)*100.0);
                float gloss = 1.0 - lerp(node_6931,(1.0 - _Reflectivity),_Trasparency); // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = node_6931;
                float specularMonochrome;
                float3 diffuseColor = (_Color.rgb*_Trasparency); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float node_8614 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelOpacity);
                float2 node_5235 = (i.uv0*_MainPatternTiling);
                float4 _MainPattern_var = tex2D(_MainPattern,TRANSFORM_TEX(node_5235, _MainPattern));
                float node_9402 = (1.0 - _MainPattern_var.r);
                float3 finalColor = diffuse * ((node_8614+lerp( node_2140, node_9402, _AddedFresnel ))*_Trasparency) + specular;
                fixed4 finalRGBA = fixed4(finalColor,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _FresnelOpacity;
            uniform sampler2D _MainPattern; uniform float4 _MainPattern_ST;
            uniform float _MainPatternTiling;
            uniform sampler2D _TranslucencyPattern; uniform float4 _TranslucencyPattern_ST;
            uniform float _TranslucencyPatternTiling;
            uniform fixed _AddTranslucencyPattern;
            uniform float _Reflectivity;
            uniform float _TranslucencyPatternSpeedX;
            uniform float _TranslucencyPatternSpeedY;
            uniform float _Trasparency;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_8614 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelOpacity);
                float3 node_4915 = (_Color.rgb*node_8614);
                float2 node_5235 = (i.uv0*_MainPatternTiling);
                float4 _MainPattern_var = tex2D(_MainPattern,TRANSFORM_TEX(node_5235, _MainPattern));
                float node_9402 = (1.0 - _MainPattern_var.r);
                float2 node_1683 = (i.uv0*_TranslucencyPatternTiling).rg;
                float4 node_8187 = _Time + _TimeEditor;
                float2 node_5162 = float2((node_1683.r+(_TranslucencyPatternSpeedX*node_8187.b)),(node_1683.g+(node_8187.b*_TranslucencyPatternSpeedY)));
                float4 _TranslucencyPattern_var = tex2D(_TranslucencyPattern,TRANSFORM_TEX(node_5162, _TranslucencyPattern));
                o.Emission = (lerp( node_4915, (1.0-(1.0-node_4915)*(1.0-((_Color.rgb*node_9402)*step(_TranslucencyPattern_var.r,0.999)))), _AddTranslucencyPattern )*_Trasparency);
                
                float3 diffColor = (_Color.rgb*_Trasparency);
                float specularMonochrome;
                float3 specColor;
                float node_2140 = 0.0;
                float node_9814_if_leA = step(_Trasparency,node_2140);
                float node_9814_if_leB = step(node_2140,_Trasparency);
                float node_8569 = (1.0-_Trasparency);
                float node_6931 = (lerp((node_9814_if_leA*node_8569)+(node_9814_if_leB*node_2140),node_8569,node_9814_if_leA*node_9814_if_leB)*100.0);
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, node_6931, specColor, specularMonochrome );
                float roughness = lerp(node_6931,(1.0 - _Reflectivity),_Trasparency);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
