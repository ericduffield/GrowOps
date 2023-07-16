using System.Collections.ObjectModel;

namespace GrowOps.Interfaces
{
    /// <summary>
    /// GrowOps
    /// Semester 6 April 28th, 2023
    /// App Dev III
    /// </summary>
    public interface IDataStore<T> where T : class, IHasUKey
  {
    Task<bool> AddItemAsync(T item); //Create operation
    Task<bool> UpdateItemAsync(T item); //Update operation
    Task<bool> DeleteItemAsync(T item); //Delete operation
    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false); //Get all items from database
    public ObservableCollection<T> Items { get; } //Item to bind to the collection
  }
}
