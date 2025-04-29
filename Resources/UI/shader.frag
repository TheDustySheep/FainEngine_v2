#version 330 core

out vec4 FragColor;

in vec4 oColor;       // Background color (with alpha)
in vec2 oTextUV;      // Texture UV coordinates for text
in vec4 oTextColor;   // Text color (with alpha)

uniform sampler2D fontAtlas;

void main()
{
    float textMask = texture(fontAtlas, oTextUV).r;

    if (textMask > 0.5)
        FragColor = vec4(oTextColor.rgb, 1.0);
    else if (oColor.a > 0.5)
        FragColor = vec4(oColor.rgb, 1.0);
    else
        discard;
}