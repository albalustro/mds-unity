using UnityEngine;
using System.Collections;

[System.Serializable]
public class LoginInfo 
{
    public LoginInfo()
    {

    }

	public StatusInfo status;
	public string token;
	public string api;
	public string assets_url;
	public string assets_version;
	public int id;
	public string name;
	public string role;
}
