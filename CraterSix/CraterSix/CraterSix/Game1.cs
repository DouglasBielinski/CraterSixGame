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
        Texture2D stars;
        SpriteFont hudFont;
        ModelManager modelManager;
        public Random rnd { get; protected set; }

        VertexPositionTexture[] verts;
        VertexBuffer vertexBuffer;
        BasicEffect effect;
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        public Game1()
        {
            rnd = new Random();

            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 1040;

            this.graphics.IsFullScreen = true;

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

            hudFont = Content.Load<SpriteFont>(@"SpriteFont1");
            texture = Content.Load<Texture2D>(@"Resources\test");
            stars = Content.Load<Texture2D>(@"Resources\stars");
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

            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
                Console.WriteLine("Game Time: " + gameTime.TotalGameTime.Seconds+"s");

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

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null);
            //Draw background
            spriteBatch.Draw(stars, new Rectangle(0, 0, 1280, 1040), Color.White);

            //Draw HUD
            drawHud(gameTime);
            spriteBatch.End();
            // Begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, verts, 0, 2);
            }

            base.Draw(gameTime);
        }

        protected void drawHud(GameTime gameTime)
        {
            //Look at this, it's flashing!
            if(gameTime.TotalGameTime.Milliseconds % 500 != 0)
                spriteBatch.DrawString(hudFont, ("Press 'K' to toggle between mouse and keyboard look!"), new Vector2(500, 950), Color.Yellow);
            //Time Elapsed
            spriteBatch.DrawString(hudFont, ("Time Elapsed: " + gameTime.TotalGameTime.Seconds), new Vector2(0, 0), Color.Yellow);
            //Models
            spriteBatch.DrawString(hudFont, ("Models: " + modelManager.modelCount), new Vector2(0, 20), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("MouseMovement: " + camera.mouseMovement), new Vector2(0, 40), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("MouseState: " + camera.prevMouseState), new Vector2(0, 60), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("Camera UP: " + MathHelper.ToDegrees(camera.cameraUp.X) + 
                " "+MathHelper.ToDegrees(camera.cameraUp.Y)+
                " " + MathHelper.ToDegrees(camera.cameraUp.Z)), new Vector2(0, 80), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("Camera DIRECTION: " + MathHelper.ToDegrees(camera.cameraDirection.X) +
                " " + MathHelper.ToDegrees(camera.cameraDirection.Y) +
                " " + MathHelper.ToDegrees(camera.cameraDirection.Z)), new Vector2(0, 100), Color.Yellow);
        }
    }
}
