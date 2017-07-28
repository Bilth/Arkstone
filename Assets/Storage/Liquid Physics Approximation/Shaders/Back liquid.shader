// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:0,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2946,x:34718,y:32849,varname:node_2946,prsc:2|emission-4048-OUT,alpha-8360-OUT,clip-6303-OUT;n:type:ShaderForge.SFN_Color,id:6193,x:33537,y:32601,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Rotator,id:1711,x:32123,y:33154,varname:node_1711,prsc:2|UVIN-19-OUT,PIV-4058-OUT,ANG-8870-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8392,x:32596,y:33022,varname:node_8392,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-6535-UVOUT;n:type:ShaderForge.SFN_FragmentPosition,id:4455,x:31195,y:32960,varname:node_4455,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:19,x:31666,y:32972,varname:node_19,prsc:2,cc1:2,cc2:1,cc3:-1,cc4:-1|IN-2906-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:7440,x:31195,y:32836,varname:node_7440,prsc:2;n:type:ShaderForge.SFN_Append,id:8664,x:32282,y:33154,varname:node_8664,prsc:2|A-629-OUT,B-1711-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:629,x:31666,y:33119,varname:node_629,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-2906-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3303,x:32439,y:33154,varname:node_3303,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8664-OUT;n:type:ShaderForge.SFN_Rotator,id:6535,x:32439,y:33023,varname:node_6535,prsc:2|UVIN-3303-OUT,PIV-4058-OUT,ANG-8945-OUT;n:type:ShaderForge.SFN_Add,id:8870,x:31966,y:33154,varname:node_8870,prsc:2|A-5652-OUT,B-315-OUT;n:type:ShaderForge.SFN_Vector1,id:5652,x:31809,y:33119,varname:node_5652,prsc:2,v1:1.55;n:type:ShaderForge.SFN_ValueProperty,id:8945,x:32282,y:33072,ptovrint:False,ptlb:RotationX,ptin:_RotationX,varname:_RotationX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:315,x:31809,y:33198,ptovrint:False,ptlb:RotationZ,ptin:_RotationZ,varname:_RotationZ,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Fresnel,id:3405,x:33537,y:32749,varname:node_3405,prsc:2|EXP-1871-OUT;n:type:ShaderForge.SFN_Multiply,id:5474,x:33849,y:32749,varname:node_5474,prsc:2|A-3405-OUT,B-6193-RGB;n:type:ShaderForge.SFN_OneMinus,id:7318,x:33694,y:32669,varname:node_7318,prsc:2|IN-3405-OUT;n:type:ShaderForge.SFN_Multiply,id:5114,x:33849,y:32595,varname:node_5114,prsc:2|A-7465-OUT,B-7318-OUT;n:type:ShaderForge.SFN_Subtract,id:7465,x:33694,y:32525,varname:node_7465,prsc:2|A-6193-RGB,B-6858-OUT;n:type:ShaderForge.SFN_Add,id:4091,x:33998,y:32662,varname:node_4091,prsc:2|A-5114-OUT,B-5474-OUT;n:type:ShaderForge.SFN_Vector1,id:1871,x:33376,y:32749,varname:node_1871,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Subtract,id:2456,x:31359,y:32911,varname:node_2456,prsc:2|A-7440-XYZ,B-4455-XYZ;n:type:ShaderForge.SFN_Blend,id:1754,x:32753,y:33102,varname:node_1754,prsc:2,blmd:8,clmp:False|SRC-8392-OUT,DST-8141-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8141,x:32596,y:33198,ptovrint:False,ptlb:Amount,ptin:_Amount,varname:_Amount,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Append,id:6858,x:33537,y:32433,varname:node_6858,prsc:2|A-4719-OUT,B-4719-OUT,C-4719-OUT;n:type:ShaderForge.SFN_Vector1,id:4719,x:33371,y:32452,varname:node_4719,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Tex2d,id:3616,x:32979,y:32797,ptovrint:False,ptlb:StaticAnimationSlicePattern,ptin:_StaticAnimationSlicePattern,varname:_StaticAnimationSlicePattern,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False|UVIN-8683-UVOUT;n:type:ShaderForge.SFN_Panner,id:8683,x:32823,y:32797,varname:node_8683,prsc:2,spu:-1,spv:0|UVIN-9494-OUT,DIST-4924-OUT;n:type:ShaderForge.SFN_Append,id:9494,x:32908,y:32379,varname:node_9494,prsc:2|A-2037-OUT,B-9792-OUT;n:type:ShaderForge.SFN_Multiply,id:2037,x:32752,y:32319,varname:node_2037,prsc:2|A-8696-OUT,B-8625-OUT;n:type:ShaderForge.SFN_Multiply,id:4924,x:32667,y:32797,varname:node_4924,prsc:2|A-8815-T,B-9102-OUT;n:type:ShaderForge.SFN_Time,id:8815,x:32514,y:32722,varname:node_8815,prsc:2;n:type:ShaderForge.SFN_SwitchProperty,id:8357,x:33684,y:32986,ptovrint:False,ptlb:EnableStaticAnimation,ptin:_EnableStaticAnimation,varname:_EnableStaticAnimation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-4855-OUT,B-3616-R;n:type:ShaderForge.SFN_Slider,id:1497,x:32436,y:32564,ptovrint:False,ptlb:StaticAnimationSliceHeight,ptin:_StaticAnimationSliceHeight,varname:_StaticAnimationSliceHeight,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:0.2,max:0.5;n:type:ShaderForge.SFN_Slider,id:8696,x:32436,y:32285,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:_Tiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:0.1,max:10;n:type:ShaderForge.SFN_Vector1,id:4855,x:33528,y:32986,varname:node_4855,prsc:2,v1:0;n:type:ShaderForge.SFN_Blend,id:9792,x:32751,y:32451,varname:node_9792,prsc:2,blmd:3,clmp:False|SRC-2776-OUT,DST-1497-OUT;n:type:ShaderForge.SFN_Blend,id:375,x:33995,y:32986,varname:node_375,prsc:2,blmd:6,clmp:False|SRC-2855-OUT,DST-1754-OUT;n:type:ShaderForge.SFN_Tex2d,id:9712,x:33998,y:32502,ptovrint:False,ptlb:Gradient,ptin:_Gradient,varname:_Gradient,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4048,x:34313,y:32662,varname:node_4048,prsc:2|A-9712-R,B-2833-OUT,C-8360-OUT;n:type:ShaderForge.SFN_Multiply,id:2855,x:33842,y:32986,varname:node_2855,prsc:2|A-8357-OUT,B-1497-OUT;n:type:ShaderForge.SFN_Vector4Property,id:6674,x:31195,y:33118,ptovrint:False,ptlb:Pivot,ptin:_Pivot,varname:_Pivot,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Append,id:5567,x:31359,y:33118,varname:node_5567,prsc:2|A-6674-X,B-6674-Y,C-6674-Z;n:type:ShaderForge.SFN_Blend,id:2906,x:31514,y:33044,varname:node_2906,prsc:2,blmd:6,clmp:False|SRC-2456-OUT,DST-5567-OUT;n:type:ShaderForge.SFN_Vector2,id:4058,x:31195,y:32744,varname:node_4058,prsc:2,v1:0,v2:0;n:type:ShaderForge.SFN_ValueProperty,id:483,x:34154,y:33317,ptovrint:False,ptlb:SliceAngleCurvature,ptin:_SliceAngleCurvature,varname:_SliceAngleCurvature,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:5520,x:32906,y:32998,varname:node_5520,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-2906-OUT;n:type:ShaderForge.SFN_Rotator,id:6180,x:33062,y:33071,varname:node_6180,prsc:2|UVIN-5520-OUT,PIV-4058-OUT,ANG-8991-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7220,x:32906,y:33457,ptovrint:False,ptlb:RotationW,ptin:_RotationW,varname:_RotationW,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:8625,x:33218,y:33071,varname:node_8625,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-6180-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:2776,x:32514,y:32401,varname:node_2776,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-2906-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8360,x:34313,y:32957,ptovrint:False,ptlb:Trasparency,ptin:_Trasparency,varname:_Opacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:8988,x:32906,y:33296,varname:node_8988,prsc:2|A-7220-OUT,B-5652-OUT;n:type:ShaderForge.SFN_Negate,id:8991,x:32906,y:33154,varname:node_8991,prsc:2|IN-8988-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9102,x:32514,y:32872,ptovrint:False,ptlb:StaticAnimationSliceSpeed,ptin:_StaticAnimationSliceSpeed,varname:_StaticAnimationSliceSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Blend,id:2408,x:34154,y:33135,varname:node_2408,prsc:2,blmd:14,clmp:False|SRC-375-OUT,DST-7627-OUT;n:type:ShaderForge.SFN_Multiply,id:812,x:33684,y:33236,varname:node_812,prsc:2|A-1541-OUT,B-1541-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5359,x:33837,y:33236,varname:node_5359,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-812-OUT;n:type:ShaderForge.SFN_Add,id:7627,x:33995,y:33236,varname:node_7627,prsc:2|A-5359-R,B-5359-G;n:type:ShaderForge.SFN_Append,id:1541,x:33526,y:33236,varname:node_1541,prsc:2|A-1002-OUT,B-5807-OUT;n:type:ShaderForge.SFN_Lerp,id:6303,x:34313,y:33135,varname:node_6303,prsc:2|A-375-OUT,B-2408-OUT,T-483-OUT;n:type:ShaderForge.SFN_Blend,id:1002,x:33372,y:33165,varname:node_1002,prsc:2,blmd:7,clmp:False|SRC-2899-OUT,DST-8625-OUT;n:type:ShaderForge.SFN_ObjectScale,id:4047,x:33062,y:33264,varname:node_4047,prsc:2,rcp:False;n:type:ShaderForge.SFN_OneMinus,id:2899,x:33218,y:33264,varname:node_2899,prsc:2|IN-4047-X;n:type:ShaderForge.SFN_Blend,id:5807,x:33372,y:33332,varname:node_5807,prsc:2,blmd:3,clmp:False|SRC-1754-OUT,DST-1602-OUT;n:type:ShaderForge.SFN_Vector1,id:1602,x:33218,y:33436,varname:node_1602,prsc:2,v1:-0.2;n:type:ShaderForge.SFN_Blend,id:2833,x:34155,y:32662,varname:node_2833,prsc:2,blmd:5,clmp:False|SRC-4091-OUT,DST-6193-RGB;proporder:6193-8360-9712-8357-8141-3616-8696-1497-9102-483-6674-8945-315-7220;pass:END;sub:END;*/

Shader "Liquid Physics Approximation/Back liquid" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Trasparency ("Trasparency", Float ) = 0
        _Gradient ("Gradient", 2D) = "white" {}
        [MaterialToggle] _EnableStaticAnimation ("EnableStaticAnimation", Float ) = 0
        _Amount ("Amount", Float ) = 0.5
        _StaticAnimationSlicePattern ("StaticAnimationSlicePattern", 2D) = "white" {}
        _Tiling ("Tiling", Range(0.1, 10)) = 0.1
        _StaticAnimationSliceHeight ("StaticAnimationSliceHeight", Range(0.1, 0.5)) = 0.2
        _StaticAnimationSliceSpeed ("StaticAnimationSliceSpeed", Float ) = 0
        _SliceAngleCurvature ("SliceAngleCurvature", Float ) = 0
        _Pivot ("Pivot", Vector) = (0,0,0,0)
        _RotationX ("RotationX", Float ) = 0
        _RotationZ ("RotationZ", Float ) = 0
        _RotationW ("RotationW", Float ) = 0
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _RotationX;
            uniform float _RotationZ;
            uniform float _Amount;
            uniform sampler2D _StaticAnimationSlicePattern; uniform float4 _StaticAnimationSlicePattern_ST;
            uniform fixed _EnableStaticAnimation;
            uniform float _StaticAnimationSliceHeight;
            uniform float _Tiling;
            uniform sampler2D _Gradient; uniform float4 _Gradient_ST;
            uniform float4 _Pivot;
            uniform float _SliceAngleCurvature;
            uniform float _RotationW;
            uniform float _Trasparency;
            uniform float _StaticAnimationSliceSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_8815 = _Time + _TimeEditor;
                float node_5652 = 1.55;
                float2 node_4058 = float2(0,0);
                float node_6180_ang = (-1*(_RotationW+node_5652));
                float node_6180_spd = 1.0;
                float node_6180_cos = cos(node_6180_spd*node_6180_ang);
                float node_6180_sin = sin(node_6180_spd*node_6180_ang);
                float2 node_6180_piv = node_4058;
                float3 node_2906 = (1.0-(1.0-(objPos.rgb-i.posWorld.rgb))*(1.0-float3(_Pivot.r,_Pivot.g,_Pivot.b)));
                float2 node_6180 = (mul(node_2906.rb-node_6180_piv,float2x2( node_6180_cos, -node_6180_sin, node_6180_sin, node_6180_cos))+node_6180_piv);
                float node_8625 = node_6180.r;
                float2 node_8683 = (float2((_Tiling*node_8625),(node_2906.g+_StaticAnimationSliceHeight-1.0))+(node_8815.g*_StaticAnimationSliceSpeed)*float2(-1,0));
                float4 _StaticAnimationSlicePattern_var = tex2D(_StaticAnimationSlicePattern,TRANSFORM_TEX(node_8683, _StaticAnimationSlicePattern));
                float node_6535_ang = _RotationX;
                float node_6535_spd = 1.0;
                float node_6535_cos = cos(node_6535_spd*node_6535_ang);
                float node_6535_sin = sin(node_6535_spd*node_6535_ang);
                float2 node_6535_piv = node_4058;
                float node_1711_ang = (node_5652+_RotationZ);
                float node_1711_spd = 1.0;
                float node_1711_cos = cos(node_1711_spd*node_1711_ang);
                float node_1711_sin = sin(node_1711_spd*node_1711_ang);
                float2 node_1711_piv = node_4058;
                float2 node_1711 = (mul(node_2906.bg-node_1711_piv,float2x2( node_1711_cos, -node_1711_sin, node_1711_sin, node_1711_cos))+node_1711_piv);
                float2 node_6535 = (mul(float3(node_2906.r,node_1711).rg-node_6535_piv,float2x2( node_6535_cos, -node_6535_sin, node_6535_sin, node_6535_cos))+node_6535_piv);
                float node_1754 = (node_6535.g+_Amount);
                float node_375 = (1.0-(1.0-(lerp( 0.0, _StaticAnimationSlicePattern_var.r, _EnableStaticAnimation )*_StaticAnimationSliceHeight))*(1.0-node_1754));
                float2 node_1541 = float2((node_8625/(1.0-(1.0 - objScale.r))),(node_1754+(-0.2)-1.0));
                float2 node_5359 = (node_1541*node_1541).rg;
                clip(lerp(node_375,( node_375 > 0.5 ? ((node_5359.r+node_5359.g) + 2.0*node_375 -1.0) : ((node_5359.r+node_5359.g) + 2.0*(node_375-0.5))),_SliceAngleCurvature) - 0.5);
////// Lighting:
////// Emissive:
                float4 _Gradient_var = tex2D(_Gradient,TRANSFORM_TEX(i.uv0, _Gradient));
                float node_4719 = 0.3;
                float node_3405 = pow(1.0-max(0,dot(normalDirection, viewDirection)),0.7);
                float3 emissive = (_Gradient_var.r*max((((_Color.rgb-float3(node_4719,node_4719,node_4719))*(1.0 - node_3405))+(node_3405*_Color.rgb)),_Color.rgb)*_Trasparency);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_Trasparency);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _RotationX;
            uniform float _RotationZ;
            uniform float _Amount;
            uniform sampler2D _StaticAnimationSlicePattern; uniform float4 _StaticAnimationSlicePattern_ST;
            uniform fixed _EnableStaticAnimation;
            uniform float _StaticAnimationSliceHeight;
            uniform float _Tiling;
            uniform float4 _Pivot;
            uniform float _SliceAngleCurvature;
            uniform float _RotationW;
            uniform float _StaticAnimationSliceSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                float4 node_8815 = _Time + _TimeEditor;
                float node_5652 = 1.55;
                float2 node_4058 = float2(0,0);
                float node_6180_ang = (-1*(_RotationW+node_5652));
                float node_6180_spd = 1.0;
                float node_6180_cos = cos(node_6180_spd*node_6180_ang);
                float node_6180_sin = sin(node_6180_spd*node_6180_ang);
                float2 node_6180_piv = node_4058;
                float3 node_2906 = (1.0-(1.0-(objPos.rgb-i.posWorld.rgb))*(1.0-float3(_Pivot.r,_Pivot.g,_Pivot.b)));
                float2 node_6180 = (mul(node_2906.rb-node_6180_piv,float2x2( node_6180_cos, -node_6180_sin, node_6180_sin, node_6180_cos))+node_6180_piv);
                float node_8625 = node_6180.r;
                float2 node_8683 = (float2((_Tiling*node_8625),(node_2906.g+_StaticAnimationSliceHeight-1.0))+(node_8815.g*_StaticAnimationSliceSpeed)*float2(-1,0));
                float4 _StaticAnimationSlicePattern_var = tex2D(_StaticAnimationSlicePattern,TRANSFORM_TEX(node_8683, _StaticAnimationSlicePattern));
                float node_6535_ang = _RotationX;
                float node_6535_spd = 1.0;
                float node_6535_cos = cos(node_6535_spd*node_6535_ang);
                float node_6535_sin = sin(node_6535_spd*node_6535_ang);
                float2 node_6535_piv = node_4058;
                float node_1711_ang = (node_5652+_RotationZ);
                float node_1711_spd = 1.0;
                float node_1711_cos = cos(node_1711_spd*node_1711_ang);
                float node_1711_sin = sin(node_1711_spd*node_1711_ang);
                float2 node_1711_piv = node_4058;
                float2 node_1711 = (mul(node_2906.bg-node_1711_piv,float2x2( node_1711_cos, -node_1711_sin, node_1711_sin, node_1711_cos))+node_1711_piv);
                float2 node_6535 = (mul(float3(node_2906.r,node_1711).rg-node_6535_piv,float2x2( node_6535_cos, -node_6535_sin, node_6535_sin, node_6535_cos))+node_6535_piv);
                float node_1754 = (node_6535.g+_Amount);
                float node_375 = (1.0-(1.0-(lerp( 0.0, _StaticAnimationSlicePattern_var.r, _EnableStaticAnimation )*_StaticAnimationSliceHeight))*(1.0-node_1754));
                float2 node_1541 = float2((node_8625/(1.0-(1.0 - objScale.r))),(node_1754+(-0.2)-1.0));
                float2 node_5359 = (node_1541*node_1541).rg;
                clip(lerp(node_375,( node_375 > 0.5 ? ((node_5359.r+node_5359.g) + 2.0*node_375 -1.0) : ((node_5359.r+node_5359.g) + 2.0*(node_375-0.5))),_SliceAngleCurvature) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
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
            #include "UnityCG.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Gradient; uniform float4 _Gradient_ST;
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
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                
                float4 _Gradient_var = tex2D(_Gradient,TRANSFORM_TEX(i.uv0, _Gradient));
                float node_4719 = 0.3;
                float node_3405 = pow(1.0-max(0,dot(normalDirection, viewDirection)),0.7);
                o.Emission = (_Gradient_var.r*max((((_Color.rgb-float3(node_4719,node_4719,node_4719))*(1.0 - node_3405))+(node_3405*_Color.rgb)),_Color.rgb)*_Trasparency);
                
                float3 diffColor = float3(0,0,0);
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
