using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AdventOfCode11
{
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
        G
    }

    public class LabObject : IEquatable<LabObject>
    {
        public Element Isotope { get; set; }
        public ObjType ObjectType { get; set; }

        Boolean IEquatable<LabObject>.Equals(LabObject other)
        {
            if (other == null)
                return false;

            return (Isotope == other.Isotope && ObjectType == other.ObjectType);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is LabObject)
                return ((IEquatable<LabObject>) this).Equals((LabObject)obj);
            return false;
        }

        public override Int32 GetHashCode()
        {
            return (ObjectType.ToString() + Isotope.ToString()).GetHashCode();
        }

        public override String ToString()
        {
            return $"{ObjectType}{Isotope}";
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

    public class State : IEquatable<State>
    {
        public List<LabObject>[] Floors = new List<LabObject>[4] { new List<LabObject>(), new List<LabObject>(), new List<LabObject>(), new List<LabObject>() };

        Boolean IEquatable<State>.Equals(State other)
        {
            if (other == null || this.Floors.Length != other.Floors.Length)
                return false;

            for (int i=0;i<Floors.Length;i++)
            {
                if (Floors[i].Count() != other.Floors[i].Count())
                    return false;

                // shouldn't contain anything that the other one doesn't
                var except1 = Floors[i].Except(other.Floors[i]);
                var except2 = other.Floors[i].Except(Floors[i]);

                if (except1.Count() != 0 || except2.Count() != 0)
                    return false;
            }
            return true;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is State)
                return ((IEquatable<State>) this).Equals((State)obj);
            return false;
        }

        public override Int32 GetHashCode()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var floor in Floors)
            {
                floor.ForEach(x => sb.Append(x));
            }
            return sb.ToString().GetHashCode();
        }

        public static bool operator ==(State a, State b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(State a, State b)
        {
            return !a.Equals(b);
        }
    }


    class Program
    {
        public static bool IsValidState(State state)
        {
            foreach (var floor in state.Floors)
            {
                var conflicts = floor.Count(x => x.ObjectType == ObjType.G && floor.Count(y => y.ObjectType == ObjType.M && floor.Count(z => z.ObjectType == ObjType.G && z.Isotope == y.Isotope) == 0 ) > 0);
                if (conflicts > 0)
                    return false;
            }
            return true;
        }

        public static void PrintState(State state)
        {
            var sb = new StringBuilder();
            foreach (var floor in state.Floors.Reverse())
            {
                sb.Append("|");
                floor.ForEach(x => sb.Append($"{x} "));
                sb.AppendLine();
            }
            Console.WriteLine(sb);
        }
        static void Main(string[] args)
        {
            State initialState = new State();
            LabObject[] floor1 =
            {
                new LabObject() { Isotope = Element.TM, ObjectType = ObjType.G },
                new LabObject() { Isotope = Element.TM, ObjectType = ObjType.M },
                new LabObject() { Isotope = Element.PU, ObjectType = ObjType.G },
                new LabObject() { Isotope = Element.SR, ObjectType = ObjType.G }
            };
            LabObject[] floor2 =
            {
                new LabObject() { Isotope = Element.PU, ObjectType = ObjType.M },
                new LabObject() { Isotope = Element.SR, ObjectType = ObjType.M }
            };
            LabObject[] floor3 =
            {
                new LabObject() { Isotope = Element.PM, ObjectType = ObjType.G },
                new LabObject() { Isotope = Element.PM, ObjectType = ObjType.M },
                new LabObject() { Isotope = Element.RU, ObjectType = ObjType.G },
                new LabObject() { Isotope = Element.RU, ObjectType = ObjType.M }
            };
            initialState.Floors[0].AddRange(floor1);
            initialState.Floors[1].AddRange(floor2);
            initialState.Floors[2].AddRange(floor3);
            Debug.Assert(IsValidState(initialState));

            var checkStates = new List<State>();
            var allStates = new List<State>();

            checkStates.Add(initialState);

            while (checkStates.Count() > 0)
            {
                // get a state, analyze all moves possible from this state, then remove this state from checkStates
                var state = checkStates.First();
                PrintState(state);
                checkStates.Remove(state);
            }

            Console.ReadLine();
        }
    }
}
