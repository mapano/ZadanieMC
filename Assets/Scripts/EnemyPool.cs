using System.Collections.Generic;
using UnityEngine;

public class EnemyPool
{
    private readonly EnemyView _prefab;
    private readonly Transform _parent;
    private readonly Stack<EnemyView> _pool = new Stack<EnemyView>();

    public EnemyPool(EnemyView prefab, Transform parent, int initialSize)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var view = Object.Instantiate(_prefab, _parent);
            view.gameObject.SetActive(false);
            _pool.Push(view);
        }
    }

    public EnemyView Get(Vector3 position)
    {
        EnemyView view = _pool.Count > 0 ? _pool.Pop() : Object.Instantiate(_prefab, _parent);
        view.transform.position = position;
        view.gameObject.SetActive(true);
        return view;
    }
}
