using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainMenuManager : MonoBehaviour
{
    // UI canvases
    [SerializeField] private GameObject levelSelectorGO;
    [SerializeField] private GameObject menuGO;
    [SerializeField] private GameObject explanationGO;
    
    // UI Buttons
    [SerializeField] private Box2dButton ShowExplanationButton;
    [SerializeField] private Box2dButton ShowLevelSelectionButton;
    [SerializeField] private Box2dButton[] ReturnToMainMenuButtons;
    
    
    // Start is called before the first frame update
    void Start()
    {
        ShowExplanationButton.OnPressed += ShowExplanation;
        ShowLevelSelectionButton.OnPressed += ShowLevelSelection;
        foreach (var returnToMainMenuButton in ReturnToMainMenuButtons)
        {
            returnToMainMenuButton.OnPressed += ShowMainMenu;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Removes the OnPressed events handlers.
    /// </summary>
    public void StopButtonEvents()
    {
        ShowExplanationButton.OnPressed -= ShowExplanation;
        ShowLevelSelectionButton.OnPressed -= ShowLevelSelection;
        foreach (var returnToMainMenuButton in ReturnToMainMenuButtons)
        {
            returnToMainMenuButton.OnPressed -= ShowMainMenu;
        }
    }

    /// <summary>
    /// Disables the other menus and shows the main menu
    /// </summary>
    private void ShowMainMenu()
    {
        DeactivateAllCanvases();
        this.menuGO.SetActive(true);
    }
    
    /// <summary>
    /// Disables the other menus and shows the level selection menu
    /// </summary>
    private void ShowLevelSelection()
    {
        DeactivateAllCanvases();
        this.levelSelectorGO.SetActive(true);
    }
    
    /// <summary>
    /// Disables the other menus and shows the explanation menu
    /// </summary>
    private void ShowExplanation()
    {
        DeactivateAllCanvases();
        this.explanationGO.SetActive(true);
    }
    
    /// <summary>
    /// Disables all menus
    /// </summary>
    private void DeactivateAllCanvases()
    {
        this.levelSelectorGO.SetActive(false);
        this.menuGO.SetActive(false);
        this.explanationGO.SetActive(false);
    }
}
