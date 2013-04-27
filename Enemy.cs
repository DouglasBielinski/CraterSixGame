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

