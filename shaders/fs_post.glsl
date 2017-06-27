#version 330

// shader input
in vec2 P;				// fragment position in screen space
in vec2 uv;				// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;

void main()
{
	// vignetting effect
	float dx = 0.5f - P.x; 
	float dy = 0.5f - P.y;
	float distance = dx*dx + dy*dy;

  // offsets for chromatic abberation
  float rOffset = (distance-0.5)/100f;
  float bOffset = (distance-0.5)/100f;  

  // retrieve input pixel and neigbouring pixels
	outputColor = texture( pixels, uv ).rgb;
  float ColorRedInner = texture( pixels, vec2(uv.x -rOffset,uv.y -rOffset)).r;
  float ColorGreenOuter = texture( pixels, vec2(uv.x ,uv.y)).g;
  float ColorBlueOuter = texture( pixels, vec2(uv.x +bOffset,uv.y +bOffset)).b;

	outputColor = (outputColor + vec3(ColorRedInner/1.5,ColorGreenOuter/1.5,ColorBlueOuter/1.5) - distance/2);

}

// EOF
