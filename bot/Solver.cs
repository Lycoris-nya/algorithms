using System.Collections.Generic;
using System.Linq;

namespace bot
{
    public class Solver
    {
        public BotCommand GetCommand(State state, Countdown countdown)
        {
            foreach (var recipe in state.Recipes)
            {
                if (SimExtentions.IsCastableSpell(state.Me.Ingredients, recipe.Ingredients))
                    return new BREW(recipe.Id);
            }

            return GetCommandBfs(state, countdown);
        }

        public static BotCommand GetCommandBfs(State state, Countdown countdown)
        {
            var learnSpells = new Dictionary<int, LearnSpell>();
            foreach (var learnSpell in state.LearnSpells)
                learnSpells[learnSpell.Index] = learnSpell;
            if (state.Me.Casts.Count < 10)
                return new LEARN(learnSpells[0].Id);
            var rootAvailableCasts = new HashSet<int>();
            foreach (var cast in state.Me.Casts)
            {
                if (cast.Castable)
                    rootAvailableCasts.Add(cast.Id);
            }
            var root = new Node()
            {
                Depth = 0,
                AvailableCasts = rootAvailableCasts,
                TotalCastsCount = state.Me.Casts.Count,
                Ingredients = state.Me.Ingredients,
                LearnSpell = learnSpells[0],
            };
            var queue = new Queue<Node>();
            queue.Enqueue(root);
            var hasSolution = false;
            var solutions = new List<Node>();
            while (queue.Count > 0)
            {
                if (countdown.TimeAvailable.Milliseconds < 15)
                {
                    if (rootAvailableCasts.Count < root.TotalCastsCount - 2)
                        return new REST();
                    return new LEARN(learnSpells[0].Id);
                }

                var node = queue.Dequeue();
                foreach (var recipe in state.Recipes)
                {
                    if (SimExtentions.IsCastableSpell(node.Ingredients, recipe.Ingredients))
                    {
                        if (!hasSolution)
                            hasSolution = true;
                        solutions.Add(node);
                    }
                }
                if (hasSolution)
                    break;
                AddAvailableCasts(state, node, queue);

                if (node.AvailableCasts.Count < node.TotalCastsCount)
                {
                    AddRest(state, node, queue);
                }
            }

            return RestorePathBfs(solutions.First());
        }

        private static BotCommand RestorePathBfs(Node node)
        {
            while (true)
            {
                if (node.Depth == 1)
                {
                    return node.Command;
                }

                node = node.Parent;
            }
        }

        private static void AddAvailableCasts(State state, Node node, Queue<Node> queue)
        {
            foreach (var cast in state.Me.Casts)
            {
                if (node.AvailableCasts.Contains(cast.Id) &&
                    SimExtentions.TryCastSpell(node.Ingredients, cast.Ingredients, out var res))
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
                        TotalCastsCount = node.TotalCastsCount,
                        LearnSpell = node.LearnSpell,
                        Command = new CAST(cast.Id)
                    };
                    queue.Enqueue(newNode);
                }
            }
        }

        private static void AddRest(State state, Node node, Queue<Node> queue)
        {
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
                TotalCastsCount = node.TotalCastsCount,
                LearnSpell = node.LearnSpell,
                Command = new REST()
            };
            queue.Enqueue(resetNode);
        }
    }

    public record Node
    {
        public Node Parent { get; set; }
        public Ingredients Ingredients { get; set; }
        public HashSet<int> AvailableCasts { get; set; }
        public int TotalCastsCount { get; set; }
        public int Depth { get; set; }
        public LearnSpell LearnSpell { get; set; }
        public BotCommand Command { get; set; }
    }
}