using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace CraterSix
{

    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors
        public Vector3 cameraPosition { get; protected set; }
        public Vector3 cameraDirection;
        public Vector3 cameraUp;

        // Speed
        float speed = 2;

        public Boolean mouseMovement = false;
        int buttonCooldown = 0;
        // Mouse stuff
        public MouseState prevMouseState;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();


            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 3000);
        }

        public override void Initialize()
        {
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //toggle mouse movement
            if (Keyboard.GetState().IsKeyDown(Keys.K) && buttonCooldown == 0)
            {
                buttonCooldown = 20;
                mouseMovement = !mouseMovement;
            }
            if (buttonCooldown > 0)
                buttonCooldown--;

            //cameraUp.X = 0;
            //cameraUp.Z = 0;
            //cameraDirection.Y = 0;
            //if (cameraDirection.X < 0.0001 && cameraDirection.X > 0)
            //    cameraDirection.X = 1;
            //if (cameraDirection.Z < 0.0001 && cameraDirection.Z > 0)
            //    cameraDirection.Z = 1;

            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition += cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= cameraDirection * speed;

            // Move side to side
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;

            if (mouseMovement)
            {
                //Mouse Yaw
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) *
                    (Mouse.GetState( ).X - prevMouseState.X)));
                //Mouse Pitch
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                    (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));
                cameraUp = Vector3.Transform(cameraUp,
                    Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                    (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));
            }
            else
            {
                // Keyboard Yaw
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    cameraDirection = Vector3.Transform(cameraDirection,
                        Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) *
                        2));
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    cameraDirection = Vector3.Transform(cameraDirection,
                        Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) *
                        -2));
                // Keyboard Pitch
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    cameraDirection = Vector3.Transform(cameraDirection,
                        Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                        (MathHelper.PiOver4 / 100) *
                        -2));
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    cameraDirection = Vector3.Transform(cameraDirection,
                        Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                        (MathHelper.PiOver4 / 100) *
                        2));
            }

            //Hijacking mouse controls, looping them
            //this code prevents clicking outside the window
            if (Mouse.GetState().X <= 1)
                Mouse.SetPosition(1277, Mouse.GetState().Y);
            if (Mouse.GetState().X >= 1278)
                Mouse.SetPosition(3, Mouse.GetState().Y);
            if (Mouse.GetState().Y <= 1)
                Mouse.SetPosition(Mouse.GetState().X, 1021);
            if (Mouse.GetState().Y >= 1022)
                Mouse.SetPosition(Mouse.GetState().X, 3);
            // Reset prevMouseState
            prevMouseState = Mouse.GetState();

            // Recreate the camera view matrix
            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }
    }
}