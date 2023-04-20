using System;
using System.Collections.Generic;
using UnityEngine;
public class FollowingIconContainer : PooledObject
{
	[SerializeField]
	private FollowTransform followTransformScript = null;
	[SerializeField]
	private List<IconRow> iconRows = null;

	private HashSet<CharacterFollowingIcon> icons;

	private void Awake()
	{
		icons = new HashSet<CharacterFollowingIcon>();
	}

	private void OnDestroy()
	{
		foreach (var icon in icons)
		{
			icon.ReturnedToPool -= OnIconReturned;
		}
	}

	public override void Init(Action<PooledObject> returnToPool)
	{
		base.Init(returnToPool);
		foreach (var row in iconRows)
		{
			row.Init();
		}
	}

	public override void Reset(bool commandFromPool = false)
	{
		base.Reset(commandFromPool);
		followTransformScript.Follow = null;
		if (icons.Count != 0)
		{
			var controller = ObjectCacher.Instance.IconCanvasController;
			foreach (var icon in icons)
			{
				controller.HideIcon(icon);
			}
		}
		icons.Clear();
	}
	
	public void Setup(Transform transformToFollow)
	{
		followTransformScript.Follow = transformToFollow;
		ToggleEnabled(true);
	}

	public void AddIcon(CharacterFollowingIcon icon, int rowIndex, int rowToPurge = -1)
	{
		List<CharacterFollowingIcon> iconsToPurge = null;
		if (rowToPurge > 0)
		{
			iconsToPurge = iconRows[rowToPurge].GetAllIcons();
		}
		icons.Add(icon);
		if (iconsToPurge != null)
		{
			foreach (var followingIcon in iconsToPurge)
			{
				followingIcon.Reset();
			}
		}
		iconRows[rowIndex].AddIcon(icon);
		icon.ReturnedToPool += OnIconReturned;
		ToggleEnabled(true);
	}
	
	public void OnIconReturned(PooledObject icon)
	{
		icon.ReturnedToPool -= OnIconReturned;
		icons.Remove(icon as CharacterFollowingIcon);
		if (icons.Count != 0)
			return;
		
		ToggleEnabled(false);
	}
}
