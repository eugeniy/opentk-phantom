using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Phantom
{
    public class Statistics
    {
        protected Dictionary<string, string> m_statistics;

        protected int m_textTexture;
        protected Bitmap m_textBitmap;
        protected Font m_textFont;
        protected SolidBrush m_textBrush;
        protected Vector2 m_textPosition = new Vector2(20, 15);
        
        protected int m_frameRate = 0;
        protected int m_frameCounter = 0;
        protected TimeSpan m_elapsedTime = TimeSpan.Zero;
        
        public Statistics(Game game)
        {
            // By default, this class only sets the frames per second statistic
            m_statistics = new Dictionary<string, string>();
            m_statistics["FPS"] = "0";

            // Load the font to be used for drawing text
            m_textFont = new Font(FontFamily.GenericSansSerif, 10);
            m_textBrush = new SolidBrush(Color.White);
            
            // Create a bitmap to store the text
            m_textBitmap = new Bitmap(game.Width, game.Height);
 
            // Create and bind a texture object
            m_textTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, m_textTexture);

            // Set linear filtering for stretching and shrinking textures
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            
            // Allocate memory, so we can update efficiently using TexSubImage2D
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                m_textBitmap.Width, m_textBitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
        }

        
        /// <summary>
        /// Called when it is time to setup the next frame.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        public void Update(FrameEventArgs e)
        {
            m_elapsedTime += TimeSpan.FromSeconds(e.Time);
   
            // Limit calculations to one per second
            if (m_elapsedTime > TimeSpan.FromSeconds(1))
            {
                // Calculate the frame rate
                m_elapsedTime -= TimeSpan.FromSeconds(1);
                m_frameRate = m_frameCounter;
                m_frameCounter = 0;


                // Render text using System.Drawing
                using (Graphics graphics = Graphics.FromImage(m_textBitmap))
                {
                    m_statistics["FPS"] = m_frameRate.ToString();
                    
                    graphics.Clear(Color.Transparent);

                    // Enumerate over all statistics and draw them
                    int line = 0;
                    foreach (KeyValuePair<string, string> stat in m_statistics)
                    {
                        graphics.DrawString(string.Format("{0}: {1}", stat.Key, stat.Value),
                            m_textFont, m_textBrush, m_textPosition.X, m_textPosition.Y + m_textFont.GetHeight() * line);
                        line++;
                    }

                    
                }
                
                // Upload the bitmap to OpenGL
                BitmapData data = m_textBitmap.LockBits(new Rectangle(0, 0, m_textBitmap.Width, m_textBitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, m_textBitmap.Width, m_textBitmap.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                m_textBitmap.UnlockBits(data);
            }
        }


        /// <summary>
        /// Called when it is time to render the next frame.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        public void Draw(FrameEventArgs e)
        {
            m_frameCounter++;

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.DstAlpha);

            // Save the matrix we had before this function call
            GL.PushMatrix();

            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
 
            GL.PushMatrix();
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, m_textBitmap.Width, m_textBitmap.Height, 0, -1, 1);
            GL.LoadMatrix(ref ortho);


            // Map the text texture
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);

            GL.TexCoord2(1, 0);
            GL.Vertex2(m_textBitmap.Width, 0);

            GL.TexCoord2(1, 1);
            GL.Vertex2(m_textBitmap.Width, m_textBitmap.Height);

            GL.TexCoord2(0, 1);
            GL.Vertex2(0, m_textBitmap.Height);
            GL.End();


            // Restore everything to the previous state
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
        }


        /// <summary>
        /// Access the dictionary used to display statistics.
        /// </summary>
        /// <param name="key">The key of the statistic to display.</param>
        /// <returns>Description and values displayed for the statistic.</returns>
        public string this [string key]
        {
            get { return m_statistics[key]; }
            set { m_statistics[key] = value; }
        }
    }
}
