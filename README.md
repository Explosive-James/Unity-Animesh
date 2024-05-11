# Animesh
A tool to export animations into textures to be played on the GPU via a shader.

Unity's skinned mesh renderer adjusts vertices on the CPU which is quite intensive for animation, this bakes animations to a texture so it can be played by a shader which reduces CPU load. Intended for background objects that don't require animation blending or complex state machine logic.
