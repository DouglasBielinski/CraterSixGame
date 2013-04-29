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
        Vector3 direction;

        public Enemy(Model m)
            : base(m)
        {
        }

        public Enemy(Model m, Vector3 Position, Vector3 Direction, float yawAngle, float pitchAngle, float rollAngle)
            : base(m)
        {
            world = Matrix.CreateTranslation(Position);
            this.yawAngle = yawAngle;
            this.pitchAngle = pitchAngle;
            this.rollAngle = rollAngle;
            direction = Direction;
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
            return rotation * world;
        }
    }

}

