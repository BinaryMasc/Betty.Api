using System.Collections;

namespace Betty.Api.Infrastructure.Data
{
    public class SqlResultCollection<T> : IEnumerable<T>, IEnumerable
    {

        public string? Query { get; set; }

        private List<T>? items = new List<T>();

        public SqlResultCollection(IEnumerable<T> objects)
        {
            items = objects.ToList();
        }

        // Implement GetEnumerator method for IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        // Implement IEnumerable.GetEnumerator explicitly
        // This is necessary to avoid ambiguity
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Add additional methods as needed
        public void Add(T item)
        {
            items.Add(item);
        }
    }
}
