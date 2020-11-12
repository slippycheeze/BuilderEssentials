﻿using BuilderEssentials.UI.UIPanels;
using BuilderEssentials.UI.UIStates;
using BuilderEssentials.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuilderEssentials.Items
{
    public class FillWand : ModItem
    {
        public override string Texture => "BuilderEssentials/Textures/Items/FillWand";

        public static int selectedTileItemType = -1;
        public static int fillSelectionSize = 3;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault
            (
                "Fills Holes" +
                "\nLeft Click to place" +
                "\nRigth Click to remove" +
                "\nMiddle Click to select working tile" +
                "\n[c/FFCC00:Use hotkeys to increase/decrease selection size]"
            );
        }

        public override void SetDefaults()
        {
            item.height = 46;
            item.width = 46;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = Item.buyPrice(0, 10, 0, 0);
            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
            item.noMelee = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-2, -7);

        private int mouseRightTimer = 0;

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer) return;

            if (!ItemsUIState.fillWandSelection.Visible)
                ItemsUIState.fillWandSelection.Show();


            BEPlayer mp = player.GetModPlayer<BEPlayer>();
            player.showItemIcon = true;

            //Middle Mouse
            if (Main.mouseMiddle && HelperMethods.IsUIAvailable(notShowingMouseIcon: false))
                selectedTileItemType = HelperMethods.PickItem(mp.PointedTile, false);

            if (selectedTileItemType != -1)
                player.showItemIcon2 = selectedTileItemType;

            //Right Mouse
            if (Main.mouseRight && player.HeldItem == item && selectedTileItemType != -1 &&
                HelperMethods.IsUIAvailable(notShowingMouseIcon: false) && ++mouseRightTimer == 9)
            {
                Item customItem = new Item();
                for (int i = 0; i < fillSelectionSize; i++)
                {
                    for (int j = 0; j < fillSelectionSize; j++)
                    {
                        int posX = Player.tileTargetX + j;
                        int posY = Player.tileTargetY - i;
                        Tile tile = Framing.GetTileSafely(posX, posY);
                        customItem.SetDefaults(selectedTileItemType);

                        if (tile.type == customItem.createTile)
                            HelperMethods.RemoveTile(posX, posY, true, false);
                        else if (tile.wall == customItem.createWall)
                            HelperMethods.RemoveTile(posX, posY, false, true);
                    }
                }
            }

            if (Main.mouseRightRelease || mouseRightTimer == 9)
                mouseRightTimer = 0;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer) return false;

            for (int i = 0; i < fillSelectionSize; i++)
            {
                for (int j = 0; j < fillSelectionSize; j++)
                {
                    int posX = Player.tileTargetX + j;
                    int posY = Player.tileTargetY - i;
                    Tile tile = Framing.GetTileSafely(posX, posY);
                    
                    HelperMethods.ItemTypes itemTypes = HelperMethods.WhatIsThisItem(selectedTileItemType);
                    if (selectedTileItemType != -1 &&
                        ((itemTypes == HelperMethods.ItemTypes.Tile && tile.type == 0) ||
                         (itemTypes == HelperMethods.ItemTypes.Wall && tile.wall == 0)) &&
                        HelperMethods.CanReduceItemStack(selectedTileItemType, true))
                        HelperMethods.PlaceTile(posX, posY, selectedTileItemType);
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MoltenHamaxe);
            recipe.AddIngredient(ItemID.DirtRod);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}