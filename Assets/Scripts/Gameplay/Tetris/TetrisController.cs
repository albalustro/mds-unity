using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using MDS.Utilities;
using UnityEngine.Events;
using MDS.Actions;
using MDS.Core.Interfaces;
using System.Linq;
using System;

namespace MDS.Gameplay.Tetris
{
	public class TetrisController : MDSBehaviour
	{
		public UnityEvent OnVictoryEvent;
		public UnityEvent OnLoseEvent;

		#region Variables
		private LaneGroup _laneGroup;
		private SpawnableGroup _spawnableGroup;
		private Lane _currentLane;
		private Spawnable _currentSpawnable;
		private AnimationCurve timeCurve;

		//Counter
		private Counter _counter;
		public bool hasCounter;
		public UnityEvent OnSpawnEvent;
		public int totalAmount;
		private int _currentAmount;

		//Lerping
		[SerializeField]
		private bool _preTweenAnimation; // usada para indicar 2 casos de animacoes especiais: antes de iniciar o tween e caso erre. Esses casos so ocorrem na cena g2w1e2c4
        private bool _isPreAnimating;
        private int _errorAmount = 0;
		[SerializeField]
		private Transform _preTweenInitialPosition;


		public float totalTime;
		private float _currentTotalTime;
		private float _currentTime;
		private bool _isAnimating;
		#endregion

		#region Unity Methods
		protected override void Awake ()
		{
			base.Awake ();

			_spawnableGroup = GameObject.FindObjectOfType<SpawnableGroup> ();
			_laneGroup = GameObject.FindObjectOfType<LaneGroup> ();

		}

		private void Start()
		{

			foreach (var item in _laneGroup.lanes)
				item.Controller = this;

			if (hasCounter)
			{
				_counter = GameObject.FindObjectOfType<Counter> ();
				_counter.Reset (totalAmount);
			}
			_currentTime = 0;
			_currentAmount = totalAmount;
			//Define os keyframes da curva de incremento da velocidade
			Keyframe[] keys = new Keyframe[2];
			keys [0] = new Keyframe (0, 1);
			keys [1] = new Keyframe (1, 0.75f);
			timeCurve = new AnimationCurve (keys);

			SpawnNewItem();
		}

        void Update()
        {
            if(_isAnimating || _isPreAnimating)
                return;


            _currentTime += Time.deltaTime;



            // executa o movimento e verifica se chegou no fim do mesmo
            if(TweenSpawnable())
            {
                if(_currentLane.Validate(_currentSpawnable))
                {
                    if(OnVictoryEvent != null)
                        OnVictoryEvent.Invoke();
                    StartCoroutine(VictoryAnimation());
                }
                else
                {
                    if(OnLoseEvent != null)
                        OnLoseEvent.Invoke();

                    if (!_preTweenAnimation)
                        StartCoroutine(LoseAnimation());
                    else
                    {
                        _isPreAnimating = true;

                        _errorAmount++;
                        List<IAction> actions = new List<IAction>();


                        OpenDialogAction ac1 = new Actions.OpenDialogAction();
                        ac1.waitFinish = true;
                        switch(_errorAmount)
                        {
                            case 1:
                                ac1.slugs = new[] { Slug.error1 };
                                actions.Add(ac1);
                                break;
                            case 2:
                                ac1.slugs = new[] { Slug.error2 };
                                actions.Add(ac1);
                                break;
                            case 3:
                                ac1.slugs = new[] { Slug.error3 };
                                actions.Add(ac1);
                                break;
                            default:
                                break;
                        }

                        TransformAnimationAction move = new TransformAnimationAction();
                        move.target = _currentSpawnable.gameObject;
                        move.waitFinish = true;
                        move.duration = 1.5f;
                        move.animationType = TransformAnimationAction.TransformAnimationType.Position;

                        move.destination = (from l in _laneGroup.lanes
                                           where l != _currentLane
                                           select l).First().destinationPoint.transform.position;
                        actions.Add(move);
                        ExecuteActions(actions.ToArray());
                        StartCoroutine(WaitLoseAnimation());
                    }
                }
            }

        }

        private IEnumerator WaitLoseAnimation()
        {
            Log("Antes do dialogo");
            yield return new WaitForSeconds(0.1f);
            yield return new WaitWhile(MDS.DialogueSystem.DialogueSystem.Instance.IsDialogueOpen);
            Log("Depois do dialogo fechar, aguardando 1,5s");
            yield return new WaitForSeconds(1.5f);
            _isPreAnimating = false;
            Log("Iniciando animacao de vitoria");
            StartCoroutine(VictoryAnimation());
        }
        #endregion

        #region Gameplay mechanics
        void SpawnNewItem()
		{
			if(_currentAmount == 0)
			{
				InvokeValidation action = new InvokeValidation();
				ExecuteActions(new []{ action});
				return;
			}
			_currentTime = 0;
			_currentSpawnable = SortNewSpawnable();
			_currentLane = SortNewLane();

            if(_preTweenAnimation)
            {
                // animacao de sair da caixa em G2W1E2C4 (A PRINCIPIO SÓ USA NESSA CENA)
                LeanTween.scale(_currentSpawnable.gameObject, Vector3.zero, 0f);
                LeanTween.alpha(_currentSpawnable.gameObject, 0f, 0f);

                _currentSpawnable.transform.position = _preTweenInitialPosition.position;
                _isPreAnimating = true;

                LeanTween.scale(_currentSpawnable.gameObject, Vector3.one, 0.5f)
                        .setDelay(0.2f);
                LeanTween.moveLocalY(_currentSpawnable.gameObject, 2f, 0.5f)
                        .setDelay(0.2f);
                LeanTween.alpha(_currentSpawnable.gameObject, 1f, 0.5f)
                        .setDelay(0.2f)
                        .setOnComplete(()=>
                        {
                            LeanTween.move(_currentSpawnable.gameObject, _currentLane.spawnPosition.position, 0.8f)
                            .setOnComplete(() =>
                            {
                                _isPreAnimating = false;
                            });
                        });

            }
            else
            {
                _currentSpawnable.transform.position = _currentLane.spawnPosition.position;
                LeanTween.scale(_currentSpawnable.gameObject, Vector3.one, 0f);
                LeanTween.alpha(_currentSpawnable.gameObject, 1, 0f);
            }
			_currentSpawnable.GetComponent<SpriteRenderer>().sortingOrder = 0;
			_currentSpawnable.gameObject.SetActive(true);
			float index = 1 - ((float)_currentAmount/(float)totalAmount);
			_currentTotalTime = totalTime * timeCurve.Evaluate (index);
			_currentAmount--;
			if (OnSpawnEvent != null)
				OnSpawnEvent.Invoke ();
		}

		private bool TweenSpawnable()
		{
			float time = _currentTime / _currentTotalTime;
			if (time > 1)
			{
				time = 1;
				_currentSpawnable.transform.position = _currentLane.destinationPoint.transform.position;
				return true;
			}
			_currentSpawnable.transform.position = Vector2.Lerp (_currentLane.spawnPosition.position, _currentLane.destinationPoint.transform.position, time);
			return false;
		}

		public void ChangeLane(Lane l)
		{
			_currentLane = l;
		}

		//Sorteia uma posição para criar o item
		Lane SortNewLane()
		{
			return _laneGroup.lanes.GetRandom();
		}

		//Sorteia um item a ser lançado nas lanes
		Spawnable SortNewSpawnable()
		{
			return _spawnableGroup.spawnables.GetRandom();
		}
		#endregion

		#region Pos Validation anims
		IEnumerator VictoryAnimation()
		{
			_isAnimating = true;
			LeanTween.alpha (_currentSpawnable.gameObject, 0, 0.5f);
			yield return new WaitForSeconds (2);
			SpawnNewItem();	
			_isAnimating = false;
		}

		IEnumerator LoseAnimation()
		{
			_isAnimating = true;
			LeanTween.moveLocalY (_currentSpawnable.gameObject, _currentSpawnable.transform.localPosition.y + 1.5f, 0.3f)
				.setOnComplete(() =>
				{
					LeanTween.scale(_currentSpawnable.gameObject, Vector3.one * 1.5f, 0.5f);
					_currentSpawnable.GetComponent<SpriteRenderer>().sortingOrder = 3;
					LeanTween.moveLocalY (_currentSpawnable.gameObject, _currentSpawnable.transform.localPosition.y - 5f, 0.5f).setEase(LeanTweenType.easeInBack)
					.setOnComplete(() => 
					{
						LeanTween.alpha (_currentSpawnable.gameObject, 0, 0.5f);
					});
				});
			yield return new WaitForSeconds (2);
			SpawnNewItem();	
			_isAnimating = false;
		}
		#endregion
	}
}