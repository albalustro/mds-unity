using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmsMessage <T>
{
	public bool Sucesso { get; set; }
	public T Data { get; set; }
	public string Error { get; set; }


}
