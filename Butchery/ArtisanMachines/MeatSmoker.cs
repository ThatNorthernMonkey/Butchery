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

namespace Butchery.ArtisanMachines
{
    public class MeatSmoker : MonkeyFramework.ArtisanMachine
    {

        public MeatSmoker(Texture2D spriteSheet) : base(spriteSheet)
        {
            name = "Meat Smoker";
            description = "Create smoked meat.";
            parentSheetIndex = 164;
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
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
                        heldObject = new MeatSmoker(Butchery.objectSpriteSheet);
                        heldObject.tileLocation = Vector2.Zero;
                        heldObject.canBeGrabbed = true;
                        heldObject.canBeSetDown = false;
                        heldObject.isHoedirt = false;
                        heldObject.isSpawnedObject = false;                     
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
                //TODO: Left this crashing when using the Meat Smoker
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
    }
}
