using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;


namespace exam
{
    public partial class LoginForm : Form
    {
        private int loginAttempts = 0;
        private string actualCaptcha;

        public LoginForm()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            captchaRichTextBox.ReadOnly = true;
            captchaRichTextBox.ShortcutsEnabled = false;
            Random random = new Random();
            actualCaptcha = "";

            for (int i = 0; i < 5; i++)
            {
                int value = random.Next(65, 90); // Генерация случайного символа (A-Z)
                char character = (char)value;

                actualCaptcha += character;
            }

            captchaRichTextBox.Text = actualCaptcha;
        }





        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;
            bool isCaptchaValid = ValidateCaptcha(captchaTextBox.Text); // Проверка капчи

            if (loginAttempts < 3 && isCaptchaValid)
            {
                string userRole = AuthenticateUser(username, password);
                if (userRole != null)
                {
                    // Успешная авторизация
                    MessageBox.Show($"Вы вошли под ролью: {userRole}");
                    OpenMainForm(username);
                }
                else
                {
                    // Неуспешная авторизация
                    MessageBox.Show("Неудачная попытка входа.");
                    loginAttempts++;
                    GenerateCaptcha(); // Генерация новой CAPTCHA при неудачной попытке
                }
            }
            else if (loginAttempts >= 3)
            {
                // Блокировка входа на 10 секунд после трех неудачных попыток
                MessageBox.Show("Превышено количество попыток входа. Попробуйте позже.");
                loginAttempts = 0; // Сброс попыток после блокировки
                LoginButton.Enabled = false; // Блокировка кнопки входа
                Task.Delay(10000).ContinueWith(_ =>
                {
                    LoginButton.Enabled = true; // Включение кнопки после задержки
                });
            }
            else
            {
                // Неуспешная авторизация
                MessageBox.Show("Неудачная попытка входа.");
                loginAttempts++;
                GenerateCaptcha(); // Генерация новой CAPTCHA при неудачной попытке
            }
        }



        private bool ValidateCaptcha(string userInput)
        {
            return userInput.Equals(actualCaptcha, StringComparison.OrdinalIgnoreCase);
        }

        private string AuthenticateUser(string username, string password)
        {
            string connectionString = "Data Source=ADCLG1; Initial Catalog=!Trade; Integrated Security=true;";
            string query = "SELECT UserRole FROM [User] WHERE UserLogin = @username AND UserPassword = @password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                connection.Open();
                var role = command.ExecuteScalar();
                return (role != null) ? role.ToString() : null;
            }
        }


        private void OpenMainForm(string username)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void Enter_as_guest_button_Click(object sender, EventArgs e)
        {
            string guestRole = "Гость";
            MessageBox.Show($"Вы вошли как {guestRole}");
            OpenMainForm(guestRole);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}