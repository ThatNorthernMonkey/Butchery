using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butchery
{
    public class ButcherKnife : Tool
    {
        private FarmAnimal animal;



        public ButcherKnife() : base("Butcher Knife", -1, 6, 6, "Butcher your animals for meat.", false, 0)
        {

        }

        public object Rectangle { get; private set; }

        public override void beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            x = (int)Game1.player.GetToolLocation(false).X;
            y = (int)Game1.player.GetToolLocation(false).Y;

            Rectangle value = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);

            if (location is Farm)
            {
                using (Dictionary<long, FarmAnimal>.ValueCollection.Enumerator enumerator = (location as Farm).animals.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        FarmAnimal current = enumerator.Current;
                        if (current.GetBoundingBox().Intersects(value))
                        {
                            animal = current;
                            break;
                        }
                    }
                    goto WeHaveActionedOnAnAnimal;
                }
            }

            if (location is AnimalHouse)
            {
                foreach (FarmAnimal current2 in (location as AnimalHouse).animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(value))
                    {
                        animal = current2;
                        break;
                    }
                }
            }

            WeHaveActionedOnAnAnimal:
            Console.WriteLine("This is where the.. meat of the tool goes.....");
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            Console.WriteLine("Doing a function so it seems.");
        }
    }
}
