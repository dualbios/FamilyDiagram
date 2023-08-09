using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.FamilyShowLib;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;
using UserControl = System.Windows.Controls.UserControl;

namespace FamilyTree.Diagram.Test.App
{
    /// <summary>
    /// Interaction logic for FamilyTreeControl.xaml
    /// </summary>
    public partial class FamilyTreeControl : UserControl
    {
        private static int ItemWidth = 100;

        public object FamilyCollection
        {
            get => (object)GetValue(FamilyCollectionProperty);
            set => SetValue(FamilyCollectionProperty, value);
        }

        public static readonly DependencyProperty FamilyCollectionProperty =
            DependencyProperty.Register(nameof(FamilyCollection),
                                        typeof(object),
                                        typeof(FamilyTreeControl),
                                        new PropertyMetadata(FamilyCollectionChangedCallback));

        private static void FamilyCollectionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FamilyTreeControl control)
            {
                if (control.FamilyCollection is IEnumerable<DiagramItem> families)
                {
                    control.MainStack.Children.Clear();

                    int maxWidth = families.GroupBy(x => x.Generation)
                                           .Select(x => x.Sum(z => z.Width))
                                           .Max(x => x);


                    Dictionary<int, IEnumerable<DiagramItem>> dictionary = families.GroupBy(x => x.Generation)
                        .OrderBy(x => x.Key)
                        .ToDictionary(
                            k => k.Key,
                            v => v.ToList().AsEnumerable());

                    foreach (KeyValuePair<int, IEnumerable<DiagramItem>> item in dictionary)
                    {
                        StackPanel gridPanel = new()
                        {
                            Orientation = Orientation.Horizontal,
                            //Width = maxWidth * ItemWidth*2
                        };

                        foreach (DiagramItem diagramItem in item.Value)
                        {
                            int diagramItemWidth = diagramItem.Width * ItemWidth * 2 / 2;
                            Grid familyStackPanel = new()
                            {
                                //Orientation = Orientation.Horizontal,
                                //Margin = new Thickness(diagramItemWidth, 0, diagramItemWidth, 0),
                                Width = diagramItem.Width * ItemWidth,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

                            Border bb = new()
                            {
                                BorderBrush = Brushes.Chartreuse,
                                BorderThickness = new Thickness(1),
                            };

                            StackPanel ss = new()
                            {
                                Orientation = Orientation.Horizontal,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

                            bb.Child = ss;
                            familyStackPanel.Children.Add(bb);

                            //List<Person> ggg = diagramItem.Persons[0].Siblings.Concat(diagramItem.Persons[1].Siblings)
                            //                              .Where(x => x.Parents.Contains(diagramItem.Persons[0]) && x.Parents.Contains(diagramItem.Persons[1]))
                            //                              .Except(diagramItem.Persons).ToList();

                            foreach (Person person in diagramItem.Persons)
                            {
                                ss.Children.Add(
                                    new Border()
                                    {
                                        BorderBrush = Brushes.CornflowerBlue,
                                        BorderThickness = new Thickness(1),
                                        Child = new TextBlock()
                                        {
                                            Text = person.FullName,
                                            TextWrapping = TextWrapping.Wrap,
                                            HorizontalAlignment = HorizontalAlignment.Center,
                                            Width = ItemWidth / 2
                                        }
                                    });
                            }

                            


                            gridPanel.Children.Add(familyStackPanel);
                        }

                        control.MainStack.Children.Add(gridPanel);
                    }
                }
            }
        }

        public PeopleCollection PeopleCollection
        {
            get => (PeopleCollection)GetValue(PeopleCollectionProperty);
            set => SetValue(PeopleCollectionProperty, value);
        }

        public static readonly DependencyProperty PeopleCollectionProperty =
            DependencyProperty.Register(nameof(Microsoft.FamilyShowLib.PeopleCollection),
                                        typeof(PeopleCollection),
                                        typeof(FamilyTreeControl),
                                        new PropertyMetadata(null));

        //public Person SelectedPerson
        //{
        //    get => (Person)GetValue(SelectedPersonProperty);
        //    set => SetValue(SelectedPersonProperty, value);
        //}

        //public static readonly DependencyProperty SelectedPersonProperty =
        //    DependencyProperty.Register(nameof(Person),
        //                                typeof(Person),
        //                                typeof(FamilyTreeControl),
        //                                new PropertyMetadata(null));

        //private static void SelectedPersonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is FamilyTreeControl control)
        //    {
        //        if (control.PeopleCollection is IEnumerable<People> peopleCollection)
        //        {
        //            if (control.SelectedPerson != null)
        //            {
        //                new FamilyTreeCreator().CreateTree(control.PeopleCollection, control.SelectedPerson);
        //            }
        //            else
        //            {

        //            }
        //        }
        //    }
        //}


        //private static void PeopleCollectionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //}


        public FamilyTreeControl()
        {
            InitializeComponent();
        }
    }
}