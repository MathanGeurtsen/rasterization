using System.IO;
using System.Collections.Generic;
using OpenTK;

namespace Template_P3
{
    public class SceneGraph
    {
        const float PI = 3.1415926535f;
        public List<Mesh> meshTree;
        public Matrix4 projectionMatrix;

        static public Vector3 lightPos = new Vector3(5, 0, -10);
        public Matrix4 modellightPos = Matrix4.CreateTranslation(lightPos);

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
                meshTree[i].transform = meshTree[i].modelMatrix * meshTree[i].viewMatrix * projectionMatrix;
        }

        public void move(Vector3 movement)
        {
            for (int i = 0; i < meshTree.Count; i++)
                meshTree[i].viewMatrix *= Matrix4.Translation(movement);
            modellightPos *= Matrix4.Translation(movement);

            //lightPos = modellightPos.
        }

        public void rotate(Vector3 rotation)
        {
            for (int i = 0; i < meshTree.Count; i++)
            {
                Vector3 tempVector = new Vector3(meshTree[i].viewMatrix.Column3.W, meshTree[i].viewMatrix.Column3.X, meshTree[i].viewMatrix.Column3.Y);
                meshTree[i].viewMatrix *= Matrix4.Translation(-tempVector);
                meshTree[i].viewMatrix *= Matrix4.Rotate(rotation, PI / 180);
                meshTree[i].viewMatrix *= Matrix4.Translation(tempVector);
            }
        }
    }
}

