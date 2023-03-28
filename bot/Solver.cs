using System;
using System.Collections.Generic;
using System.Linq;

namespace bot
{
    public class Solver
    {
        public  BotCommand GetCommand(State state, Countdown countdown)
        {
            var bestScore = 0;
            Recipe recipe11 = null;
            Recipe recipe12 = null;
            
            foreach (var recipe1 in state.Recipes)
            {
                foreach (var recipe2 in state.Recipes)
                {
                    var flag = true;
                    for (int i = 0; i < recipe1.Ingredients.Count; i++)
                    {
                        if (state.Me.Ingredients[i] + recipe1.Ingredients[i] + recipe2.Ingredients[i] < 0)
                        {
                            flag = false;
                        }
                        
                    }

                    if (flag && bestScore < recipe1.Price + recipe2.Price)
                    {
                        bestScore = recipe1.Price + recipe2.Price;
                        recipe11 = recipe1;
                        recipe12 = recipe2;
                    }

                }
            }

            if (recipe11 != null)
            {
                if (recipe11.Price > recipe12.Price)
                {
                    return new BREW(recipe11.Id); //$"BREW {recipe11.Id}";
                }

                return new BREW(recipe12.Id); //$"BREW {recipe12.Id}";
            }

            foreach (var recipe in state.Recipes.OrderBy(x=> -x.Price))
            {
                var flag = true;
                for (int i = 0; i < recipe.Ingredients.Count; i++)
                {
                    if (state.Me.Ingredients[i] + recipe.Ingredients[i] <  0)
                    {
                        flag = false;
                    }
                        
                }

                if (flag)
                    return new BREW(recipe.Id); //$"BREW {recipe.Id}";
            }


            return GetCommandBfs(state, countdown);
        }

        public  bool IsEnable(List<int> now, List<int> wish)
        {
            for (int i = 0; i < now.Count; i++)
            {
                if (now[i] + wish[i] < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool TryCust(List<int> now, List<int> wish, out List<int> res)
        {
            res = new List<int>();
            for (int i = 0; i < now.Count; i++)
            {
                res.Add(now[i] + wish[i]);
            }

            return !res.Any(x => x < 0);
        }

        public static BotCommand GetCommandBfs(State state, Countdown countdown)
        {
            var rootAvailableCasts = new HashSet<int>();
            foreach (var huj in state.Me.Casts)
            {
                if (huj.Castable)
                    rootAvailableCasts.Add(huj.Id);
            }
            var root = new Node()
            {
                Depth = 0,
                AvailableCasts = rootAvailableCasts,
                Ingredients = state.Me.Ingredients
                
            };
            var queue = new Queue<Node>();
            queue.Enqueue(root);
            var hasSolution = false;
            var solutionDepth= 0;
            var solutions = new List<Node>();
            while (queue.Count > 0)
            {
                if (countdown.TimeAvailable.Milliseconds < 15)
                {
                    foreach (var cast in state.Me.Casts)
                    {
                        if (cast.Castable && TryCust(root.Ingredients, cast.Ingredients, out var res))
                        {
                            return new CAST(cast.Id); //$"CAST {cast.Id}";
                        }
                    }

                    return new REST(); //"REST";
                }
                var node = queue.Dequeue();
                if (hasSolution && node.Depth > solutionDepth)
                    continue;
                foreach (var recipe in state.Recipes)
                {
                    if (TryCust(node.Ingredients, recipe.Ingredients, out var res))
                    {
                        if (!hasSolution)
                        {
                            
                            hasSolution = true;
                            solutionDepth = node.Depth;
                        }
                        solutions.Add(node);
                    }

                }
                if (hasSolution || hasSolution && node.Depth > solutionDepth)
                    continue;
                foreach (var cast in state.Me.Casts)
                {
                    if (node.AvailableCasts.Contains(cast.Id) &&
                        TryCust(node.Ingredients, cast.Ingredients, out var res))
                    {
                        var availableCasts = new HashSet<int>();
                        foreach (var c in node.AvailableCasts)
                        {
                            availableCasts.Add(c);
                        }

                        availableCasts.Remove(cast.Id);
                        var newNode = new Node()
                        {
                            Depth = node.Depth + 1,
                            AvailableCasts = availableCasts,
                            Ingredients = res,
                            Parent = node,
                            Cast = cast
                        };
                        queue.Enqueue(newNode);
                    }

                }
                
                var resetCasts = new HashSet<int>();
                foreach (var c in state.Me.Casts)
                {
                    resetCasts.Add(c.Id);
                }
                
                var resetNode = new Node()
                {
                    Depth = node.Depth + 1,
                    AvailableCasts = resetCasts,
                    Ingredients = node.Ingredients,
                    Parent = node,
                    Cast = null
                };
                queue.Enqueue(resetNode);

            }

            var solution = solutions.First();
            var n = solution;
            while (true)
            {
                if (n.Depth == 1)
                {
                    return n.Cast == null ? new REST() : new CAST(n.Cast.Id); //$"CAST {n.Cast.Id}");
                }

                n = n.Parent;
            }
        }
    }

    public record Node
    {
        public Node Parent { get; set; }
        public List<int> Ingredients { get; set; }
        public HashSet<int> AvailableCasts {get; set;}
        public int Depth { get; set; }
        public Cast Cast { get; set; }
    }
}   