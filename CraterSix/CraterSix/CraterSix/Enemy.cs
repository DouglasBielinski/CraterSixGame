using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraterSix
{
    class Enemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        public Vector3 position { get; protected set;}
        Vector3 direction;
        public int scale { get; protected set;}

        public Enemy(Model m)
            : base(m)
        {
        }

        public Enemy(Model m, Vector3 Position, Vector3 Direction, float yawAngle, float pitchAngle, float rollAngle, Matrix Rotation, int scale)
            : base(m)
        {
            world = Matrix.CreateTranslation(Position);
            this.yawAngle = yawAngle;
            this.pitchAngle = pitchAngle;
            this.rollAngle = rollAngle;
            this.scale = scale;
            rotation = Rotation;
            direction = Direction;
            position = Position;
        }

        public override void Update()
        {
            // Rotate model
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle,
            pitchAngle, rollAngle);
            // Move model
            world *= Matrix.CreateTranslation(direction);
        }

        public override Matrix GetWorld()
        {
            return Matrix.CreateScale(scale) * rotation * world;
            //return rotation * world;
        }
    }

}

