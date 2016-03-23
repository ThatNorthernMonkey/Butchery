using Microsoft.Xna.Framework;

namespace Butchery.SaveModels
{
    public class ArtisanGoodsSaveModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int Price { get; set; }
        public int ParentSheetIndex { get; set; }
        public string Location { get; set; }
        public Vector2 Position { get; set; }
    }
}
