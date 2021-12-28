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
    // [SerializeField] private Button _levelSelectButton;
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

    private void AddButton(LevelConfig levelConfig)
    {
        var levelButton = Instantiate(_levelSelectButton, _grid.transform);
        var textComponent = levelButton.gameObject.GetComponentInChildren<Text>();
        textComponent.text = $"Level {levelConfig.LevelId}";
        // levelButton.onClick.AddListener(() =>
        // {
        //     Debug.Log("CLICKED");
        //     OnLevelSelectClick(levelConfig);
        // });
        levelButton.OnLevelSelected += () => OnLevelSelectClick(levelConfig);
    }
    
    private void OnLevelSelectClick(LevelConfig levelConfig)
    {
        LevelConfigManager.SelectedLevelConfig = levelConfig;
        SceneManager.LoadScene("FloorplanCameraPlacementScene");
    }
}
