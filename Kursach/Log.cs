using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace HomeBudget
{
    public partial class Log : MaterialForm
    {

        Registration registration;
        string login;
        string password;
        public int userId;
        Form1 form1 = null;
        bool checkUser = false;

        public Log()
        { 
            InitializeComponent();

            // Create a material theme manager and add the form to manage (this)
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Green600, Primary.Green900,
                Primary.Green600, Accent.LightBlue200,
                TextShade.WHITE);

            label1.BackColor = Color.FromArgb(138, 89, 59);
            label2.BackColor = Color.FromArgb(138, 89, 59);
            label3.BackColor = Color.FromArgb(138, 89, 59);
            label1.ForeColor = Color.FromArgb(255, 232, 203);
            label2.ForeColor = Color.FromArgb(255, 232, 203);
            button1.BackColor = Color.FromArgb(3, 166, 121);
            button1.ForeColor = Color.FromArgb(254, 203, 70);
            label3.ForeColor = Color.FromArgb(101, 190, 166);
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.ForeColor = Color.FromArgb(3, 166, 121);
            label3.Font = new Font(label3.Font, FontStyle.Underline);
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = Color.FromArgb(101, 190, 166);
            label3.Font = new Font(label3.Font, FontStyle.Regular);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            registration = new Registration(this);
            registration.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkUser = false;

            login = textBox1.Text;
            password = textBox2.Text;

            using(BudgetEntities db = new BudgetEntities())
            {
                var users = db.Users;

                foreach(var item in users)
                {
                    if ((login == item.Login) && (password == item.Password))
                    {
                        checkUser = true;
                        form1 = new Form1(this);

                        MemoryStream ms = new MemoryStream(item.Picture);
                        form1.pictureBox1.Image = Image.FromStream(ms);
                        form1.label5.Text = item.Name;
                        form1.textBox1.Text = item.Cash.ToString();
                        userId = item.Id;

                        form1.Show();
                    }                     
                }
            }

            if(checkUser==false)
                MessageBox.Show("Неверный логин или пароль!");
        }
    }
}
