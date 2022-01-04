using UnityEngine;
using System;
using System.Collections.Generic;

public class XmsLoginInfo : XmsMessage<LoginInfo> { }

[System.Serializable]
public class LoginInfo
{
	public LoginInfo()
	{

	}

	public StatusInfo Status { get; set; }

	public string Token { get; set; }
	public string Name { get; set; }
	public string Role { get; set; }

	public List<ConceptData> Concepts { get; set; }
}
