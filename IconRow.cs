using System.Collections.Generic;
using UnityEngine;
public class IconRow : MonoBehaviour
{
	public int IconCount => icons.Count;
	
	private Transform myTransform;
	private HashSet<CharacterFollowingIcon> icons;

	public void Init()
	{
		myTransform = transform;
		icons = new HashSet<CharacterFollowingIcon>();
	}

	public void AddIcon(CharacterFollowingIcon icon)
	{
		icon.SetParent(myTransform);
		icon.ReturnedToPool += OnIconReturned;
		icons.Add(icon);

		if (gameObject.activeSelf)
			return;
		
		gameObject.SetActive(true);
	}

	public List<CharacterFollowingIcon> GetAllIcons()
	{
		var array = new CharacterFollowingIcon[icons.Count];
		icons.CopyTo(array);
		return new List<CharacterFollowingIcon>(array);
	}

	private void OnIconReturned(PooledObject icon)
	{
		icons.Remove((CharacterFollowingIcon)icon);
		if (icons.Count != 0)
			return;
		
		gameObject.SetActive(false);
	}
}
