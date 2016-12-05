using UnityEngine;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class PersistenceManager : Singleton<PersistenceManager> {

	public UserProfile LoadLocalLoginInfo(string user)
	{
		return JsonConvert.DeserializeObject<UserProfile>(PlayerPrefs.GetString(user));
	}
		
	public void LoadConceptMap(UserProfile profile, ref ConceptMap cm)
	{
		if (!PlayerPrefs.HasKey(profile.login))
			cm = null;
	}

	public void SaveUserProfile(UserProfile profile)
	{
		List<object> serial = new List<object> ();
		serial.Add (profile.pass);
		serial.Add (profile.loginInfo);
		serial.Add (profile.conceptMap);
		PlayerPrefs.SetString (profile.login, JsonConvert.SerializeObject (serial));
	}

	#region Segurança
	/// <summary>
	/// Método para gerar o MD5 de uma string
	/// </summary>
	/// <param name="text">Texto a ser gerado a Hash MD5</param>
	/// <returns>MD5 Hash do texto informado</returns>
	public string GetMD5Hash(string text)
	{
		MD5 md5Hash = MD5.Create();
		// Converter a String para array de bytes
		byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
		// Cria-se um StringBuilder para recompôr a string.
		StringBuilder sBuilder = new StringBuilder();
		// Loop para formatar cada byte como uma String em hexadecimal
		for (int i = 0; i < data.Length; i++)
			sBuilder.Append(data[i].ToString("x2"));
		return sBuilder.ToString();
	}
	#endregion
}
