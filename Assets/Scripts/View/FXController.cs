using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FXController : MonoBehaviour
{
    [Header("PS Holográfico (Sheen)")]
    public ParticleSystem psHoloSheen;   // barra diagonal (Trace)

    [Header("PS Súper Brillo (Stars)")]
    public ParticleSystem psSparkles;    // estrellitas

    [Header("Rates por defecto")]
    public float starsRate = 18f;  // rateOverTime cuando hay al menos 1 carta con stars
    public float sheenRate = 1.5f; // rateOverTime cuando hay al menos 1 carta con sheen

    // ref-counts para evitar que los efectos se pisen entre sí
    int _starsUsers = 0;
    int _sheenUsers = 0;

    // ------------------ helpers internos ------------------
    void SetRate(ParticleSystem ps, float rate)
    {
        if (!ps) return;

        var em = ps.emission;
        em.enabled = rate > 0f || em.burstCount > 0;
        em.rateOverTime = rate;

        if (rate > 0f && !ps.isPlaying) ps.Play();
        if (rate == 0f && ps.isPlaying && em.burstCount == 0)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void Refresh()
    {
        SetRate(psSparkles, _starsUsers > 0 ? starsRate : 0f);
        SetRate(psHoloSheen, _sheenUsers > 0 ? sheenRate : 0f);
    }

    // ------------------ API para las cartas ------------------
    /// Llama CardFXRouter al mostrar/ocultar una carta SuperBrillo
    public void UseStars(bool on)
    {
        _starsUsers += on ? 1 : -1;
        if (_starsUsers < 0) _starsUsers = 0;
        Refresh();
    }

    /// Llama CardFXRouter al mostrar/ocultar una carta Holográfica
    public void UseSheen(bool on)
    {
        _sheenUsers += on ? 1 : -1;
        if (_sheenUsers < 0) _sheenUsers = 0;
        Refresh();
    }

    /// Disparo puntual extra para holográfica (al revelarse)
    public void HolographicBurst()
    {
        if (!psHoloSheen) return;

        var em = psHoloSheen.emission;
        em.SetBursts(new ParticleSystem.Burst[] { new(0f, (short)Random.Range(3, 7)) });
        psHoloSheen.Play();

        // limpiar bursts tras salir las partículas
        float delay = psHoloSheen.main.startDelay.constantMax +
                      psHoloSheen.main.startLifetime.constantMax + 0.05f;
        StartCoroutine(ClearBurstsAfter(psHoloSheen, delay));
    }

    // ------------------ utilidades ------------------
    IEnumerator ClearBurstsAfter(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!ps) yield break;

        var em = ps.emission;
        em.SetBursts(System.Array.Empty<ParticleSystem.Burst>());
    }

    void OnDisable()
    {
        _starsUsers = 0;
        _sheenUsers = 0;
        Refresh();
    }
}
