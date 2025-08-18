using UnityEngine;
using UnityEngine.UI;
public enum CardFxType { None, SuperBrillo, Holografico }

public class CardFXRouter : MonoBehaviour
{
    [Header("Refs (UI)")]
    public Image frontImage;      // Front Image
    public RawImage sparklesRT;      // hijo: "Stars RT" (Texture = RT_Stars)
    public RawImage sheenRT;      // hijo: "Sheen RT" (Texture = RT_Sheen)

    [Header("Identificadores por Sprite")]
    public Sprite superBrilloSprite;
    public Sprite holograficoSprite;

    [Header("Global FX")]
    public FXController fx;       // se auto-busca si null

    CardFxType _current = CardFxType.None;
    bool _visible = false;
    bool _claimedStars = false;
    bool _claimedSheen = false;

    void Awake()
    {
        if (!frontImage) frontImage = GetComponentInChildren<Image>(true);

        if (!sparklesRT || !sheenRT)
        {
            var raws = GetComponentsInChildren<RawImage>(true);
            foreach (var r in raws)
            {
                if (r.name.Contains("Stars")) sparklesRT = r;
                else if (r.name.Contains("Sheen")) sheenRT = r;
            }
        }

#if UNITY_2022_2_OR_NEWER
        if (!fx) fx = FindFirstObjectByType<FXController>();
#else
        if (!fx) fx = FindObjectOfType<FXController>();
#endif

        if (sparklesRT) sparklesRT.gameObject.SetActive(false);
        if (sheenRT) sheenRT.gameObject.SetActive(false);
    }

    public void RefreshFxForFront()
    {
        var newType = DetectType(frontImage ? frontImage.sprite : null);
        if (newType == _current) return;

        // Si estaba visible, soltar lo anterior y reclamar lo nuevo.
        if (_visible) ReleaseClaims();

        _current = newType;

        if (_visible) AcquireClaims(); // vuelve a pedir lo que corresponda
        UpdateRawImagesVisibility();
        UpdateUvRects();
    }

    public void SetFxVisible(bool on)
    {
        if (_visible == on) return;
        _visible = on;

        if (_visible) AcquireClaims();
        else ReleaseClaims();

        UpdateRawImagesVisibility();
        UpdateUvRects();
    }

    public void PlayRevealBurstIfAny()
    {
        if (_visible && _current == CardFxType.Holografico && fx)
            fx.HolographicBurst();
    }

    CardFxType DetectType(Sprite s)
    {
        if (!s) return CardFxType.None;
        bool Same(Sprite a, Sprite b) => a && b && (a == b || a.name == b.name);
        if (Same(s, superBrilloSprite)) return CardFxType.SuperBrillo;
        if (Same(s, holograficoSprite)) return CardFxType.Holografico;
        return CardFxType.None;
    }

    void AcquireClaims()
    {
        if (!fx) return;
        if (_current == CardFxType.SuperBrillo && !_claimedStars)
        { fx.UseStars(true); _claimedStars = true; }
        if (_current == CardFxType.Holografico && !_claimedSheen)
        { fx.UseSheen(true); _claimedSheen = true; }
    }

    void ReleaseClaims()
    {
        if (!fx) return;
        if (_claimedStars)
        { fx.UseStars(false); _claimedStars = false; }
        if (_claimedSheen)
        { fx.UseSheen(false); _claimedSheen = false; }
    }

    void UpdateRawImagesVisibility()
    {
        if (sparklesRT) sparklesRT.gameObject.SetActive(_visible && _current == CardFxType.SuperBrillo);
        if (sheenRT) sheenRT.gameObject.SetActive(_visible && _current == CardFxType.Holografico);
    }

    void LateUpdate()
    {
        if (_visible && ((sparklesRT && sparklesRT.gameObject.activeSelf) || (sheenRT && sheenRT.gameObject.activeSelf)))
            UpdateUvRects();
    }

    void UpdateUvRects()
    {
        if (!frontImage) return;
        var rt = frontImage.rectTransform;
        Vector3[] wc = new Vector3[4];
        rt.GetWorldCorners(wc);
        Vector2 bl = RectTransformUtility.WorldToScreenPoint(null, wc[0]);
        Vector2 tr = RectTransformUtility.WorldToScreenPoint(null, wc[2]);

        float x = Mathf.Min(bl.x, tr.x) / Screen.width;
        float y = Mathf.Min(bl.y, tr.y) / Screen.height;
        float w = Mathf.Abs(tr.x - bl.x) / Screen.width;
        float h = Mathf.Abs(tr.y - bl.y) / Screen.height;

        var rect = new Rect(x, y, w, h);
        if (sparklesRT) sparklesRT.uvRect = rect;
        if (sheenRT) sheenRT.uvRect = rect;
    }

    void OnDisable() => ReleaseClaims();
    void OnDestroy() => ReleaseClaims();
}
