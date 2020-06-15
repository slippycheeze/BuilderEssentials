﻿
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuilderEssentials.Items
{
    class InfiniteWrench : ModItem
    {
        //Made this item sort of obsolete with CreativeWrench/CreativeWheel?
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infinite Wrench (Disabled temporarily)");
            Tooltip.SetDefault("Allows infinite range and fast placement");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.vanity = false;
            item.width = 24;
            item.height = 24;
            item.value = Item.sellPrice(0, 10, 0, 0);
            item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.AddBuff(mod.BuffType("InfinitePlacementBuff"), 10);
            //player.blockRange += 55;
            //player.wallSpeed += 10;
            //player.tileSpeed += 50;
            //Player.tileRangeX = 65;
            //Player.tileRangeY = 55;
        }

        public override void AddRecipes()
        {
            //Not really worried about balancing at this point


            //ModRecipe modRecipe = new ModRecipe(mod);
            //modRecipe.AddIngredient(mod.GetItem("InfinityUpgrade"), 1);
            //modRecipe.AddIngredient(mod.GetItem("PlacementWrench"), 1);
            //modRecipe.AddTile(TileID.TinkerersWorkbench);
            //modRecipe.SetResult(this);
            //modRecipe.AddRecipe();
        }
    }
}
