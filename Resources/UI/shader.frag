#version 330 core
out vec4 FragColor;

in vec2 oTextUVCoords;
in vec4 oColor;

uniform sampler2D textAtlas;

void main()
{
    vec4 textCol = texture2D(textAtlas, oTextUVCoords);

    FragColor = vec4(textCol.rgb, 0);
}