﻿using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

    class Game
    {
	    // member variables
	    public Surface screen;					// background surface for printing etc.
	    Mesh mesh, floor;						// a mesh to draw using OpenGL
	    const float PI = 3.1415926535f;			// PI
	    Stopwatch timer;						// timer for measuring frame duration
	    Shader shader;							// shader to use for rendering
	    Shader postproc;						// shader to use for post processing
	    Texture wood;							// texture to use for rendering
	    RenderTarget target;					// intermediate render target
	    ScreenQuad quad;						// screen filling quad for post processing
	    bool useRenderTarget = true;
        SceneGraph scenegraph;
        float speed = 1f;

        // initialize
        public void Init()
	    {
            // load meshes
            initMeshes();
            // initialize stopwatch
            timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();
		    // create shaders
		    shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
		    postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
		    // load a texture
		    wood = new Texture( "../../assets/wood.jpg" );
		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad();
            // create scene graph
            scenegraph = new SceneGraph();
            scenegraph.meshTree.Add(floor);
            scenegraph.meshTree.Add(mesh);
            scenegraph.Render();
   	    }

        public void initMeshes()
        {
		    mesh = new Mesh( "../../assets/teapot.obj" );
		    floor = new Mesh( "../../assets/floor.obj" );

            floor.modelMatrix = Matrix4.CreateTranslation(0, -4, -15);
            mesh.Parent = floor;
        }

	    // tick for background surface
	    public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( speed.ToString(), 2, 2, 0xffff00 );
            control();
	    }

        public void control()
        {
            var keystate = OpenTK.Input.Keyboard.GetState();

            if (keystate[OpenTK.Input.Key.W])
                scenegraph.move(new Vector3(0, 0, 0.5f) * speed);
            if (keystate[OpenTK.Input.Key.A])
                scenegraph.move(new Vector3(0.5f, 0, 0) * speed);
            if (keystate[OpenTK.Input.Key.S])
                scenegraph.move(new Vector3(0, 0, -0.5f) * speed);
            if (keystate[OpenTK.Input.Key.D])
                scenegraph.move(new Vector3(-0.5f, 0, 0) * speed);
            if (keystate[OpenTK.Input.Key.Q])
                scenegraph.move(new Vector3(0, -0.5f, 0) * speed);
            if (keystate[OpenTK.Input.Key.E])
                scenegraph.move(new Vector3(0, 0.5f, 0) * speed);

            if (keystate[OpenTK.Input.Key.Up])
                scenegraph.rotate(new Vector3(-1, 0, 0));
            if (keystate[OpenTK.Input.Key.Down])
                scenegraph.rotate(new Vector3(1, 0, 0));
            if (keystate[OpenTK.Input.Key.Left])
                scenegraph.rotate(new Vector3(0, -1, 0));
            if (keystate[OpenTK.Input.Key.Right])
                scenegraph.rotate(new Vector3(0, 1, 0));

            if (keystate[OpenTK.Input.Key.R])
                scenegraph.reset();

            if (keystate[OpenTK.Input.Key.KeypadAdd] || keystate[OpenTK.Input.Key.Plus])
                speed += 0.1f;
            if (keystate[OpenTK.Input.Key.KeypadMinus] || keystate[OpenTK.Input.Key.Minus])
                speed -= 0.1f;

            if (speed < 0)
                speed = 0;
        }

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

		    if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                // render scene to render target
                for (int i = 0; i < scenegraph.meshTree.Count; i++)
                    scenegraph.meshTree[i].Render(shader, scenegraph.meshTree[i].transform, wood);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
                // render scene directly to the screen
                for (int i = 0; i < scenegraph.meshTree.Count; i++)
                    scenegraph.meshTree[i].Render(shader, scenegraph.meshTree[i].transform, wood);
            }
	    }
    }


} // namespace Template_P3
