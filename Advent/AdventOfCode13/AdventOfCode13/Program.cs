using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode13
{
    using State = Tuple<int,int,int>;
    using Point = Tuple<int, int>;
    class Program
    {
        static Point InitialState = new Point(1, 1);
        static State EndState = new State(31, 39, int.MaxValue);
        static readonly int MagicNumber = 1350;

        public class DictionaryQueue<T>
        {
            Dictionary<int, Dictionary<T, bool>> byDistance = new Dictionary<int, Dictionary<T, bool>>(); // key: distance, value: dictionary<state, distance>

            public bool HasItems
            {
                get
                {
                    foreach (var dict in byDistance)
                    {
                        if (dict.Value.Count > 0)
                            return true;
                    }
                    return false;
                }
            }

            public void Add(T state, int distance)
            {
                if (!byDistance.ContainsKey(distance))
                {
                    byDistance.Add(distance, new Dictionary<T, bool>());
                }
                var distDict = byDistance[distance];
                distDict.Add(state, true);
            }

            public int Contains(T state)
            {
                foreach (var distKey in byDistance.Keys)
                {
                    if (byDistance[distKey].ContainsKey(state))
                        return distKey;
                }
                return -1;
            }

            public T Dequeue(out int distance)
            {
                // get one from the smallest queue
                var orderedKeys = byDistance.Keys.OrderBy(x => x);
                foreach (var key in orderedKeys)
                {
                    if (byDistance[key].Count > 0)
                    {
                        var first = byDistance[key].First();
                        T result = first.Key;
                        byDistance[key].Remove(first.Key);
                        distance = key;
                        return result;
                    }
                }
                throw new Exception("Can't dequeue when empty");
            }

            public void UpdateDistance(T s, int newDistance)
            {
                foreach (var key in byDistance.Keys)
                {
                    var distDict = byDistance[key];
                    if (distDict.ContainsKey(s))
                    {
                        if (key != newDistance)
                        {
                            var val = distDict[s];
                            distDict.Remove(s);

                            if (!byDistance.ContainsKey(newDistance))
                                byDistance.Add(newDistance, new Dictionary<T, bool>());

                            byDistance[newDistance].Add(s, val);
                            return;
                        }
                    }
                }
                throw new Exception("Can't update state distance, state not found");
            }
        }

        static void Main(string[] args)
        {
            Dictionary<Point, int> knownStates = new Dictionary<Point, int>();
            DictionaryQueue<Point> dq = new DictionaryQueue<Tuple<int, int>>();

            dq.Add(InitialState, 0);

            while (dq.HasItems)
            {
                int currentDist;
                var current = dq.Dequeue(out currentDist);

                // part 1 solution
                if (current.Item1 == EndState.Item1 && current.Item2 == EndState.Item2)
                {
                    //Console.WriteLine($"Found solution. Distance {currentDist}");
                    //break;
                }

                // part 2 solution
                if (currentDist > 50)
                {
                    Console.WriteLine("Known states under 50: " + knownStates.Count);
                    break;
                }

                knownStates.Add(current, currentDist);
                var neighborStates = GetNeighborStates(current.Item1, current.Item2);

                foreach (var neighbor in neighborStates)
                {
                    if (!knownStates.ContainsKey(neighbor))
                    {
                        int dist = dq.Contains(neighbor);
                        if (dist == -1)
                        {
                            dq.Add(neighbor, currentDist + 1);
                        }
                        else if (dist > currentDist + 1)
                        {
                            dq.UpdateDistance(neighbor, currentDist + 1);
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        public static bool IsOpenSpace(int x, int y)
        {
            if (x < 0 || y < 0)
                return false;

            var num = MagicNumber + (x * x) + (3 * x) + (2 * x * y) + y + (y * y);
            int bitCount = 0;
            int mask = 1;
            for (int i = 0; i < 32; i++)
            {
                if ((num & mask) == 1)
                    bitCount++;
                num = num >> 1;
            }
            return bitCount % 2 == 0;
        }

        static List<Tuple<int,int>> GetNeighborStates(int x, int y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentException();

            var tryPoints = new List<Point>();
            tryPoints.Add(Tuple.Create(x - 1, y));
            tryPoints.Add(Tuple.Create(x + 1, y));
            tryPoints.Add(Tuple.Create(x, y - 1));
            tryPoints.Add(Tuple.Create(x, y + 1));

            var neighborStates = new List<Tuple<int,int>>();
            foreach (var point in tryPoints)
            {
                if (IsOpenSpace(point.Item1, point.Item2))
                    neighborStates.Add(point);
            }
            return neighborStates;
        }
    }
}
