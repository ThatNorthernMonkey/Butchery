using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Butchery
{
    public class MonkeyRecipe : CraftingRecipe
    {
        public static Texture2D monkeySpriteSheet;
        ContentManager MonkeyManager;
        static MonkeyRecipe()
        {
            cookingRecipes.Add("Beef Burger", "78 2 245 1 -6 1/60 1/243/s Mining 3");
            /*

            Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIndexOfMenuView(), 16, 16), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, false, layerDepth, -1, -1, 0.35f);
            public static Texture2D objectSpriteSheet;

            We need to create a new public static Texture2D monkeySpriteSheet;

            if we hook into the ctor and add our recipe to the list, we can pass the first Torch check.



            */
            //craftingRecipes = Game1.content.Load<Dictionary<string, string>>("Data//CraftingRecipes");
            //cookingRecipes = Game1.content.Load<Dictionary<string, string>>("Data//CookingRecipes");
        }
        public MonkeyRecipe(string name, bool isCookingRecipe) : base(name, isCookingRecipe)
        {
            /*
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(root);
            MonkeyManager = new ContentManager(Game1.content.ServiceProvider, root);
            //monkeySpriteSheet = Game1.content.Load<Texture2D>("");
            */
            this.isCookingRecipe = isCookingRecipe;
            this.name = name;
            
        }

        public new void drawMenuView(SpriteBatch b, int x, int y, float layerDepth = 0.88f, bool shadow = true)
        {
            Console.WriteLine("Draw in menu");
        }
    }
}
