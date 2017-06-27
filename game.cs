using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

    class Game
    {
        // member variables
        Shader shader;                  // shader to use for rendering
        Shader postproc;                // shader to use for post processing
        public Surface screen;			// background surface for printing etc.
	    Mesh teapot, floor, earth, moon, light2Teapot;// a mesh to draw using OpenGL
	    const float PI = 3.1415926535f;	// PI
	    Stopwatch timer;				// timer for measuring frame duration
	    Texture wood, iron, marble, earthDayTexture, earthNightTexture, moonTexture, the_bikker; // texture to use for rendering
	    RenderTarget target;			// intermediate render target
	    ScreenQuad quad;				// screen filling quad for post processing
	    bool useRenderTarget = true;    // render to texture
        SceneGraph scenegraph;          // scene graph containing all models
        Light light1, light2;           // light sources
        float speed = 3f;               // camera movementspeed modifier

        // initialize
        public void Init()
	    {
            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            // create scene graph
            scenegraph = new SceneGraph();
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
           

   	    }//Init

        // initializes all textures
        public void initTextures()
        {
            wood         = new Texture("../../assets/wood.jpg");
            marble       = new Texture("../../assets/marble.jpg");
            earthDayTexture = new Texture("../../assets/earthTextures/4096_earth.jpg");
            moonTexture = new Texture("../../assets/MoonMap2_2500x1250.jpg");
        }//initTextures()

        // initializes all meshes
        public void initMeshes()
        {
            // adding objects
            teapot = new Mesh( "../../assets/teapot.obj" );
            floor = new Mesh( "../../assets/floor.obj" );
            earth = new Mesh("../../assets/earth.obj");
            moon = new Mesh("../../assets/moon.obj");
            light2Teapot = new Mesh("../../assets/teapot.obj");

            // setting translation and rotation matrices

            floor.Axisrotation = 0;
            floor.ParentRotation = 0;
            
            earth.modelMatrix *= Matrix4.CreateTranslation(0, 0, -800);
            earth.modelMatrix *= Matrix4.Rotate(new Vector3(0, 0, 1), PI);
            earth.ParentRotation = 0;
            earth.Axisrotation = .25f;

            moon.modelMatrix *= Matrix4.CreateTranslation(0, 0, -600);
            moon.ParentRotation = 0.02f;
            moon.Axisrotation = 0;

            teapot.ParentRotation = 1f;
            teapot.Axisrotation = 0.5f;
            teapot.modelMatrix *= Matrix4.Rotate(new Vector3(-1, 1, 0), PI);
            teapot.modelMatrix *= Matrix4.CreateTranslation(-75f, 0, 0);

            // Lights
            light1 = new Light(shader.uniform_lightpos1);
            light1.modelMatrix = Matrix4.CreateTranslation(10, 0, 0);
            light1.name = "light1";

            light2 = new Light(shader.uniform_lightpos2);
            light2.modelMatrix = Matrix4.CreateTranslation(0, 400, -800);
            light2.rotationspeed = 0.5f;
            light2.name = "light2";

            // Visual indicator for light2
            light2Teapot.modelMatrix = Matrix4.CreateTranslation(0, 400, -800);
            light2Teapot.Parent = floor;
            light2Teapot.ParentRotation = 0.5f;
            light2Teapot.Axisrotation = -1f;

            scenegraph.lights.Add(light1);
            scenegraph.lights.Add(light2);

            // setting parents
            teapot.Parent = moon;
            moon.Parent = earth;

            // setting textures
            floor.texture = wood;
            teapot.texture = marble;
            earth.texture = earthDayTexture;
            moon.texture = moonTexture;
            light2Teapot.texture = marble;

            // adding names for showing the tree structure
            teapot.name = "teapot";
            floor.name  = "floor";
            earth.name  = "earth";
            moon.name   = "moon";
            light2Teapot.name = "Light2Teapot";

            // adding meshes to the meshTree
            scenegraph.meshTree.Add(floor);
            scenegraph.meshTree.Add(teapot);
            scenegraph.meshTree.Add(earth);
            scenegraph.meshTree.Add(moon);
            scenegraph.meshTree.Add(light2Teapot);

            scenegraph.render();
        }//initMeshes()

        // prints all meshes as an upside-down tree structure
        public void printObjTree()
        {
            List<string> meshList = new List<string>();
            for (int i = 0; i < scenegraph.meshTree.Count; i++)
                if (scenegraph.meshTree[i].Parent == null)
                    printObjTreeRecur(scenegraph.meshTree[i], 0, ref meshList);

            System.Threading.Thread.Sleep(500); // sleep function as to not spam the console when someone presses the button a bit too long
            for (int i = meshList.Count - 1; i >= 0; i--)
                Console.WriteLine(meshList[i]);
            
        }//printObjTree()

        // recursive part of printObjTree() as to find all childs for each mesh
        public void printObjTreeRecur(Mesh mesh, int depth, ref List<string> meshlist)
        {
            string result = "";
            for (int i = 0; i < scenegraph.meshTree.Count; i++)
                if (scenegraph.meshTree[i].Parent == mesh)
                    printObjTreeRecur(scenegraph.meshTree[i], depth + 1, ref meshlist);
            for (int i = 0; i < depth; i++)
                result += "  ";
            result += "+--" + mesh.name;
            meshlist.Add(result);
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
                scenegraph.rotate(new Vector3(-1, 0, 0), (PI / 120) * speed/4);
            // Keypress to "turn camera downwards"
            if (keystate[OpenTK.Input.Key.Down])
                scenegraph.rotate(new Vector3(1, 0, 0), (PI / 120) * speed/4);
            // Keypress to "turn camera to the left"
            if (keystate[OpenTK.Input.Key.Left])
                scenegraph.rotateHorizontal(new Vector3(0, -1, 0), (PI / 120) * speed/4);
            // Keypress to "turn camera to the right"
            if (keystate[OpenTK.Input.Key.Right])
                scenegraph.rotateHorizontal(new Vector3(0, 1, 0), (PI / 120) * speed/4);

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
    }// Game

} // namespace Template_P3
