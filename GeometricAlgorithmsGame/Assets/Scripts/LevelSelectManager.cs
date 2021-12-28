using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private LevelSelectButton _levelSelectButton;
    [SerializeField] private TextAsset _levelConfigJsonTextAsset;
    [SerializeField] private GameObject _grid;

    // Start is called before the first frame update
    void Start()
    {
        var levelConfigs = LevelConfigManager.LoadLevelConfigs(_levelConfigJsonTextAsset.text);
        foreach (var levelConfig in levelConfigs.OrderBy(lc => lc.LevelId))
        {
            AddButton(levelConfig);
        }
    }

    /// <summary>
    /// Add a level selection button for a level config to the list of levels.
    /// </summary>
    /// <param name="levelConfig"></param>
    private void AddButton(LevelConfig levelConfig)
    {
        var levelButton = Instantiate(_levelSelectButton, _grid.transform);
        var textComponent = levelButton.gameObject.GetComponentInChildren<Text>();
        textComponent.text = $"Level {levelConfig.LevelId}";
        levelButton.OnLevelSelected += () => OnLevelSelectClick(levelConfig);
    }
    
    /// <summary>
    /// Handles the selection of a level.
    /// </summary>
    /// <param name="levelConfig"></param>
    private void OnLevelSelectClick(LevelConfig levelConfig)
    {
        LevelConfigManager.SelectedLevelConfig = levelConfig;
        SceneManager.LoadScene("FloorplanCameraPlacementScene");
    }
}
