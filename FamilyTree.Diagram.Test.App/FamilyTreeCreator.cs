using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.FamilyShowLib;

public class FamilyTreeCreator
{
    private readonly List<DiagramItem> families = new();

    public IEnumerable<DiagramItem> CreateTree(PeopleCollection peopleCollection, Person current)
    {
        DiagramItem grandPaItem = new() { Persons = current.Parents[0].Parents, Generation = -2 };
        DiagramItem grandMaItem = new() { Persons = current.Parents[1].Parents, Generation = -2 };
        families.Clear();
        families.Add(grandPaItem);
        families.Add(grandMaItem);
        DrillDown(grandPaItem, -1);
        DrillDown(grandMaItem, -1);
        DrillUp(grandPaItem);
        DrillUp(grandMaItem);

        Dictionary<int, List<Person>> ppp = families.GroupBy(x => x.Generation)
                                                    .OrderBy(x => x.Key)
                                                    .ToDictionary(
                                                        k => k.Key, v => v.SelectMany(x => x.Persons).ToList());


        foreach (DiagramItem f in families.Where(x => x.Generation == -1))
        {
            CalcChildrenWidth(f);
        }

        foreach (DiagramItem f in families.Where(x => x.Generation == -2))
        {
            CalcParentWidth(f);
        }

        Dictionary<int, int> dictionary = families.GroupBy(x => x.Generation)
                                                  .OrderBy(x => x.Key)
                                                  .ToDictionary(k => k.Key, v => v.Sum(x => x.Width));

        return families;
    }

    private void DrillUp(DiagramItem item)
    {
        foreach (Person person in item.Persons)
        {
            if (person.Parents.Count > 1 && families.FirstOrDefault(x => x.Persons.Contains(person.Parents[0])
                                                                         && x.Persons.Contains(
                                                                             person.Parents[1])) == null)
            {
                DiagramItem di = new()
                {
                    Children = new List<Person> { person },
                    Generation = item.Generation - 1,
                    Persons = person.Parents
                };
                families.Add(di);
                DrillUp(di);
            }
        }
    }

    private void DrillDown(DiagramItem di, int generation)
    {
        foreach (Person child in di.Persons.SelectMany(x => x.Children).Distinct())
        {
            if (!di.Children.Contains(child))
            {
                di.Children.Add(child);
                if (child.Spouses.Any())
                {
                    foreach (Person spouse in child.Spouses)
                    {
                        if (families.FirstOrDefault(x => x.Persons.Contains(child) && x.Persons.Contains(spouse)) == null)
                        {
                            DiagramItem dd = new()
                            {
                                Persons = new[] { child, spouse },
                                Generation = generation,
                                Width = 2
                            };
                            families.Add(dd);
                            DrillDown(dd, generation + 1);
                        }
                    }
                }
                else
                {
                    DiagramItem dd = new()
                    {
                        Persons = new[] { child },
                        Generation = generation,
                        Width = 2
                    };
                    families.Add(dd);
                }
            }
        }
    }

    private int CalcParentWidth(DiagramItem family)
    {
        int width = 0;
        IEnumerable<DiagramItem> items = family.Persons
                                               .Select(x => families.FirstOrDefault(f => f.Children.Contains(x)))
                                               .Where(x => x != null)
                                               .Distinct()
                                               .ToList()!;
        if (!items.Any())
        {
            family.Width = 2;
            return family.Width;
        }

        foreach (DiagramItem diagramItem in items)
        {
            int parentWidth = CalcParentWidth(diagramItem);
            width += parentWidth;
        }

        family.Width = Math.Max(family.Width, width);

        return family.Width;
    }

    /// <summary>
    /// The Source contains all of items from List
    /// </summary>
    private bool ListContains<T>(IEnumerable<T> source, IEnumerable<T> list)
    {
        foreach (T item in list)
        {
            if (!source.Contains(item))
                return false;
        }

        return true;
    }

    private int CalcChildrenWidth(DiagramItem family)
    {
        int width = 0;
        IEnumerable<Person> children = family.Children.Where(x => ListContains(x.Parents, family.Persons)).ToList();
        IEnumerable<DiagramItem> items = children
                                         .SelectMany(x => families.Where(f => f.Persons.Contains(x)))
                                         .Where(x => x != null)
                                         .ToList()!;
        foreach (DiagramItem diagramItem in items)
        {
            int childrenWidth = CalcChildrenWidth(diagramItem);
            width += childrenWidth;
        }

        width += children
                 .Select(x => families.FirstOrDefault(f => f.Persons.Contains(x)))
                 .Count(x => x == null);

        family.Width = Math.Max(2, width);
        return family.Width;
    }
}