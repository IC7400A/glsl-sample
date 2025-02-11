#ifdef GL_FRAGMENT_PRECISION_HIGH
precision highp float;
#else
precision mediump float;
#endif

// Uniforms for screen resolution and pointer interaction
uniform vec2 resolution;
uniform int pointerCount;
uniform vec3 pointers[10];  // Pointers array
uniform sampler2D backbuffer; // Texture of the previous frame

// Function to fetch pixel value at coordinates (x, y)
float get(float x, float y) {
    return texture2D(backbuffer, (gl_FragCoord.xy + vec2(x, y)) / resolution).r;
}

// Helper function to return 1 if value is approximately zero
float oneIfZero(float value) {
    return step(abs(value), 0.1);
}

// Evaluate whether a cell becomes alive based on neighbors
vec3 evaluate(float sum) {
    float has3 = oneIfZero(sum - 3.0);  // Cell becomes alive with 3 neighbors
    float has2 = oneIfZero(sum - 2.0);  // Cell stays alive with 2 neighbors

    // A cell becomes alive if it has 3 neighbors or 2 neighbors and was already alive
    return vec3(has3 + has2 * get(0.0, 0.0));
}

void main() {
    // Sum of surrounding cells' values (neighbors)
    float sum =
        get(-1.0, -1.0) + get(-1.0, 0.0) + get(-1.0, 1.0) +
        get(0.0, -1.0) + get(0.0, 1.0) +
        get(1.0, -1.0) + get(1.0, 0.0) + get(1.0, 1.0);

    // Define the "tap area" for pointer influence
    float tap = min(resolution.x, resolution.y) * 0.05;

    // Check if any pointer is close enough to set the cell to 3 (alive)
    for (int n = 0; n < pointerCount; ++n) {
        if (distance(pointers[n].xy, gl_FragCoord.xy) < tap) {
            sum = 3.0;  // Override with 3 if pointer is close
            break;
        }
    }

    // Set the final color based on the evaluation of the cell state
    gl_FragColor = vec4(evaluate(sum), 1.0);
}
