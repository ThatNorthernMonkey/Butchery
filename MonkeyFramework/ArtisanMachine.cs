using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Xml.Serialization;

namespace MonkeyFramework
{

    public class ArtisanMachine : StardewValley.Object
    {

        private Texture2D _spriteSheet;
        /*

            Index: "Name/Price/Ediblity/CatD I/Desc/Outdoors/Indoors/Fragility/?isLamp?/
            12: "Keg/50/-300/Crafting -9/Place a fruit or vegetable in here. Eventually it will turn into a beverage./true/true/0"



            This draws your artisan machine in the world.
            public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)

            This handles dropping items into your artisan machine
            public virtual bool performObjectDropInAction(Object dropIn, bool probe, Farmer who)

            Draws placement bounds when machine is in your hands
            public virtual void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)

            See if we're clicking on the machine, and what to do.
            public virtual bool checkForAction(Farmer who, bool justCheckingForActivity = false)

            Updates your machines state at the start of each day
            public virtual void DayUpdate(GameLocation location)

            Re-implement this for our version of the sprite sheet.
            public static Rectangle getSourceRectForBigCraftable(int index)
            {
            	return new Rectangle(index % (Game1.bigCraftableSpriteSheet.Width / 16) * 16, index * 16 / Game1.bigCraftableSpriteSheet.Width * 16 * 2, 16, 32);
            }

            isPlaceable()
            bigCraftable : bool <--- Unsure whether to use.

        */

        public string description;

        public Rectangle getSourceRectForButcheryMachine(int index)
        {
            return new Rectangle(index % (_spriteSheet.Width / 16) * 16, index * 16 / _spriteSheet.Width * 16 * 2, 16, 32);
        }

        public ArtisanMachine(Texture2D spriteSheet)
        {
            _spriteSheet = spriteSheet;

            category = -9;
            price = 50;
            canBeSetDown = true;
            setOutdoors = true;
            setIndoors = true;
            bigCraftable = true;
            isOn = false;
            fragility = 0;
            Edibility = -300;
            type = "Crafting";

        }

        public override bool isPassable()
        {
            return false;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            Rectangle sourceRectForButcheryMachine = getSourceRectForButcheryMachine(parentSheetIndex);
            spriteBatch.Draw(_spriteSheet, location + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2), new Rectangle?(sourceRectForButcheryMachine), Color.White * transparency, 0f, new Vector2(8f, 16f), Game1.pixelZoom * (scaleSize < 0.2 ? scaleSize : (scaleSize / 2f)), SpriteEffects.None, layerDepth);
        }

        public override string getDescription()
        {
            return description;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            //TODO: Implement ready for harvest icon when ready
            Vector2 value = getScale();
            value *= Game1.pixelZoom;
            Vector2 value2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize));
            Rectangle destinationRectangle = new Rectangle((int)(value2.X - value.X / 2f) + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(value2.Y - value.Y / 2f) + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(Game1.tileSize + value.X), (int)(Game1.tileSize * 2 + value.Y / 2f));
            spriteBatch.Draw(_spriteSheet, destinationRectangle, new Rectangle?(getSourceRectForButcheryMachine(parentSheetIndex)), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, Math.Max(0f, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f) + ((parentSheetIndex == 105) ? 0.0035f : 0f) + x * 1E-08f);
        }

        public override bool performToolAction(Tool t)
        {
            return false;
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            //Console.WriteLine("Minutes Elapsed: {0}", minutes);
            if (minutesUntilReady > 0 && isOn)
            {
                minutesUntilReady -= minutes;
                readyForHarvest = false;
                return false;
            }

            if (minutesUntilReady <= 0 && isOn)
            {
                readyForHarvest = true;
                return false;
            }
            return false;
        }

        public override bool performObjectDropInAction(StardewValley.Object dropIn, bool probe, Farmer who)
        {
            if (dropIn != null)
            {
                if (dropIn.parentSheetIndex >= 639 && dropIn.parentSheetIndex <= 644)
                {
                    if (!probe)
                    {
                        Game1.playSound("coin");
                        minutesUntilReady = 10;
                        isOn = true;
                        hasBeenPickedUpByFarmer = false;
                        /*
                        heldObject = new Butchery.ButcherArtisanGoods("Smoked " + dropIn.name);
                        heldObject.tileLocation = Vector2.Zero;
                        heldObject.canBeGrabbed = true;
                        heldObject.canBeSetDown = false;
                        heldObject.isHoedirt = false;
                        heldObject.isSpawnedObject = false;
                        */
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }


        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
            {

            }
            else
            {
                if (heldObject != null && readyForHarvest && !hasBeenPickedUpByFarmer)
                {
                    heldObject = (StardewValley.Object)heldObject.getOne();
                    who.addItemToInventory(heldObject);
                    hasBeenPickedUpByFarmer = true;
                    heldObject = null;
                    isOn = false;
                    readyForHarvest = false;
                }
                else
                {
                    Console.WriteLine("null");
                }
            }

            return true;
        }

        public override void DayUpdate(GameLocation location)
        {

        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            spriteBatch.Draw(_spriteSheet, objectPosition, new Rectangle?(getSourceRectForButcheryMachine(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 2) / 10000f));
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            boundingBox.X = x;
            boundingBox.Y = y;
            boundingBox.Width = Game1.tileSize;
            boundingBox.Height = Game1.tileSize;
            Game1.playSound("thudStep");
            location.objects.Add(new Vector2 { X = x / Game1.tileSize, Y = y / Game1.tileSize }, this);
            return true;
        }

        public override bool performDropDownAction(Farmer who)
        {
            return true;
        }
    }
}
