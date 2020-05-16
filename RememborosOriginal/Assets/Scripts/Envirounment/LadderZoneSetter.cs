using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rememboros
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LadderZoneSetter : MonoBehaviour
    {
        public LadderZone Zone;

        public void OnTriggerEnter2D(Collider2D o)
        {
            Player character = o.GetComponent<Player>();
            if (character)
            {
                character.SetLadderZone(Zone);
            }
        }
        public void OnTriggerStay2D(Collider2D o)
        {
            Player character = o.GetComponent<Player>();
            if (character)
            {
                character.SetLadderZone(Zone);
            }
        }

        private void Reset()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }
}

