using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public event Action OnLevelSelected;
    
    private void OnMouseUpAsButton()
    {
        this.OnLevelSelected?.Invoke();
    }
}
