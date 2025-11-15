using UnityEngine;

public class CameraBounds
{
    private readonly Camera _camera;
    private readonly Transform _cameraTransform;

    public CameraBounds(Camera camera)
    {
        _camera = camera;
        _cameraTransform = camera.transform;
    }

    public Vector2 Clamp(Vector2 position, float radius)
    {
        var halfHeight = _camera.orthographicSize;
        var halfWidth = halfHeight * _camera.aspect;

        var center = (Vector2)_cameraTransform.position;
        var minX = center.x - halfWidth + radius;
        var maxX = center.x + halfWidth - radius;
        var minY = center.y - halfHeight + radius;
        var maxY = center.y + halfHeight - radius;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

    public Rect GetWorldRect()
    {
        var halfHeight = _camera.orthographicSize;
        var halfWidth = halfHeight * _camera.aspect;
        var size = new Vector2(halfWidth * 2f, halfHeight * 2f);
        var min = (Vector2)_cameraTransform.position - size * 0.5f;
        return new Rect(min, size);
    }
}
