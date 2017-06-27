using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template_P3
{
    public class Light
    {
        // data members
        public Mesh Parent;                     // parent mesh
        public Matrix4 modelMatrix;             // model matrix
        public Matrix4 transform;               // tranform for obj
        public string name;
        public int lightpos;

        // constructor
        public Light(int lightp)
        {
            modelMatrix = Matrix4.Identity;
            transform = Matrix4.Identity;
            name = "Light";
            lightpos = lightp;
        }

        public void Render(Shader shader)
        {
            Vector4 newVector = new Vector4(modelMatrix.Row0.X, modelMatrix.Row1.Y, modelMatrix.Row2.Z,1);
            Vector4 another = Vector4.Transform(newVector, transform);

            Matrix4 result = Matrix4.Identity;
            result.Row0.X = another.X;
            result.Row1.Y = another.Y;
            result.Row2.Z = another.Z;

            GL.UseProgram(shader.programID);
            if (name == "light1")
                GL.UniformMatrix4(shader.uniform_lightpos1, false, ref result);
            else if (name == "light2")
                GL.UniformMatrix4(shader.uniform_lightpos2, false, ref result);
        }
    }
}
