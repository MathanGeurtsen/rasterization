using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace template_P3
{
    class DOF
    {
        void GenerateDOF(uint width, uint height)
        {
            /*
            int n = 10; // number of light rays
            Vector3 eye = new Vector3(0, 0, 0);
            float aperture = 0.05f;
            Matrix4 projection = perspective(...);

            Vector3 right = normalize(glm::cross(object - eye, up));
            Vector3 p_up = normalize(glm::cross(object - eye, right));

            for (int i = 0; i < n; i++)
            {
                Vector3 bokeh = right * cosf(i * 2 * M_PI / n) + p_up * sinf(i * 2 * M_PI / n);
                Matrix4 modelview = glm::lookAt(eye + aperture * bokeh, object, p_up);
                Matrix4 mvp = projection * modelview;
                glUniformMatrix4fv(uniform_mvp, 1, GL_FALSE, glm::value_ptr(mvp));
                draw_scene();
                glAccum(i ? GL_ACCUM : GL_LOAD, 1.0 / n);
            }

            glAccum(GL_RETURN, 1);
            glSwapBuffers();
            */
        }
    }
}
