#version 330 core
layout (location = 0) in vec2  vPosition;
layout (location = 1) in float vDepth;
layout (location = 2) in vec2  vTextUVCoords;
layout (location = 3) in vec4  vColor;

uniform mat4 uModel;

out vec2 oTextUVCoords;
out vec4 oColor;

void main()
{
    oColor = vColor;
    oTextUVCoords = vTextUVCoords;
    gl_Position = vec4(vPosition, vDepth, 1.0);
}