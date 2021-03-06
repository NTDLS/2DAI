﻿using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors.Enemies
{
    /// <summary>
    /// These are a heavy fighting unit, employing various types of weapons and seemingly various stratigies.
    /// They like to stay at a medium range. Some units have seeking-missiles.
    /// </summary>
    public class EnemyAvvol : EnemyBase
    {
        public const int ScoreMultiplier = 25;

        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Avvol\";
        private readonly string[] _imagePaths = {
            #region images.
            "1.png",
            "2.png",
            "3.png",
            "4.png",
            "5.png",
            "6.png"
            #endregion
        };

        public EnemyAvvol(Core core)
            : base(core, EnemyBase.GetGenericHP(), ScoreMultiplier)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            base.SetHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
            Velocity.MaxSpeed = Utility.Random.Next(Constants.Limits.MaxSpeed - 4, Constants.Limits.MaxSpeed - 2); //Upper end of the speed spectrum.

            AddSecondaryWeapon(new WeaponPhotonTorpedo(_core)
            {
                RoundQuantity = 5,
                FireDelayMilliseconds = 1000,
            });

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 100,
                FireDelayMilliseconds = 500
            });

            AddSecondaryWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 100,
                FireDelayMilliseconds = 500
            });

            if (imageIndex == 0 || imageIndex == 2 || imageIndex == 5)
            {
                AddSecondaryWeapon(new WeaponGuidedFragMissile(_core)
                {
                    RoundQuantity = 10,
                    FireDelayMilliseconds = 2000
                });
            }

            SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
        }

        #region Artificial Intelligence.

        enum AIMode
        {
            Approaching,
            MovingToFallback,
            MovingToApproach,
        }

        const double baseDistanceToKeep = 100;
        double distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
        const double baseFallbackDistance = 400;
        double fallbackDistance;
        Angle<double> fallToAngle;
        AIMode mode = AIMode.Approaching;

        public override void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);

            if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    MoveInDirectionOf(_core.Actors.Player);
                }
                else
                {
                    mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + Utility.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (Utility.Random.NextDouble() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.Angle - fallToAngle;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 1;
                    }
                }

                if (distanceToPlayer > fallbackDistance)
                {
                    mode = AIMode.MovingToApproach;
                }
            }

            if (mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngle(_core.Actors.Player);

                if (deltaAngle > 10)
                {
                    if (deltaAngle >= 180.0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 180.0)
                    {
                        Velocity.Angle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
                }
            }

            if (distanceToPlayer < 700)
            {
                if (distanceToPlayer > 500 && HasSecondaryWeaponAndAmmo(typeof(WeaponGuidedFragMissile)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponGuidedFragMissile));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 300 && HasSecondaryWeaponAndAmmo(typeof(WeaponPhotonTorpedo)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponPhotonTorpedo));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 200 && HasSecondaryWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 100 && HasSecondaryWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponDualVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
            }
        }

        #endregion
    }
}
