using Microsoft.FamilyShowLib;

namespace TestBlazorDrawing;

public class GlobalState
{
    private static GlobalState _instanse;
    public PeopleCollection PeopleCollection { get; } = new PeopleCollection();
    public Person SelectedPerson { get; set; }

    public static GlobalState Instance { get; } = _instanse ??= new GlobalState();

    private GlobalState()
    {
        
    }
}