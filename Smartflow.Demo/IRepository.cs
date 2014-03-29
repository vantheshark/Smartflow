using System.Collections.Generic;

namespace Smartflow.Demo
{
    public interface IRepository<T> where T : class, new()
    {
        void Save(T entity);
        IEnumerable<T> GetAll();
    }

    public class InMemoryDatabase<T> : IRepository<T> where T : class, new()
    {
        private readonly List<T> _store = new List<T>(); 
        public void Save(T entity)
        {
            _store.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _store;
        }
    }
}
