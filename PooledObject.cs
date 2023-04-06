using UnityEngine;
public abstract class PooledObject : MonoBehaviour
{
	public void ToggleEnabled(bool enable)
	{
		gameObject.SetActive(enable);
	}

	public virtual void Setup(params object[] parameters)
	{
	}
	
	public virtual void Reset()
	{
	}
}
