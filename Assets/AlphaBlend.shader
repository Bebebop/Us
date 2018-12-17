// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaBlend" {
    Properties {
        _Color ("Main Tint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AlphaScale("Alpha Scale",Range(0,1))=1
    }
    SubShader {
    /*
    "Queue"="Transparent"：Unity中透明度测试使用的渲染队列是名为AlphaTest的队列
    "IgnoreProjectors"="True"：意味着这个Shader不会受到投影器的影响
    RenderType标签可以让Unity把这个Shader归入到提前定义的组（这里就是Transparent组）
    */
    Tags{"Queue"="Transparent""IgnoreProjectors"="True""RenderType"="Transparent"}
        Pass
        {
        Tags{"LightMode"="ForwardBase"}
        Cull Front
        ZWrite On//开启深度写入
        ColorMask 0//当ColorMask设为0时，意味着该pass不写任何颜色通道，即不会输出任何颜色。这正是我们需要的-该Pass只需要写入深度缓冲即可
        
        }
        Pass
        {
        Tags{"LightMode"="ForwardBase"}
        Cull Back
        ZWrite Off//关闭深度写入
        Blend  SrcAlpha OneMinusSrcAlpha
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include"Lighting.cginc"

        fixed4 _Color;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        fixed _AlphaScale;

        struct a2v
        {
        float4 vertex:POSITION;
        float3 normal:NORMAL;
        float4 texcoord:TEXCOORD0;
        };
        struct v2f
        {
        float4 pos:SV_POSITION;
        float3 worldNormal:TEXCOORD0;
        float3 worldPos:TEXCOORD1;
        float2 uv:TEXCOORD2;
        };
        v2f vert(a2v v)
        {
        v2f o;
        o.pos=UnityObjectToClipPos(v.vertex);
        o.worldNormal=UnityObjectToWorldNormal(v.normal);
        o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
        o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);
        return o;
        }

        fixed4 frag(v2f i):SV_Target
        {
        fixed3 worldNormal=normalize(i.worldNormal);
        fixed3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
        fixed4 texColor=tex2D(_MainTex,i.uv);
        fixed3 albedo=texColor.rgb*_Color.rgb;
        fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
        fixed3 diffuse=_LightColor0.rgb*albedo*max(0,dot(worldNormal,worldLightDir));
        return fixed4(ambient+diffuse,texColor.a*_AlphaScale);//颜色乘以透明程度
        }

        ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
