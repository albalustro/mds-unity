using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Core.SceneManagement
{
	public enum FadeDirection
	{
		In = -1,            // Fade In
		Out = 1             // Fade Out
	}

	public class FadeTransition : Singleton<FadeTransition>
	{
		public float FadeTime = 0.5f;
		private int m_DrawDepth = -1000;
		private Texture2D m_fadeOutTexture;
		private FadeDirection m_FadeDirection = FadeDirection.In;
		private float m_CurrentAlphaColor = 1.0f;
		private float m_StartTime;

        protected override void Awake()
        {
            base.Awake();

            if(FadeTransition.Instance != this)
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }

        void Start()
		{
			m_fadeOutTexture = new Texture2D(2, 2);

			for (var mip = 0; mip < 2; ++mip)
			{
				for (var cols = 0; cols < 2; ++cols)
				{
					m_fadeOutTexture.SetPixel(cols, mip, Color.black);
				}
			}

			m_fadeOutTexture.Apply();
		}

		void OnGUI()
		{
			if (m_FadeDirection == FadeDirection.Out)
				m_CurrentAlphaColor = Mathf.Lerp(0.0f, 1.0f, ((Time.time - m_StartTime) / FadeTime));
			else
				m_CurrentAlphaColor = Mathf.Lerp(1.0f, 0.0f, ((Time.time - m_StartTime) / FadeTime));

			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, m_CurrentAlphaColor);
			GUI.depth = m_DrawDepth;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_fadeOutTexture);
		}

		public float BeginFade(FadeDirection fadeDirection)
		{
			m_StartTime = Time.time;
			m_FadeDirection = fadeDirection;
			return FadeTime;
		}

		void OnEnable()
		{
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
		}

		void OnDisable()
		{
			SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		}

		void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
		{
			BeginFade(FadeDirection.In);
		}
	}

}
