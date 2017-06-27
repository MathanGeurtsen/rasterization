#version 330 core
in vec3 vPosition;

out vec3 TexCoords;

uniform mat4 transform;

void main()
{
    TexCoords = vPosition;
    gl_Position = transform * vec4(vPosition, 1.0);
}  