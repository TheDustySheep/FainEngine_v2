#version 330 core

out vec4 FragColor;

in vec4 oColor;

void main()
{
    //FragColor = vec4(oColor.rgb, 1.0);
    FragColor = vec4(1.0, 0.0, 0.0, 1.0);
}