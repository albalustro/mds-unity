using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using MDS.Utilities;
using UnityEngine.Events;

namespace MDS.Gameplay.Tetris
{
    public class TetrisController : MDSBehaviour
    {
		#region Variables
		private LaneGroup _laneGroup;
		private SpawnableGroup _spawnableGroup;
		private Lane _currentLane;
		private Spawnable _currentSpawnable;
		private AnimationCurve timeCurve;

		//Counter
		private Counter _counter;
		public bool hasCounter;
		public UnityEvent events;
		public int totalAmount;
		private int _currentAmount;

		//Lerping
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
			foreach (var item in _laneGroup.lanes)
				item.Controller = this;
		}

        private void Start()
        {
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
			keys [1] = new Keyframe (1, 0.5f);
			timeCurve = new AnimationCurve (keys);

			SpawnNewItem();
        }

		void Update()
		{
			if (_isAnimating)
				return;
			_currentTime += Time.deltaTime;
			if (TweenSpawnable()) 
			{
				if (_currentLane.Validate (_currentSpawnable))
					StartCoroutine (VictoryAnimation());
				else
					StartCoroutine (LoseAnimation());	
			}
		}
		#endregion

		#region Gameplay mechanics
        void SpawnNewItem()
        {
			if (_currentAmount == 0)
				return;
			_currentTime = 0;
			_currentSpawnable = SortNewSpawnable();
			_currentLane = SortNewLane();
			_currentSpawnable.transform.position = _currentLane.spawnPosition.position;
			LeanTween.scale(_currentSpawnable.gameObject, Vector3.one, 0f);
			LeanTween.alpha (_currentSpawnable.gameObject, 1, 0f);
			_currentSpawnable.GetComponent<SpriteRenderer>().sortingOrder = 0;
			_currentSpawnable.gameObject.SetActive(true);
			float index = 1 - ((float)_currentAmount/(float)totalAmount);
			_currentTotalTime = totalTime * timeCurve.Evaluate (index);
			_currentAmount--;
			if (events != null)
				events.Invoke ();
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