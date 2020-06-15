﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace BuilderEssentials.Items.Placeable
{
    class HardmodeCraftingStation : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used to craft Hardmode Recipes");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 150;
            item.createTile = TileType<Tiles.HardmodeCraftingStation>();
            item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("BuilderEssentials:HardmodeAnvils", 1);
            recipe.AddRecipeGroup("BuilderEssentials:Forge", 1);
            recipe.AddRecipeGroup("BuilderEssentials:Bookcase", 1);
            recipe.AddIngredient(ItemID.CrystalBall, 1);
            recipe.AddIngredient(ItemID.Autohammer, 1);
            recipe.AddIngredient(ItemID.LunarCraftingStation, 1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}