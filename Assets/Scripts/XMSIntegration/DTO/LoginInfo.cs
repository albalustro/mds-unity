using UnityEngine;
using System.Collections;

public class XmsLoginInfo : XmsMessage<LoginInfo> { }

[System.Serializable]
public class LoginInfo
{
    public LoginInfo()
    {

    }

	public StatusInfo status;
	public string token;
	public string name;
	public string role;
}

public class LoginRequest
{
	public string Login { get; set; }
	public string Password { get; set; }
	public string Game { get; set; }
	public string SeasonId { get; set; }
}