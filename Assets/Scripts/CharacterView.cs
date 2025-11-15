using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class CharacterView : MonoBehaviour
{
    private float _radius = 0.2f;

    public Transform Transform => transform;
    public float Radius => _radius;

    public virtual void Init(float configuredRadius)
    {
        _radius = configuredRadius;
    }
}
