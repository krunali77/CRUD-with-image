

// ICarRepository.cs
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cars.Models
{
    public interface ICarRepository
    {
        Car GetById(int id);
        IEnumerable<Car> GetAll();
        IEnumerable<Car> Find(Expression<Func<Car, bool>> predicate);
        Car Details(int id);  // New method for retrieving details by Id
        void Add(Car entity);
        void Update(Car entity);
        void Delete(Car entity);
    }
}
