using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HudKey : MonoBehaviour
{

	public GameObject[] crystals;

	public CanvasGroup canvasGroup;

	void Start()
	{
		Scene scene = SceneManager.GetActiveScene();
		if (scene.IsChallenge() || scene.IsEpisode())
		{
			canvasGroup.alpha = 1;
			SetCristals(scene);
		}
		else
		{
			canvasGroup.alpha = 0;
		}

	}

	private void SetCristals(Scene scene)
	{
		for (int i = 0 ; i < 5 ; i++)
		{
			bool active = UserProfile.Instance.conceptMap.CheckChallengeComplete(scene, i);
			crystals[i].SetActive(active);
		}
	}
}
