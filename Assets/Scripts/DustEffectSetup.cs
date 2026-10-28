using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DustEffectSetup : MonoBehaviour
{
    [SerializeField] private Sprite dustSprite;
    [SerializeField] private Color dustColour = new Color(0.78f, 0.65f, 0.45f, 0.8f);
    [SerializeField] private float sizeScale = 1f;
    [SerializeField] private int sortingOrder = 1;

    void Awake()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f * sizeScale, 0.3f * sizeScale);
        main.startColor = dustColour;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 18f;

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f * sizeScale;

        ParticleSystem.ColorOverLifetimeModule colour = ps.colorOverLifetime;
        colour.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        colour.color = fade;

        ParticleSystem.SizeOverLifetimeModule size = ps.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        if (dustSprite != null)
        {
            ParticleSystem.TextureSheetAnimationModule sheet = ps.textureSheetAnimation;
            sheet.enabled = true;
            sheet.mode = ParticleSystemAnimationMode.Sprites;
            sheet.AddSprite(dustSprite);
        }

        ParticleSystemRenderer rend = GetComponent<ParticleSystemRenderer>();
        rend.material = new Material(Shader.Find("Sprites/Default"));
        rend.sortingOrder = sortingOrder;
    }
}