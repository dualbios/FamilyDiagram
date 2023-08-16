
using FamilyShow.Lib.Net7;

namespace TestBlazorDrawing;

public class GlobalState
{
    private static GlobalState _instanse;
    public PeopleCollection PeopleCollection { get; } = new PeopleCollection();
    public Person SelectedPerson { get; set; }

    public static GlobalState Instance { get; } = _instanse ??= new GlobalState();
    public IEnumerable<DiagramItem> Families { get; set; }

    private GlobalState()
    {
        
    }
}