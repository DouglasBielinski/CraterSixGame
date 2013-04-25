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

namespace CraterSix
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera { get; protected set; }

        Texture2D texture;

        ModelManager modelManager;

        VertexPositionTexture[] verts;
        VertexBuffer vertexBuffer;
        BasicEffect effect;
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Console.WriteLine("Game initializing... ");
            base.Initialize();
            // Initialize camera
            camera = new Camera(this, new Vector3(0, 0, 5),
            Vector3.Zero, Vector3.Up);
            Components.Add(camera);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Time for models
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            texture = Content.Load<Texture2D>(@"Resources\test");
            // Initialize vertices
            verts = new VertexPositionTexture[4];
            verts[0] = new VertexPositionTexture(
                new Vector3(-1, 1, 0), new Vector2(0, 0));
            verts[1] = new VertexPositionTexture(
                new Vector3(1, 1, 0), new Vector2(1, 0));
            verts[2] = new VertexPositionTexture(
                new Vector3(-1, -1, 0), new Vector2(0, 1));
            verts[3] = new VertexPositionTexture(
                new Vector3(1, -1, 0), new Vector2(1, 1));

            // Set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture),
                verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            // Initialize the BasicEffect
            effect = new BasicEffect(GraphicsDevice);

            // Set cullmode to none <THIS SHOULD BE REMOVED TO IMPROVE PERFORMANCE>
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            // Translation
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                worldTranslation *= Matrix.CreateTranslation(-.01f, 0, 0);
            if (keyboardState.IsKeyDown(Keys.Right))
                worldTranslation *= Matrix.CreateTranslation(.01f, 0, 0);
            // Rotation
            worldRotation *= Matrix.CreateFromYawPitchRoll(
                MathHelper.PiOver4 / 60,
                0,
                0);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            //Set object and camera info
            effect.World = worldRotation * worldTranslation * worldRotation;
            effect.View = camera.view;
            effect.Projection = camera.projection;
            effect.Texture = texture;
            effect.TextureEnabled = true;

            // Begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 0, 2);
            }

            base.Draw(gameTime);
        }
    }
}
