#version 330
 
// shader input
in vec4 normal;					// interpolated normal
in vec2 uv;						// interpolated texture coordinates
in vec3 worldPos; // fragment position in worldspace


uniform sampler2D pixels;		// texture sampler

float ambientLight = 0.1f;
uniform vec3 lightPos; 
uniform vec3 cameraPos = vec3(0,0,0);
vec3 lightColor = vec3(1,1,1);

float specularStrength = 0.5;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    vec3 norm = normalize(normal.xyz);

    // diffuse lighting
    vec3 lightDir = normalize(lightPos - worldPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
				
    // Specular lighting
    vec3 viewDir = normalize(cameraPos - worldPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;
    if (dot(diffuse,diffuse )==0)
    {
        specular = vec3(0,0,0);
    }
				
				
    // add diffuse, ambient and specular lighting
    vec3 result = (ambientLight + diffuse+ specular) * texture( pixels, uv ).xyz;

    outputColor = vec4(result,1.0); //	+ 0.2f * vec4( normal.xyz * , 1 );
}
