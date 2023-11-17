using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace exam
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            DisplayProducts(); // Вызываем метод загрузки продуктов при загрузке формы
            UpdateDiscountComboBox();
        }
        private void DisplayProducts()
        {
            string connectionString = "Data Source=ADCLG1; Initial Catalog=!Trade; Integrated Security=true;";
            string query = "SELECT [ProductArticleNumber],[ProductName],[ProductDescription],[ProductCategory] ,[ProductManufacturer] ,[ProductCost] ,[ProductDiscountAmount]  ,[ProductQuantityInStock]  ,[ProductStatus] FROM Product";


            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    productsDataGridView.DataSource = dt;
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchString = searchTextBox.Text;
            DataView dv = (productsDataGridView.DataSource as DataTable).DefaultView;

            string selectedRange = discountComboBox.SelectedItem?.ToString();
            switch (selectedRange)
            {
                case "0-4,99%":
                    dv.RowFilter = $"ProductName LIKE '%{searchString}%' AND ProductDiscountAmount >= 0 AND ProductDiscountAmount < 5";
                    break;
                case "5% и более":
                    dv.RowFilter = $"ProductName LIKE '%{searchString}%' AND ProductDiscountAmount >= 5";
                    break;
                default:
                    dv.RowFilter = $"ProductName LIKE '%{searchString}%'";
                    break;
            }
        }


        private void SortByPriceButton_Click(object sender, EventArgs e)
        {
            DataView dv = (productsDataGridView.DataSource as DataTable).DefaultView;

            if (dv.Sort == "ProductCost ASC")
            {
                dv.Sort = "ProductCost DESC";
                priceSortLabel.Text = "Цена: по убыванию";
            }
            else
            {
                dv.Sort = "ProductCost ASC";
                priceSortLabel.Text = "Цена: по возрастанию";
            }
        }


        private void UpdateDiscountComboBox()
        {
            discountComboBox.Items.Clear();
            discountComboBox.Items.AddRange(new string[] { "0-4,99%", "5% и более" });
        }

        private void DiscountComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRange = discountComboBox.SelectedItem.ToString();
            DataView dv = (productsDataGridView.DataSource as DataTable).DefaultView;

            switch (selectedRange)
            {
                case "0-4,99%":
                    dv.RowFilter = "ProductDiscountAmount >= 0 AND ProductDiscountAmount < 5";
                    break;
                case "5% и более":
                    dv.RowFilter = "ProductDiscountAmount >= 5";
                    break;
                default:
                    dv.RowFilter = string.Empty;
                    break;
            }
        }

    }
}
