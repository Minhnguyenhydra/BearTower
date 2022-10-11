using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class SafeArea : UIBehaviour
{
	public enum AdsPosition
	{
		NONE,
		TOP,
		BOTTOM,
	}

	[SerializeField] private AdsPosition adsPosition; 

	[System.NonSerialized] private RectTransform cachedRectTransform;

	[System.NonSerialized] private bool cachedRectTransformSet;

	/// <summary>This method will instantly update the safe area RectTransform.</summary>
	[ContextMenu("Update Safe Area")]
	public void UpdateSafeArea()
	{
		if (cachedRectTransformSet == false)
		{
			cachedRectTransform = GetComponent<RectTransform>();
			cachedRectTransformSet = true;
		}

		var safeRect = Screen.safeArea;
		var screenW = Screen.width;
		var screenH = Screen.height;
		var safeMin = safeRect.min;
		var safeMax = safeRect.max;


		safeMin.x = Mathf.Max(safeMin.x, 0);
		safeMax.x = Mathf.Min(safeMax.x, screenW);


		safeMin.y = Mathf.Max(safeMin.y, 0);
		safeMax.y = Mathf.Min(safeMax.y, screenH);


		cachedRectTransform.anchorMin = new Vector2(safeMin.x / screenW, safeMin.y / screenH);
		cachedRectTransform.anchorMax = new Vector2(safeMax.x / screenW, safeMax.y / screenH);

		cachedRectTransform.offsetMin = new Vector2(cachedRectTransform.offsetMin.x,
			adsPosition == AdsPosition.BOTTOM ? BannerHeight : 0);
		cachedRectTransform.offsetMax = new Vector2(cachedRectTransform.offsetMax.x,
			adsPosition == AdsPosition.TOP ? -BannerHeight : 0);
	}
	private static int BannerHeight {
		get
		{
			if (Screen.height <= 400 * Mathf.RoundToInt(Screen.dpi / 160))
				return 32 * Mathf.RoundToInt(Screen.dpi / 160);
			if (Screen.height <= 720 * Mathf.RoundToInt(Screen.dpi / 160))
				return 50 * Mathf.RoundToInt(Screen.dpi / 160);
			return 90 * Mathf.RoundToInt(Screen.dpi / 160);
		}
	}
	protected override void Start()
	{
		UpdateSafeArea();
	}

	protected virtual void Update()
	{
		//todo
		UpdateSafeArea();
	}
}