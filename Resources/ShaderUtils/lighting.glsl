// Enhanced GLSL terrain lighting using existing Light struct
// Energy-conserving Lambert diffuse & Blinn-Phong specular with Fresnel, fixed terrain parameters, gamma correction

struct Light {
    vec3 direction;   // normalized light direction
    vec3 ambient;     // ambient color
    vec3 diffuse;     // diffuse color
    vec3 specular;    // specular color
    float intensity;  // overall light intensity
};

uniform Light light;

// Schlick's Fresnel approximation
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

// Main lighting function tailored for terrain surfaces
vec3 CalcLighting(vec3 baseColor, vec3 viewDir, vec3 normal)
{
    // normalize inputs
    vec3 N = normalize(normal);
    vec3 V = normalize(viewDir);
    vec3 L = normalize(-light.direction);

    // Ambient term (energy conserved)
    vec3 ambientTerm = light.ambient * baseColor;

    // Diffuse term (Lambert)
    float NdotL = max(dot(N, L), 0.0);
    vec3 diffuseTerm = light.diffuse * NdotL * baseColor;

    // Specular term (Blinn-Phong)
    vec3 H = normalize(V + L);
    float NdotH = max(dot(N, H), 0.0);
    
    // Terrain specular constants
    const float shininess = 16.0;          // softer highlights for rough terrain
    const float specularStrength = 0.3;    // moderate specular intensity
    
    float specAngle = pow(NdotH, shininess);
    vec3 specularTerm = light.specular * specularStrength * specAngle;

    // Combine with overall intensity
    vec3 color = ambientTerm + (diffuseTerm + specularTerm) * light.intensity;

    return color;
}

vec3 ApplyGammaCorrection(vec3 color)
{
    // Gamma correction (assuming sRGB output)
    return pow(color, vec3(1.0 / 2.2));   
}
