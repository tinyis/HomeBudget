using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace HomeBudget
{
    public partial class Registration : MaterialForm
    {
        Log log;
        Form1 form1;

        string name;
        string login;
        string password;
        decimal cash;
        string picturePath = null;
        string pictureName = null;
        bool flagReg = false;
        bool flagEdit = false;

        public Registration(Log obj)
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

            log = obj;

            button2.BackColor = Color.FromArgb(27, 94, 32);
            button1.BackColor = Color.FromArgb(67, 160, 71);

            flowLayoutPanel1.BackColor = Color.FromArgb(51, 51, 51);

            flagReg = true;
            flagEdit = false;
        }

        public Registration(Form1 obj)
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

            form1 = obj;

            this.Text = "Редактирование";
            label3.Text = "Новый пароль";
            button2.Text = "Сохранить";

            button2.BackColor = Color.FromArgb(27, 94, 32);
            button1.BackColor = Color.FromArgb(67, 160, 71);

            flagReg = false;
            flagEdit = true;

            using (BudgetEntities db = new BudgetEntities())
            {
                var user = db.Users.Where(i => i.Id == form1.IdUser);

                foreach (var item in user)
                {
                    textBox1.Text = item.Name;
                    textBox2.Text = item.Login;
                    textBox4.Text = item.Cash.ToString();
                    label5.Text = item.PictureName;
                }
            }

            textBox3.Text = "";

            flowLayoutPanel1.BackColor = Color.FromArgb(51, 51, 51);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Grafics File|*.bmp;*.gif;*.jpg;*.png";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                picturePath = ofd.FileName;
            }

            FileInfo fi = new FileInfo(picturePath);
            pictureName = fi.Name;
            label5.Text = pictureName;
        }

        private void LoadUser()
        {
            byte[] bytes;
            bytes = CreateCopy();

            name = textBox1.Text;
            login = textBox2.Text;
            password = textBox3.Text;
            cash = Convert.ToDecimal(textBox4.Text);

            using (BudgetEntities db = new BudgetEntities())
            {
                Users user = new Users
                {
                    Login = login,
                    Password = password,
                    Name = name,
                    Picture = bytes,
                    Cash = cash,
                    PictureName = pictureName
                };

                db.Users.Add(user);
                db.SaveChanges();

                //    try
                //    {

                //        db.SaveChanges();

                //    }
                //    catch (DbEntityValidationException ex)
                //    {
                //        foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                //        {
                //            MessageBox.Show("Object: " + validationError.Entry.Entity.ToString());

                //            foreach (DbValidationError err in validationError.ValidationErrors)
                //            {
                //                MessageBox.Show(err.ErrorMessage + "");
                //            }
                //        }
                //    }
            }
        }
        
        //в этом методе анализируется ориентация картинки и создается копия этой картинки
        //таким образом, чтобы максимальный размер картинки(высота и ширина) не превышал 300 пикселей
        //Пропорции картинки при этом не искажаются

        private byte[] CreateCopy()
        {
            if (picturePath == null)
                picturePath = "C:\\Users\\User\\Downloads\\Kursach\\bin\\Debug\\user.png";
            Image img = Image.FromFile(picturePath);
            int maxWidth = 300, maxHeight = 300;
            double ratioX = (double)maxWidth / img.Width;
            double ratioY = (double)maxHeight / img.Height;
            double ratio = Math.Min(ratioX, ratioY);
            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);
            Image mi = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(mi);
            g.DrawImage(img, 0, 0, newWidth, newHeight);

            MemoryStream ms = new MemoryStream();
            //поток для ввода/вывода байт из памяти
            mi.Save(ms, ImageFormat.Jpeg);
            ms.Flush();//выносим в поток все данные из буфера
            ms.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(ms);
            byte[] buf = br.ReadBytes((int)ms.Length);
            return buf;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(flagReg == true)
            {
                LoadUser();
                MessageBox.Show("Регистрация прошла успешно!");
                this.Close();
            }
            else if(flagEdit==true)
            {
                byte[] bytes;
                bytes = CreateCopy();

                using (BudgetEntities db = new BudgetEntities())
                {
                    var user = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();

                    user.Name = textBox1.Text;
                    user.Login = textBox2.Text;
                    user.Cash = Convert.ToDecimal(textBox4.Text);
                    if (textBox3.Text != "")
                        user.Password = textBox3.Text;
                    if(label5.Text!=user.PictureName)
                    {
                        user.Picture = bytes;
                        user.PictureName = pictureName;
                        MemoryStream ms = new MemoryStream(bytes);
                        form1.pictureBox1.Image = Image.FromStream(ms);
                    }

                    db.SaveChanges();
                }

                form1.label5.Text = textBox1.Text;
                form1.textBox1.Text = textBox4.Text;               

                MessageBox.Show("Успешно изменено!");
                this.Close();
            }   
        }
    }
}
