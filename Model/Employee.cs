using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Model
{
    public class Employee : Person
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime HireDate { get; set; }
        public bool Released { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }

        public override string GetInfo()
        {
            return $"Pracownik: {FirstName} {LastName}";
        }
    }
}
