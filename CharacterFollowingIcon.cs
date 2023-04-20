using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CharacterFollowingIcon : PooledObject, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Image icon = null;
	[SerializeField]
	private Button button = null;
	[SerializeField]
	private Image iconBackground = null;
	[SerializeField]
	private IconTimer iconTimer = null;

	private Action onClick;
	private Action<bool> onHover;

	private void Awake()
	{
		button.onClick.AddListener(OnClick);
	}

	public override void Init(Action<PooledObject> returnToPool)
	{
		base.Init(returnToPool);
		iconTimer.gameObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!button.enabled)
			return;
		
		onHover(true);
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!button.enabled)
			return;
		
		onHover(false);
	}
	
	public void Setup(Sprite iconSprite, bool hasBackground = false, Action onClickAction = null, Action<bool> onHoverAction = null)
	{
		iconTimer.ToggleEnabled(false);
		icon.sprite = iconSprite;
		iconBackground.enabled = hasBackground;
		onClick = onClickAction;
		button.enabled = onClick != null;
		onHover = onHoverAction;
		ToggleEnabled(true);
	}

	public void Setup(Sprite iconSprite, float displayTime, Action<PooledObject> returnToPoolAction)
	{
		ToggleEnabled(false);
		icon.sprite = iconSprite;
		iconBackground.enabled = false;
		button.enabled = false;
		ToggleEnabled(true);
		if (displayTime <= 0)
			return;
		
		iconTimer.StartTimer(displayTime, returnToPoolAction, this);
	}

	public override void Reset(bool commandFromPool = false)
	{
		iconTimer.StopAllCoroutines();
		base.Reset(commandFromPool);
		onClick = null;
		onHover = null;
		iconBackground.enabled = false;
		button.enabled = false;
	}

	private void OnClick()
	{
		onClick();
	}
}
