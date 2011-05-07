using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Phantom
{
    public class Statistics
    {
        protected int m_textTexture;
        protected Bitmap m_textBitmap;
        protected Font m_textFont;
        protected SolidBrush m_textBrush;
        protected Vector2 m_textPosition = Vector2.One;
        
        protected int m_frameRate = 0;
        protected int m_frameCounter = 0;
        protected TimeSpan m_elapsedTime = TimeSpan.Zero;
        
        protected int m_width;
        protected int m_height;
        
        
        public Statistics (Game game)
        {
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.ReplaceExt);
            
            // Load the font to be used for drawing text
            // FIXME: Do something when can't load the font
            m_textFont = new Font(FontFamily.GenericSansSerif, 16);
            
            m_textBrush = new SolidBrush(Color.White);
            
            // Create Bitmap and OpenGL texture
            m_textBitmap = new Bitmap(game.Width, game.Height);
 
            m_textTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, m_textTexture);
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
                    string fps = string.Format("fps: {0}", m_frameRate);
                    
                    System.Console.WriteLine(string.Format("{0}/{1}", m_textBitmap.Width, m_textBitmap.Height));
                    
                    graphics.Clear(Color.Transparent);
                    //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    graphics.DrawString(fps, m_textFont, m_textBrush, m_textPosition.X, m_textPosition.Y);
                }
                
                // Upload the Bitmap to OpenGL
                BitmapData data = m_textBitmap.LockBits(new Rectangle(0, 0, m_textBitmap.Width, m_textBitmap.Height),
                                        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, m_textBitmap.Width, m_textBitmap.Height, 0,
                                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 
                //GL.Finish();
                m_textBitmap.UnlockBits(data);
            }
        }
        
        
        public void Draw(FrameEventArgs e)
        {
            m_frameCounter++;
            
            GL.PushMatrix();
            GL.LoadIdentity();
            
            Matrix4 ortho_projection = Matrix4.CreateOrthographicOffCenter(0, m_textBitmap.Width, m_textBitmap.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Projection);
 
            GL.PushMatrix();
            GL.LoadMatrix(ref ortho_projection);
            
   
            // Finally, render text using a quad.
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, m_textBitmap.Width, m_textBitmap.Height, 0, -1, 1);
             
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.DstAlpha);
            
            //GL.BindTexture(TextureTarget.Texture2D, m_textTexture);
             
            /*GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(1f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 1f);
            GL.End();*/
            
            
            GL.Begin(BeginMode.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
                GL.TexCoord2(1, 0); GL.Vertex2(m_textBitmap.Width, 0);
                GL.TexCoord2(1, 1); GL.Vertex2(m_textBitmap.Width, m_textBitmap.Height);
                GL.TexCoord2(0, 1); GL.Vertex2(0, m_textBitmap.Height);
                GL.End();
                GL.PopMatrix();
            
            GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.Texture2D);
 
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
            
        }
        
    }
}
