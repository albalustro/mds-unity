using System;
using System.Collections;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MDS.Player
{
    public class PlayerController : MDSBehaviour
    {
        private PlayerEpisodeController _episodeController;
        private PlayerAnimController _animController;

        protected override void Awake()
        {
            base.Awake();

            _episodeController = GetComponent<PlayerEpisodeController>();
            _animController = GetComponent<PlayerAnimController>();
        }

        private void Start()
        {
//            _animController.RestoreOriginalAvatar();
        }

        public void SetAvatar(PlayerAvatar? avatar, bool setMounted = false, bool setIdleBack = false)
        {
            if (avatar.HasValue)
                _animController.SetAvatar(avatar.Value);
            _animController.SetMounted(setMounted);
            _animController.SetIdleBack(setIdleBack);
        }

        public void RestoreOriginalAvatar()
        {
            _animController.RestoreOriginalAvatar();
        }

#if UNITY_EDITOR

        [InspectorButton]
        void SetBarril()
        {
            _animController.SetAvatar(PlayerAvatar.Barril);
        }

        [InspectorButton]
        void SetBlup()
        {
            _animController.SetAvatar(PlayerAvatar.Blup);
        }

        [InspectorButton]
        void SetDraco()
        {
            _animController.SetAvatar(PlayerAvatar.Draco);
        }

        [InspectorButton]
        void SetNave()
        {
            _animController.SetAvatar(PlayerAvatar.Nave);
        }

        [InspectorButton]
        void SetRobot()
        {
            _animController.SetAvatar(PlayerAvatar.Robot);
        }

        [InspectorButton]
        void SetTrog()
        {
            _animController.SetAvatar(PlayerAvatar.Trog);
        }

        [InspectorButton]
        void SetTufo()
        {
            _animController.SetAvatar(PlayerAvatar.Tufo);
        }

        [InspectorButton]
        void SetTuti()
        {
            _animController.SetAvatar(PlayerAvatar.Tuti);
        }

		[InspectorButton]
		void SetNimbusShip()
		{
			_animController.SetAvatar(PlayerAvatar.NimbusShip );
		}

		[InspectorButton]
		void SetPirateShip()
		{
			_animController.SetAvatar(PlayerAvatar.PirateShip );
		}

#endif

    }
}