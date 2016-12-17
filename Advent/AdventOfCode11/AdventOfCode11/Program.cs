using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AdventOfCode11
{
    using State = List<LabObject>;
    using Route = List<List<LabObject>>;

    // Thulium, Plutonium, Strontium, Promethium, Ruthenium
    public enum Element
    {
        PM,
        PU,
        RU,
        SR,
        TM,
        EL,
        LI
    }

    public enum ObjType
    {
        M,
        G,
        E
    }

    public class LabObject : IEquatable<LabObject>, ICloneable
    {
        public Element Isotope { get; set; }
        public ObjType ObjectType { get; set; }
        public int Floor { get; set; }

        Boolean IEquatable<LabObject>.Equals(LabObject other)
        {
            if (other == null)
                return false;

            return (Isotope == other.Isotope && ObjectType == other.ObjectType && Floor == other.Floor);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is LabObject)
                return ((IEquatable<LabObject>) this).Equals((LabObject)obj);
            return false;
        }

        public override Int32 GetHashCode()
        {
            return ObjectType.ToString().GetHashCode() * Isotope.ToString().GetHashCode() * Floor.ToString().GetHashCode();
        }

        public override String ToString()
        {
            if (this.ObjectType != ObjType.E)
                return $"{ObjectType}{Isotope}";
            else
                return "E";
        }

        public Object Clone()
        {
            return new LabObject() { Isotope = this.Isotope, ObjectType = this.ObjectType, Floor = this.Floor };
        }

        public static bool operator ==(LabObject a, LabObject b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LabObject a, LabObject b)
        {
            return !a.Equals(b);
        }
    }

    public static class Extensions
    {
        public static State Clone(this State donor)
        {
            if (donor == null)
                throw new ArgumentNullException();

            State newState = new State(donor.Count);
            donor.ForEach(x => newState.Add((LabObject)x.Clone()));
            return newState;
        }

        public static Route Clone(this Route donor)
        {
            if (donor == null)
                throw new ArgumentNullException();

            Route newRoute = new Route(donor.Count);
            donor.ForEach(x => newRoute.Add(x.Clone()));
            return newRoute;
        }

        public static bool ContainsState(this List<State> haystack, State needle)
        {
            // ignore elevator
            return haystack.Any(x => Program.AreStatesEquivalent(x, needle));
        }

        public static bool ContainsRoute(this List<Route> haystack, Route needle)
        {
            foreach (var route in haystack)
            {
                if (Program.AreRoutesEquivalent(route, needle))
                    return true;
            }
            return false;
        }

        public static int ToNumeric(this State state)
        {
            int result = 0;
            // binary representation, 2 bits (floors 0-3) for each object including elevator
            int shift = 0;
            foreach (var item in state)
            {
                var floor = item.Floor - 1;
                floor = floor << shift;
                //Console.WriteLine($"Floor: {floor:X}");
                Debug.Assert((result + floor) == (result ^ floor));
                result = result + floor;
                //Console.WriteLine($"Result: {result:X}");
                shift += 2;
            }
            return result;
        }
    }

    public class DictionaryQueue
    {
        Dictionary<int, Dictionary<int, StateNode>> byDistance = new Dictionary<int, Dictionary<int, StateNode>>(); // key: distance, value: dictionary<state, distance>

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

        public void Add(StateNode state)
        {
            int distance = state.distance;
            int stateCode = state.state.ToNumeric();

            if (!byDistance.ContainsKey(distance))
            {
                byDistance.Add(distance, new Dictionary<int, StateNode>());
            }
            var distDict = byDistance[distance];
            distDict.Add(stateCode, state);
        }

        public StateNode Contains(int stateCode)
        {
            foreach (var distDict in byDistance)
            {
                if (distDict.Value.ContainsKey(stateCode))
                    return distDict.Value[stateCode];
            }
            return null;
        }

        public bool Contains(StateNode s)
        {
            var distance = s.distance;
            if (!byDistance.ContainsKey(distance))
                return false;

            return byDistance[distance].ContainsKey(s.state.ToNumeric());
        }

        public StateNode Dequeue()
        {
            // get one from the smallest queue
            var orderedKeys = byDistance.Keys.OrderBy(x => x);
            foreach (var key in orderedKeys)
            {
                if (byDistance[key].Count > 0)
                {
                    KeyValuePair<int, StateNode> first = byDistance[key].First();
                    StateNode state = first.Value;
                    byDistance[key].Remove(first.Key);
                    return state;
                }
            }
            throw new Exception("Can't dequeue when empty");
        }

        public void UpdateDistance(StateNode s, int newDistance)
        {
            if (Contains(s))
            {
                var stateCode = s.state.ToNumeric();
                var stateNode = byDistance[s.distance][stateCode];
                
                if (stateNode.distance != newDistance)
                {
                    int oldDistance = stateNode.distance;
                    stateNode.distance = newDistance;
                    byDistance[oldDistance].Remove(stateCode);

                    if (!byDistance.ContainsKey(newDistance))
                        byDistance.Add(newDistance, new Dictionary<int, StateNode>());

                    byDistance[newDistance].Add(stateCode, stateNode);
                }
            }
            else
                throw new Exception("Can't update state distance, state not found");
        }
    }

    public class PriorityQueue<T> where T: IComparable <T>
    {
        public List<T> data;
        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public bool HasItems
        {
            get
            {
                return data.Count > 0;
            }
        }

        public void Enqueue(T item)
        {
            data.Add(item);
            int i = data.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (data[i].CompareTo(data[parent]) >= 0)
                    break;

                T tmp = data[i];
                data[i] = data[parent];
                data[parent] = tmp;
                i = parent;
            }
        }

        public T Dequeue()
        {
            if (data.Count == 0)
                throw new Exception();

            int lastIndex = data.Count - 1;
            T front = data[0];
            data[0] = data[lastIndex];
            data.RemoveAt(lastIndex);

            lastIndex -= 1;
            int parentIndex = 0;

            while (true)
            {
                int childIndex = parentIndex * 2 + 1;
                if (childIndex > lastIndex)
                    break;
                int rightChild = childIndex + 1;

                if (rightChild <= lastIndex && data[rightChild].CompareTo(data[childIndex]) < 0)
                    childIndex = rightChild;

                if (data[parentIndex].CompareTo(data[childIndex]) <= 0)
                    break;

                T tmp = data[parentIndex];
                data[parentIndex] = data[childIndex];
                data[childIndex] = tmp;

                parentIndex = childIndex;
            }

            return front;
        }

        public void MinHeapify(int index)
        {
            int smallest;
            int l = 2 * (index + 1) - 1;
            int r = 2 * (index + 1) - 1 + 1;

            if (l < data.Count && (data[l].CompareTo(data[index]) < 0))
            {
                smallest = l;
            }
            else
            {
                smallest = index;
            }

            if (r < data.Count && (data[r].CompareTo(data[smallest]) < 0))
                smallest = r;

            if (smallest != index)
            {
                T tmp = data[index];
                data[index] = data[smallest];
                data[smallest] = tmp;
                MinHeapify(smallest);

            }
        }
    }

    public class StateNode : IComparable <StateNode>
    {
        public State state { get; set; }
        public int distance { get; set; }

        Int32 IComparable<StateNode>.CompareTo(StateNode other)
        {
            return distance.CompareTo(other.distance);
        }
    }

    class Program
    {
        public static bool AreStatesEquivalent(State a, State b)
        {
            // don't consider the elevator
            return a.Where(x => x.ObjectType != ObjType.E).SequenceEqual(b.Where(y => y.ObjectType != ObjType.E));
        }

        public static bool AreRoutesEquivalent(Route a, Route b)
        {
            if ((a != null && b == null) || (a == null && b != null))
                return false;

            if (a.Count() != b.Count())
                return false;

            for (int i=0;i<a.Count();i++)
            {
                if (!AreStatesEquivalent(a[i], b[i]))
                    return false;
            }
            return true;
        }
        public static bool IsValidState(State state)
        {
            var conflicts = state.Count(x => x.ObjectType == ObjType.G && state.Count(y => y.ObjectType == ObjType.M && y.Floor == x.Floor && state.Count(z => z.ObjectType == ObjType.G && z.Floor == y.Floor && z.Isotope == y.Isotope) == 0) > 0);
            return conflicts == 0;
        }

        public static void PrintState(State state)
        {
            var sb = new StringBuilder();

            for (int i=4;i>=1;i--)
            {
                if (state.First(x => x.ObjectType == ObjType.E).Floor == i)
                    sb.Append("*|");
                else
                    sb.Append(" |");

                state.Where(x => x.Floor == i && x.ObjectType != ObjType.E).ToList().ForEach(x => sb.Append(x.ToString() + " "));
                sb.AppendLine();
            }

            Console.WriteLine(sb);
        }

        public static State EndState = new State()
        {
            new LabObject() { ObjectType = ObjType.E, Floor = 4 },
            new LabObject() { Isotope = Element.TM, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.TM, ObjectType = ObjType.M, Floor = 4 },
            new LabObject() { Isotope = Element.PU, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.SR, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.EL, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.EL, ObjectType = ObjType.M, Floor = 4 },
            new LabObject() { Isotope = Element.LI, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.LI, ObjectType = ObjType.M, Floor = 4 },

            new LabObject() { Isotope = Element.PU, ObjectType = ObjType.M, Floor = 4 },
            new LabObject() { Isotope = Element.SR, ObjectType = ObjType.M, Floor = 4 },

            new LabObject() { Isotope = Element.PM, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.PM, ObjectType = ObjType.M, Floor = 4 },
            new LabObject() { Isotope = Element.RU, ObjectType = ObjType.G, Floor = 4 },
            new LabObject() { Isotope = Element.RU, ObjectType = ObjType.M, Floor = 4 }

        };

        public static State InitialState = new State()
        {
            new LabObject() { ObjectType = ObjType.E, Floor = 1 },
            new LabObject() { Isotope = Element.TM, ObjectType = ObjType.G, Floor = 1 },
            new LabObject() { Isotope = Element.TM, ObjectType = ObjType.M, Floor = 1 },
            new LabObject() { Isotope = Element.PU, ObjectType = ObjType.G, Floor = 1 },
            new LabObject() { Isotope = Element.SR, ObjectType = ObjType.G, Floor = 1 },
            new LabObject() { Isotope = Element.EL, ObjectType = ObjType.G, Floor = 1 },
            new LabObject() { Isotope = Element.EL, ObjectType = ObjType.M, Floor = 1 },
            new LabObject() { Isotope = Element.LI, ObjectType = ObjType.G, Floor = 1 },
            new LabObject() { Isotope = Element.LI, ObjectType = ObjType.M, Floor = 1 },

            new LabObject() { Isotope = Element.PU, ObjectType = ObjType.M, Floor = 2 },
            new LabObject() { Isotope = Element.SR, ObjectType = ObjType.M, Floor = 2 },

            new LabObject() { Isotope = Element.PM, ObjectType = ObjType.G, Floor = 3 },
            new LabObject() { Isotope = Element.PM, ObjectType = ObjType.M, Floor = 3 },
            new LabObject() { Isotope = Element.RU, ObjectType = ObjType.G, Floor = 3 },
            new LabObject() { Isotope = Element.RU, ObjectType = ObjType.M, Floor = 3 }
        };

        static List<State> GetMovesFromState(State startState)
        {
            var validMoves = new List<State>();
            LabObject elevator = startState.First(x => x.ObjectType == ObjType.E);
            Debug.Assert(elevator != null);

            // only pieces on the same floor as the elevator are eligible to move
            foreach (var piece in startState.Where(x => x.ObjectType != ObjType.E && x.Floor == elevator.Floor))
            {
                var validNeighborFloors = new List<int>();
                if (piece.Floor == 1)
                {
                    validNeighborFloors.Add(2);
                }
                else if (piece.Floor == 4)
                {
                    validNeighborFloors.Add(3);
                }
                else if (piece.Floor == 2 || piece.Floor == 3)
                {
                    validNeighborFloors.Add(piece.Floor - 1);
                    validNeighborFloors.Add(piece.Floor + 1);
                }
                else
                {
                    throw new Exception("Unexpected floor: " + piece.Floor);
                }

                // get valid states for moving each piece to neighboring floor
                foreach (var floor in validNeighborFloors)
                {
                    var newState = startState.Clone();
                    newState.First(x => x == piece).Floor = floor;
                    newState.First(x => x.ObjectType == ObjType.E).Floor = floor;

                    if (!IsValidState(newState) || validMoves.ContainsState(newState))
                        continue;
                    else
                        validMoves.Add(newState);
                }

                // get valid states for moving this piece and a friend to neighboring floor
                var buddies = startState.Where(x => x.Floor == piece.Floor && x != piece && x.ObjectType != ObjType.E && !(piece.ObjectType != x.ObjectType && piece.Isotope != x.Isotope));
                foreach (var floor in validNeighborFloors)
                {
                    foreach (var buddy in buddies)
                    {
                        var newState = startState.Clone();
                        newState.First(x => x == piece).Floor = floor;
                        newState.First(x => x == buddy).Floor = floor;
                        newState.First(x => x.ObjectType == ObjType.E).Floor = floor;

                        if (!IsValidState(newState) || validMoves.ContainsState(newState))
                            continue;
                        else
                            validMoves.Add(newState);
                    }
                }
            }
            Debug.Assert(validMoves.Distinct().Count() == validMoves.Count());
            return validMoves;
        }

        public static List<Route> Solutions = new List<Route>();
        public static List<State> DeadEnds = new List<State>();
        public static int MinSolution = 0;
        public static List<State> UniqueStates = new List<State>();

        static void Main(string[] args)
        {
            Debug.Assert(IsValidState(InitialState));

            //PrintState(InitialState);

            Route initialRoute = new Route();
            initialRoute.Add(InitialState);

            //FindStates();

            //CheckRoute(initialRoute);

            ShortestPath();

            if (Solutions.Count > 0)
            {
                Console.WriteLine($"Solutions: {Solutions.Count()}, Lengths: {string.Join(",", Solutions.Select(x => x.Count() - 1))}");
                var quickest = Solutions.OrderBy(x => x.Count()).First();
                {
                    Console.WriteLine($"Solution {quickest.Count() - 1} steps:");
                    //quickest.ForEach(x => PrintState(x));
                    //Console.ReadLine();
                }
            }

            Console.ReadLine();
        }

        static void ShortestPath()
        {
            var startState = new StateNode { state = InitialState, distance = 0 };
            var dq = new DictionaryQueue();
            dq.Add(startState);

            var knownStates = new Dictionary<Int32, State>();

            var start = DateTime.Now;

            while (dq.HasItems)
            {
                var current = dq.Dequeue();

                if (current.state.SequenceEqual(EndState))
                {
                    Console.WriteLine($"Found solution in {current.distance} steps");
                    Console.WriteLine($"Time to solve: {DateTime.Now - start}");
                    break;
                }

                knownStates.Add(current.state.ToNumeric(), current.state);
                
                var neighborStates = GetMovesFromState(current.state);

                foreach (var neighbor in neighborStates)
                {
                    if (!knownStates.ContainsKey(neighbor.ToNumeric()))
                    {
                        //int foundIndex = pq.data.FindIndex(x => x.state.SequenceEqual(neighbor));
                        //var foundState = foundIndex >= 0 ? pq.data[foundIndex] : null;
                        var foundState = dq.Contains(neighbor.ToNumeric());

                        if (null == foundState)
                        {
                            dq.Add(new StateNode() { state = neighbor, distance = current.distance + 1 });
                        }
                        else if (foundState.distance > current.distance + 1)
                        {
                            Console.WriteLine("Found state with distance higher than just discovered");
                            dq.UpdateDistance(foundState, current.distance + 1);
                        }
                    }
                }
            }
            //Console.WriteLine($"States discovered: {knownStates.Count}");
        }

        static void FindStates()
        {
            List<Tuple<State,State>> allStates = new List<Tuple<State,State>>();
            Queue<Tuple<State, int, State>> statesToCheck = new Queue<Tuple<List<LabObject>, int, List<LabObject>>>();
            statesToCheck.Enqueue(Tuple.Create<State, int, State>(InitialState, 0, null));
            int solutionCount = 0;

            while (statesToCheck.Count > 0)
            {
                var stateTuple = statesToCheck.Dequeue();
                var state = stateTuple.Item1;
                
                if (AreStatesEquivalent(state, EndState))
                {
                    //Console.WriteLine($"Found a solution! Steps: {stateTuple.Item2}");
                    solutionCount++;

                    var current = Tuple.Create(stateTuple.Item1, stateTuple.Item3);
                    Route solution = new Route();
                    while (current.Item1 != InitialState)
                    {
                        solution.Add(current.Item1);
                        current = allStates.First(x => x.Item1.SequenceEqual(current.Item2));
                    }
                    solution.Add(InitialState);
                    solution.Reverse();
                    Solutions.Add(solution);

                    // don't add end state to seen, so we visit it again
                    continue;
                }

                if (!allStates.Any(x => x.Item1.SequenceEqual(state)))
                {
                    allStates.Add(Tuple.Create(state, stateTuple.Item3));

                    var movesFromState = GetMovesFromState(state);
                    foreach (var nextState in movesFromState)
                    {
                        if (!allStates.Any(x => x.Item1.SequenceEqual(nextState)) && !statesToCheck.Any(x => x.Item1.SequenceEqual(nextState)))
                        {
                            statesToCheck.Enqueue(Tuple.Create(nextState, stateTuple.Item2 + 1, state.Clone()));
                        }
                    }
                }
                else
                    Debug.Assert(false);

                //Console.WriteLine($"Total states {allStates.Count()}");
            }
            Console.WriteLine($"Total states:{allStates.Count} Solutions:{solutionCount}");
        }

        static bool CheckRoute(Route route)
        {
            // includes move that was made to get here
            var currentState = route.Last();

            //PrintState(currentState);
            //Console.ReadLine();

            // detect if we have won
            if (AreStatesEquivalent(currentState, EndState))
            {
                if (!Solutions.ContainsRoute(route))
                {
                    Console.WriteLine($"Solution found with {route.Count()-1} steps");
                    //Console.ReadLine();
                    Solutions.Add(route.Clone());
                }
                else
                {
                    Debug.Assert(false);
                }
                return true;
            }

            var allMovesFromHere = GetMovesFromState(currentState);
            var movesFromHere = allMovesFromHere.Where(x => !route.ContainsState(x));
            //Console.WriteLine($"Step {route.Count()} All possible moves: {allMovesFromHere.Count()} excluding backtracks: {movesFromHere.Count()}");
            //Console.ReadLine();

            foreach (var nextMove in movesFromHere)
            {
                if (DeadEnds.ContainsState(nextMove))
                {
                    //Console.WriteLine("Dead end!");
                    continue;
                }
                route.Add(nextMove);
                
                bool solved = CheckRoute(route);
                if (!solved)
                {
                    DeadEnds.Add(route.Last().Clone());
                }
                route.RemoveAt(route.Count() - 1);
                
                
            }
            return false;
        }
    }
}
