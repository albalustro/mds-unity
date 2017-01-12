using UnityEngine;

public class Singleton<T> : MDSBehaviour where T : MDSBehaviour
{
	private static T m_Instance;
	private static object m_Lock = new object();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public static T Instance
	{
		get
		{

			lock (m_Lock)
			{
				if (m_Instance == null)
				{
					m_Instance = (T) FindObjectOfType(typeof(T));

                    if(FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        DontDestroyOnLoad(m_Instance);
                        return m_Instance;
                    }
 
					if (m_Instance == null)
					{
						GameObject singleton = new GameObject();
						m_Instance = singleton.AddComponent<T>();
						singleton.name = typeof(T).ToString();
						DontDestroyOnLoad(singleton);
					} 
				}
 
				return m_Instance;
			}
		}
	}

	public void OnDestroy()
	{
		m_Instance = null;
	}
}