using BuilderEssentials.UI.ItemsUI.Wheels;
using BuilderEssentials.Utilities;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace BuilderEssentials.Items
{
    //TODO: Make the tool use the first paint color it finds in the inventory if red cross is selected
    class SuperPaintingTool : ModItem
    {
        public override string Texture => "BuilderEssentials/Textures/Items/SuperPaintingTool";
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Able to paint and remove paint from tiles and walls!" +
            "\nRight Click to open selection menu");
        }

        List<int> paints;
        bool foundModdedPaint;
        bool firstTimeOpeningUI = true;
        int toolRange;

        public override void SetDefaults()
        {
            paints = new List<int>();
            for (int i = 0; i < 27; i++) //Basic && Deep colors type
                paints.Add(1073 + i);
            for (int i = 0; i < 3; i++)
                paints.Add(1966 + i);   //Extra Color Effects type
            foundModdedPaint = false;

            item.height = 44;
            item.width = 44;
            item.useTime = 1;
            item.useAnimation = 1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = Item.buyPrice(0, 10, 0, 0);
            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
            toolRange = 8;
        }

        public override Vector2? HoldoutOffset() => new Vector2(5, -8);

        int mouseRightTimer = 0;
        public override void UpdateInventory(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (PaintWheel.PaintWheelPanel != null && !player.HeldItem.IsTheSameAs(item))
                {
                    PaintWheel.PaintWheelPanel.Remove();
                    PaintWheel.PaintingUIOpen = false;
                }

                if (Main.mouseRight && Tools.IsUIAvailable()
                        && (!player.mouseInterface || (PaintWheel.PaintingUIOpen && PaintWheel.PaintWheelPanel.IsMouseHovering))
                        && player.HeldItem.IsTheSameAs(item))
                {
                    if (++mouseRightTimer == 2)
                        PaintWheel.PaintingUIOpen = !PaintWheel.PaintingUIOpen;
                }

                if (Main.mouseRightRelease)
                    mouseRightTimer = 0;
            }
        }

        public override void HoldItem(Player player)
        {
            BuilderPlayer modPlayer = Main.LocalPlayer.GetModPlayer<BuilderPlayer>();
            Tile pointedTile = Main.tile[Player.tileTargetX, Player.tileTargetY];

            if (modPlayer.infiniteRange || Tools.ToolHasRange(toolRange) &&
                PaintWheel.selectedIndex != 30) //Color selected
            {
                player.showItemIcon = true;
                switch (PaintWheel.selectedToolIndex)
                {
                    case 0:
                        if (pointedTile.type >= 0 && pointedTile.active())
                            player.showItemIcon2 = ItemID.SpectrePaintbrush;
                        break;
                    case 1:
                        if (pointedTile.type >= 0 && pointedTile.wall > 0)
                            player.showItemIcon2 = ItemID.SpectrePaintRoller;
                        break;
                    case 2:
                        if (pointedTile.color() != 0)
                            player.showItemIcon2 = ItemID.SpectrePaintScraper;
                        break;
                }
            }
            else
            {
                if (PaintWheel.selectedIndex == 30 && PaintWheel.selectedToolIndex == 2)
                {
                    if (pointedTile.color() != 0)
                    {
                        player.showItemIcon = true;
                        player.showItemIcon2 = ItemID.SpectrePaintScraper;
                    }
                }

                player.showItemIcon = false;
            }
        }

        public override bool CanUseItem(Player player)
        {
            BuilderPlayer modPlayer = player.GetModPlayer<BuilderPlayer>();
            foundModdedPaint = false;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ItemType<InfinitePaintBucket>())
                {
                    foundModdedPaint = true;
                    break;
                }
            }

            int posX = Player.tileTargetX;
            int posY = Player.tileTargetY;
            Tile pointedTile = Main.tile[posX, posY];

            if ((modPlayer.infiniteRange || Tools.ToolHasRange(toolRange)) &&
                (PaintWheel.PaintWheelPanel != null && !PaintWheel.PaintWheelPanel.IsMouseHovering) || firstTimeOpeningUI)
            {
                if (firstTimeOpeningUI)
                    firstTimeOpeningUI = false;

                bool anyOperationDone = false;
                bool paintScraper = false;
                byte selectedColor = (byte)(PaintWheel.selectedIndex + 1);
                //selectedindex + 1 because paint bytes don't start at 0
                switch (PaintWheel.selectedToolIndex)
                {
                    case 0:
                        if (pointedTile.color() != selectedColor && selectedColor != 31)
                        {
                            if (CheckIfPaintIsInInventoryAndUseIt(selectedColor))
                            {
                                pointedTile.color(selectedColor);
                                anyOperationDone = true;
                            }
                        }
                        break;
                    case 1:
                        if (pointedTile.wallColor() != selectedColor && selectedColor != 31)
                        {
                            if (CheckIfPaintIsInInventoryAndUseIt(selectedColor))
                            {
                                pointedTile.wallColor(selectedColor);
                                anyOperationDone = true;
                            }
                        }
                        break;
                    case 2:
                        if (pointedTile.color() != 0)
                        {
                            pointedTile.color(0);
                            anyOperationDone = true;
                            paintScraper = true;
                        }
                        if (pointedTile.wallColor() != 0)
                        {
                            pointedTile.wallColor(0);
                            anyOperationDone = true;
                            paintScraper = true;
                        }
                        break;
                }

                if (anyOperationDone && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendTileSquare(-1, posX, posY, 1); //syncs painting tiles and walls
                    if (paintScraper)
                    {
                        //WorldGen.SquareTileFrame(posX, posY, true); //Not necessary to sync the scraper?
                        NetMessage.SendData(MessageID.PaintTile, -1, -1, null, posX, posY, 0, 0f, 0, 0, 0); //Syncs the Paint Scraper
                        NetMessage.SendData(MessageID.PaintWall, -1, -1, null, posX, posY, 0, 0f, 0, 0, 0); //Syncs the Paint Scraper
                    }
                }
            }
            return true;
        }

        private bool CheckIfPaintIsInInventoryAndUseIt(byte paintColor)
        {
            if (!foundModdedPaint)
            {
                List<Item> paintInInventory = new List<Item>();
                //Grabs all paint in the inventory to check if player is trying to use it
                foreach (Item item in Main.LocalPlayer.inventory)
                {
                    if (paints.Contains(item.type))
                        paintInInventory.Add(item);
                }

                foreach (Item item in paintInInventory)
                {
                    //Check if selected color byte (converted to int item.type) is present in the paintInInventory
                    if (PaintByteToItemType(paintColor) == item.type)
                    {
                        item.stack--;
                        return true;
                    }
                }

                //No result found and InfinitePaintBucket isn't in the inventory
                return false;
            }
            else //InfinitePaintBucket is in the inventory
                return true;

            int PaintByteToItemType(byte color)
            {
                if (color <= 27)
                    return color + 1072;
                else if (color >= 28 && color <= 30)
                    return color + 1938;

                return 31;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpectrePaintbrush);
            recipe.AddIngredient(ItemID.SpectrePaintRoller);
            recipe.AddIngredient(ItemID.SpectrePaintScraper);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}