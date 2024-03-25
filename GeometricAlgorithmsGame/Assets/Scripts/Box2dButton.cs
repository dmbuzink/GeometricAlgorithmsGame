using System;
using UnityEngine;

public class Box2dButton : MonoBehaviour
{
    public event Action OnPressed;

    private void OnMouseUpAsButton()
    {
        this.OnPressed?.Invoke();
    }
}
