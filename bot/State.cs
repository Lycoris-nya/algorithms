using System.Collections.Generic;

namespace bot
{
    public class StateInit
    {
    }

    public class State
    {
        public readonly List<Recipe> Recipes;
        public readonly WitchState Me;
        public WitchState Opponent;
        public readonly List<LearnSpell> LearnSpells;

        public State(WitchState me, WitchState opponent, List<Recipe> recipes, List<LearnSpell> learnSpells)
        {
            Me = me;
            Opponent = opponent;
            Recipes = recipes;
            LearnSpells = learnSpells;
        }
    }

    public class Recipe
    {
        public readonly int Price;
        public readonly Ingredients Ingredients;
        public readonly int Id;
        public readonly int Bonus;
        public readonly int BonusTimesCount;

        public Recipe(int id, int price, Ingredients ingredients, int bonus, int bonusTimesCount)
        {
            Price = price;
            Ingredients = ingredients;
            Id = id;
            Bonus = bonus;
            BonusTimesCount = bonusTimesCount;
        }
    }

    public class Cast
    {
        public readonly bool Castable;
        public readonly Ingredients Ingredients;
        public readonly int Id;
        public readonly bool Repeatable;

        public Cast(int id, bool castable, Ingredients ingredients, bool repeatable)
        {
            Castable = castable;
            Ingredients = ingredients;
            Id = id;
            Repeatable = repeatable;
        }
    }

    public class LearnSpell
    {
        public readonly int Id;
        public readonly Ingredients Ingredients;
        public readonly bool Repeatable;
        public readonly int Index;
        public readonly int Taxed;

        public LearnSpell(int id, Ingredients ingredients, bool repeatable, int index, int taxed)
        {
            Id = id;
            Ingredients = ingredients;
            Repeatable = repeatable;
            Index = index;
            Taxed = taxed;
        }
    }

    public class WitchState
    {
        public int Rupees;
        public readonly Ingredients Ingredients;
        public List<Cast> Casts;

        public WitchState(int rupees, Ingredients ingredients)
        {
            Rupees = rupees;
            Ingredients = ingredients;
        }
    }

    public struct Ingredients
    {
        public readonly int Zero;
        public readonly int One;
        public readonly int Two;
        public readonly int Three;

        public Ingredients(int zero, int one, int two, int three)
        {
            Zero = zero;
            One = one;
            Two = two;
            Three = three;
        }
    }
}