using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Enchanted,
    Frozen
}

public class PtrnElementalTile : Patron
{
    public ElementType spawnElement;
    public bool effectTriggered = false;
    public int initialTileIncrease;
    public int lvlUpTileIncrease;
    private BoardManager board;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        if (!effectTriggered)
        {
            Debug.Log(spawnElement + " Elemental Tile Spawn Effect Triggered");
            board = FindObjectOfType<BoardManager>();

            if (spawnElement == ElementType.Frozen)
            {
                if (level == 1)
                {
                    board.increaseMaxFrozenTiles(initialTileIncrease);
                }
                else
                {
                    board.increaseMaxFrozenTiles(lvlUpTileIncrease);
                }
            }

            if (spawnElement == ElementType.Enchanted)
            {
                if (level == 1)
                {
                    board.increaseMaxEnchantedTiles(initialTileIncrease);
                }
                else
                {
                    board.increaseMaxEnchantedTiles(lvlUpTileIncrease);
                }
            }

            effectTriggered = true;
        }
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;

            effectTriggered = false;
            triggerEffect();
        }
    }
}
