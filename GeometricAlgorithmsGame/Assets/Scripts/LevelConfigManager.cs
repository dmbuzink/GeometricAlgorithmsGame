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
        public static LevelConfig SelectedLevelConfig = GetDefaultLevelConfig();
        public static IEnumerable<LevelConfig> LevelConfigs;

        /// <summary>
        /// Load the level configs based on the json file
        /// </summary>
        /// <param name="levelConfigJson"></param>
        /// <returns></returns>
        public static IEnumerable<LevelConfig> LoadLevelConfigs(string levelConfigJson) =>
            LevelConfigs ??= Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<LevelConfig>>(levelConfigJson);

        /// <summary>
        /// Gets the configuration for the default level.
        /// </summary>
        /// <returns></returns>
        private static LevelConfig GetDefaultLevelConfig()
        {
            var entrance = new Vertex(11.7, 7);
            var desiredObject = new Vertex(5.1, 5);
        
            var v1 = new Vertex(1, 1);
            var v2 = new Vertex(2, 2);
            var v3 = new Vertex(1.5, 3);
            var v4 = new Vertex(3, 4);
            var v5 = new Vertex(5, 7);
            var v6 = new Vertex(12, 7);
            var v7 = new Vertex(16, 5);
            var v8 = new Vertex(15, 2.5);
            var v9 = new Vertex(14, 1.7);
            var v10 = new Vertex(6, 1.2);

            return new LevelConfig()
            {
                LevelId = 0,
                Entrance = entrance,
                DesiredObject = desiredObject,
                Vertices = new[] { v10, v9, v8, v7, v6, v5, v4, v3, v2, v1 }
            };
        }
    }

    [Serializable]
    public class LevelConfig
    {
        public int LevelId;
        public Vertex Entrance { get; set; }
        public Vertex DesiredObject { get; set; }
        public IEnumerable<Vertex> Vertices { get; set; }

        public SimplePolygon GetSimplePolygon() => new SimplePolygon(Vertices);
    }
}