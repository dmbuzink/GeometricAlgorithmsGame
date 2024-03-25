using System;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public event Action OnLevelSelected;
    
    private void OnMouseUpAsButton()
    {
        this.OnLevelSelected?.Invoke();
    }
}
