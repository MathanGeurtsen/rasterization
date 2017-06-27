using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

    class Game
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Mesh mesh, floor, teapot, vloer;        // a mesh to draw using OpenGL
        const float PI = 3.1415926535f;         // PI
        Stopwatch timer;                        // timer for measuring frame duration
        Shader shader;                          // shader to use for rendering
        Shader postproc;                        // shader to use for post processing
        Shader skyboxshader;
        Texture wood;                           // texture to use for rendering
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        bool useRenderTarget = true;
        SceneGraph scenegraph;                  // scene graph containing all models
        float speed = 1f;                       // camera movementspeed modifier
        Light light1, light2;
        //skybox
        Bitmap[] faces;
        uint cubemapTexture;
        int skyvertexBufferId;
        int VAO;
        float[] skyboxVertices;

        // initialize
        public void Init()
        {
            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            skyboxshader = new Shader("../../shaders/skyvs.glsl", "../../shaders/skyfs.glsl");
            scenegraph = new SceneGraph();
            //skybox
            faces = new Bitmap[6];
            faces[0] = new Bitmap("../../assets/right.jpg"); faces[1] = new Bitmap("../../assets/left.jpg"); faces[2] = new Bitmap("../../assets/top.jpg");
            faces[3] = new Bitmap("../../assets/bottom.jpg"); faces[4] = new Bitmap("../../assets/back.jpg"); faces[5] = new Bitmap("../../assets/front.jpg");
            cubemapTexture = loadCubemap(faces);
            skyboxVertices = new float[]
            {
            // positions          
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
            };
            GL.GenVertexArrays(1, out VAO);

            GL.GenBuffers(1, out skyvertexBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, skyvertexBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(skyboxVertices.Length * sizeof(float)), skyboxVertices, BufferUsageHint.StaticDraw);
            // load textures
            initTextures();
            // load meshes
            initMeshes();
            // initialize stopwatch
            timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();
		    // create the render target
		    target = new RenderTarget( screen.width, screen.height);
		    quad = new ScreenQuad();
            // create scene graph
            scenegraph.meshTree.Add(floor);
            scenegraph.meshTree.Add(mesh);
            scenegraph.render();
   	    }//Init

        // initializes all textures
        public void initTextures()
        {
            wood = new Texture("../../assets/wood.jpg");
        }//initTextures()

        // initializes all meshes
        public void initMeshes()
        {
		    mesh = new Mesh( "../../assets/teapot.obj" );
		    floor = new Mesh( "../../assets/floor.obj" );

            floor.modelMatrix = Matrix4.CreateTranslation(0, -4, -15);
            mesh.Parent = floor;
            
            floor.texture = wood;
            mesh.texture = wood;

            floor.name = "floor";

            //lights
            light1 = new Light(shader.uniform_lightpos1);
            light1.modelMatrix = Matrix4.CreateTranslation(10, 0, 0);

            light2 = new Light(shader.uniform_lightpos2);
            light2.modelMatrix = Matrix4.CreateTranslation(-5, 10, 0);

            scenegraph.lights.Add(light1);
            scenegraph.lights.Add(light2);
        }//initMeshes()

        // prints all meshes as an upside-down tree structure
        public void printObjTree()
        {
            for (int i = 0; i < scenegraph.meshTree.Count; i++)
                if (scenegraph.meshTree[i].Parent == null)
                    printObjTreeRecur(scenegraph.meshTree[i], 0);
            System.Threading.Thread.Sleep(500); // sleep function as to not spam the console when someone presses the button a bit too long
        }//printObjTree()

        // recursive part of printObjTree() as to find all childs for each mesh
        public void printObjTreeRecur(Mesh mesh, int depth)
        {
            for (int i = 0; i < scenegraph.meshTree.Count; i++)
                if (scenegraph.meshTree[i].Parent == mesh)
                    printObjTreeRecur(scenegraph.meshTree[i], depth + 1);
            for (int i = 0; i < depth; i++)
                Console.Write("  ");
            Console.Write("+--" + mesh.name + '\n');
        }//printObjeTreeRecur()

	    // tick for background surface
	    public void Tick()
	    {
		    screen.Clear( 0 );
            control();
	    }//Tick()

        // Keypress handlers
        public void control()
        {
            var keystate = OpenTK.Input.Keyboard.GetState(); //current input key

            // Keypress to go forwards
            if (keystate[OpenTK.Input.Key.W])
                scenegraph.move(new Vector3(0, 0, 0.5f) * speed);
            // Keypress to go left
            if (keystate[OpenTK.Input.Key.A])
                scenegraph.move(new Vector3(0.5f, 0, 0) * speed);
            // Keypress to go backwards
            if (keystate[OpenTK.Input.Key.S])
                scenegraph.move(new Vector3(0, 0, -0.5f) * speed);
            // Keypress to go right
            if (keystate[OpenTK.Input.Key.D])
                scenegraph.move(new Vector3(-0.5f, 0, 0) * speed);
            // Keypress to go upwards
            if (keystate[OpenTK.Input.Key.Q])
                scenegraph.move(new Vector3(0, -0.5f, 0) * speed);
            // Keypress to go downwards
            if (keystate[OpenTK.Input.Key.E])
                scenegraph.move(new Vector3(0, 0.5f, 0) * speed);

            // Keypress to "turn camera upwards"
            if (keystate[OpenTK.Input.Key.Up])
                scenegraph.rotate(new Vector3(-1, 0, 0), (PI / 120) * speed);
            // Keypress to "turn camera downwards"
            if (keystate[OpenTK.Input.Key.Down])
                scenegraph.rotate(new Vector3(1, 0, 0), (PI / 120) * speed);
            // Keypress to "turn camera to the left"
            if (keystate[OpenTK.Input.Key.Left])
                scenegraph.rotateHorizontal(new Vector3(0, -1, 0), (PI / 120) * speed);
            // Keypress to "turn camera to the right"
            if (keystate[OpenTK.Input.Key.Right])
                scenegraph.rotateHorizontal(new Vector3(0, 1, 0), (PI / 120) * speed);

            // Keypress to reset camera position
            if (keystate[OpenTK.Input.Key.R])
                scenegraph.reset();
            // Keypress to print mesh tree in the console (note: it's upside down)
            if (keystate[OpenTK.Input.Key.P])
                printObjTree();

            // Keypress to speed up movement
            if (keystate[OpenTK.Input.Key.KeypadAdd] || keystate[OpenTK.Input.Key.Plus])
                speed += 0.1f;
            // Keypress to slow down movement
            if (keystate[OpenTK.Input.Key.KeypadMinus] || keystate[OpenTK.Input.Key.Minus])
                speed -= 0.1f;
            
            // sets speed to 0 if it would become a negative value
            if (speed < 0)
                speed = 0;
        }//control()

	    // tick for OpenGL rendering code
	    public void RenderGL()
	    {
		    // measure frame duration
		    float frameDuration = timer.ElapsedMilliseconds;
		    timer.Reset();
		    timer.Start();

            //skybox
            GL.Uniform1(GL.GetUniformLocation(skyboxshader.programID, "skybox"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubemapTexture);

            GL.UseProgram(skyboxshader.programID);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            // prepare matrix for vertex shader
            scenegraph.transform();

            // update rotation
            light1.Render(shader);
            light2.Render(shader);

		    if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                // render scene to render target
                for (int i = 0; i < scenegraph.meshTree.Count; i++)
                    scenegraph.meshTree[i].Render(shader);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }// if
		    else
		    {
                // render scene directly to the screen
                for (int i = 0; i < scenegraph.meshTree.Count; i++)
                    scenegraph.meshTree[i].Render(shader);
            }// else

        }// RenderGL

        public void addmesh(string objectFile, Texture texture)
        {
            Mesh mesh;
            if (File.Exists(objectFile) && texture != null)
            {
                mesh = new Mesh(objectFile);
                mesh.usedTexture = texture;
                scenegraph.meshTree.Add(mesh);
            }
        }
        public void addmesh(string objectFile, Texture texture, Mesh parentmesh)
        {
            Mesh mesh;
            if (File.Exists(objectFile) && texture != null && parentmesh != null)
            {
                mesh = new Mesh(objectFile);
                mesh.usedTexture = texture;
                mesh.Parent = parentmesh;
                
                scenegraph.meshTree.Add(mesh);
                
            }
        }

        public void addmesh(string objectFile, Texture texture, Matrix4 modelMatrix)
        {
            Mesh mesh;
            if (File.Exists(objectFile) && texture != null)
            {
                mesh = new Mesh(objectFile);
                mesh.usedTexture = texture;
                mesh.modelMatrix = modelMatrix;
                scenegraph.meshTree.Add(mesh);

            }
        }


        // skybox
        uint loadCubemap(Bitmap[] faces)
        {
            uint textureID;
            GL.GenTextures(1, out textureID);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID);

            for (int i = 0; i < faces.Length; i++)
            {
                System.Drawing.Imaging.BitmapData data = faces[i].LockBits(new Rectangle(0, 0, faces[i].Width, faces[i].Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                                 0, PixelInternalFormat.Rgb, data.Width, data.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, data.Scan0
                    );
                faces[i].UnlockBits(data);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            return textureID;
        }


    }// Game

} // namespace Template_P3
