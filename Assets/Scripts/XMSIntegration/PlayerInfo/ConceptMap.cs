using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using MDS.Utilities;
using System.Threading.Tasks;

/// <summary>
/// Informações sobre a Temporada para composição do mapa de conceitos
/// </summary>
[System.Serializable]
public class ConceptMap
{
	public ConceptMap()
	{

	}

	public ConceptData[] Concepts { get; set; }

	private IEnumerable<ConceptData> FromEpisode(Scene scene)
	{
		var worldId = scene.GetWorldIndex() - 1;
		var episodeId = scene.GetEpisodeIndex() - 1;
		return FromEpisode(worldId, episodeId);
	}

	private IEnumerable<ConceptData> FromEpisode(int worldId, int episodeId)
	{
		return Concepts.Where(cd => cd.WorldIndex == worldId && cd.EpisodeIndex == episodeId);
	}

	public ConceptData GetConceptData(int w, int e, int c)
	{
		return Concepts.First(cd => cd.WorldIndex == w && cd.EpisodeIndex == e && cd.ChallengeIndex == c);
	}
	
	public void SetLiberationStatusForEpisode(int w, int e, EpisodeLiberationTypes liberationType = EpisodeLiberationTypes.ALLOW_BY_CONCEPT)
	{
		foreach (var cd in FromEpisode(w, e))
        {
            cd.LiberationType = liberationType;
        }
	}

	/// <summary>
	/// This method will give the liberarion status of a given episode
	/// </summary>
	/// <param name="scene">Episode scene</param>
	/// <returns><see cref="EpisodeLiberationTypes"/></returns>
	public EpisodeLiberationTypes EpisodeLiberationStatusByScene(Scene scene)
	{
		var releasedId = FromEpisode(scene).First().LiberationType;
		return releasedId.HasValue ? releasedId.Value : EpisodeLiberationTypes.BLOCK_BY_CONCEPT;
	}

	public EpisodeLiberationTypes EpisodeLiberationStatusByWorldAndEpisode(int world, int episode)
	{
		var releasedId = FromEpisode(world, episode).First().LiberationType;
		return releasedId.HasValue ? releasedId.Value : EpisodeLiberationTypes.BLOCK_BY_CONCEPT;
	}

	/// <summary>
	/// This method will check if exist a next episode in the world and
	///   if the case, it will check all challenges of the current episode 
	///   and determine what should be the status
	///   of the next episode.
	/// </summary>
	/// <param name="scene">Current Episode Scene</param>
	public void TryUpdateNextEpisodeLiberationStatus(Scene scene)
	{
		var w = scene.GetWorldIndex() - 1;
		var e = scene.GetEpisodeIndex() - 1;

		if (e == 7) // last episode of the world so no next episode to check
		{
			return;
		}

		var finishedCurrentEpisode = CheckEpisodeComplete(w, e); //FromEpisode(w, e).All(c => c.Concept.HasValue && c.Concept.Value == (int)ConceptTypes.CONCEPT_GREEN);

		if (!finishedCurrentEpisode)
		{
			return;
		}

		e++; // next episode
		var nextEpisodeLiberationType = FromEpisode(w, e).First().LiberationType;

		if (nextEpisodeLiberationType == EpisodeLiberationTypes.BLOCK_BY_CONCEPT)
		{
			//Set next episode as ALLOW_BY_CONCEPT
			SetLiberationStatusForEpisode(w, e);
		}
		
		// if (nextEpisodeLiberationType == EpisodeLiberationTypes.BLOCK_BY_TEACHER)
		// {
		// 	return;
		// }

		// next episode should be ok to play
		//await NetworkManager.Instance.RefreshConceptMap();
	}


	/// <summary>
	/// Check if all challenges in this episode already have green concept
	/// </summary>
	/// <param name="scene">Chellenge scene</param>
	/// <returns>TRUE if all have green in all challenges, false otherwise</returns>
	public bool CheckEpisodeComplete(Scene scene)
	{
		return FromEpisode(scene).All(c => c.Concept.HasValue && c.Concept.Value == (int)ConceptTypes.CONCEPT_GREEN);
	}

	public bool CheckEpisodeComplete(int world, int episode)
	{
		return FromEpisode(world, episode).All(c => c.Concept.HasValue && c.Concept.Value != (int)ConceptTypes.CONCEPT_NOT_PLAYED);
	}


	/// <summary>
	/// Check if a given challenge already have green concept
	/// </summary>
	/// <param name="scene">Challenge scene</param>
	/// <param name="challengeIndex">Index of the challenge, starting at 0</param>
	/// <returns>TRUE when concept is already green, false otherwise</returns>
	public bool CheckChallengeComplete(Scene scene, int challengeIndex)
	{
		return FromEpisode(scene).First(c => c.ChallengeIndex == challengeIndex).Concept == (int)ConceptTypes.CONCEPT_GREEN;
	}

	public bool CheckChallengeComplete(int world, int episode, int challengeIndex)
	{
		return FromEpisode(world, episode).First(c => c.ChallengeIndex == challengeIndex).Concept == (int)ConceptTypes.CONCEPT_GREEN;
	}


	///  <summary>
	///  Allow the player to play directly a certain challenge when 
	/// 		all challenges in this episode was already played before
	///  </summary>
	///  <param name="world"></param>
	///  <param name="episode"></param>
	///  <returns>TRUE if already have concepts in all challenges of this episode, false otherwise</returns>
	public bool CheckDirectAccessToChallenge(int world, int episode)
	{
		return FromEpisode(world, episode).All(c => c.Concept.HasValue && c.Concept.Value != (int)ConceptTypes.CONCEPT_NOT_PLAYED);
	}
}
