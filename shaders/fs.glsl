#version 330
 
// shader input
in vec4 normal;		// interpolated normal
in vec2 uv;			// interpolated texture coordinates
in vec3 worldPos;	// fragment position in worldspace

uniform sampler2D pixels;		// texture sampler

float ambientLight = 1.0f;
uniform mat4 lightPos1;
uniform mat4 lightPos2;
uniform vec3 cameraPos = vec3(0,0,0);
uniform mat4 projectionMatrix; 
vec3 lightColor1 = vec3(1,1,1);
vec3 lightColor2 = vec3(1,1,1);
float specularStr1 = 1f;
float specularStr2 = 3;
float diffStr1 = 1f;
float diffStr2 = 0f;
// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    // Prep
    vec3 norm = normalize(normal.xyz);
    vec3 lightp1 = vec3(lightPos1[0][0], lightPos1[1][1], lightPos1[2][2]);
    vec3 lightp2 = vec3(lightPos2[0][0], lightPos2[1][1], lightPos2[2][2]);
    vec3 viewDir = normalize(cameraPos - worldPos);
    
    // diffuse lighting 1
    vec3 lightDir1 = normalize(lightp1 - worldPos);
    float diff1 = max(dot(norm, lightDir1), 0.0);
    vec3 diffuse1 = diff1 * lightColor1 * diffStr1;
				
    // Specular lighting 1
    vec3 reflectDir1 = reflect(-lightDir1, norm);

    float spec1 = pow(max(dot(viewDir, reflectDir1), 0.0), 32);
    vec3 specular1 = specularStr1 * spec1 * lightColor1;

	
    vec3 lightDir2 = normalize(lightp2 - worldPos);
  
	// diffuse lighting 2
    float diff2 = max(dot(norm, lightDir2), 0.0);
    vec3 diffuse2 = diff2 * lightColor2 * diffStr2;
				
    // Specular lighting 2
    vec3 reflectDir2 = reflect(-lightDir2, norm);

    float spec2 = pow(max(dot(viewDir, reflectDir2), 0.0), 32);
    vec3 specular2 = specularStr2 * spec2 * lightColor2;
				
    // add diffuse, ambient and specular lighting of 1 and 2
    vec3 result = (ambientLight + diffuse1/2 + specular1/2 + diffuse2/2 + specular2/2) * texture( pixels, uv ).xyz;	

    outputColor = vec4(result,1.0);
}
