using System.Collections.Generic;

public class InventoryItemSelectContainer
{
    private readonly int _maxSelectAbleItems = 1;

    // Overrides the selected Item if the max selected Items is reach
    private readonly bool _overrideSelectedItem = true;

    private readonly List<IInventoryGridElement> _selectedItems = new();

    public void AddSelectedItem(IInventoryGridElement item)
    {
        if (_selectedItems.Count >= _maxSelectAbleItems)
        {
            if (_overrideSelectedItem)
            {
                _selectedItems.Clear();
                _selectedItems.Add(item);
            }

            return;
        }

        _selectedItems.Add(item);
    }

    public void ClearAllSelectedItems()
    {
        _selectedItems.Clear();
    }

    public List<IInventoryGridElement> GetSelectedItems()
        => _selectedItems;

}