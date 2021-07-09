using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using LiveCharts.Wpf;
using System.Data.Entity.Validation;

namespace HomeBudget
{
    public partial class AddItem : MaterialForm
    {
        Form1 form1;
        public bool flagIncome = false;
        public bool flagExpense = false;

        public int categoryId = 0;
        public DateTime date;
        decimal sum;
        string description = "";

        public Dictionary<Button, string> tooltipCollectionEx;
        public Dictionary<Button, string> tooltipCollectionInc;

        Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
  
        public AddItem(Form1 obj)
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

            button4.Tag = 1;
            sport.Tag = 2;
            family.Tag = 3;
            bus.Tag = 4;
            giftExp.Tag = 5;
            education.Tag = 6;
            cafe.Tag = 7;
            house.Tag = 8;
            leisure.Tag = 9;
            health.Tag = 10;
            clothes.Tag = 11;
            cosmetics.Tag = 12;
            other.Tag = 13;

            salary.Tag = 1;
            interest.Tag = 2;
            giftInc.Tag = 3;
            otherInc.Tag = 4;

            tooltipCollectionEx = new Dictionary<Button, string>();
            tooltipCollectionEx.Add(button4, "Продукты");
            tooltipCollectionEx.Add(sport, "Спорт");
            tooltipCollectionEx.Add(house, "Дом");
            tooltipCollectionEx.Add(clothes, "Одежда и обувь");
            tooltipCollectionEx.Add(giftExp, "Подарки");
            tooltipCollectionEx.Add(cosmetics, "Косметика");
            tooltipCollectionEx.Add(health, "Здоровье");
            tooltipCollectionEx.Add(cafe, "Кафе");
            tooltipCollectionEx.Add(leisure, "Досуг");
            tooltipCollectionEx.Add(education, "Образование");
            tooltipCollectionEx.Add(other, "Другое");
            tooltipCollectionEx.Add(bus, "Транспорт");
            tooltipCollectionEx.Add(family, "Семья");

            tooltipCollectionInc = new Dictionary<Button, string>();
            tooltipCollectionInc.Add(salary, "Зарплата");
            tooltipCollectionInc.Add(interest, "Проценты по вкладу");
            tooltipCollectionInc.Add(giftInc, "Подарок");
            tooltipCollectionInc.Add(otherInc, "Другое");
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            flagExpense = true;
            flagIncome = false;
            panel2.Visible = true;
            panel1.Visible = false;

            button1.BackColor = Color.MediumPurple;
            button2.BackColor = Color.FromArgb(128, 255, 128);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            flagIncome = true;
            flagExpense = false;

            panel1.Visible = true;
            panel2.Visible = false;

            button2.BackColor = Color.MediumPurple;
            button1.BackColor = Color.FromArgb(255, 128, 128);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (categoryId != 0)
            {
                date = dateTimePicker1.Value.Date;
                sum = Convert.ToDecimal(textBox1.Text);
                description = textBox2.Text;

                form1.listView1.Items.Clear();
                form1.pieChart1.Series = null;

                if (form1.editClick == false)
                {
                    form1.checkedListBox1.Items.Clear();

                    if (flagExpense == true)
                    {
                        using (BudgetEntities db = new BudgetEntities())
                        {
                            var titles = db.CategoriesExpense;

                            foreach (var item in titles)
                            {
                                form1.checkedListBox1.Items.Add(item.Title);
                            }
                        }

                        using (BudgetEntities db = new BudgetEntities())
                        {
                            Expenses expenses = new Expenses
                            {
                                CategoryId = categoryId,
                                Date = date,
                                Sum = sum,
                                Description = description,
                                UserId = form1.IdUser
                            };

                            db.Expenses.Add(expenses);
                            db.SaveChanges();

                            var user = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            user.Cash -= sum;
                            db.SaveChanges();

                            var userUpdate = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            form1.textBox1.Text = userUpdate.Cash.ToString();
                        }

                        form1.monthClick = false;
                        form1.dayClick = true;
                        form1.dateTimePicker1.Visible = true;
                        form1.dateTimePickerYearMonth1.Visible = false;
                        form1.flagExpense = true;
                        form1.flagIncome = false;
                        form1.button4.BackColor = Color.MediumPurple;
                        form1.button3.BackColor = Color.FromArgb(128, 255, 128);
                        form1.label1.ForeColor = Color.MediumPurple;
                        form1.label2.ForeColor = Color.White;

                        if (form1.dateTimePicker1.Value != date)
                            form1.dateTimePicker1.Value = date;
                        else if (form1.dateTimePicker1.Value == date)
                        {
                            using (BudgetEntities db = new BudgetEntities())
                            {
                                var expenses = from ex in db.Expenses
                                               join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                               where ex.Date == date && ex.UserId == form1.IdUser
                                               select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                                var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, Sum = ex.Sum, Date = ex.Date, UserId = ex.UserId }).Where(d => (d.Date == date) && (d.UserId == form1.IdUser)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                                foreach (var item in expenses)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    form1.listView1.Items.Add(lvi);
                                }

                                SeriesCollection series = new SeriesCollection();

                                foreach (var item in expensesCategory)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    form1.pieChart1.Series = series;
                                }

                                for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                                {
                                    foreach (var item in expensesCategory)
                                    {
                                        if (form1.checkedListBox1.Items[i].ToString() == item.Category)
                                            form1.checkedListBox1.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }
                    else if (flagIncome == true)
                    {
                        using (BudgetEntities db = new BudgetEntities())
                        {
                            var titles = db.CategoriesIncome;

                            foreach (var item in titles)
                            {
                                form1.checkedListBox1.Items.Add(item.Title);
                            }
                        }

                        using (BudgetEntities db = new BudgetEntities())
                        {
                            Incomes incomes = new Incomes
                            {
                                CategoryId = categoryId,
                                Date = date,
                                Sum = sum,
                                Description = description,
                                UserId = form1.IdUser
                            };

                            db.Incomes.Add(incomes);
                            db.SaveChanges();

                            var user = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            user.Cash += sum;
                            db.SaveChanges();

                            var userUpdate = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            form1.textBox1.Text = userUpdate.Cash.ToString();
                        }

                        form1.monthClick = false;
                        form1.dayClick = true;
                        form1.dateTimePicker1.Visible = true;
                        form1.dateTimePickerYearMonth1.Visible = false;
                        form1.flagExpense = false;
                        form1.flagIncome = true;
                        form1.button3.BackColor = Color.MediumPurple;
                        form1.button4.BackColor = Color.FromArgb(255, 128, 128);
                        form1.label1.ForeColor = Color.MediumPurple;
                        form1.label2.ForeColor = Color.White;

                        if (form1.dateTimePicker1.Value != date)
                            form1.dateTimePicker1.Value = date;
                        else if (form1.dateTimePicker1.Value == date)
                        {
                            using (BudgetEntities db = new BudgetEntities())
                            {
                                var incomes = from inc in db.Incomes
                                              join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                              where inc.Date == date && inc.UserId == form1.IdUser
                                              select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                                var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, Sum = inc.Sum, Date = inc.Date, UserId = inc.UserId }).Where(d => (d.Date == date) && (d.UserId == form1.IdUser)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                                foreach (var item in incomes)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    form1.listView1.Items.Add(lvi);
                                }

                                SeriesCollection series = new SeriesCollection();

                                foreach (var item in incomesCategory)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    form1.pieChart1.Series = series;
                                }

                                for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                                {
                                    foreach (var item in incomesCategory)
                                    {
                                        if (form1.checkedListBox1.Items[i].ToString() == item.Category)
                                            form1.checkedListBox1.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }

                    textBox1.Text = null;
                    textBox2.Text = null;
                }
                else
                {
                    for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                    {
                        form1.checkedListBox1.SetItemChecked(i, false);
                    }

                    if (flagExpense == true)
                    {
                        using (BudgetEntities db = new BudgetEntities())
                        {
                            var expense = db.Expenses.Where(i => i.Id == form1.Id).FirstOrDefault();

                            var user = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            user.Cash += expense.Sum;

                            db.SaveChanges();

                            expense.Sum = sum;
                            expense.CategoryId = categoryId;
                            expense.Date = date;
                            expense.Description = description;

                            db.SaveChanges();

                            var userUpdate1 = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            user.Cash -= sum;

                            db.SaveChanges();

                            var userUpdate2 = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            form1.textBox1.Text = userUpdate2.Cash.ToString();
                        }

                        if (form1.dateTimePicker1.Visible == true && form1.dateTimePicker1.Value != date)
                            form1.dateTimePicker1.Value = date;
                        else if (form1.dateTimePicker1.Visible == true && form1.dateTimePicker1.Value == date)
                        {
                            using (BudgetEntities db = new BudgetEntities())
                            {
                                var expenses = from ex in db.Expenses
                                               join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                               where ex.Date == date && ex.UserId == form1.IdUser
                                               select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                                var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, Sum = ex.Sum, Date = ex.Date, UserId = ex.UserId }).Where(d => (d.Date == date) && (d.UserId == form1.IdUser)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                                foreach (var item in expenses)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    form1.listView1.Items.Add(lvi);
                                }

                                SeriesCollection series = new SeriesCollection();

                                foreach (var item in expensesCategory)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    form1.pieChart1.Series = series;
                                }

                                for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                                {
                                    foreach (var item in expensesCategory)
                                    {
                                        if (form1.checkedListBox1.Items[i].ToString() == item.Category)
                                            form1.checkedListBox1.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                        else if (form1.dateTimePickerYearMonth1.Visible == true && form1.dateTimePickerYearMonth1.Value != date)
                            form1.dateTimePickerYearMonth1.Value = date;
                        else if (form1.dateTimePickerYearMonth1.Visible == true && form1.dateTimePickerYearMonth1.Value == date)
                        {
                            int m = date.Month;
                            int y = date.Year;

                            using (BudgetEntities db = new BudgetEntities())
                            {
                                var expenses = from ex in db.Expenses
                                               join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                               where ex.Date.Month == m && ex.Date.Year == y && ex.UserId == form1.IdUser

                                               select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                                var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, Sum = ex.Sum, Date = ex.Date, UserId = ex.UserId }).Where(d => (d.Date.Month == m) && (d.Date.Year == y) && (d.UserId == form1.IdUser)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                                foreach (var item in expenses)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    form1.listView1.Items.Add(lvi);
                                }

                                SeriesCollection series = new SeriesCollection();

                                foreach (var item in expensesCategory)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    form1.pieChart1.Series = series;

                                }

                                for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                                {
                                    foreach (var item in expensesCategory)
                                    {
                                        if (form1.checkedListBox1.Items[i].ToString() == item.Category)
                                            form1.checkedListBox1.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }
                    else if (flagIncome == true)
                    {
                        using (BudgetEntities db = new BudgetEntities())
                        {
                            var income = db.Incomes.Where(i => i.Id == form1.Id).FirstOrDefault();

                            var user = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            user.Cash -= income.Sum;

                            db.SaveChanges();

                            income.Sum = sum;
                            income.CategoryId = categoryId;
                            income.Date = date;
                            income.Description = description;

                            db.SaveChanges();

                            var userUpdate1 = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            user.Cash += sum;

                            db.SaveChanges();

                            var userUpdate2 = db.Users.Where(i => i.Id == form1.IdUser).FirstOrDefault();
                            form1.textBox1.Text = userUpdate2.Cash.ToString();
                        }

                        if (form1.dateTimePicker1.Visible == true && form1.dateTimePicker1.Value != date)
                            form1.dateTimePicker1.Value = date;
                        else if (form1.dateTimePicker1.Visible == true && form1.dateTimePicker1.Value == date)
                        {
                            using (BudgetEntities db = new BudgetEntities())
                            {
                                var incomes = from inc in db.Incomes
                                              join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                              where inc.Date == date && inc.UserId == form1.IdUser
                                              select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                                var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, Sum = inc.Sum, Date = inc.Date, UserId = inc.UserId }).Where(d => (d.Date == date) && (d.UserId == form1.IdUser)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                                foreach (var item in incomes)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    form1.listView1.Items.Add(lvi);
                                }

                                SeriesCollection series = new SeriesCollection();

                                foreach (var item in incomesCategory)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    form1.pieChart1.Series = series;
                                }

                                for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                                {
                                    foreach (var item in incomesCategory)
                                    {
                                        if (form1.checkedListBox1.Items[i].ToString() == item.Category)
                                            form1.checkedListBox1.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                        else if (form1.dateTimePickerYearMonth1.Visible == true && form1.dateTimePickerYearMonth1.Value != date)
                            form1.dateTimePickerYearMonth1.Value = date;
                        else if (form1.dateTimePickerYearMonth1.Visible == true && form1.dateTimePickerYearMonth1.Value == date)
                        {
                            int m = date.Month;
                            int y = date.Year;

                            using (BudgetEntities db = new BudgetEntities())
                            {
                                var incomes = from inc in db.Incomes
                                              join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                              where inc.Date.Month == m && inc.Date.Year == y && inc.UserId == form1.IdUser

                                              select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                                var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, Sum = inc.Sum, Date = inc.Date, UserId = inc.UserId }).Where(d => (d.Date.Month == m) && (d.Date.Year == y) && (d.UserId == form1.IdUser)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                                foreach (var item in incomes)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    form1.listView1.Items.Add(lvi);
                                }

                                SeriesCollection series = new SeriesCollection();

                                foreach (var item in incomesCategory)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    form1.pieChart1.Series = series;
                                }

                                for (int i = 0; i < form1.checkedListBox1.Items.Count; i++)
                                {
                                    foreach (var item in incomesCategory)
                                    {
                                        if (form1.checkedListBox1.Items[i].ToString() == item.Category)
                                            form1.checkedListBox1.SetItemChecked(i, true);
                                    }
                                }
                            }
                        }
                    }
                    form1.editClick = false;
                    this.Close();
                }
            }
            else
                MessageBox.Show("Выберите категорию!");

            categoryId = 0;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button4, "Продукты");        
        }

        private void sport_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(sport, "Спорт");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(button4.Tag);
           
            button4.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            clothes.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }


        private void clothes_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(clothes.Tag);

            clothes.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void cafe_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(cafe.Tag);

            cafe.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            button4.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            clothes.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void house_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(house, "Дом");
        }

        private void leisure_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(leisure, "Досуг");
        }

        private void family_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(family, "Семья");
        }

        private void education_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(education, "Образование");
        }

        private void cosmetics_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cosmetics, "Косметика");
        }

        private void health_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(health, "Здоровье");
        }

        private void bus_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(bus, "Транспорт");
        }

        private void other_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(other, "Другое");
        }

        private void giftExp_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(giftExp, "Подарки");
        }

        private void clothes_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(clothes, "Одежда и обувь");
        }

        private void cafe_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cafe, "Кафе");
        }

        private void sport_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(sport.Tag);

            sport.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            clothes.BackColor = Color.White;
            button4.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void giftExp_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(giftExp.Tag);

            giftExp.BackColor = Color.LightBlue;
            button4.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            clothes.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void house_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(house.Tag);

            house.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            clothes.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            button4.BackColor = Color.White;
        }

        private void leisure_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(leisure.Tag);

            leisure.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            clothes.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void family_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(family.Tag);

            family.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            clothes.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void education_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(education.Tag);

            education.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            clothes.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void cosmetics_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(cosmetics.Tag);

            cosmetics.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            health.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            clothes.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void health_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(health.Tag);

            health.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            clothes.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void bus_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(bus.Tag);

            bus.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            other.BackColor = Color.White;
            clothes.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            health.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void other_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(other.Tag);

            other.BackColor = Color.LightBlue;
            giftExp.BackColor = Color.White;
            education.BackColor = Color.White;
            family.BackColor = Color.White;
            leisure.BackColor = Color.White;
            cafe.BackColor = Color.White;
            clothes.BackColor = Color.White;
            health.BackColor = Color.White;
            button4.BackColor = Color.White;
            sport.BackColor = Color.White;
            bus.BackColor = Color.White;
            cosmetics.BackColor = Color.White;
            house.BackColor = Color.White;
        }

        private void salary_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(salary, "Зарплата");
        }

        private void interest_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(interest, "Проценты по вкладу");
        }

        private void giftInc_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(giftInc, "Подарок");
        }

        private void otherInc_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(otherInc, "Другое");
        }

        private void salary_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(salary.Tag);

            salary.BackColor = Color.LightBlue;
            interest.BackColor = Color.White;
            giftInc.BackColor = Color.White;
            otherInc.BackColor = Color.White;
        }

        private void interest_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(interest.Tag);

            interest.BackColor = Color.LightBlue;
            salary.BackColor = Color.White;
            giftInc.BackColor = Color.White;
            otherInc.BackColor = Color.White;
        }

        private void giftInc_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(giftInc.Tag);

            giftInc.BackColor = Color.LightBlue;
            interest.BackColor = Color.White;
            salary.BackColor = Color.White;
            otherInc.BackColor = Color.White;
        }

        private void otherInc_Click(object sender, EventArgs e)
        {
            categoryId = Convert.ToInt32(otherInc.Tag);

            otherInc.BackColor = Color.LightBlue;
            interest.BackColor = Color.White;
            giftInc.BackColor = Color.White;
            salary.BackColor = Color.White;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if (flagExpense == true)
                button1.BackColor = Color.MediumPurple;
            else
                button1.BackColor = Color.FromArgb(255, 128, 128);
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = Color.Lime;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            if (flagIncome == true)
                button2.BackColor = Color.MediumPurple;
            else
                button2.BackColor = Color.FromArgb(128, 255, 128);
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            if(form1.editClick == false)
            {
                panel1.Visible = false;
                flagExpense = true;
                button1.BackColor = Color.MediumPurple;
            }      
        }

        private void AddItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            form1.button2.BackColor = Color.DeepSkyBlue;
        }
    }
}
