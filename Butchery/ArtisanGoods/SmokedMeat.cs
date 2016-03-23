using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Xml.Serialization;


namespace Butchery
{
    public class SmokedMeat : MonkeyFramework.ArtisanGoods
    {

        public SmokedMeat(Texture2D spriteSheet):base(spriteSheet)
        {
            price = 600;
            parentSheetIndex = 666;
            name = "Smoked Meat";
            description = "Smoked meat.";
        }
    }
}
