using System;
using FullInspector;
using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;

namespace MDS.Core
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ProcessAnswerButton : MDSBehaviour, IAnswerProcessor
    {

        public event ProcessAnswerDelegate OnProcessAnswer;

        public Sprite downSprite;
        public Sprite upSprite;

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;

        private Color enabledColor = Color.white;
        private Color disabledColor = new Color(1, 1, 1, 0.5f);

        [SerializeField, InspectorTooltip("GameObject (child) que será acionado quando estiver habilitado. Se mais de um efeito for necessário, coloque todos como filhos de um GO comum e use-o nessa propriedade")]
        private GameObject enabledEffectGO;

        protected override void Awake()
        {
            base.Awake();
            gameObject.tag = "IAnswerProcessor";
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
            Disable();
        }

        public void Disable()
        {
            _spriteRenderer.color = disabledColor;
            _collider.enabled = false;

            if(enabledEffectGO != null)
                enabledEffectGO.SetActive(false);
        }

        public void Enable()
        {
            _spriteRenderer.color = enabledColor;
            _collider.enabled = true;

            if(enabledEffectGO != null)
                enabledEffectGO.SetActive(true);
        }

        public void OnMouseUp()
        {
            if(OnProcessAnswer != null)
                OnProcessAnswer();
            _spriteRenderer.sprite = upSprite;
        }

        public void OnMouseDown()
        {
            _spriteRenderer.sprite = downSprite;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            gameObject.name = "CheckAnswerButton";
            if(_spriteRenderer != null)
                _spriteRenderer.sprite = upSprite;

        }
    }

}