using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScreenManager : MonoBehaviour
{
    [SerializeField] private Image _backgroundScreenImage;
    [SerializeField] private Text _text;
    [SerializeField] private float _alphaOfBackgroundScreen;
    [SerializeField] private Button _retryButton;

    /// <summary>
    /// Handles the showing of the success screen.
    /// </summary>
    public void ShowSuccessScreen()
    {
        this._text.text = "You win!";
        var green = Color.green;
        this._backgroundScreenImage.color = new Color(green.r, green.g, green.b, _alphaOfBackgroundScreen);
        this.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Handles the showing of the failure screen.
    /// </summary>
    public void ShowFailureScreen()
    {
        this._text.text = "You lost";
        var red = Color.red;
        this._backgroundScreenImage.color = new Color(red.r, red.g, red.b, _alphaOfBackgroundScreen);
        this._retryButton.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Handles the loading of the next scene when the player clicks the continue button.
    /// </summary>
    public void LoadNextScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Reloads the current camera placement scene.
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene("FloorplanCameraPlacementScene");
    }
}
