﻿using System;
using System.Collections.Generic;

namespace bot
{
    public static class StateReader
    {
        public static State ReadState(this ConsoleReader reader)
        {
            var init = reader.ReadInit();
            return reader.ReadState(init);
        }
        
        // ReSharper disable once InconsistentNaming
        public static State ReadState(this ConsoleReader Console, StateInit init)
        {
            string[] inputs;
            int actionCount = int.Parse(Console.ReadLine()); // the number of spells and recipes in play
            var  recipes = new List<Recipe>();
            var  myCasts = new List<Cast>();
            var  opponentCasts = new List<Cast>();
            var  learnSpells = new List<LearnSpell>();
            for (int i = 0; i < actionCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int actionId = int.Parse(inputs[0]); // the unique ID of this spell or recipe
                string actionType = inputs[1]; // in the first league: BREW; later: CAST, OPPONENT_CAST, LEARN, BREW
                int delta0 = int.Parse(inputs[2]); // tier-0 ingredient change
                int delta1 = int.Parse(inputs[3]); // tier-1 ingredient change
                int delta2 = int.Parse(inputs[4]); // tier-2 ingredient change
                int delta3 = int.Parse(inputs[5]); // tier-3 ingredient change
                int price = int.Parse(inputs[6]); // the price in rupees if this is a potion
                int tomeIndex = int.Parse(inputs[7]); // in the first two leagues: always 0; later: the index in the tome if this is a tome spell, equal to the read-ahead tax; For brews, this is the value of the current urgency bonus
                int taxCount = int.Parse(inputs[8]); // in the first two leagues: always 0; later: the amount of taxed tier-0 ingredients you gain from learning this spell; For brews, this is how many times you can still gain an urgency bonus
                bool castable = inputs[9] != "0"; // in the first league: always 0; later: 1 if this is a castable player spell
                bool repeatable = inputs[10] != "0"; // for the first two leagues: always 0; later: 1 if this is a repeatable player spell
                var ingredients = new Ingredients(delta0, delta1, delta2, delta3);
                if (actionType == "BREW")
                {
                    var recipe = new Recipe(actionId, price, ingredients, tomeIndex, taxCount);
                    recipes.Add(recipe);
                }
                else if (actionType == "CAST")
                {
                    var cast = new Cast(actionId, castable, ingredients,  repeatable);
                    myCasts.Add(cast);
                }
                else if (actionType == "LEARN")
                {
                    var learnSpell = new LearnSpell(actionId, ingredients, repeatable, tomeIndex, taxCount);
                    learnSpells.Add(learnSpell);
                }
                else
                {
                    var cast = new Cast(actionId, castable, ingredients, repeatable);
                    opponentCasts.Add(cast);
                }
            }

            var witchStates = new WitchState[2];
            for (int i = 0; i < 2; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int inv0 = int.Parse(inputs[0]); // tier-0 ingredients in inventory
                int inv1 = int.Parse(inputs[1]);
                int inv2 = int.Parse(inputs[2]);
                int inv3 = int.Parse(inputs[3]);
                int score = int.Parse(inputs[4]); // amount of rupees
                var ingredients = new Ingredients(inv0, inv1, inv2, inv3);
                var witchState = new WitchState(score, ingredients);
                witchStates[i] = witchState;
            }

            witchStates[0].Casts = myCasts;
            witchStates[1].Casts = opponentCasts;
            return new State(witchStates[0], witchStates[1], recipes, learnSpells);
        }

        // ReSharper disable once InconsistentNaming
        public static StateInit ReadInit(this ConsoleReader Console)
        {
            // Copy paste here the code for initialization input data (or delete if no initialization data in this game)
            return new StateInit();
        }
    }
}