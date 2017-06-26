using System.IO;
using System.Collections.Generic;
using OpenTK;

namespace Template_P3
{
    public class SceneGraph
    {
        public List<Mesh> meshTree;
        public Matrix4 projectionMatrix;
        public Matrix4 viewMatrix2 = Matrix4.Identity;
        public Matrix4 viewMatrix = Matrix4.Identity;

        public SceneGraph()
        {
            meshTree = new List<Mesh>();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        }

        public void Render()
        {
            for (int i = 0; i < meshTree.Count; i++)
                meshTree[i].modelMatrix = toWorldSpace(meshTree[i]);
        }

        public Matrix4 toWorldSpace(Mesh mesh)
        {
            if (mesh.Parent != null)
                mesh.modelMatrix = toWorldSpace(mesh.Parent) * mesh.modelMatrix;
            return mesh.modelMatrix;
        }

        public void transform()
        {
            for (int i = 0; i < meshTree.Count; i++)
                meshTree[i].transform = meshTree[i].modelMatrix * viewMatrix * viewMatrix2 * projectionMatrix;
        }

        public void move(Vector3 movement)
        {
            for (int i = 0; i < meshTree.Count; i++)
                viewMatrix *= Matrix4.Translation(movement);
        }

        public void rotate(Vector3 rotation, float speed)
        {
            viewMatrix2 *= Matrix4.Rotate(rotation, speed);
        }

        public void rotateHorizontal(Vector3 rotation, float speed)
        {
            viewMatrix *= Matrix4.Rotate(rotation, speed);
        }

        public void reset()
        {
            viewMatrix = Matrix4.Identity;
            viewMatrix2 = Matrix4.Identity;
        }
    }
}

