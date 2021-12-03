using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DefaultNamespace
{
    public static class LevelConfigManager
    {
        public static int SelectedLevelId { get; set; }
        private static IEnumerable<LevelConfig> _levelConfigs; 
        
        /// <summary>
        /// Gets the selected level config
        /// </summary>
        /// <param name="levelConfigsJson"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static LevelConfig GetSelectedLevelConfig(string levelConfigsJson)
        {
            if (_levelConfigs is null)
            {
                _levelConfigs = JsonUtility.FromJson<IEnumerable<LevelConfig>>(levelConfigsJson);
            }

            var levelConfig = _levelConfigs.FirstOrDefault(lc => lc.LevelId == SelectedLevelId);
            if (levelConfig is null)
            {
                throw new ArgumentException(
                    $"No level configuration with id {SelectedLevelId} could be found in the given json file");
            }

            return levelConfig;
        }
    }

    [Serializable]
    public class LevelConfig
    {
        public int LevelId;
        public Vertex Entrance { get; set; }
        public Vertex DesiredObject { get; set; }
        public IEnumerable<PolygonVertex> polygonVertices { get; set; }
    }
}