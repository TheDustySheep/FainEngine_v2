#version 330 core
layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec4 vColor;

layout (location = 2) in vec2 vTextUV;
layout (location = 3) in vec4 vTextColor;

out vec4 oColor;
out vec2 oTextUV;
out vec4 oTextColor;

uniform mat4x4 uView;

void main()
{
    oColor     = vColor;
    oTextUV    = vTextUV;
    oTextColor = vTextColor;

    gl_Position = uView * vec4(vPosition, 1.0);
}