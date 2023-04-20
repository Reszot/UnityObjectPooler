using System;
using UnityEngine;
public abstract class PooledObject : MonoBehaviour
{
	public event Action<PooledObject> ReturnedToPool = delegate{ };

	private Transform objectTransform;
	private Action<PooledObject> returnToPoolAction;

	public virtual void Init(Action<PooledObject> returnToPool)
	{
		returnToPoolAction = returnToPool;
		objectTransform = transform;
		ToggleEnabled(false);
	}

	public void ToggleEnabled(bool enable)
	{
		if (gameObject.activeSelf == enable)
			return;
		
		gameObject.SetActive(enable);
	}

	public void SetParent(Transform newParent)
	{
		objectTransform.SetParent(newParent);
	}
	
	public virtual void Reset(bool commandFromPool = false)
	{
		if (!commandFromPool)
		{
			returnToPoolAction(this);
		}
		ReturnedToPool(this);
		ToggleEnabled(false);
	}

}
