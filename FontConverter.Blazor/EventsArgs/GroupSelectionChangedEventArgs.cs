namespace FontConverter.Blazor.EventsArgs;

public class GroupSelectionChangedEventArgs : EventArgs
{
    public GroupSelectionChangedEventArgs()
    {
        GroupsList = [];
    }

    public GroupSelectionChangedEventArgs(List<(int GroupID, int SelectedItemsCount)> groupsList) : this()
    {
        GroupsList = groupsList;
    }

    public List<(int GroupID, int SelectedItemsCount)> GroupsList { get; set; }
}
