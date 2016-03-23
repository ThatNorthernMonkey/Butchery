using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MonkeySaveGameSerializer;
using Butchery.SaveModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;

namespace Butchery
{
    public class Butchery : Mod
    {
        const int BEEF = 639;
        const int PORK = 640;
        const int CHICKEN = 641;       
        const int DUCK = 642;
        const int RABBIT = 643;
        const int GOAT = 644;
        const int MUTTON = 644;

        public static Texture2D objectSpriteSheet;
        public static Texture2D machineSpriteSheet;
        public static Dictionary<int, string> monkeyInformation;

        public static List<ArtisanGoodsSaveModel> ArtisanGoodsToSave;
        public static List<ArtisanMachinesSaveModel> ArtisanMachinesToSave;

        private MonkeySaveGame Butchery_ArtisanGoods;
        private MonkeySaveGame Butchery_ArtisanMachines;


        ContentManager MonkeyManager;


        public StardewValley.Object Beef { get; set; }
        public int ButcheryLevel { get; set; }
        private bool MeatHasLoaded { get; set; }
        private bool HasReceivedItem { get; set; }
        public bool HasCreatedUniqueSaveItems { get; set; }

        public Butchery()
        {
            ArtisanGoodsToSave = new List<ArtisanGoodsSaveModel>();
            ArtisanMachinesToSave = new List<ArtisanMachinesSaveModel>();

            MeatHasLoaded = false;
            HasReceivedItem = false;
            HasCreatedUniqueSaveItems = false;

        }

        public override void Entry(params object[] objects)
        {
            ControlEvents.MouseChanged += Events_WhenHittingAnimal;
            ControlEvents.KeyPressed += Events_OnKeyPressed;
            LocationEvents.CurrentLocationChanged += Events_OnAreaChange;        
        }

        private void Events_OnKeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                if (!HasCreatedUniqueSaveItems)
                {
                    Butchery_ArtisanGoods.CreateUniqueIdForThisSave(Game1.player.Name, Game1.player.eyeColor, Game1.player.hairColor, Game1.player.hair, Game1.player.farmName);
                    Butchery_ArtisanMachines.CreateUniqueIdForThisSave(Game1.player.Name, Game1.player.eyeColor, Game1.player.hairColor, Game1.player.hair, Game1.player.farmName);
                    HasCreatedUniqueSaveItems = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                // Update and save
                ArtisanGoodsToSave = new List<ArtisanGoodsSaveModel>();
                ArtisanMachinesToSave = new List<ArtisanMachinesSaveModel>();
                UpdateArtisanGoodsToSave();
                UpdateArtisanMachinesToSave();
                Console.WriteLine("Update and save");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                // Delete
                //ArtisanGoodsToSave = new List<ArtisanGoodsSaveModel>();
                DeleteAllArtisanMachinesBeforeSleep();
                DeleteAllArtisanGoodsBeforeSleep();
                Console.WriteLine("Delete");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                // Load and respawn
                LoadArtisanMachines();
                LoadArtisanGoods();
                Console.WriteLine("Load and spawn");
            }
        }

        private void Events_OnAreaChange(object sender, EventArgsCurrentLocationChanged e)
        {

            if(!MeatHasLoaded)
            {
                Game1.objectInformation.Add(BEEF, "Beef/200/15/Basic -14/Meat from a Cow. Fatty and slightly sweet.");
                Game1.objectInformation.Add(PORK, "Pork/300/15/Basic -14/Meat from a Pig.");
                Game1.objectInformation.Add(CHICKEN, "Chicken/100/15/Basic -14/The meat of a Chicken. It's mild.");
                Game1.objectInformation.Add(DUCK, "Duck/150/15/Basic -14/The meat of a Duck. It's darker and richer than chicken meat.");
                Game1.objectInformation.Add(RABBIT, "Rabbit/150/15/Basic -14/The meat from a Rabbit. It's very lean.");
                Game1.objectInformation.Add(GOAT, "Goat/200/15/Basic -14/Meat from a Goat. Leaner than Chicken.");
                MeatHasLoaded = true;

                var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                MonkeyManager = new ContentManager(Game1.content.ServiceProvider, root);
                objectSpriteSheet = MonkeyManager.Load<Texture2D>("springobjects");
                machineSpriteSheet = MonkeyManager.Load<Texture2D>("Craftables");
                monkeyInformation = MonkeyManager.Load<Dictionary<int, string>>("ObjectInformation");


                Butchery_ArtisanGoods = new MonkeySaveGame(root, "butchery_artisangoods");
                Butchery_ArtisanMachines = new MonkeySaveGame(root, "butchery_artisanmachines");

            }

            if (Game1.currentLocation.name.Equals("Farm") && !HasReceivedItem)
            {

                
                //var burger = new ButcherArtisanGoods();
                //Game1.player.addItemToInventory(burger);
                
                /*
                for(int i = 0; i < 6; i++)
                {
                    var smoker = new ButcherArtisanMachine();
                    Game1.player.addItemToInventory(smoker);
                }
                */
            }
        }

        private void Events_WhenHittingAnimal(object sender, EventArgsMouseStateChanged e)
        {
            if (Game1.didPlayerJustLeftClick() && Game1.hasLoadedGame)
            {
                var terrFeats = Game1.currentLocation.terrainFeatures;
                var currentItem = Game1.player.Items[Game1.player.CurrentToolIndex];

                if (currentItem is MeleeWeapon)
                {

                    

                    var farm = Game1.getFarm();
                    var animals = farm.getAllFarmAnimals();
                    
                    var toolAct = Game1.player.GetToolLocation();

                    if(animals != null)
                    {
                        var hitAnimal = animals.Where(x => (int)(x.position.X / Game1.tileSize) == (int)(toolAct.X / Game1.tileSize) && (int)(x.position.Y / Game1.tileSize) == (int)(toolAct.Y / Game1.tileSize));
                        var hitAnimalFront = animals.Where(x => (int)(x.frontBackBoundingBox.X / Game1.tileSize) == (int)(toolAct.X / Game1.tileSize) && (int)(x.frontBackBoundingBox.Y / Game1.tileSize) == (int)(toolAct.Y / Game1.tileSize));
                        var hitAnimalSide = animals.Where(x => (int)(x.sidewaysBoundingBox.X / Game1.tileSize) == (int)(toolAct.X / Game1.tileSize) && (int)(x.sidewaysBoundingBox.Y / Game1.tileSize) == (int)(toolAct.Y / Game1.tileSize));

                        Vector2 zero = Vector2.Zero;
                        var melWeap = currentItem as MeleeWeapon;
                        Rectangle areaOfEffect = melWeap.getAreaOfEffect((int)toolAct.X, (int)toolAct.Y, Game1.player.FacingDirection, ref zero, ref zero, Game1.player.GetBoundingBox(), Game1.player.FarmerSprite.indexInCurrentAnimation);

                        foreach(var animal in animals)
                        {
                            if(animal.GetBoundingBox().Intersects(areaOfEffect))
                            {

                                var animationPos = new Vector2 { X = toolAct.X, Y = toolAct.Y - 43 };
                                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, animationPos, Color.DarkRed, 10, false, 100f, 0, -1, -1f, -1, 0));
                                
                                animal.hitGlowTimer = 300;
                                animal.health--;
                                Game1.playSound(animal.sound);

                                if(animal.health <= 0)
                                {
                                    /*
                                    < 30 is sad
                                    < 200 is fine
                                    >= 200 is happy
                                    */
                                    var quality = 0;
                                    
                                    if(animal.happiness <= 30)
                                    {
                                        quality = 0;
                                    }

                                    if(animal.happiness > 30 && animal.happiness < 200)
                                    {
                                        quality = 1;
                                    }

                                    if(animal.happiness >= 200)
                                    {
                                        quality = 2;
                                    }

                                    
                                    var animToRemove = farm.animals.FirstOrDefault(x => x.Value == animal);
                                    farm.animals.Remove(animToRemove.Key);

                                    var meatObjectToCreate = CHICKEN;

                                    if(animal.type.Contains("Chicken"))
                                    {
                                        meatObjectToCreate = CHICKEN;
                                    }

                                    if(animal.type.Contains("Cow"))
                                    {
                                        meatObjectToCreate = BEEF;
                                    }

                                    if (animal.type.Contains("Pig"))
                                    {
                                        meatObjectToCreate = PORK;
                                    }

                                    if (animal.type.Contains("Duck"))
                                    {
                                        meatObjectToCreate = DUCK;
                                    }

                                    if (animal.type.Contains("Rabbit"))
                                    {
                                        meatObjectToCreate = RABBIT;
                                    }

                                    if (animal.type.Contains("Goat"))
                                    {
                                        meatObjectToCreate = GOAT;
                                    }

                                    if (animal.type.Contains("Sheep"))
                                    {
                                        //meatObjectToCreate = MUTTON;
                                    }

                                    Game1.createObjectDebris(meatObjectToCreate, (int)toolAct.X / 64, (int)toolAct.Y / 64, -1, quality, 1f);
                                }                               
                            }
                        }
                    }
                }
            }
        }

        private void TestSaving()
        {
            if(ArtisanMachinesToSave != null && ArtisanMachinesToSave.Count > 0)
            {
                Butchery_ArtisanMachines.Save(ArtisanMachinesToSave);
            }

            if(ArtisanGoodsToSave != null && ArtisanGoodsToSave.Count > 0)
            {
                Butchery_ArtisanGoods.Save(ArtisanGoodsToSave);
            }
            
            
        }

        private void TestLoading()
        {

            var dyn = Butchery_ArtisanGoods.Load<List<ArtisanGoodsSaveModel>>();

            foreach (var d in dyn)
            {
                Console.WriteLine(d.Name);
            }

        }


        #region ArtisanMachines
        private void DeleteAllArtisanMachinesBeforeSleep()
        {
            var objToRemove = new SerializableDictionary<Vector2, StardewValley.Object>();
            if (ArtisanMachinesToSave != null && ArtisanMachinesToSave.Count > 0)
            {
                var locations = Game1.locations;
                foreach (var item in ArtisanMachinesToSave)
                {
                    foreach (var loc in locations)
                    {
                        foreach (var c in loc.objects.Where(c => c.Value is ButcherArtisanMachine))
                        {
                            try
                            {
                                objToRemove.Add(c.Key, c.Value);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                            }
                        }

                        foreach(var o in objToRemove)
                        {
                            loc.removeObject(o.Key, false);
                        }
                    }
                }
            }
        }

        private void UpdateArtisanMachinesToSave()
        {
            var locations = Game1.locations;

            foreach (var loc in locations)
            {
                foreach (var c in loc.objects.Values.Where(c => c is ButcherArtisanMachine))
                {
                    var machine = c as ButcherArtisanMachine;

                    var newMachine = new ArtisanMachinesSaveModel();
                    newMachine.Name = machine.Name;
                    newMachine.Category = machine.Category;
                    newMachine.Description = machine.getDescription();
                    newMachine.Position = new Vector2 { X = machine.boundingBox.X, Y = machine.boundingBox.Y };
                    newMachine.ParentSheetIndex = machine.ParentSheetIndex;
                    newMachine.Location = loc.Name;
                    newMachine.Price = machine.Price;
                    newMachine.MinutesUntilReady = machine.minutesUntilReady;
                    newMachine.CanBeSetDown = machine.canBeSetDown;
                    newMachine.SetOutdoors = machine.setOutdoors;
                    newMachine.SetIndoors = machine.setIndoors;
                    newMachine.BigCraftable = machine.bigCraftable;
                    newMachine.Fragility = machine.fragility;
                    newMachine.Edibility = machine.edibility;
                    newMachine.Type = machine.type;
                    newMachine.IsOn = machine.isOn;

                    if(!ArtisanMachinesToSave.Contains(newMachine))
                    {
                        ArtisanMachinesToSave.Add(newMachine);
                    }
                }
            }

            if (ArtisanMachinesToSave != null)
            {
                TestSaving();
            }
        }

        private void LoadArtisanMachines()
        {
            var dyn = Butchery_ArtisanMachines.Load<List<ArtisanMachinesSaveModel>>();
            var locations = Game1.locations;
            foreach (var d in dyn)
            {       

                foreach (var loc in locations)
                {
                    if(d.Location.Equals(loc.Name))
                    {
                        var newGoods = new ButcherArtisanMachine();
                        newGoods.Name = d.Name;
                        newGoods.ParentSheetIndex = d.ParentSheetIndex;
                        newGoods.Category = d.Category;
                        newGoods.description = d.Description;
                        newGoods.Price = d.Price;
                        newGoods.CanBeSetDown = d.CanBeSetDown;
                        newGoods.setOutdoors = d.SetOutdoors;
                        newGoods.setIndoors = d.SetIndoors;
                        newGoods.bigCraftable = d.BigCraftable;
                        newGoods.fragility = d.Fragility;
                        newGoods.Edibility = d.Edibility;
                        newGoods.Type = d.Type;
                        newGoods.minutesUntilReady = d.MinutesUntilReady;
                        newGoods.isOn = d.IsOn;

                        var pos = new Vector2 { X = d.Position.X / Game1.tileSize, Y = d.Position.Y / Game1.tileSize };
                        newGoods.tileLocation = pos;
                        //newGoods.placementAction(loc, (int)pos.X, (int)pos.Y, Game1.player);
                        
                        if(!loc.objects.ContainsKey(pos))
                        {
                            loc.objects.Add(pos, newGoods);
                        }
                    }
                }
            }

            foreach(var loc in locations)
            {
                foreach (var c in loc.objects.Values.Where(c => c is ButcherArtisanMachine))
                {
                    c.boundingBox.X = (int)c.tileLocation.X * Game1.tileSize;
                    c.boundingBox.Y = (int)c.tileLocation.Y * Game1.tileSize;
                    c.boundingBox.Width = Game1.tileSize;
                    c.boundingBox.Height = Game1.tileSize;
                }
            }
        }
        #endregion
        #region ArtisanGoods
        private void DeleteAllArtisanGoodsBeforeSleep()
        {
            if(ArtisanGoodsToSave != null && ArtisanGoodsToSave.Count > 0)
            {
                var locations = Game1.locations;
                foreach(var item in ArtisanGoodsToSave)
                {
                    // Chests
                    foreach(var loc in locations)
                    {
                        foreach(var c in loc.objects.Values.Where(c => c is Chest))
                        {
                            Chest chest = c as Chest;
                            chest.items.RemoveAll(i => i.GetType() == typeof(ButcherArtisanGoods));
                        }
                    }

                    //Player inventory
                    for(int i = 0; i < Game1.player.items.Count; i++)
                    {
                        if(Game1.player.items[i] != null)
                        {
                            if (Game1.player.items[i].GetType() == typeof(ButcherArtisanGoods))
                            {
                                Game1.player.items[i] = null;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateArtisanGoodsToSave()
        {
            var locations = Game1.locations;

            //Player inv
            for (int i = 0; i < Game1.player.items.Count; i++)
            {
                if(Game1.player.items[i] != null)
                {
                    if (Game1.player.items[i].GetType() == typeof(ButcherArtisanGoods))
                    {
                        var obj = Game1.player.items[i];
                        var newGoods = new ArtisanGoodsSaveModel
                        {
                            Name = obj.Name,
                            ParentSheetIndex = obj.parentSheetIndex,
                            Category = obj.category,
                            Description = obj.getDescription(),
                            Price = obj.salePrice(),
                            Location = "Char",
                            Position = Vector2.Zero
                        };

                        if (!ArtisanGoodsToSave.Contains(newGoods))
                        {
                            ArtisanGoodsToSave.Add(newGoods);
                        }
                    }
                }
            }

            //Chests
            foreach (var loc in locations)
            {
                foreach(var c in loc.objects.Values.Where(c => c is Chest))
                {
                    Chest chest = c as Chest;
                    var items = chest.items.Select(x => x).Where(x => x.GetType() == typeof(ButcherArtisanGoods));
                    if(items != null && items.Count() > 0)
                    {
                        foreach(var i in items)
                        {
                            var newGoods = new ArtisanGoodsSaveModel
                            {
                                Name = i.Name,
                                ParentSheetIndex = i.parentSheetIndex,
                                Category = i.category,
                                Description = i.getDescription(),
                                Price = i.salePrice(),
                                Location = loc.Name,
                                Position = new Vector2 { X = c.boundingBox.X, Y = c.boundingBox.Y }
                            };

                            if(ArtisanGoodsToSave == null)
                            {
                                ArtisanGoodsToSave = new List<ArtisanGoodsSaveModel>();
                            }

                            if(!ArtisanGoodsToSave.Contains(newGoods))
                            {
                                ArtisanGoodsToSave.Add(newGoods);
                            }                          
                        }
                    }
                }
            }

            if(ArtisanGoodsToSave != null)
            {
                TestSaving();
            }
        }

        private void LoadArtisanGoods()
        {
            var dyn = Butchery_ArtisanGoods.Load<List<ArtisanGoodsSaveModel>>();

            foreach (var d in dyn)
            {

                //Char
                if(d.Location.Equals("Char"))
                {
                    var newGoods = new ButcherArtisanGoods(d.Name);
                    newGoods.ParentSheetIndex = d.ParentSheetIndex;
                    newGoods.Category = d.Category;
                    newGoods.description = d.Description;
                    newGoods.Price = d.Price;

                    Game1.player.addItemToInventory(newGoods);
                }

                var locations = Game1.locations;

                //Chests
                foreach (var loc in locations)
                {
                    foreach (var c in loc.objects.Values.Where(c => c is Chest))
                    {
                        Chest chest = c as Chest;
                        if(chest.boundingBox.X == d.Position.X && chest.boundingBox.Y == d.Position.Y && loc.Name.Equals(d.Location))
                        {
                            var newGoods = new ButcherArtisanGoods(d.Name);
                            newGoods.ParentSheetIndex = d.ParentSheetIndex;
                            newGoods.Category = d.Category;
                            newGoods.description = d.Description;
                            newGoods.Price = d.Price;

                            chest.items.Add(newGoods);
                        }
                    }
                }
            }
        }
        #endregion
    }
}


/*
var recipe = new MonkeyRecipe("Beef Burger", true);
Game1.player.cookingRecipes.Add("Beef Burger", 0);
Console.WriteLine("break");
*/

/*

Spaghetti: "246 1 256 1/5 5/224/f Lewis 3"
Key: IngrId Quant IngrId Quant/???/ObjectID/How to obtain

 //Game1.objectInformation.Add(MUTTON, "Mutton/150/15/Basic -14/Meat from a Sheep. Tough.");
//var butcherKnife = new ButcherKnife();
//Game1.player.addItemToInventory(butcherKnife);
*/
