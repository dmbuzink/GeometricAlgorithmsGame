using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmButton : MonoBehaviour
{
    public event Action OnPressed;

    private void OnMouseUpAsButton()
    {
        this.OnPressed?.Invoke();
    }
}
