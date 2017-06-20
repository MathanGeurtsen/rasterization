#version 330
 
// shader input
in vec4 normal;					// interpolated normal
in vec2 uv;						// interpolated texture coordinates
in vec3 worldPos; // fragment position in worldspace


uniform sampler2D pixels;		// texture sampler

float ambientLight = 0.05f;
uniform vec3 lightPos = vec3( -30,3,3);
// shader output
out vec4 outputColor;

// fragment shader
void main()
{

				vec3 norm = normalize(normal.xyz);
				vec3 lightDir = normalize(lightPos - worldPos);
    float diff = max(dot(norm, lightDir), 0.0);
	   vec3 diffuse = diff * vec3(1,1,1);
				vec3 result = (ambientLight + diffuse) * texture( pixels, uv ).xyz;

				outputColor = vec4(result,1.0); //	+ 0.2f * vec4( normal.xyz * , 1 );
}
