using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pain_and_Suffering
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D characterSpriteSheet, rectangleTexture, dungeonTexture;
        List<Rectangle> barriers;

        KeyboardState keyboardState;

        MouseState mouseState;

        Rectangle window, playerCollisionRect, playerDrawRect, leverRect, buttonRect;

        int rows, columns, //number of rows/columns in the spritesheet
            frame, //frame number (column) in the sequence to draw
            frames, //number of frames for each direction, usually same as column
            directionRow, //row number containing frames for current direction
            leftRow, rightRow, upRow, downRow, //row number of each directional set of frames
            width, height;

        float speed, time, frameSpeed;

        Vector2 playerLocation, playerDirection;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Sizes window
            window = new Rectangle(0, 0, 800, 500);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.ApplyChanges();

            //Creates obstacles
            barriers = new List<Rectangle>();
            barriers.Add(new Rectangle(0, 0, 156, 27));
            barriers.Add(new Rectangle(0, 0, 25, 268));
            barriers.Add(new Rectangle(154, 20, 224, 30));
            barriers.Add(new Rectangle(154, 20, 35, 40));
            barriers.Add(new Rectangle(240, 50, 109, 36));
            barriers.Add(new Rectangle(548, 0, 65, 105));
            barriers.Add(new Rectangle(548, 0, 222, 35));
            barriers.Add(new Rectangle(770, 0, 30, 61));
            //barriers.Add(new Rectangle(525, 315, 160, 5)); // barrier in front of door that gets unlocked
           
            barriers.Add(new Rectangle(701, 235, 100, 5));

            barriers.Add(new Rectangle(475, 92, 25, 14));
            barriers.Add(new Rectangle(673, 173, 87, 12));
            barriers.Add(new Rectangle(762, 130, 38, 97));
            barriers.Add(new Rectangle(509, 256, 15, 60));
            barriers.Add(new Rectangle(378, 0, 170, 25));
            barriers.Add(new Rectangle(379, 256, 129, 5));
            barriers.Add(new Rectangle(241, 192, 107, 39));
            barriers.Add(new Rectangle(348, 232, 29, 24));
            barriers.Add(new Rectangle(153, 231, 115, 41));
            barriers.Add(new Rectangle(686, 234, 14, 80));
            barriers.Add(new Rectangle(24, 270, 127, 5));
            barriers.Add(new Rectangle(25, 177, 47, 5));
            barriers.Add(new Rectangle(107, 177, 91, 5));
            barriers.Add(new Rectangle(184, 182, 15, 47));

            //barriers for second screen once it's made. Will temporarily be added to og barrier list
            barriers.Add(new Rectangle(122, 430, 58, 63));
            barriers.Add(new Rectangle(180, 471, 148, 29));
            barriers.Add(new Rectangle(172, 375, 76, 6));
            barriers.Add(new Rectangle(0, 392, 122, 43));
            barriers.Add(new Rectangle(0, 307, 82, 19));
            barriers.Add(new Rectangle(83, 270, 118, 36));
            barriers.Add(new Rectangle(155, 305, 17, 75));
            barriers.Add(new Rectangle(270, 273, 108, 51));
            barriers.Add(new Rectangle(332, 328, 15, 47));
            barriers.Add(new Rectangle(284, 375, 172, 6));
            barriers.Add(new Rectangle(488, 376, 35, 7));
            barriers.Add(new Rectangle(509, 316, 14, 66));
            barriers.Add(new Rectangle(448, 472, 315, 28));
            barriers.Add(new Rectangle(686, 314, 14, 68));
            barriers.Add(new Rectangle(585, 431, 166, 23));
            barriers.Add(new Rectangle(762, 410, 38, 23));
            barriers.Add(new Rectangle(762, 331, 38, 12));

            //Barriers after the door is unlocked, will be on second screen but not third
            barriers.Add(new Rectangle(525, 315, 77, 5));
            barriers.Add(new Rectangle(637, 315, 60, 5));

            //Processing sprite sheet
            rows = 4;
            columns = 9;
            upRow = 0;
            leftRow = 1;
            downRow = 2;
            rightRow = 3;
            directionRow = downRow;

            //Time
            time = 0.0f;
            frameSpeed = 0.08f;
            frames = 9;
            frame = 0;

            //Player
            playerLocation = new Vector2(228, 280);
            playerCollisionRect = new Rectangle(79, 41, 20, 48);
            playerDrawRect = new Rectangle(79, 41, 50, 65);
            speed = 1.5f;

            //Things to interact with
            leverRect = new Rectangle(769, 387, 13, 5);
            buttonRect = new Rectangle(34, 337, 23, 18);

            UpdateRects();

            base.Initialize();

            //width and height
            width = characterSpriteSheet.Width / columns;
            height = characterSpriteSheet.Height / rows;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            characterSpriteSheet = Content.Load<Texture2D>("skeleton_spritesheet");
            rectangleTexture = Content.Load<Texture2D>("rectangle");
            dungeonTexture = Content.Load<Texture2D>("dungeon 1");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            mouseState = Mouse.GetState();

            this.Window.Title = "x = " + mouseState.X + ", y = " + mouseState.Y;

            keyboardState = Keyboard.GetState();

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time > frameSpeed && playerDirection != Vector2.Zero)
            {
                time = 0f;
                frame = (frame + 1) % frames;  // ensures frame is a value 0-8
            }

            SetPlayerDirection();
            playerLocation += playerDirection * speed;
            UpdateRects();

            //Collision detection with window
            if (!window.Contains(playerCollisionRect))
            {
                playerLocation -= playerDirection * speed;
                UpdateRects();
            }

            foreach (Rectangle barrier in barriers)
                if (barrier.Intersects(playerCollisionRect))
                {
                    playerLocation -= playerDirection * speed;
                    UpdateRects();
                }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.Draw(dungeonTexture, window, Color.White);

            //_spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.3f);
            // ^ draws hitbox
            _spriteBatch.Draw(characterSpriteSheet, playerDrawRect,
                new Rectangle(frame * width, directionRow * height, width, height), Color.White);

            foreach (Rectangle barrier in barriers)
                _spriteBatch.Draw(rectangleTexture, barrier, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void UpdateRects()
        {
            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.Location = new Point(playerCollisionRect.X - 15, playerCollisionRect.Y - 15); // the horizontal and vertical offset are both 15 pixels to align the skeleton with our hitbox
        }

        private void SetPlayerDirection()
        {
            playerDirection = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                playerDirection.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                playerDirection.X += 1;
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                playerDirection.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                playerDirection.Y += 1;

            if (playerDirection != Vector2.Zero)
            {
                playerDirection.Normalize(); // Sets the directional vector to the unit vector so the speed is 1 regardless of direction
                if (playerDirection.X < 0)  // Moving left
                    directionRow = leftRow;
                else if (playerDirection.X > 0)  // Moving right
                    directionRow = rightRow;
                else if (playerDirection.Y < 0)  // Moving up
                    directionRow = upRow;
                else
                    directionRow = downRow;
            }

            else
                frame = 0;
        }
    }
}
