﻿using AI2D.Objects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Game
    {
        private bool _shutdown = false;
        private System.Threading.Thread _graphicsThread;
        public UserInput Input { get; private set; }
        public Display Display { get; private set; }
        public ActorAssets Actors { get; private set; }

        public PointD _backgroundOffset { get; set; } = new PointD();
        private RectangleD _renderedBackgroundArea { get; set; } = new RectangleD();

        public void Start()
        {
            _renderedBackgroundArea = new RectangleD(0, 0, Display.VisibleSize.Width, Display.VisibleSize.Height);

            for (int i = 0; i < 100; i++)
            {
                Actors.CreateStar();
            }

            for (int i = 0; i < 0; i++)
            {
                Actors.CreateEnemy();
            }

            _shutdown = false;
            _graphicsThread.Start();
            Actors.ShowNewPlayer();
        }

        public void Stop()
        {
            _shutdown = true;
        }

        #region ~/ctor.

        public Game(Control drawingSurface, Size visibleSize)
        {
            Display = new Display(drawingSurface, visibleSize);
            Actors = new ActorAssets(this);
            Input = new UserInput(this);

            _graphicsThread = new System.Threading.Thread(GraphicsThreadProc);
        }

        #endregion

        #region Engine Graphics.

        private void GraphicsThreadProc()
        {
            while (_shutdown == false)
            {
                AdvanceFrame();
                System.Threading.Thread.Sleep(10);
            }
        }

        void AdvanceFrame()
        {
            #region Enemies Frame Advancement.

            foreach (var enemy in Actors.Enemies)
            {
                enemy.AdvanceFrame();

                if (Actors.Player.Visable)
                {
                    double distanceToPlayer = Utility.CalculeDistance(enemy, Actors.Player);
                    if (distanceToPlayer < 100)
                    {
                        enemy.FireGun();
                    }

                    if (enemy.X < (0 - (enemy.Size.Width + 40)) || enemy.Y < (0 - (enemy.Size.Height + 40))
                        || enemy.X >= (Display.VisibleSize.Width + enemy.Size.Width) + 40
                        || enemy.Y >= (Display.VisibleSize.Height + enemy.Size.Height) + 40)
                    {
                        enemy.MoveInDirectionOf(Actors.Player);
                    }

                    if (enemy.Intersects(Actors.Player))
                    {
                        Actors.Player.Hit();
                    }
                }

                enemy.X += (enemy.Velocity.Angle.X * enemy.Velocity.Speed);
                enemy.Y += (enemy.Velocity.Angle.Y * enemy.Velocity.Speed);

                foreach (var bullet in Actors.Bullets)
                {
                    if (bullet.FiredFromType == FiredFromType.Player)
                    {
                        if (bullet.Intersects(enemy))
                        {
                            enemy.Hit();
                            bullet.ReadyForDeletion = true;
                        }
                    }
                }
            }

            #endregion

            #region Player Frame Advancement.

            Actors.Player.AdvanceFrame();

            if (Input.IsKeyPressed(PlayerKey.Fire))
            {
                Actors.Player.FireGun();
            }

            double bgAppliedOffsetX = 0;
            double bgAppliedOffsetY = 0;
      
            if (Input.IsKeyPressed(PlayerKey.Forward))
            {
                //Close to the right wall and travelling in that direction.
                if (Actors.Player.X > Display.VisibleSize.Width - (Actors.Player.Size.Width + 100) && Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the bottom wall and travelling in that direction.
                if (Actors.Player.Y > Display.VisibleSize.Height - (Actors.Player.Size.Height + 100) && Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                //Close to the left wall and travelling in that direction.
                if (Actors.Player.X < 100 && Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the top wall and travelling in that direction.
                if (Actors.Player.Y < 100 && Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                Actors.Player.X += (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed) - bgAppliedOffsetX;
                Actors.Player.Y += (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed) - bgAppliedOffsetY;
                Actors.ShipEngineRoar.Play();
            }
            else if (Input.IsKeyPressed(PlayerKey.Reverse))
            {
                //Close to the right wall and travelling in that direction.
                if (Actors.Player.X > Display.VisibleSize.Width - (Actors.Player.Size.Width + 100) && Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = -(Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the bottom wall and travelling in that direction.
                if (Actors.Player.Y > Display.VisibleSize.Height - (Actors.Player.Size.Height + 100) && Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = -(Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                //Close to the left wall and travelling in that direction.
                if (Actors.Player.X < 100 && Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = -(Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the top wall and travelling in that direction.
                if (Actors.Player.Y < 100 && Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = -(Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                Actors.Player.X -= (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed) + bgAppliedOffsetX;
                Actors.Player.Y -= (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed) + bgAppliedOffsetY;
                Actors.ShipEngineRoar.Play();
            }
            else
            {
                Actors.ShipEngineRoar.Fade();
            }

            //Scroll the background.
            _backgroundOffset.X += bgAppliedOffsetX;
            _backgroundOffset.Y += bgAppliedOffsetY;

            //Keep track of the areas for which we have rendered background.
            var bgRange = new RectangleD()
            {
                X = _backgroundOffset.X < _renderedBackgroundArea.X ? _backgroundOffset.X : _renderedBackgroundArea.X,
                Y = _backgroundOffset.Y < _renderedBackgroundArea.Y ? _backgroundOffset.Y : _renderedBackgroundArea.Y,
                Width = _backgroundOffset.X + Display.VisibleSize.Width > _renderedBackgroundArea.Width ? _backgroundOffset.X + Display.VisibleSize.Width : _renderedBackgroundArea.Width,
                Height = _backgroundOffset.Y + Display.VisibleSize.Height > _renderedBackgroundArea.Height ? _backgroundOffset.Y + Display.VisibleSize.Height : _renderedBackgroundArea.Width
            };

            _renderedBackgroundArea = bgRange;

            Actors.DebugBlock.Text = $"{bgRange.X.ToString("####.###")}x,{bgRange.Y.ToString("####.###")}y"
                + $" x {bgRange.Width.ToString("####.###")}x,{bgRange.Height.ToString("####.###")}y";

            //Actors.DebugBlock.Text = $"P: {Actors.Player.X.ToString("####.###")},{Actors.Player.Y.ToString("####.###")}"
            //    + $" B: {_backgroundOffset.X.ToString("####.###")},{_backgroundOffset.Y.ToString("####.###")}";

            if (Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                Actors.Player.Rotate(-Actors.Player.RotationSpeed);
            }
            else if (Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                Actors.Player.Rotate(Actors.Player.RotationSpeed);
            }

            if (Input.IsKeyPressed(PlayerKey.Escape))
            {
                Actors.Player.Visable = false;
            }

            if (Actors.Player.ReadyForDeletion)
            {
                Actors.Player.Cleanup();
            }

            #endregion

            #region Bullet Frame Advancement.

            foreach (var bullet in Actors.Bullets)
            {
                bullet.AdvanceFrame();

                if (bullet.X < 0)
                {
                    bullet.ReadyForDeletion = true;
                }
                else if (bullet.X >= Display.VisibleSize.Width)
                {
                    bullet.ReadyForDeletion = true;
                }

                if (bullet.Y < 0)
                {
                    bullet.ReadyForDeletion = true;
                }
                else if (bullet.Y >= Display.VisibleSize.Height)
                {
                    bullet.ReadyForDeletion = true;
                }

                bullet.X += (bullet.Velocity.Angle.X * bullet.Velocity.Speed);
                bullet.Y += (bullet.Velocity.Angle.Y * bullet.Velocity.Speed);

                if (bullet.FiredFromType == FiredFromType.Enemy)
                {
                    if (bullet.Intersects(Actors.Player))
                    {
                        Actors.Player.Hit();
                        bullet.ReadyForDeletion = true;
                    }
                }
            }

            #endregion

            #region Stars Frame Advancement.

            if (bgAppliedOffsetX != 0 || bgAppliedOffsetY != 0)
            {
                foreach (var star in Actors.Stars)
                {
                    star.AdvanceFrame();

                    star.X -= bgAppliedOffsetX;
                    star.Y -= bgAppliedOffsetY;
                }
            }

            #endregion

            #region Cleanup (cant be done in a foreach).

            for (int i = 0; i < Actors.Enemies.Count; i++)
            {
                if (Actors.Enemies[i].ReadyForDeletion)
                {
                    Actors.DeleteEnemy(Actors.Enemies[i]);
                }
            }

            for (int i = 0; i < Actors.Bullets.Count; i++)
            {
                if (Actors.Bullets[i].ReadyForDeletion)
                {
                    Actors.DeleteBullet(Actors.Bullets[i]);
                }
            }

            #endregion

            //Actors.DebugBlock.Text = $"HP: {Actors.Player.HitPoints}";
        }

        public void RenderObjects(Graphics dc)
        {
            Actors.RenderObjects(dc);
        }

        #endregion
    }
}
