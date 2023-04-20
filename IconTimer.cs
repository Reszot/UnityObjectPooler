using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class IconTimer : MonoBehaviour
{
	[SerializeField]
	private Image fillImage = null;

	public void ToggleEnabled(bool enable)
	{
		if (gameObject.activeSelf == enable)
			return;
		
		gameObject.SetActive(enable);
	}
	
	public void StartTimer(float time, Action<PooledObject> returnToPoolAction, PooledObject iconToReturn)
	{
		fillImage.fillAmount = 0f;
		ToggleEnabled(true);
		StartCoroutine(TimeIconLifetime(time, returnToPoolAction, iconToReturn));
	}
	
	private IEnumerator TimeIconLifetime(float time, Action<PooledObject> returnToPoolAction, PooledObject iconToReturn)
	{
		var timeElapsed = 0f;
		var step = 1 / time;
		while (time > timeElapsed)
		{
			var delta = Time.deltaTime;
			timeElapsed += delta;
			fillImage.fillAmount += step * delta;
			yield return null;
		}
		returnToPoolAction(iconToReturn);
		ToggleEnabled(false);
	}
}
