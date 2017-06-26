#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;
	// postprocessing effect
	float dx = 0.5f - P.x; 
	float dy = 0.5f - P.y;
	float distance = dx*dx + dy*dy;
	outputColor = outputColor - distance / 2;

}

// EOF