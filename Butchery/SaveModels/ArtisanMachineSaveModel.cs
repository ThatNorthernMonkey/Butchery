using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butchery.SaveModels
{
    public class ArtisanMachineSaveModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int Price { get; set; }
        public int ParentSheetIndex { get; set; }
        public string Location { get; set; }
        public Vector2 Position { get; set; }
        public int MinutesUntilReady { get; set; }
        public bool CanBeSetDown { get; set; }
        public bool SetOutdoors { get; set; }
        public bool SetIndoors { get; set; }
        public bool BigCraftable { get; set; }
        public int Fragility { get; set; }
        public int Edibility { get; set; }
        public string Type { get; set; }
        public bool IsOn { get; set; }
    }
}
