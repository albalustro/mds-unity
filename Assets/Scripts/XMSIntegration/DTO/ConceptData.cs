using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

public class ConceptData
{
	public int ChallengeId { get; set; }

	public int ChallengeIndex { get; set; }
	public int EpisodeIndex { get; set; }
	public int WorldIndex { get; set; }

	[JsonProperty("ReleasedId"), JsonConverter(typeof(StringEnumConverter))] 
	public EpisodeLiberationTypes? LiberationType { get; set; }

	public int? Concept { get; set; }
	public DateTime? StartDate { get; set; }
	public DateTime? EndDate { get; set; }

}
