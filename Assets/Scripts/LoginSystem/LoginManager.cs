using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	string _url;
	public LoginInfo loginInfo;
	[SerializeField] private InputField userField;
	[SerializeField] private InputField passField;

	void Start () {
		_url = "https://stage-xms.xmile.com.br/api/gamelogin";
	}

	public void Login()
	{
		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("login", userField.text);
		loginForm.AddField ("password", passField.text);
		loginForm.AddField ("game", "4");
		loginForm.AddField ("season_id", "1");
		WWW www = new WWW (_url, loginForm);
		StartCoroutine (ValidateLogin(www));
	}

	IEnumerator ValidateLogin(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			string wsReturn = www.text.Trim ();
			JsonUtility.FromJsonOverwrite (wsReturn, loginInfo);
		} 
		else
		{
			Debug.LogError (www.error);
		}
	}
}
