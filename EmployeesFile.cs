using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HumanResources.Model;


// program kadrowy
// imie, nazwisko, data zatrudnienia, data zwolnienia, numer procownika, wysokość zarobków
// zapisujemy do json'a dla odmiany
// dodawanie, edycja, brak możliwości usuwania, funkcja zwalniania pracownika
// filtrowanie danych

namespace HumanResources
{
    public partial class EmployeesFile : Form
    {
        private FileHelper<List<Employee>> _fileHelper =
            new FileHelper<List<Employee>>(Program.FilePath);

        public int AddEditStudent_FormClosing { get; private set; }

        public EmployeesFile()
        {
            InitializeComponent();
            Text = "";

            var ListOfDepartments = new List<Department>();
            ListOfDepartments.Add(new Department { Id = -1, Name = "All" });

            // Groups.Departments.CopyTo(ListOfDepartments);

            foreach(var x in Groups.Departments)
            {
                ListOfDepartments.Add(x);
            }

            ListOfDepartments = ListOfDepartments.OrderBy(x => x.Id).ToList();

            cboDepartment.DataSource = ListOfDepartments;
            cboDepartment.DisplayMember = "Name";
            cboDepartment.ValueMember = "Id";
            cboDepartment.SelectedIndex = 0;


            //PopulateEmployee();
            RefreshList();
            SetDgvProperities();
            SetDgvColumnHeader();




        }


        /// <summary>
        /// Ustawienie tytułów kolumn oraz przypisanie pól do kolumn w DataGridView
        /// </summary>
        private void SetDgvColumnHeader()
        {
            dgvEmployees.Columns[nameof(Employee.Id)].DisplayIndex = 0;
            dgvEmployees.Columns[nameof(Employee.Number)].DisplayIndex = 1;
            dgvEmployees.Columns[nameof(Employee.FirstName)].DisplayIndex = 2;
            dgvEmployees.Columns[nameof(Employee.LastName)].DisplayIndex = 3;
            dgvEmployees.Columns[nameof(Employee.HireDate)].DisplayIndex = 4;
            dgvEmployees.Columns[nameof(Employee.Salary)].DisplayIndex = 5;
            dgvEmployees.Columns[nameof(Employee.Released)].DisplayIndex = 6;
            dgvEmployees.Columns[nameof(Employee.ReleaseDate)].DisplayIndex = 7;
            dgvEmployees.Columns["Department"].DisplayIndex = 8;

            dgvEmployees.Columns[nameof(Employee.Id)].HeaderText = "Id";
            dgvEmployees.Columns[nameof(Employee.Number)].HeaderText = "Numer";
            dgvEmployees.Columns[nameof(Employee.FirstName)].HeaderText = "Imię";
            dgvEmployees.Columns[nameof(Employee.LastName)].HeaderText = "Nazwisko";
            dgvEmployees.Columns[nameof(Employee.HireDate)].HeaderText = "Data Zatrudnienia";
            dgvEmployees.Columns[nameof(Employee.Salary)].HeaderText = "Wynagrodzenie";
            dgvEmployees.Columns[nameof(Employee.Released)].HeaderText = "Zwolniony";
            dgvEmployees.Columns[nameof(Employee.ReleaseDate)].HeaderText = "Data Zwolnienia";
            dgvEmployees.Columns["Department"].HeaderText = "Department";

            dgvEmployees.Columns[9].Visible = false;

            /*
            dgvEmployees.Columns[0].HeaderText = "Id";
            dgvEmployees.Columns[1].HeaderText = "Numer";
            dgvEmployees.Columns[2].HeaderText = "Imię";
            dgvEmployees.Columns[3].HeaderText = "Nazwisko";
            dgvEmployees.Columns[4].HeaderText = "Data Zatrudnienia";
            dgvEmployees.Columns[5].HeaderText = "Wynagrodzenie";
            dgvEmployees.Columns[6].HeaderText = "Zwolniony";
            dgvEmployees.Columns[7]
            dgvEmployees.Columns[8]
            */


        }

        /// <summary>
        /// Ustawienie cech DataGridView
        /// </summary>
        private void SetDgvProperities()
        {
            dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.RowHeadersVisible = false;
            dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployees.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }


        private void RefreshList()
        {
            var employees = _fileHelper.DeserializeFromFile();
            //var selectedGroupId = (cboGroupOfStudent.SelectedItem as GroupOfStudent).Id;


            var employeesQueryable2 = employees
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    Id = x.Id,
                    Number = x.Number,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    HireDate = x.HireDate,
                    Salary = x.Salary,
                    Released = x.Released,
                    ReleaseDate = x.ReleaseDate,
                    DepartmentId = x.DepartmentId,
                }).AsQueryable();

            
            var employeesQueryable = employees
            .Join(Groups.Departments, left => left.DepartmentId, right => right.Id,
               (left, right) => new { EmployeeColumns = left, DepartmentsColumns = right })
            .Select(x => new
            {
                Id = x.EmployeeColumns.Id,
                Number = x.EmployeeColumns.Number,
                FirstName = x.EmployeeColumns.FirstName,
                LastName = x.EmployeeColumns.LastName,
                HireDate = x.EmployeeColumns.HireDate,
                Salary = x.EmployeeColumns.Salary,
                Released = x.EmployeeColumns.Released,
                ReleaseDate = x.EmployeeColumns.ReleaseDate,
                Department  = x.DepartmentsColumns.Name,
                DepartmentId = x.EmployeeColumns.DepartmentId,
            }).AsQueryable();

            
            

            if (!chkReleased.Checked)
            {
                employeesQueryable = employeesQueryable.Where(x => x.Released == false);
            }

            var selectedGroupId = (cboDepartment.SelectedItem as Department).Id;
            if (selectedGroupId >= 0)
            {
                employeesQueryable = employeesQueryable.Where(x => x.DepartmentId == selectedGroupId);
            }

            dgvEmployees.DataSource = employeesQueryable.OrderBy(x=>x.Id).ToList();
        }


        /// <summary>
        /// wypełnienie listy studentów przykładowymi danymi
        /// </summary>        
        public void PopulateEmployee()
        {
            var employees = new List<Employee>();
            // w tym przypadku może też być z nawiasami po new Student() lub bez 
            employees.Add(new Employee { Id = 1, Number = "000011", FirstName = "Jan", LastName = "Kowalski", HireDate = DateTime.Parse("2001-01-01"), Salary = 7500.00M,  Address = new Address { City = "Szczecin" } });
            employees.Add(new Employee { Id = 2, Number = "002201", FirstName = "Jan", LastName = "Nowak", HireDate = DateTime.Parse("2001-01-01"), Salary = 4500.00M, });
            employees.Add(new Employee { Id = 3, Number = "002301", FirstName = "Alfred", LastName = "Kowalski", HireDate = DateTime.Parse("2020-01-01"), Salary = 3200.00M, });
            employees.Add(new Employee { Id = 4, Number = "002009", FirstName = "Joanna", LastName = "Bartkowiak", HireDate = DateTime.Parse("1995-01-01"), Salary = 6200.00M, });
            _fileHelper.SerializeToFile(employees);
        }
        
        // przyciski, obsługa zdarzeń

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            // sprawdzamy, czy jakiś wiersz został zaznaczony
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Zaznacz pracownika, którego dane chcesz edytować");
                return;
            }

            // okno jest zwykłą klasą więc tworzymy jego instancję
            // F12 na nazwie klasy
            int id = Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells[0].Value);
            int rowIndex = dgvEmployees.CurrentCell.RowIndex;
            var addEditEmployee = new AddEditEmployee(id);

            addEditEmployee.FormClosing += AddEditEmployee_FormClosing;
            addEditEmployee.ShowDialog();
            addEditEmployee.FormClosing -= AddEditEmployee_FormClosing;

            if (dgvEmployees.RowCount >= (rowIndex + 1))
                dgvEmployees.CurrentCell = dgvEmployees.Rows[rowIndex].Cells[0];
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // okno jest zwykłą klasą więc tworzymy jego instancję
            var addEditEmployee = new AddEditEmployee();

            addEditEmployee.FormClosing += AddEditEmployee_FormClosing;
            addEditEmployee.ShowDialog();
            addEditEmployee.FormClosing -= AddEditEmployee_FormClosing;

        }

        // zdarzenia

        private void AddEditEmployee_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshList();
        }

        private void chkReleased_CheckedChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void cboDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Edit();
        }
    }
}
