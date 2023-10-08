#version 330 core
out vec3 FragColor;
in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;
uniform sampler2D texLighting;

uniform vec3 samples[64];

// parameters (you'd probably want to use them as uniforms to more easily tweak the effect)
int kernelSize = 64;
float radius = 1.0;

// tile noise texture over screen based on screen dimensions divided by noise size
const vec2 noiseScale = vec2(800.0f/4.0f, 600.0f/4.0f); 

uniform mat4 projection;

void main()
{
    // Get input for SSDO algorithm
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = texture(gNormal, TexCoords).rgb;
    vec3 randomVec = texture(texNoise, TexCoords * noiseScale).xyz;
    // Create TBN change-of-basis matrix: from tangent-space to view-space
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);
    // Iterate over the sample kernel and calculate indirect light
	vec3 indirBounce = vec3(0.0, 0.0, 0.0);
    for(int i = 0; i < kernelSize; ++i)
    {
        // get sample position
        vec3 sample = TBN * samples[i]; // From tangent to view-space
        sample = fragPos + sample * radius; 
        
        // project sample position (to sample texture) (to get position on screen/texture)
        vec4 offset = vec4(sample, 1.0);
        offset = projection * offset; // from view to clip-space
        offset.xyz /= offset.w; // perspective divide
        offset.xyz = offset.xyz * 0.5 + 0.5; // transform to range 0.0 - 1.0
        
        // get sample depth
        float sampleDepth = -texture(gPosition, offset.xy).w; // Get depth value of kernel sample
		vec3 sampleNormal = texture(gNormal, offset.xy).rgb;
		vec3 samplePos = texture(gPosition, offset.xy).xyz;
		vec3 sampleColor = texture(texLighting, offset.xy).xyz;
        
        // accumulate    
        if (sampleDepth >= sample.z) 
        {
            // indirBounce += max(dot(sampleNormal, normalize(fragPos - samplePos)), 0.0) * sampleColor;
            indirBounce += dot(sampleNormal, normalize(fragPos - samplePos)) * sampleColor;
        }
    }
    indirBounce = (indirBounce/kernelSize) * 10;
	
	FragColor = indirBounce;
		
}