using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Player
{
    public class PlayerAnimController : MDSBehaviour
    {
        public static PlayerAvatar OriginalAvatar
        {
            set
            {
                _originalAvatar = value;
            }
        }
        public static PlayerAvatar Avatar
        {
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


        private readonly int paramAvatarHash = Animator.StringToHash(paramAvatarName);
        private readonly int paramWalkingHash = Animator.StringToHash(paramWalkingName);
        private readonly int paramBackHash = Animator.StringToHash(paramBackName);
        private readonly int paramIdleBackHash = Animator.StringToHash(paramIdleBackName);
        private readonly int paramMountedHash = Animator.StringToHash(paramMountedName);

        private static PlayerAvatar _originalAvatar = PlayerAvatar.Blup;
        private static PlayerAvatar _avatar = PlayerAvatar.Blup;


        public bool ShouldFlipX
        {
            get
            {
                return _avatar != PlayerAvatar.Blup;
            }
        }

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

            bool mounted = _animator.GetBool(paramMountedHash);

            _spriteRenderer.flipX = mounted? false : ( ShouldFlipX ? direction.x <= 0 : direction.x > 0);

        }

        public void RestoreOriginalAvatar()
        {
            SetAvatar(_originalAvatar);
            SetMounted(false);
            SetIdleBack(false);
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

        public void SetIdleBack(bool isIdleBack)
        {
            _animator.SetBool(paramIdleBackHash, isIdleBack);
        }
    }
}