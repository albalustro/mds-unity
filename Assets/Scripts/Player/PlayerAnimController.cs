using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Player
{
    public class PlayerAnimController : MDSBehaviour
    {

        public static PlayerAvatar Avatar
        {
            //get
            //{
            //    return _avatar;
            //}
            set
            {
                _avatar = value;
            }
        }

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private const string paramAvatarName = "avatar";
        private const string paramWalkingName = "walking";
        private const string paramBackName = "back";
        private const string paramIdleBackName = "idle_back";
        private const string paramMountedName = "mounted";
        private const string paramRobotName = "robot";

        private readonly int paramAvatarHash = Animator.StringToHash(paramAvatarName);
        private readonly int paramWalkingHash = Animator.StringToHash(paramWalkingName);
        private readonly int paramBackHash = Animator.StringToHash(paramBackName);
        private readonly int paramIdleBackHash = Animator.StringToHash(paramIdleBackName);
        private readonly int paramMountedHash = Animator.StringToHash(paramMountedName);
        private readonly int paramRobotHash = Animator.StringToHash(paramRobotName);

        private static PlayerAvatar _avatar = PlayerAvatar.Blup;
        
        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponentInChildren<Animator>();
            if(_animator == null)
                LogError("Não existe Animator neste GameObject ou nos filhos");

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void SetWalking(bool walking, Vector2 direction)
        {
            _animator.SetBool(paramWalkingHash, walking);
            _animator.SetBool(paramBackHash, direction.y > 0);

            _spriteRenderer.flipX = direction.x > 0;
        }

        public void ResetAvatar()
        {
            SetAvatar(_avatar);
        }

        public void SetAvatar(PlayerAvatar avatar)
        {
            _avatar = avatar;
            _animator.SetInteger(paramAvatarHash, (int)_avatar);
        }

        public void SetMounted(bool isMounted)
        {
            _animator.SetBool( paramMountedHash, isMounted);
        }

        public void SetRobot(bool isRobot)
        {
            _animator.SetBool(paramRobotHash, isRobot);
        }
    }
}