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
using System.Collections;

namespace CraterSix
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Game States
        //0 - Splash Screen
        //1 - Active Game
        //2 - Paused
        //3 - Game Over. Credits
        public int gameState = 0;
        //0 - Start Game
        //1 - Options
        //2 - Exit
        int menuSelection = 0;
        int keyCooldown = 0;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera { get; protected set; }

        Texture2D texture;
        Texture2D bartexture;
        Texture2D stars;
        Texture2D splashscreen;
        SpriteFont hudFont;
        ModelManager modelManager;
        public Random rnd { get; protected set; }

        Song milkyWay;
        ArrayList music = new ArrayList();

        BasicEffect effect;
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;

        public Game1()
        {
            rnd = new Random();

            graphics = new GraphicsDeviceManager(this);
            
            //this.graphics.PreferredBackBufferWidth = 1280;
            //this.graphics.PreferredBackBufferHeight = 1040;

            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;

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
            new Vector3(-20, 0, 5), Vector3.Up);
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
            splashscreen = Content.Load<Texture2D>(@"Resources\SplashPage2");
            bartexture = Content.Load<Texture2D>(@"Resources\bartexture");

            milkyWay = Content.Load<Song>(@"Resources\Audio\MilkyWay");
            music.Add(milkyWay);

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

            //if no music playing, play some music
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                Song currentSong = (Song)music[0];
                MediaPlayer.Play(currentSong);
                //songstart = true;
            }

            if (gameState == 0)
                Console.WriteLine("Menu screen");
            else
            {
                if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
                    Console.WriteLine("Game Time: " + gameTime.TotalGameTime.Seconds + "s");

                // Translation
                //KeyboardState keyboardState = Keyboard.GetState();
                //if (keyboardState.IsKeyDown(Keys.Left))
                //    worldTranslation *= Matrix.CreateTranslation(-.01f, 0, 0);
                //if (keyboardState.IsKeyDown(Keys.Right))
                //    worldTranslation *= Matrix.CreateTranslation(.01f, 0, 0);
                // Rotation
                worldRotation *= Matrix.CreateFromYawPitchRoll(
                    MathHelper.PiOver4 / 60,
                    0,
                    0);

                // TODO: Add your update logic here
            
                base.Update(gameTime);
            }//end if not gameState 0
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null);
            if (gameState == 0)
            {
                RunSplashScreen(gameTime);
                spriteBatch.Draw(splashscreen, new Rectangle(0, 0, 1280, 1040),null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
                spriteBatch.End();
            }
            else
            {

                
                //Set object and camera info
                effect.World = worldRotation * worldTranslation * worldRotation;
                effect.View = camera.view;
                effect.Projection = camera.projection;
                effect.Texture = texture;
                effect.TextureEnabled = true;

                //Draw background
                spriteBatch.Draw(stars, new Rectangle(0, 0, 1280, 1040), Color.White);

                //Draw HUD
                drawHud(gameTime);
                spriteBatch.End();
                base.Draw(gameTime);
            } //end if not gameState 0
            
            
        }
        protected void RunSplashScreen(GameTime gameTime)
        {
            if (gameState == 0)
            {
                if (menuSelection == 0)
                {
                    if (gameTime.TotalGameTime.Milliseconds % 1000 < 500)
                        spriteBatch.DrawString(hudFont, ("Start"), new Vector2(850, 340), Color.Yellow, 0, Vector2.Zero, 3, SpriteEffects.None, 0.2f);
                } else
                    spriteBatch.DrawString(hudFont, ("Start"), new Vector2(850, 340), Color.Yellow, 0, Vector2.Zero, 3, SpriteEffects.None, 0.2f);
                if (menuSelection == 1)
                {
                    if (gameTime.TotalGameTime.Milliseconds % 1000 < 500)
                        spriteBatch.DrawString(hudFont, ("Options"), new Vector2(850, 400), Color.Yellow, 0, Vector2.Zero, 3, SpriteEffects.None, 0.2f);
                }
                else
                    spriteBatch.DrawString(hudFont, ("Options"), new Vector2(850, 400), Color.Yellow, 0, Vector2.Zero, 3, SpriteEffects.None, 0.2f);
                if (menuSelection == 2)
                {
                    if (gameTime.TotalGameTime.Milliseconds % 1000 < 500)
                        spriteBatch.DrawString(hudFont, ("Exit"), new Vector2(850, 460), Color.Yellow, 0, Vector2.Zero, 3, SpriteEffects.None, 0.2f);
                }
                else
                    spriteBatch.DrawString(hudFont, ("Exit"), new Vector2(850, 460), Color.Yellow, 0, Vector2.Zero, 3, SpriteEffects.None, 0.2f);
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && keyCooldown == 0)
                {
                    if (menuSelection == 0)
                        menuSelection = 1;
                    else if (menuSelection == 1)
                        menuSelection = 2;
                    else
                        menuSelection = 0;
                    keyCooldown = 20;
                }
                else if (keyCooldown > 0)
                    keyCooldown--;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && keyCooldown == 0)
                {
                    if (menuSelection == 0)
                        menuSelection = 2;
                    else if (menuSelection == 1)
                        menuSelection = 0;
                    else
                        menuSelection = 1;
                    keyCooldown = 20;
                }
                else if (keyCooldown > 0)
                    keyCooldown--;
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (menuSelection == 0)
                        gameState = 1;
                    else if (menuSelection == 2)
                        this.Exit();
                }
                
                //credit to us! wooh!
                spriteBatch.DrawString(hudFont, ("Game by the CraterSix team: Douglas Bielinski, Edward Loza, Jaime Baylon, Tracey Steinman, and Onur Gunduz"), new Vector2(0, 700), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0.2f);
            }//endwhile
        }

        protected void drawHud(GameTime gameTime)
        {
            //Look at this, it's flashing!
            //if(gameTime.TotalGameTime.Milliseconds % 500 != 0)
                //spriteBatch.DrawString(hudFont, ("Press 'K' to toggle between mouse and keyboard look!"), new Vector2(500, 950), Color.Yellow);
            //Time Elapsed
            spriteBatch.DrawString(hudFont, ("Time Elapsed: " + gameTime.TotalGameTime.Seconds), new Vector2(0, 0), Color.Yellow);
            //Models
            spriteBatch.DrawString(hudFont, ("Models: " + modelManager.modelCount), new Vector2(0, 20), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("MouseMovement: " + camera.mouseMovement), new Vector2(0, 40), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("MouseState: " + camera.prevMouseState), new Vector2(0, 60), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("Camera UP: " + MathHelper.ToDegrees(camera.cameraUp.X) + 
                " "+MathHelper.ToDegrees(camera.cameraUp.Y)+
                " " + MathHelper.ToDegrees(camera.cameraUp.Z)), new Vector2(0, 80), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("Camera DIRECTION: " + camera.cameraDirection.X +
                " " + MathHelper.ToDegrees(camera.cameraDirection.Y) +
                " " + MathHelper.ToDegrees(camera.cameraDirection.Z)), new Vector2(0, 100), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("Speed: "+ camera.speed), new Vector2(0, 120), Color.Yellow);
            spriteBatch.DrawString(hudFont, ("Afterburner: " + camera.afterburner), new Vector2(0, 140), Color.Cyan);
            spriteBatch.DrawString(hudFont, ("Music: " + MediaPlayer.State), new Vector2(0, 160), Color.Green);

            //speed bar
            Rectangle speedbar = new Rectangle(1100, 300, 10, (int)(camera.speed * 10000));
            Rectangle speedbarbackdrop = new Rectangle(1100, 300, 10, 700);
            spriteBatch.Draw(bartexture, speedbar, null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
            spriteBatch.Draw(bartexture, speedbarbackdrop, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.09f);
            Rectangle afterburnerbar = new Rectangle(1130, 300, 10, (int)(camera.afterburner * 140));
            Rectangle afterburnerbarbackdrop = new Rectangle(1130, 300, 10, 700);
            spriteBatch.Draw(bartexture, afterburnerbar, null, Color.Cyan, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
            spriteBatch.Draw(bartexture, afterburnerbarbackdrop, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.09f);

            spriteBatch.DrawString(hudFont, ("W to accelerate    K to toggle between mouse/keyboard pitch/yaw"), new Vector2(20, 900), Color.GhostWhite, 0, Vector2.Zero, 2, SpriteEffects.None, 0.2f);
            if (gameTime.TotalGameTime.Milliseconds % 1000 < 500)
                spriteBatch.DrawString(hudFont, ("Shift for afterburners"), new Vector2(20, 940), Color.Cyan, 0, Vector2.Zero, 2, SpriteEffects.None, 0.2f);
        }
    }
}
