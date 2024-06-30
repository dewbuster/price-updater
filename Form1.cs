using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace PriceUpdater
{
    public partial class Form1 : Form
    {
        private string defaultConnectionString = "Initial Catalog=bakok;Integrated Security=True";
        private string connectionString;

        public Form1()
        {
            if (Directory.Exists("C:/home"))
            {
                connectionString = "Data Source=DESKTOP-PN9M8RU\\SQLEXPRESS;" + defaultConnectionString;
            }
            else
            {
                connectionString = "Data Source=DESKTOP-58Q9HMK\\SQLEXPRESS;" + defaultConnectionString;
            }

            InitializeComponent();

            InitializeForm();
        }

        private void InitializeForm()
        {
            // 콤보박스 초기화
            LoadComboBox();

            // 버튼 클릭 이벤트 핸들러 등록
            btnUpdatePrice.Click += btnUpdatePrice_Click;
            cmbProducts.SelectedIndexChanged += CmbProducts_SelectedIndexChanged;
        }

        private void LoadComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 콤보박스에 데이터 로드
                    string query = "SELECT DISTINCT product FROM productinfo";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        cmbProducts.Items.Add(reader["product"].ToString());
                    }

                    reader.Close();
                }
                cmbProducts.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void CmbProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 선택된 콤보박스 항목에 대한 가격 불러오기
                    string selectedProduct = cmbProducts.SelectedItem.ToString();
                    string query = $"SELECT price FROM productinfo WHERE product = '{selectedProduct}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    object result = command.ExecuteScalar();

                    // 결과가 null이 아니면 레이블에 표시
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        lblPrice.Text = "가격: " + FormatPrice(price);
                    }
                    else
                    {
                        lblPrice.Text = "Price not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving price: " + ex.Message);
            }
        }

        private string FormatPrice(decimal price)
        {
            // N0 형식 지정자를 사용하여 3자리마다 쉼표를 찍습니다.
            return string.Format("{0:N0}", price);
        }

        private void btnUpdatePrice_Click(object sender, EventArgs e)
        {
            if (txtNewPrice.Text == string.Empty)
                return;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 선택된 콤보박스 항목과 텍스트박스 값으로 가격 업데이트
                    string selectedProduct = cmbProducts.SelectedItem.ToString();
                    decimal newPrice = Convert.ToDecimal(txtNewPrice.Text);

                    string query = $"UPDATE productinfo SET price = {newPrice} WHERE product = '{selectedProduct}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"가격 변경 완료\n{selectedProduct} {newPrice}");
                        // 업데이트 후 다시 콤보박스 항목 로드
                        cmbProducts.Items.Clear();
                        txtNewPrice.Text = string.Empty;
                        LoadComboBox();
                    }
                    else
                    {
                        MessageBox.Show("No rows affected. Update failed.");
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}
