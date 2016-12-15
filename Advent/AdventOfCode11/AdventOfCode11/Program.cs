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
        TM
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
            return (ObjectType.ToString() + Isotope.ToString()).GetHashCode() + Floor.GetHashCode();
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
            return haystack.Where(x => x.Where(y => y.ObjectType != ObjType.E).SequenceEqual(needle.Where(z => z.ObjectType != ObjType.E))).Count() > 0;
            //return haystack.Where(x => x.SequenceEqual(needle)).Count() > 0;
        }

        public static bool ContainsRoute(this List<Route> haystack, Route needle)
        {
            foreach (var route in haystack)
            {
                if (route.Count() != needle.Count())
                    continue;

                int i;
                for (i=0;i<route.Count();i++)
                {
                    if (!Program.AreStatesEquivalent(route[i], needle[i]))
                        break;
                }
                if (i == route.Count())
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Program
    {
        public static bool AreStatesEquivalent(State a, State b)
        {
            // don't consider the elevator
            return a.Where(x => x.ObjectType != ObjType.E).SequenceEqual(b.Where(y => y.ObjectType != ObjType.E));
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

        public static State InitialState = new State()
        {
            new LabObject() { ObjectType = ObjType.E, Floor = 1 },
            new LabObject() { Isotope = Element.TM, ObjectType = ObjType.G, Floor = 1 },
            new LabObject() { Isotope = Element.TM, ObjectType = ObjType.M, Floor = 1 },
            new LabObject() { Isotope = Element.PU, ObjectType = ObjType.G, Floor = 1 },
            //new LabObject() { Isotope = Element.SR, ObjectType = ObjType.G, Floor = 1 },

            new LabObject() { Isotope = Element.PU, ObjectType = ObjType.M, Floor = 2 },
            //new LabObject() { Isotope = Element.SR, ObjectType = ObjType.M, Floor = 2 },

            //new LabObject() { Isotope = Element.PM, ObjectType = ObjType.G, Floor = 3 },
            //new LabObject() { Isotope = Element.PM, ObjectType = ObjType.M, Floor = 3 },
            //new LabObject() { Isotope = Element.RU, ObjectType = ObjType.G, Floor = 3 },
            //new LabObject() { Isotope = Element.RU, ObjectType = ObjType.M, Floor = 3 }
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

                    if (!IsValidState(newState))
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

            return validMoves;
        }

        public static List<Route> Solutions = new List<Route>();

        static void Main(string[] args)
        {
            Debug.Assert(IsValidState(InitialState));

            PrintState(InitialState);

            Route initialRoute = new Route();
            initialRoute.Add(InitialState);

            CheckRoute(initialRoute);

            Console.WriteLine($"Solutions: {Solutions.Count()}");
            foreach (var solution in Solutions)
            {
                Console.WriteLine($"Solution {solution.Count() - 1} steps:");
                solution.ForEach(x => PrintState(x));
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        static void CheckRoute(Route route)
        {
            // includes move that was made to get here
            var currentState = route.Last();

            //PrintState(currentState);
            //Console.ReadLine();

            // detect if we have won
            if (currentState.All(x => x.Floor == 4))
            {
                if (!Solutions.ContainsRoute(route))
                {
                    //Console.WriteLine("Found unique solution!");
                    //Console.ReadLine();
                    Console.WriteLine($"Solution {route.Count() - 1} steps:");
                    //route.ForEach(x => PrintState(x));
                    Console.ReadLine();
                    Solutions.Add(route.Clone());
                }
                return;
            }

            var allMovesFromHere = GetMovesFromState(currentState);
            var movesFromHere = allMovesFromHere.Where(x => !route.ContainsState(x));
            //Console.WriteLine($"Step {route.Count()} All possible moves: {allMovesFromHere.Count()} excluding backtracks: {movesFromHere.Count()}");
            //Console.ReadLine();

            foreach (var nextMove in movesFromHere)
            {
                route.Add(nextMove);
                CheckRoute(route);
                route.RemoveAt(route.Count() - 1);
            }
        }
    }
}
