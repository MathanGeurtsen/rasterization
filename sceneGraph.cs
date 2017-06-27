using System.IO;
using System.Collections.Generic;
using OpenTK;
using System;

namespace Template_P3
{
    public class SceneGraph
    {
        public List<Mesh> meshTree;                             // List of all meshes
        public List<Light> lights;                              // List of all lights (non tree like)
        public Matrix4 projectionMatrix;                        // matrix for FOV
        public Matrix4 viewMatrix = Matrix4.Identity;           // matrix for movement and Y-axis rotation
        public Matrix4 viewMatrix2 = Matrix4.Identity;          // matrix for X-axis rotation
        

        float a = 0.001f;

        // Constructor
        public SceneGraph()
        {
            meshTree = new List<Mesh>();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 10000);
            a = 0;
            lights = new List<Light>();

        }//SceneGraph()

        // function to set modelMatrix for every mesh
        public void render()
        {
            for (int i = 0; i < meshTree.Count; i++)
            {
                meshTree[i].initialModelMatrix = meshTree[i].modelMatrix;
                meshTree[i].modelMatrix = toWorldSpace(meshTree[i]);
            }

        }//render()

        // function to transfrom a mesh's modelmatrix to correspond to it's parent's modelmatrix 
        public Matrix4 toWorldSpace(Mesh mesh)
        {
            if (mesh.Parent != null)
                mesh.modelMatrix = toWorldSpace(mesh.Parent) * mesh.modelMatrix;
            return mesh.modelMatrix;
        }//toWorldSpace()

        // function to set the transform of all meshes
        public void transform()
        {
            for (int i = 0; i < meshTree.Count; i++)
            {

                if (meshTree[i].Parent != null)
                    meshTree[i].transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a * meshTree[i].Axisrotation) * meshTree[i].initialModelMatrix * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a * meshTree[i].ParentRotation) * meshTree[i].Parent.transform * projectionMatrix;
                else
                    meshTree[i].transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a * meshTree[i].Axisrotation) * meshTree[i].modelMatrix * viewMatrix * viewMatrix2 * projectionMatrix;
            }
            a += 0.01f;
            for (int i = 0; i < lights.Count; i++)
                lights[i].transform = lights[i].modelMatrix * viewMatrix * viewMatrix2 * projectionMatrix;

        }//transform()

        // function to change the viewMatrix as to "move the camera"
        public void move(Vector3 movement)
        {
            for (int i = 0; i < meshTree.Count; i++)
                viewMatrix *= Matrix4.Translation(movement);
        }//move()


        // function to change the viewMatrix as to "rotate the camera around the X-axis"
        public void rotate(Vector3 rotation, float speed)
        {
            viewMatrix2 *= Matrix4.Rotate(rotation, speed);
        }//rotate()

        // function to change the viewMatrix as to "move the camera around the Y-axis"
        public void rotateHorizontal(Vector3 rotation, float speed)
        {
            viewMatrix *= Matrix4.Rotate(rotation, speed);
        }//rotateHorizontal()

        // changes the viewMatrix to default
        public void reset()
        {
            viewMatrix = Matrix4.Identity;
            viewMatrix2 = Matrix4.Identity;
        }//reset()
    }
}

