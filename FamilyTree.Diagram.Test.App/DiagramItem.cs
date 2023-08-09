using System.Collections.Generic;
using Microsoft.FamilyShowLib;

namespace FamilyTree.Diagram.Test.App;

public class DiagramItem
{
    public IList<Person> Persons { get; set; } = new List<Person>();
    public IList<Person> Children { get; set; } = new List<Person>();

    public int Generation { get; set; }

    public int Width { get; set; }

    public override string ToString()
    {
        return $"{Persons[0]}, {Persons[1]}";
    }
}