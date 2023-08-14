using FamilyShow.Lib.Net7;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.IO;
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
            familyTreeCreator = new FamilyTreeCreator();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog ofd = new() { IsFolderPicker = false };
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                new GedcomImport().Import(PeopleCollection, File.OpenRead(ofd.FileName));

                People.ItemsSource = PeopleCollection;
            }
        }

        private readonly FamilyTreeCreator familyTreeCreator;

        private void People_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (People.SelectedItem is Person person)
            {
                DrawTree(person);
            }
        }

        private void DrawTree(Person person)
        {
            IEnumerable<DiagramItem> families = familyTreeCreator.CreateTree(PeopleCollection, person);

            Dictionary<int, List<Person>> dictionary = families.GroupBy(x => x.Generation)
                                                               .OrderBy(x => x.Key)
                                                               .ToDictionary(
                                                                   k => k.Key,
                                                                   v => v.SelectMany(x => x.Persons).ToList());

            FamilyTreeDiagram.PeopleCollection = PeopleCollection;
            FamilyTreeDiagram.FamilyCollection = null;
            FamilyTreeDiagram.FamilyCollection = families;

        }
    }
}