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
        //Customer model
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
            //Set to empty
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

        //For add/update operation
        private void buttonSave_Click(object sender, EventArgs e)
        {
            customer.FirstName = textBoxFirstName.Text.Trim();
            customer.LastName = textBoxLastName.Text.Trim();
            customer.City = textBoxCity.Text.Trim();
            customer.Address = textBoxAddress.Text.Trim();
            using (DBEntities db = new DBEntities())
            {
                if (customer.CustomerId != 0) //Updating info
                {
                    db.Entry(customer).State = EntityState.Modified;
                }
                else //Inserting info
                {
                    db.Customers.Add(customer);
                }
                db.SaveChanges();
            }
            Clear();
            PopulateDataGridView();

            if (customer.CustomerId != 0)
            {
                MessageBox.Show("Updated Successfully");
            }
            else
            {
                MessageBox.Show("Submitted Successfully");
            }
        }

        //show all data from db
        void PopulateDataGridView()
        {
            //dgv properties modification
            dataGridViewCustomer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCustomer.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCustomer.AutoGenerateColumns = false;
            //data source listing
            using (DBEntities db = new DBEntities())
            {
                dataGridViewCustomer.DataSource = db.Customers.ToList<Customer>();
            }
        }

        //Load data for update operation
        private void dataGridViewCustomer_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewCustomer.CurrentRow.Index != -1)
            {
                //Select any customer to load info
                customer.CustomerId = Convert.ToInt32(dataGridViewCustomer.CurrentRow.Cells["CustomerId"].Value);
                using (DBEntities db = new DBEntities())
                {
                    //Get customer's info by ID
                    customer = db.Customers.Where(x => x.CustomerId == customer.CustomerId).FirstOrDefault();
                    textBoxFirstName.Text = customer.FirstName;
                    textBoxLastName.Text = customer.LastName;
                    textBoxCity.Text = customer.City;
                    textBoxAddress.Text = customer.Address;
                }
                buttonSave.Text = "Update";
                //Can delete as well
                buttonDelete.Enabled = true;
            }
        }

        //For delete operation
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            //Warning before delete customer's info
            if(MessageBox.Show("Are You Sure to Delete this Record?", "EF - Delete Operation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (DBEntities db = new DBEntities())
                {
                    //Using Attach method on DbSet to track any entry
                    var entry = db.Entry(customer);
                    //If the entity isn't being tracked by dbcontext
                    if (entry.State == EntityState.Detached)
                    {
                        //The entity now is in unchanged state
                        db.Customers.Attach(customer);
                    }
                    //Delete data from db
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                    Clear();
                    PopulateDataGridView();
                    MessageBox.Show("Deleted Successfully");
                }
            }
        }
    }
}
