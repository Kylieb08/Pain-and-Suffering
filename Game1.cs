﻿using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pain_and_Suffering
{
    enum Screen
    {
        Intro,
        Story,
        Controls,
        Dungeon1,
        Dungeon2,
        Dungeon3,
        End
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D characterSpriteSheet, rectangleTexture, leverFlippedTexture, buttonPressedTexture, 
            tunnelTexture, dungeonTexture, dungeon2Texture, dungeon3Texture, exitTexture, hintTexture, ETexture,
            storyTexture, controlsTexture;
        List<Rectangle> barriers, barriers2;

        KeyboardState keyboardState;

        MouseState mouseState;

        Rectangle window, playerCollisionRect, playerDrawRect, hintShowingRect, E,
            leverRect, buttonRect, secondButtonRect, lockedDoor, exitDoor, hintRect;

        int rows, columns, //number of rows/columns in the spritesheet
            frame, //frame number (column) in the sequence to draw
            frames, //number of frames for each direction, usually same as column
            directionRow, //row number containing frames for current direction
            leftRow, rightRow, upRow, downRow, //row number of each directional set of frames
            width, height;

        float speed, time, frameSpeed, seconds;

        Vector2 playerLocation, playerDirection, ESpeed;

        Screen screen;

        bool leverFlipped, buttonPressed, secondButtonPressed, hintShowing;


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
            screen = Screen.Intro;
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

            barriers2 = new List<Rectangle>();
            //Barriers after the door is unlocked, will be on second screen but not third
            barriers2.Add(new Rectangle(525, 315, 77, 5));
            barriers2.Add(new Rectangle(637, 315, 60, 5));

            /* barrier for locked door. Couldn't be on list because it's only
            on the first screen and I didn't want to make a whole new list minus one barrier*/
            lockedDoor = new Rectangle(525, 315, 160, 5);

            //Rectangle for exit door
            exitDoor = new Rectangle(74, 20, 30, 75);

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
            secondButtonRect = new Rectangle(300, 98, 22, 16);
            hintRect = new Rectangle(329, 398, 52, 34);
            hintShowingRect = new Rectangle(200, 50, 400, 400);

            //E
            E = new Rectangle(340, 370, 20, 25);
            ESpeed = new Vector2(0, 1);

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
            dungeon2Texture = Content.Load<Texture2D>("dungeon 2");
            dungeon3Texture = Content.Load<Texture2D>("dungeon 3");
            exitTexture = Content.Load<Texture2D>("dungeon exit");
            leverFlippedTexture = Content.Load<Texture2D>("lever flipped");
            buttonPressedTexture = Content.Load<Texture2D>("button pressed 1");
            hintTexture = Content.Load<Texture2D>("hint no bg");
            ETexture = Content.Load<Texture2D>("E");

            //intro screens
            tunnelTexture = Content.Load<Texture2D>("dungeon intro");
            storyTexture = Content.Load<Texture2D>("dungeon story");
            controlsTexture = Content.Load<Texture2D>("dungeon controls");
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

            if (screen == Screen.Intro)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    screen = Screen.Story;
                    buttonPressed = false;
                    leverFlipped = false;
                }
            }

            else if (screen == Screen.Story)
            {
                if (keyboardState.IsKeyDown((Keys)Keys.C))
                {
                    screen = Screen.Controls;
                }
            }

            else if (screen == Screen.Controls)
            {
                if (keyboardState.IsKeyDown((Keys)Keys.Space))
                {
                    screen = Screen.Dungeon1;
                }
            }

            else if (screen == Screen.Dungeon1)
            {
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

                //Collision detection with barriers
                foreach (Rectangle barrier in barriers)
                    if (barrier.Intersects(playerCollisionRect))
                    {
                        playerLocation -= playerDirection * speed;
                        UpdateRects();
                    }

                foreach (Rectangle barrier2 in barriers2)
                    if (barrier2.Intersects(playerCollisionRect))
                    {
                        playerLocation -= playerDirection * speed;
                        UpdateRects();
                    }

                if (lockedDoor.Intersects(playerCollisionRect))
                {
                    playerLocation -= playerDirection * speed;
                    UpdateRects();
                }

                //Interactions with buttons etc.
                if (leverRect.Intersects(playerCollisionRect))
                {
                    if (keyboardState.IsKeyDown(Keys.E))
                    {
                        leverFlipped = true;
                    }
                }

                if (buttonRect.Intersects(playerCollisionRect))
                {
                    if (keyboardState.IsKeyDown(Keys.E))
                    {
                        buttonPressed = true;
                    }
                }

                if (hintRect.Intersects(playerCollisionRect) && keyboardState.IsKeyDown(Keys.E))
                {
                    hintShowing = true;
                }
                else
                    hintShowing = false;

                if (buttonPressed == true && leverFlipped == true)
                    screen = Screen.Dungeon2;

                //E moves
                seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (seconds >= 1)
                {
                    E.Y += (int)ESpeed.Y;

                    if (E.Y >= 372)
                        ESpeed *= -1;

                    if (E.Y <= 371)
                        ESpeed *= -1;

                    seconds = 0;
                }



                base.Update(gameTime);
            }

            else if (screen == Screen.Dungeon2)
            {
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

                //Collision detection with barriers
                foreach (Rectangle barrier in barriers)
                    if (barrier.Intersects(playerCollisionRect))
                    {
                        playerLocation -= playerDirection * speed;
                        UpdateRects();
                    }

                foreach (Rectangle barrier2 in barriers2)
                    if (barrier2.Intersects(playerCollisionRect))
                    {
                        playerLocation -= playerDirection * speed;
                        UpdateRects();
                    }

                //Interactions with second button, hint
                if (secondButtonRect.Intersects(playerCollisionRect))
                {
                    if (keyboardState.IsKeyDown((Keys)Keys.E))
                        secondButtonPressed = true;
                }

                if (hintRect.Intersects(playerCollisionRect) && keyboardState.IsKeyDown(Keys.E))
                {
                    hintShowing = true;
                }
                else
                    hintShowing = false;

                //E moves
                seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (seconds >= 1)
                {
                    E.Y += (int)ESpeed.Y;

                    if (E.Y >= 372)
                        ESpeed *= -1;

                    if (E.Y <= 371)
                        ESpeed *= -1;

                    seconds = 0;
                }

                base.Update(gameTime);


                if (secondButtonPressed)
                    screen = Screen.Dungeon3;
            }

            else if (screen == Screen.Dungeon3)
            {
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

                //Collision detection with barriers
                foreach (Rectangle barrier in barriers)
                    if (barrier.Intersects(playerCollisionRect))
                    {
                        playerLocation -= playerDirection * speed;
                        UpdateRects();
                    }

                foreach (Rectangle barrier2 in barriers2)
                    if (barrier2.Intersects(playerCollisionRect))
                    {
                        playerLocation -= playerDirection * speed;
                        UpdateRects();
                    }

                //Exit door contains player rect
                if (exitDoor.Contains(playerCollisionRect))
                {
                    screen = Screen.End;
                }

                base.Update(gameTime);

            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            if (screen == Screen.Intro)
            {
                _spriteBatch.Draw(tunnelTexture, window, Color.White);
            }

            else if (screen == Screen.Story)
            {
                _spriteBatch.Draw(storyTexture, window, Color.White);
            }

            else if (screen == Screen.Controls)
            {
                _spriteBatch.Draw(controlsTexture, window, Color.White);
            }

            else if (screen == Screen.Dungeon1)
            {
                if (!leverFlipped && !buttonPressed)
                    _spriteBatch.Draw(dungeonTexture, window, Color.White);

                else if (leverFlipped && !buttonPressed)
                    _spriteBatch.Draw(leverFlippedTexture, window, Color.White);

                else if (buttonPressed && !leverFlipped)
                    _spriteBatch.Draw(buttonPressedTexture, window, Color.White);

                //Draws E
                _spriteBatch.Draw(ETexture, E, Color.White);

                //Draws player
                _spriteBatch.Draw(characterSpriteSheet, playerDrawRect,
                    new Rectangle(frame * width, directionRow * height, width, height), Color.White);

                //Draws hint
                if (hintShowing)
                    _spriteBatch.Draw(hintTexture, hintShowingRect, Color.White);
            }
            
            else if (screen == Screen.Dungeon2)
            {
                _spriteBatch.Draw(dungeon2Texture, window, Color.White);

                //Draws E
                _spriteBatch.Draw(ETexture, E, Color.White);

                //Draws player
                _spriteBatch.Draw(characterSpriteSheet, playerDrawRect,
                    new Rectangle(frame * width, directionRow * height, width, height), Color.White);

                //Draws hint
                if (hintShowing)
                    _spriteBatch.Draw(hintTexture, hintShowingRect, Color.White);
            }

            else if (screen == Screen.Dungeon3)
            {
                _spriteBatch.Draw(dungeon3Texture, window, Color.White);

                //Draws player
                _spriteBatch.Draw(characterSpriteSheet, playerDrawRect,
                    new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            }

            else if (screen == Screen.End)
            {
                _spriteBatch.Draw(exitTexture, window, Color.White);
            }

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
