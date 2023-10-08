#version 330 core
layout (location = 0) out vec4 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedo;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

const float nearDepth = 0.1f;
const float farDepth = 50.0f;

void main()
{    
    // store the fragment position vector in the first gbuffer texture
    float linearDepth = (2.0 * nearDepth * farDepth) / (nearDepth + farDepth - (gl_FragCoord.z *2.0 - 1.0) * (farDepth-nearDepth));
	gPosition = vec4(FragPos, linearDepth);
    // also store the per-fragment normals into the gbuffer
    gNormal = normalize(Normal);
    // and the diffuse per-fragment color
    gAlbedo.rgb = vec3(0.95);
}