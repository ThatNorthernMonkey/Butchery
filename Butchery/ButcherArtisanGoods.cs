using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Xml.Serialization;
using Butchery.SaveModels;

namespace Butchery
{
    public class ButcherArtisanGoods : StardewValley.Object
    {

        public string description;

        public ButcherArtisanGoods(string artisanName)
        {
            name = artisanName;
            description = "Smoked meat.";
            category = -26;
            price = 600;
            parentSheetIndex = 666;        
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            drawStackNumber = true;
            spriteBatch.Draw(Butchery.objectSpriteSheet, location + new Vector2((Game1.tileSize / 2) * scaleSize, ((Game1.tileSize / 2) * scaleSize)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Butchery.objectSpriteSheet, parentSheetIndex, 16, 16)), Color.White * transparency, 0f, new Vector2(8f, 8f) * scaleSize, Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);

            //Draw stack size in menu
            if (drawStackNumber && maximumStackSize() > 1 && scaleSize > 0.3 && Stack != 2147483647 && Stack > 1)
            {
                float num = 0.5f + scaleSize;
                Game1.drawWithBorder(string.Concat(stack), Color.Black, Color.White, location + new Vector2((float)Game1.tileSize - Game1.tinyFont.MeasureString(string.Concat(stack)).X * num, Game1.tileSize - Game1.tinyFont.MeasureString(string.Concat(stack)).Y * 3f / 4f * num), 0f, num, 1f, true);
            }
        }

        public override string getDescription()
        {
            return description;
        }

    }
}
