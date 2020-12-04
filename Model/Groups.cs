using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Model
{

    public static class Groups
    {
        public static List<Department> Departments = new List<Department>
        {
        new Department { Id = 0, Name = "nie przypisany" },
        new Department { Id = 1, Name = "IT" },
        new Department { Id = 2, Name = "Kadry" },
        new Department { Id = 3, Name = "Księgowość" },
        new Department { Id = 4, Name = "Zarząd" },
        new Department { Id = 5, Name = "Produkcja" },
        new Department { Id = 6, Name = "Sprzedaż" },
        new Department { Id = 6, Name = "Zakupy" },
        };
    }
}
