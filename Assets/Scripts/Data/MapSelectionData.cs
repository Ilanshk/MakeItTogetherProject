using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(menuName = "Data/MapSelectionData", fileName = "MapSelectionData")]
    public class MapSelectionData : ScriptableObject
    {
        public List<MapInfo> Maps;
    }
}

[Serializable]
public struct MapInfo
{
    public Color MapThumbnail;//may be map image
    public string MapName;
    public string SceneName;
}

