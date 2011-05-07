using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Phantom
{
	public class Game : GameWindow
	{
		Camera camera;
		
		/// <summary>
		/// Create a new game window with the specified title.
		/// </summary>
        public Game() : base(960, 640, GraphicsMode.Default)
        {
            VSync = VSyncMode.On;
			CursorVisible = false;
			Title = "Phantom Engine";
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
			
			camera = new Camera(this);

            GL.ClearColor(Color4.SlateGray);
            GL.Enable(EnableCap.DepthTest);
			
			GL.Enable(EnableCap.Multisample);			
			GL.Enable(EnableCap.SampleAlphaToCoverage);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            var projection = camera.Projection;
			GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();
			
			camera.Update();
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			Matrix4 modelview = camera.View;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            GL.Begin(BeginMode.Triangles);

            GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 4.0f);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(0.0f, 1.0f, 4.0f);

            GL.End();

            SwapBuffers();
        }
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
    	[STAThread]
		public static void Main (string[] args)
		{
			using (Game game = new Game())
            {
                game.Run(60.0);
            }
		}
	}
}
