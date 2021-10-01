using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD.EF.WFA
{
    public partial class FormEF_CRUD : Form
    {
        Customer customer = new Customer();
        public FormEF_CRUD()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }
        void Clear()
        {
            textBoxFirstName.Text = 
                textBoxLastName.Text = 
                textBoxCity.Text = 
                textBoxAddress.Text = "";
            buttonSave.Text = "Save";
            buttonDelete.Enabled = false;
            customer.CustomerId = 0;
        }

        private void FormEF_CRUD_Load(object sender, EventArgs e)
        {
            Clear();
            PopulateDataGridView();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            customer.FirstName = textBoxFirstName.Text.Trim();
            customer.LastName = textBoxLastName.Text.Trim();
            customer.City = textBoxCity.Text.Trim();
            customer.Address = textBoxAddress.Text.Trim();
            using (DBEntities db = new DBEntities())
            {
                if (customer.CustomerId != 0) //Update
                    db.Entry(customer).State = EntityState.Modified;
                else //Insert
                    db.Customers.Add(customer);
                db.SaveChanges();
            }
            Clear();
            PopulateDataGridView();

            if (customer.CustomerId != 0)
                MessageBox.Show("Updated successfully");
            else
                MessageBox.Show("Submitted successfully");
        }
        void PopulateDataGridView()
        {
            dataGridViewCustomer.AutoGenerateColumns = false;
            using (DBEntities db = new DBEntities())
            {
                dataGridViewCustomer.DataSource = db.Customers.ToList<Customer>();
            }
        }

        private void dataGridViewCustomer_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridViewCustomer.CurrentRow.Index != -1)
            {
                customer.CustomerId = Convert.ToInt32(dataGridViewCustomer.CurrentRow.Cells["CustmerId"].Value);
                using (DBEntities db = new DBEntities())
                {
                    customer = db.Customers.Where(x => x.CustomerId == customer.CustomerId).FirstOrDefault();
                    textBoxFirstName.Text = customer.FirstName;
                    textBoxLastName.Text = customer.LastName;
                    textBoxCity.Text = customer.City;
                    textBoxAddress.Text = customer.Address;
                }
                buttonSave.Text = "Update";
                buttonDelete.Enabled = true;
            }
        }
    }
}
