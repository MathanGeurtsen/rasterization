using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template_P3
{
    class Light
    {
        // data members
        public Mesh Parent;                     // parent mesh
        public Matrix4 modelMatrix;             // model matrix
        public Matrix4 transform;               // tranform for obj
        public string name;

        // constructor
        public Light()
        {
            modelMatrix = Matrix4.Identity;
            name = "Light1";
        }

        public void Render(Shader shader)
        {
            GL.UniformMatrix4(shader.uniform_lightpos, false, ref transform);
        }
    }
}
