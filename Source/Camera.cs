using System;

using OpenTK;
using OpenTK.Input;

namespace Phantom
{
    public class Camera
    {
        protected Vector3 m_position = new Vector3(0, 10, 30);
        protected Vector3 m_up = Vector3.UnitY;
        protected Vector3 m_direction;

        protected float m_pitch = 0;
        protected float m_pitchLimit = MathHelper.DegreesToRadians(80);

        protected const float m_speed = 0.25f;
        protected const float m_mouseSpeedX = 0.25f;
        protected const float m_mouseSpeedY = 0.15f;

        protected MouseState m_prevMouse;


        /// <summary>
        /// Creates the instance of the camera.
        /// </summary>
        public Camera (Game game)
        {
            // Create the direction vector and normalize it since it will be used for movement
            m_direction = Vector3.Zero - m_position;
            m_direction.Normalize();

            // Create default camera matrices
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.Width / (float)game.Height, 0.01f, 1000);
            View = CreateLookAt();
        }


        /// <summary>
        /// Creates the instance of the camera at the given location.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">The target towards which the camera is pointing.</param>
        public Camera(Game game, Vector3 position, Vector3 target) : this(game)
        {
            m_position = position;
            m_direction = target - m_position;
            m_direction.Normalize();

            View = CreateLookAt();
        }


        /// <summary>
        /// Handle the camera movement using user input.
        /// </summary>
        protected virtual void ProcessInput()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            // Move camera with WASD keys
            if (keyboard.IsKeyDown(Key.W))
                // Move forward and backwards by adding m_position and m_direction vectors
                m_position += m_direction * m_speed;

            if (keyboard.IsKeyDown(Key.S))
                m_position -= m_direction * m_speed;

            if (keyboard.IsKeyDown(Key.A))
                // Strafe by adding a cross product of m_up and m_direction vectors
                m_position += Vector3.Cross(m_up, m_direction) * m_speed;

            if (keyboard.IsKeyDown(Key.D))
                m_position -= Vector3.Cross(m_up, m_direction) * m_speed;

            if (keyboard.IsKeyDown(Key.Space))
                m_position += m_up * m_speed;

            if (keyboard.IsKeyDown(Key.ControlLeft) || keyboard.IsKeyDown(Key.X))
                m_position -= m_up * m_speed;


            // Calculate yaw to look around with a mouse
            m_direction = Vector3.Transform(m_direction,
                Matrix4.CreateFromAxisAngle(m_up, -MathHelper.DegreesToRadians(m_mouseSpeedX) * (mouse.X - m_prevMouse.X))
            );

            // Pitch is limited to m_pitchLimit
            float angle = MathHelper.DegreesToRadians(m_mouseSpeedY) * (mouse.Y - m_prevMouse.Y);
            if (Math.Abs(m_pitch + angle) < m_pitchLimit)
            {
                m_direction = Vector3.Transform(m_direction,
                    Matrix4.CreateFromAxisAngle(Vector3.Cross(m_up, m_direction), angle)
                );
                m_pitch += angle;
            }

            m_prevMouse = mouse;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        public void Update()
        {
            // Handle camera movement
            ProcessInput();

            View = CreateLookAt();
        }


        /// <summary>
        /// Create a view (modelview) matrix using camera vectors.
        /// </summary>
        protected Matrix4 CreateLookAt()
        {
            return Matrix4.LookAt(m_position, m_position + m_direction, m_up);
        }


        /// <summary>
        /// Position vector accessor.
        /// </summary>
        public Vector3 Position
        {
            get { return m_position; }
            protected set { m_position = value; }
        }

        /// <summary>
        /// Target vector accessor.
        /// </summary>
        public Vector3 Target
        {
            get { return m_position + m_direction; }
        }

        /// <summary>
        /// View (modelview) matrix accessor.
        /// </summary>
        public Matrix4 View { get; protected set; }

        /// <summary>
        /// Projection matrix accessor.
        /// </summary>
        public Matrix4 Projection { get; protected set; }

    }
}
