using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MDS.Interactable
{
    public class MoveInteraction : InteractableBase
    {

        protected PlayerEpisodeController _player;

        protected override void Awake()
        {
            base.Awake();
            _player = FindObjectOfType<PlayerEpisodeController>();
        }

        public override void Interact()
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            _player.Move(pos);
        }
    }
}
