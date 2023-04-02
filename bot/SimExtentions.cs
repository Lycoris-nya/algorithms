using System.Runtime.InteropServices;

namespace bot
{
    public class SimExtentions
    {
        public static bool IsCastableSpell(Ingredients witchIngredients, Ingredients transformation)
        {
            return witchIngredients.Zero + transformation.Zero > -1
                   && witchIngredients.One + transformation.One > -1
                   && witchIngredients.Two + transformation.Two > -1
                   && witchIngredients.Three + transformation.Three > -1;
        }

        public static Ingredients CastSpell(Ingredients witchIngredients, Ingredients transformation)
        {
            return new Ingredients(
                witchIngredients.Zero + transformation.Zero,
                witchIngredients.One + transformation.One,
                witchIngredients.Two + transformation.Two,
                witchIngredients.Three + transformation.Three);
        }

        public static bool TryCastSpell(Ingredients witchIngredients, Ingredients transformation, out Ingredients res)
        {
            res = CastSpell(witchIngredients, transformation);
            return IsCastableSpell(witchIngredients, transformation);
        }

        public static int GetFullPrice(Recipe recipe)
        {
            if (recipe.BonusTimesCount > 0)
                return recipe.Price + recipe.Bonus;
            return recipe.Price;
        }

        public static bool CanLearn(Ingredients witchIngredients, LearnSpell learnSpell)
        {
            return witchIngredients.Zero >= learnSpell.Taxed * learnSpell.Index;
        }
    }
}