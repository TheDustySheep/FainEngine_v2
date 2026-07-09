#version 330 core
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec4 vColor;

out vec4 oColor;

uniform mat4 uProjection;

void main()
{
    oColor = vColor;
    gl_Position = uProjection * vec4(vPosition, 1.0);
}