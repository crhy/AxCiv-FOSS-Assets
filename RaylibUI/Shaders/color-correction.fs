#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform float brightness;
uniform float saturation;
uniform float gamma;

out vec4 finalColor;

void main()
{
    vec4 texel = texture(texture0, fragTexCoord) * colDiffuse * fragColor;
    vec3 color = texel.rgb + vec3(brightness - 1.0);
    float luminance = dot(color, vec3(0.299, 0.587, 0.114));
    color = mix(vec3(luminance), color, saturation);
    color = pow(max(color, vec3(0.0)), vec3(1.0 / max(gamma, 0.01)));
    finalColor = vec4(clamp(color, 0.0, 1.0), texel.a);
}
