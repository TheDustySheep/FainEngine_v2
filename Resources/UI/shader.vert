#version 330 core
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec4 vColor;

uniform mat4 uModel;

void main()
{
    gl_Position = vec4(vPosition, 1.0);
}