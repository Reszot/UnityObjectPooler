using System.Collections.Generic;
using UnityEngine;
public class Pooler : MonoBehaviour
{
	[SerializeField]
	private PooledObject pooledObjectPrefab = null;
	[SerializeField]
	private int poolCount = 0;
	
	private Stack<PooledObject> availableObjects;
	private List<PooledObject> usedObjects;

	private void Awake()
	{
		availableObjects = new Stack<PooledObject>();
		usedObjects = new List<PooledObject>();
		for (int i = 0; i < poolCount; i++)
		{
			var newObj = Instantiate(pooledObjectPrefab);
			availableObjects.Push(newObj);
			newObj.ToggleEnabled(false);
		}
	}

	public PooledObject GetObjectFromPool()
	{
		if (availableObjects.Count != 0)
		{
			var obj = availableObjects.Pop();
			usedObjects.Add(obj);
			return obj;
		}
		
		Debug.LogError("No more objects available in pool");
		return null;
	}

	public void ReturnObjectToPool(PooledObject pooledObject)
	{
		pooledObject.Reset();
		usedObjects.Remove(pooledObject);
		availableObjects.Push(pooledObject);
	}
}
