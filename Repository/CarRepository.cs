
// CarRepository.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Cars.Data;
using Microsoft.EntityFrameworkCore;

namespace Cars.Models
{
    public class CarRepository : ICarRepository
    {
        private readonly CarDbContext _context;

        public CarRepository(CarDbContext context)
        {
            _context = context;
        }

        public Car GetById(int id)
        {
            return _context.CarModels.Find(id);
        }

        public IEnumerable<Car> GetAll()
        {
            return _context.CarModels.ToList();
        }

        public IEnumerable<Car> Find(Expression<Func<Car, bool>> predicate)
        {
            return _context.CarModels.Where(predicate).ToList();
        }

        public Car Details(int id)
        {
            // Retrieve a single instance of Car based on Id
            return _context.CarModels.FirstOrDefault(car => car.Id == id);
        }

        public void Add(Car entity)
        {
            _context.CarModels.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Car entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Car entity)
        {
            _context.CarModels.Remove(entity);
            _context.SaveChanges();
        }
    }
}
