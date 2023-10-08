#version 330 core
out vec3 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform samplerCube cubemap;
uniform vec3 samples[64];

// parameters (you'd probably want to use them as uniforms to more easily tweak the effect)
int kernelSize = 64;
float radius = 0.5;

// tile noise texture over screen based on screen dimensions divided by noise size
const vec2 noiseScale = vec2(800.0/4.0, 600.0/4.0); 

uniform mat4 projection;
uniform mat4 view;
void main()
{
    // get input for SSDO algorithm
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = normalize(texture(gNormal, TexCoords).rgb);
    vec3 randomVec = normalize(texture(texNoise, TexCoords * noiseScale).xyz);
    // create TBN change-of-basis matrix: from tangent-space to view-space
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);
    // iterate over the sample kernel and calculate occlusion factor
    // float dirOcclusion = 0.0;
    vec3 dirOcclusion = vec3(0.0,0.0,0.0);
    for(int i = 0; i < kernelSize; ++i)
    {
        // get sample position
        vec3 sample = TBN * samples[i]; // from tangent to view-space
        sample = fragPos + sample * radius; 
        
        // project sample position (to sample texture) (to get position on screen/texture)
        vec4 offset = vec4(sample, 1.0);
        offset = projection * offset; // from view to clip-space
        offset.xyz /= offset.w; // perspective divide
        offset.xyz = offset.xyz * 0.5 + 0.5; // transform to range 0.0 - 1.0
        
        // get sample depth
        float sampleDepth = -texture(gPosition, offset.xy).w; // get depth value of kernel sample
        vec3 sampleNormal = texture(gNormal, offset.xy).rgb;
        vec3 samplePos = texture(gPosition, offset.xy).xyz;
        // range check & accumulate          
        if (sampleDepth < sample.z) {
            vec4 envDir = inverse(view) * vec4(sample - fragPos, 0.0);
			vec3 envColor = texture(cubemap, envDir.xyz).xyz;
			dirOcclusion += envColor * dot(normal, normalize(sample - fragPos));
		}
    }
    FragColor = dirOcclusion / kernelSize;
}
