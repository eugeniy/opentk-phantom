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
		Statistics stats;
		
		/// <summary>
		/// Create a new game window with the specified title.
		/// </summary>
        public Game() : base(960, 640, new GraphicsMode(DisplayDevice.Default.BitsPerPixel, 16, 0, 4))
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
			stats = new Statistics(this);
			

            GL.ClearColor(Color4.SlateGray);
            GL.Enable(EnableCap.DepthTest);
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

			if (Keyboard[Key.F11])
                WindowState = WindowState.Fullscreen;
			
			camera.Update(e);

			stats.Update(e);
            stats["Position"] = String.Format("({0:0.###}, {1:0.###}, {2:0.###})", camera.Position.X, camera.Position.Y, camera.Position.Z);
            stats["Yaw"] = String.Format("{0:0.###}°", MathHelper.RadiansToDegrees(camera.Yaw));
            stats["Pitch"] = String.Format("{0:0.###}°", MathHelper.RadiansToDegrees(camera.Pitch));
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			



            GL.MatrixMode(MatrixMode.Modelview);

			Matrix4 modelview = camera.View;
            GL.LoadMatrix(ref modelview);






            GL.Begin(BeginMode.Triangles);

            GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 4.0f);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(0.0f, 1.0f, 4.0f);

            GL.End();

            stats.Draw(e);
			
			

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
