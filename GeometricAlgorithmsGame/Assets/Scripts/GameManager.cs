using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string _levelConfigJson;
    [SerializeField] private Camera _cameraPrefab;
    [SerializeField] private Floorplan _floorplan; 
    private Camera _currentCamera;
    private LevelConfig _levelConfig;


    // Start is called before the first frame update
    void Start()
    {
        _levelConfig = LevelConfigManager.GetSelectedLevelConfig(this._levelConfigJson);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Confirms the location of the current camera and add it to the floorplan
    /// </summary>
    private async Task PlaceCamera()
    {
        // TODO: To be implemented by Damian M. Buzink
        throw new NotImplementedException();
    }

    /// <summary>
    /// Confirms the camera placements of all cameras, currently in the floorplan.
    /// Make such are proceeding processes are started (like checking for a path, etc.)
    /// </summary>
    private async Task ConfirmAllCameras()
    {
        // TODO: To be implemented by Damian M. Buzink
        throw new NotImplementedException();
    }

    /// <summary>
    /// Show the screen for when the player succeeds
    /// </summary>
    private void ShowSuccessScreen()
    {
        // TODO: To be implemented by Teun van Zon
        throw new NotImplementedException();
    }

    /// <summary>
    /// Show the screen for when the player fails
    /// </summary>
    private void ShowFailureScreen()
    {
        // TODO: To be implemented by Teun van Zon
        throw new NotImplementedException();
    }
}
