using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rememboros
{
    public enum LadderZone
    {
        Top,
        Middle,
        Bottom
    }

    public enum MotorState
    {
        OnGround,           // on ground
        Jumping,            //  velocity.y > 0
        Falling,            //  velocity.y < 0
        WallSliding,        // on wall
        Dashing,            // dashing
        OnLadder,           // freedom state, can move vertically and horizontally freely
        Frozen,             // frozen
        OnLedge,            // ledge grabbing
        Attack,                // attack
        AirAttack,           // air attack
        CustomAction       // custom action performed by action module
    }

    public struct MotorCollision2D
    {
        public enum CollisionSurface
        {
            None = 0x0,
            Ground = 0x1,
            Left = 0x2,
            Right = 0x4,
            Ceiling = 0x8
        }

        public CollisionSurface Surface;

        public bool IsSurface(CollisionSurface surface)
        {
            return (surface & Surface) != CollisionSurface.None;
        }
    }
}