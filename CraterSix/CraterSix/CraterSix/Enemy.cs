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
        Vector3 up;

        public Enemy(Model m)
            : base(m)
        {
        }

        public Enemy(Model m, float yawAngle, float pitchAngle, float rollAngle)
            : base(m)
        {
            this.yawAngle = yawAngle;
            this.pitchAngle = pitchAngle;
            this.rollAngle = rollAngle;
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

