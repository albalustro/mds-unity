using System;
using FullInspector;
using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Validators.Interfaces;

namespace MDS.Core.ProcessActivator
{

    //[ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ProcessAnswerButton : BaseValidationActivator
    {

        public Sprite downSprite;
        public Sprite upSprite;

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;

        private Color enabledColor = Color.white;
        private Color disabledColor = new Color(1, 1, 1, 0.5f);

        [SerializeField, InspectorTooltip("GameObject (child) que será acionado quando estiver habilitado. Se mais de um efeito for necessário, coloque todos como filhos de um GO comum e use-o nessa propriedade")]
        private GameObject enabledEffectGO;

        private IValidator _validador;

        protected override void Awake()
        {
            base.Awake();
           
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
            Disable();
        }

        public override void Disable()
        {
            base.Disable();

            _spriteRenderer.color = disabledColor;
            _collider.enabled = false;

            if(enabledEffectGO != null)
                enabledEffectGO.SetActive(false);
        }

        public override void Enable()
        {
            base.Enable();

            _spriteRenderer.color = enabledColor;
            _collider.enabled = true;

            if(enabledEffectGO != null)
                enabledEffectGO.SetActive(true);
        }

        public void OnMouseUp()
        {
            FireValidationEvent();
            _spriteRenderer.sprite = upSprite;
        }

        public void OnMouseDown()
        {
            _spriteRenderer.sprite = downSprite;
        }


        public void Update()
        {
            //TODO: ficar verificando o validator atual é bem ruim.. preciso providenciar um modo mais performatico de fazer isso
            //if(_validador == null)
                _validador = Challenge.GetCurrentValidador();

            if(_validador.ReadyToValidate())
                Enable();
            else
                Disable();
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