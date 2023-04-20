using System.Collections.Generic;
using UnityEngine;
public class Pooler : MonoBehaviour
{
	[SerializeField]
	private PooledObject pooledObjectPrefab = null;
	[SerializeField]
	private int poolCount = 0;
	[SerializeField]
	private Transform objectsContainer = null;
	
	private Stack<PooledObject> availableObjects;
	private HashSet<PooledObject> usedObjects;

	public void Init()
	{
		availableObjects = new Stack<PooledObject>();
		usedObjects = new HashSet<PooledObject>();
		for (int i = 0; i < poolCount; i++)
		{
			var newObj = Instantiate(pooledObjectPrefab, objectsContainer);
			availableObjects.Push(newObj);
			newObj.Init(ReturnObjectToPool);
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
		
		var newObj = Instantiate(pooledObjectPrefab, objectsContainer);
		availableObjects.Push(newObj);
		newObj.Init(ReturnObjectToPool);
		return newObj;
	}

	public void ReturnObjectToPool(PooledObject pooledObject)
	{
		pooledObject.Reset(true);
		pooledObject.SetParent(objectsContainer);
		usedObjects.Remove(pooledObject);
		availableObjects.Push(pooledObject);
	}
}
