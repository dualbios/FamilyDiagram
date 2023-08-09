using Microsoft.FamilyShowLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyTree.Diagram.Test.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PeopleCollection PeopleCollection = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new() { IsFolderPicker = false };
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                new GedcomImport().Import(PeopleCollection, ofd.FileName);
                Person current = PeopleCollection[15];
                CreateTree(PeopleCollection, current);
            }
        }

        private HashSet<Person> visitedPersons = new();

        private List<DiagramItem> Families = new();

        private void CreateTree(PeopleCollection peopleCollection, Person current)
        {
            DiagramItem grandPaItem = new() { Persons = current.Parents[0].Parents, Generation = -2 };
            DiagramItem grandMaItem = new() { Persons = current.Parents[1].Parents, Generation = -2 };
            Families.Clear();
            Families.Add(grandPaItem);
            Families.Add(grandMaItem);
            DrillDown(grandPaItem, -1);
            DrillDown(grandMaItem, -1);
            DrillUp(grandPaItem);
            DrillUp(grandMaItem);


            foreach (DiagramItem f in Families.Where(x => x.Generation == 0))
            {
                CalcChildrenWidth(f);
            }

            Dictionary<int, int> dictionary = Families.GroupBy(x => x.Generation)
                                                      .ToDictionary(k => k.Key, v => v.Sum(x => x.Width));
        }

        private void DrillUp(DiagramItem item)
        {
            foreach (Person person in item.Persons)
            {
                if (Families.FirstOrDefault(x => person.Parents.Count > 1
                                                 && x.Persons.Contains(person.Parents[0])
                                                 && x.Persons.Contains(person.Parents[1])) == null)
                {
                    DiagramItem di = new()
                    {
                        Children = new List<Person> { person },
                        Generation = item.Generation - 1,
                        Persons = person.Parents
                    };
                    Families.Add(di);
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
                    foreach (Person spouse in child.Spouses)
                    {
                        if (Families.FirstOrDefault(x => x.Persons.Contains(child) && x.Persons.Contains(spouse)) ==
                            null)
                        {
                            DiagramItem dd = new()
                            {
                                Persons = new[] { child, spouse },
                                Generation = generation,
                                Width = 2
                            };
                            Families.Add(dd);
                            DrillDown(dd, generation + 1);
                        }
                    }
                }
            }
        }

        private int CalcChildrenWidth(DiagramItem family)
        {
            int width = 0;
            IEnumerable<DiagramItem?> items = family.Children
                                                    .Select(x => Families.FirstOrDefault(f => f.Persons.Contains(x)))
                                                    .Where(x => x != null)
                                                    .ToList();
            foreach (DiagramItem diagramItem in items)
            {
                width += CalcChildrenWidth(diagramItem);
            }

            width += family.Children.Count - items.Count();

            family.Width = Math.Max(2, width);
            return family.Width;
        }
    }

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
}