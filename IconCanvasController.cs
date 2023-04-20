using System;
using System.Collections.Generic;
using UnityEngine;
public class IconCanvasController : MonoBehaviour
{
	[Header("↑0 ↓3")]
	[SerializeField, Range(0,3)]
	private int clickableRowIndex = 0;
	[SerializeField, Range(0,3)]
	private int pickpocketRowIndex = 1;
	[SerializeField, Range(0,3)]
	private int statusRowIndex = 2;
	[SerializeField, Range(0,3)]
	private int mimeticRowIndex = 3;
	
	[Header("Poolers")]
	[SerializeField]
	private Pooler iconPooler = null;
	[SerializeField]
	private Pooler iconContainerPooler = null;
	
	[Space]
	[SerializeField]
	private SkillIconSO skillIcons = null;

	private Dictionary<Transform, FollowingIconContainer> iconsContainers;
	private HashSet<CharacterFollowingIcon> pickpocketIcons;
	private Dictionary<OldEnemy, CharacterFollowingIcon> enemyMimeticIcons;
	private Dictionary<PlayerUnit, CharacterFollowingIcon> playerMimeticIcons;

	private void Awake()
	{
		skillIcons.Init();
		iconPooler.Init();
		iconContainerPooler.Init();
		enemyMimeticIcons = new Dictionary<OldEnemy, CharacterFollowingIcon>();
		playerMimeticIcons = new Dictionary<PlayerUnit, CharacterFollowingIcon>();
		pickpocketIcons = new HashSet<CharacterFollowingIcon>();
		iconsContainers = new Dictionary<Transform, FollowingIconContainer>();
		MimeticScanner.MimeticChanged += OnMimeticChanged;
	}

	private void OnDestroy()
	{
		foreach (var container in iconsContainers.Values)
		{
			container.ReturnedToPool -= OnContainerReturned;
		}
		MimeticScanner.MimeticChanged -= OnMimeticChanged;
	}

	public void ShowIcon(EPickpocketType pickpocketType, Transform transformToFollow)
	{
		var icon = (CharacterFollowingIcon) iconPooler.GetObjectFromPool();
		GetContainer(transformToFollow).AddIcon(icon, DetermineIconRowIndex(true, false,null));
		icon.Setup(skillIcons.PickpocketIcons[pickpocketType]);
		pickpocketIcons.Add(icon);
	}

	public CharacterFollowingIcon ShowIcon(EEnemyAlertedState state, Transform transformToFollow, float iconLife = -1f)
	{
		var icon = (CharacterFollowingIcon) iconPooler.GetObjectFromPool();
		GetContainer(transformToFollow).AddIcon(icon, DetermineIconRowIndex(false, false, null), statusRowIndex);
		icon.Setup(skillIcons.EnemyStateIcons[state], iconLife, HideIcon);
		return icon;
	}

	public CharacterFollowingIcon ShowIcon(ESkillID skillID, Transform transformToFollow, bool hasBackground = false, Action onClick = null, Action<bool> onHover = null)
	{
		var icon = (CharacterFollowingIcon) iconPooler.GetObjectFromPool();
		GetContainer(transformToFollow).AddIcon(icon, DetermineIconRowIndex(false, false, onClick));
		icon.Setup(skillIcons.SkillIcons[skillID], hasBackground, onClick, onHover);
		return icon;
	}

	public void ShowIcon(ESkillID skillID, Transform transformToFollow, float iconLife)
	{
		var icon = (CharacterFollowingIcon) iconPooler.GetObjectFromPool();
		GetContainer(transformToFollow).AddIcon(icon, DetermineIconRowIndex(false, false, null), statusRowIndex);
		icon.Setup(skillIcons.SkillIcons[skillID], iconLife, HideIcon);
	}

	public void ShowIcon(int mimeticLevel, Transform transformToFollow, OldEnemy enemy)
	{
		ShowMimeticIcon(mimeticLevel, transformToFollow, enemy);
	}
	public void ShowIcon(int mimeticLevel, Transform transformToFollow, PlayerUnit player)
	{
		ShowMimeticIcon(mimeticLevel, transformToFollow, null, player);
	}

	private void ShowMimeticIcon(int mimeticLevel, Transform transformToFollow, OldEnemy enemy = null, PlayerUnit player = null)
	{
		if (--mimeticLevel < 0)
			return;
		
		var icon = (CharacterFollowingIcon) iconPooler.GetObjectFromPool();
		GetContainer(transformToFollow).AddIcon(icon, DetermineIconRowIndex(false, true,null), mimeticRowIndex);
		icon.Setup(GetMimeticIcon(enemy, mimeticLevel));
		if (enemy != null)
		{
			enemyMimeticIcons[enemy] = icon;
		}
		else if (player != null)
		{
			playerMimeticIcons[player] = icon;
		}
	}

	public void HideIcon(PooledObject icon)
	{
		iconPooler.ReturnObjectToPool(icon);
	}

	public void HidePickpocketIcons()
	{
		foreach (var icon in pickpocketIcons)
		{
			HideIcon(icon);
		}
		pickpocketIcons.Clear();
	}
	public void HideMimeticIcons()
	{
		foreach (var icon in enemyMimeticIcons.Values)
		{
			HideIcon(icon);
		}
		foreach (var icon in playerMimeticIcons.Values)
		{
			HideIcon(icon);
		}
		enemyMimeticIcons.Clear();
		playerMimeticIcons.Clear();
	}

	private FollowingIconContainer GetContainer(Transform transformToFollow)
	{
		if (iconsContainers.ContainsKey(transformToFollow))
		{
			return iconsContainers[transformToFollow];
		}

		var container = iconContainerPooler.GetObjectFromPool() as FollowingIconContainer;
		iconsContainers.Add(transformToFollow, container);
		container.ReturnedToPool += OnContainerReturned;
		container.Setup(transformToFollow);
		return container;
	}

	private int DetermineIconRowIndex(bool pickpocketIcon, bool mimeticIcon, Action onClick)
	{
		return pickpocketIcon ?
			pickpocketRowIndex :
			mimeticIcon?
				mimeticRowIndex:
				onClick != null ?
					clickableRowIndex :
					statusRowIndex;
	}

	private Sprite GetMimeticIcon(OldEnemy enemy, int mimeticLevel)
	{
		return enemy == null ?
			skillIcons.TeamMimeticIcons[mimeticLevel] :
			enemy.MimeticDetectionLevel < mimeticLevel ?
				skillIcons.PositiveMimeticIcons[mimeticLevel] :
				skillIcons.NegativeMimeticIcons[mimeticLevel];
	}

	private void SetupMimeticIcon()
	{
		
	}
	
	private void OnContainerReturned(PooledObject container)
	{
		foreach (var iconsContainer in iconsContainers)
		{
			if (iconsContainer.Value != container)
				continue;

			iconsContainer.Value.ReturnedToPool -= OnContainerReturned;
			iconsContainers.Remove(iconsContainer.Key);
			return;
		}
	}

	private void OnMimeticChanged(OldEnemy enemy, int mimeticLevel)
	{
		if (enemy == null)
		{
			HideMimeticIcons();
			return;
		}
		if (--mimeticLevel >= 0)
		{
			if (enemyMimeticIcons.TryGetValue(enemy, out var icon))
			{
				icon.Setup(GetMimeticIcon(enemy, mimeticLevel));
			}
			else
			{
				ShowIcon(mimeticLevel, enemy.IconTransform, enemy);
			}
		}
		else if (enemyMimeticIcons.TryGetValue(enemy, out var icon))
		{
			HideIcon(icon);
			foreach (var mimeticIcon in enemyMimeticIcons)
			{
				if (mimeticIcon.Value != icon)
					continue;
				
				enemyMimeticIcons.Remove(mimeticIcon.Key);
				return;
			}
		}
	}
}
