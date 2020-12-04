using HumanResources.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HumanResources
{
    public partial class AddEditEmployee : Form
    {
        private int _currentEmployeeId;
        private Employee _currentEmployee;
        private bool _newRecord;
        private FileHelper<List<Employee>> _fileHelper = new FileHelper<List<Employee>>(Program.FilePath);

        public AddEditEmployee(int id = 0)
        {
            _currentEmployeeId = id;
            _newRecord = (id == 0) ? true : false;
//            MessageBox.Show(_currentEmployeeId.ToString());
            InitializeComponent();
            GetCurrentEmployeeData();
            SetVisibility();
            tbId.Enabled = false;
            mtbSalary.Mask = "0,000.00";
            
            cboDepartment.DataSource = Groups.Departments;
            cboDepartment.DisplayMember = "Name";
            cboDepartment.ValueMember = "Id";
            cboDepartment.SelectedIndex = 0;

            StartPosition = FormStartPosition.CenterScreen;
            this.Text = _newRecord? "Dodawanie Nowego Pracownika":"Edycja Danych Pracownika";

            tbNumber.Select();

        }

        /// <summary>
        /// Metoda pobierająca dane o studencie
        /// </summary>
        private void GetCurrentEmployeeData()
        {
            if (_currentEmployeeId != 0)
            {
                var employees = _fileHelper.DeserializeFromFile();
                _currentEmployee = employees.FirstOrDefault(x => x.Id == _currentEmployeeId);

                if (_currentEmployee == null)
                    throw new Exception($"Brak pracownika o Id = {_currentEmployeeId}");

                FillTextBoxes();

            }
        }

        /// <summary>
        /// Uzupałniamy pola tekstowe wczytanymi danymi
        /// </summary>
        private void FillTextBoxes()
        {
            tbId.Text = _currentEmployee.Id.ToString();
            tbNumber.Text = _currentEmployee.Number;
            tbFirstName.Text = _currentEmployee.FirstName;
            tbLastName.Text = _currentEmployee.LastName;
            dtpHireDate.Value = _currentEmployee.HireDate;
            mtbSalary.Text = _currentEmployee.Salary.ToString();
            chkReleased.Checked = _currentEmployee.Released;

            if (_currentEmployee.ReleaseDate != new DateTime(1,1,1))
                dtpReleaseDate.Value = _currentEmployee.ReleaseDate;
            else
                dtpReleaseDate.Value = DateTime.Now;

            cboDepartment.DataSource = Groups.Departments;
            cboDepartment.DisplayMember = "Name";
            cboDepartment.ValueMember = "Id";
            cboDepartment.SelectedIndex = _currentEmployee.DepartmentId;
        }


        /// <summary>
        /// Dodajemy nowego pracownika do listy
        /// przypisujemy wartości z pól tekstowych do obiektu
        /// obiekt dodajemy do listy
        /// na koniec wykonujemy serializację
        /// </summary>
        /// <param name="employees"></param>
        private void AddNewEmployeeToList(List<Employee> employees)
        {
            var employee = new Employee
            {
                Id = _currentEmployeeId,
                Number = tbNumber.Text,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                HireDate = dtpHireDate.Value,
                Salary = Decimal.Parse(mtbSalary.Text), 
                Released = chkReleased.Checked,
                DepartmentId = (cboDepartment.SelectedItem as Department).Id,
            };

            if (chkReleased.Checked)
                employee.ReleaseDate = dtpReleaseDate.Value;
            else
                employee.ReleaseDate = new DateTime(1,1,1);

            employees.Add(employee);
            _fileHelper.SerializeToFile(employees);
           // _fileHelper.SerializeToFileJson(employees);
        }


        /// <summary>
        /// Nadajemy Id nowemu pracownikowi
        /// </summary>
        /// <param name="employees"></param>
        private void AssignNewIdToEmployee(List<Employee> employees)
        {
            var employeeWithHighestId = employees
                    .OrderByDescending(x => x.Id).FirstOrDefault();

            _currentEmployeeId = employeeWithHighestId == null ? 1 : employeeWithHighestId.Id + 1;
        }

        /// <summary>
        /// Zatwierdzamy zmiany
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var employees = _fileHelper.DeserializeFromFile();

            // pobieramy jeszcze raz Listę wszystkich uczniów
            // jeżeli edytujamy, to usuwamy z listy ucznia któego dane będziemy edytować
            // później dodamy tego ucznia i zapiszemy w pliku
            // robimy tak aby było jak najwięcej wspólnej logiki
            // pole _studentId aby mieć dostęp do parametru przekazanego do konstruktora
            if (_currentEmployeeId != 0)
            {
                employees.RemoveAll(x => x.Id == _currentEmployeeId);
            }
            else
            {
                AssignNewIdToEmployee(employees);
            }

            AddNewEmployeeToList(employees);

            // event – krok 4
            // wywołujemy metodę pomocniczą przed zamknięciem ekranu
            // gdy metoda zostanie wywołana wyzwoli zdarzenie StudentAdded
            // która powiadomi o tym zdarzeniu swoich subskrybentów

            //OnStudentAdded();
            // await LongProcessAsync();
            Close();
        }

   
        /// <summary>
        /// Zamykamy Formatkę
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkReleased_CheckedChanged(object sender, EventArgs e)
        {
            SetVisibility();
        }

        private void SetVisibility()
        {
            dtpReleaseDate.Visible = chkReleased.Checked;
            lblReleaseDate.Visible = chkReleased.Checked;
        }

    }
}
