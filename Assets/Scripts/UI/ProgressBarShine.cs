using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgressBarShine : MonoBehaviour
{
    [Header("Refs")]
    public Image fill;          // tu progressBarFill (Fill Amount)
    public Image glow;          // la Image "Glow"
    public Image shine;         // la Image "Shine" (la banda)

    [Header("Glow")]
    [Range(0f, 1f)] public float minGlowA = 0.08f;
    [Range(0f, 1f)] public float maxGlowA = 0.55f;
    [Range(0.5f, 3f)] public float glowCurve = 1.6f;

    [Header("Shine sweep")]
    public float sweepTime = 0.45f;
    public float sweepAngle = 20f;
    public float sweepPadding = 80f;     // cuánto sale fuera de los bordes
    public float triggerStep = 0.08f;    // cada cuánto progreso lanzamos un sweep
    [Range(0f, 1f)] public float shineAlpha = 0.7f;

    float _lastFill;
    float _lastSweepAt;

    void Awake()
    {
        if (!fill) fill = GetComponent<Image>(); // en vez de GetComponentInChildren
        if (shine) shine.gameObject.SetActive(false);
        _lastFill = fill ? fill.fillAmount : 0f;
        _lastSweepAt = _lastFill;
    }

    void LateUpdate()
    {
        if (!fill) return;

        float f = fill.fillAmount;

        // Glow: sube con el progreso
        if (glow)
        {
            var c = glow.color;
            c.a = Mathf.Lerp(minGlowA, maxGlowA, Mathf.Pow(f, glowCurve));
            glow.color = c;
        }

        // Shine: barre cada cierto incremento
        if (f > _lastSweepAt + triggerStep)
        {
            RunSweep();
            _lastSweepAt = f;
        }

        _lastFill = f;
    }

    void RunSweep()
    {
        if (!shine) return;

        var rt = shine.rectTransform;
        var parent = rt.parent as RectTransform;
        if (!parent) return;

        float halfW = parent.rect.width * 0.5f;
        float startX = -halfW - sweepPadding;
        float endX = halfW + sweepPadding;

        rt.DOKill();
        shine.DOFade(0f, 0f);
        rt.localEulerAngles = new Vector3(0, 0, sweepAngle);
        rt.anchoredPosition = new Vector2(startX, 0f);
        shine.gameObject.SetActive(true);

        // entra, cruza y se va
        shine.DOFade(shineAlpha, 0.10f);
        rt.DOAnchorPosX(endX, sweepTime).SetEase(Ease.InOutSine)
          .OnComplete(() => shine.DOFade(0f, 0.12f));
    }

    public void ResetEffects(float startFill = 0f)
    {
        if (fill)
        {
            fill.DOKill();
            fill.fillAmount = startFill;
        }

        // Reset glow (si luego lo volvés a usar)
        if (glow)
        {
            glow.DOKill();
            var gc = glow.color;
            gc.a = Mathf.Lerp(minGlowA, maxGlowA, Mathf.Pow(startFill, glowCurve));
            glow.color = gc;
        }

        // Reset shine
        if (shine)
        {
            shine.DOKill();
            var sc = shine.color; sc.a = 0f; shine.color = sc;
            shine.gameObject.SetActive(false);
            var rt = shine.rectTransform;
            rt.anchoredPosition = Vector2.zero;
            rt.localEulerAngles = Vector3.zero;
        }

        _lastFill = startFill;
        _lastSweepAt = startFill;   // <- clave para que el primer avance dispare sweep
    }
}
