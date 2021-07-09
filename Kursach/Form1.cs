using LiveCharts;
using LiveCharts.Wpf;
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

namespace HomeBudget
{
    public partial class Form1 : MaterialForm
    {
        AddItem addItem = null;
        Charts charts = null;
        Log log = null;
        public bool flagIncome = false;
        public bool flagExpense = false;
        public bool dayClick = false;
        public bool monthClick = false;
        public bool editClick = false;
        public int Id = 0;
        DateTime date;

        public int IdUser;

        Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

        public Form1(Log obj)
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

            listView1.View = View.Details;
            listView1.Columns.Add("Id");
            listView1.Columns.Add("Категория");
            listView1.Columns.Add("Сумма");
            listView1.Columns.Add("Описание");
            listView1.Columns.Add("Дата");

            listView1.Columns[0].Width = 0;

            listView1.Columns[1].Width = 110;
            listView1.Columns[2].Width = 110;
            listView1.Columns[3].Width = 110;
            listView1.Columns[4].Width = 110;

            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            checkedListBox1.ColumnWidth = 190;

            panel1.BackColor = Color.FromArgb(27, 94, 32);
            checkedListBox1.BackColor = Color.FromArgb(51, 51, 51);
            pieChart1.BackColor = Color.FromArgb(51, 51, 51);
            label1.BackColor = Color.FromArgb(51, 51, 51);
            label2.BackColor = Color.FromArgb(51, 51, 51);
            label3.BackColor = Color.FromArgb(51, 51, 51);
            label4.BackColor = Color.FromArgb(51, 51, 51);
            listView1.BackColor = Color.FromArgb(88, 88, 88);

            this.TransparencyKey = Color.Empty;

            log = obj;
        }

        private void dateTimePickerYearMonth1_ValueChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            pieChart1.Series = null;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            int m = dateTimePickerYearMonth1.Value.Month;
            int y = dateTimePickerYearMonth1.Value.Year;

            if (flagExpense == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expenses = from ex in db.Expenses
                                   join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                   where ex.UserId == IdUser && ex.Date.Month == m && ex.Date.Year == y
                                   select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                    var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in expenses)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in expensesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;

                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in expensesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
            else if (flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomes = from inc in db.Incomes
                                  join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                  where inc.UserId == IdUser && inc.Date.Month == m && inc.Date.Year == y
                                  select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                    var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in incomes)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in incomesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in incomesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            pieChart1.Series = null;

            if (flagExpense == true)
            {
                if (dateTimePicker1.Visible == true)
                {
                    date = dateTimePicker1.Value.Date;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var expenses = from ex in db.Expenses
                                       join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                       where ex.UserId == IdUser && ex.Date == date
                                       select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                        var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in expenses)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    listView1.Items.Add(lvi);
                                }
                            }
                        }

                        SeriesCollection series = new SeriesCollection();

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in expensesCategory)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    pieChart1.Series = series;
                                }
                            }
                        }
                    }
                }
                else if (dateTimePickerYearMonth1.Visible == true)
                {
                    int m = dateTimePickerYearMonth1.Value.Month;
                    int y = dateTimePickerYearMonth1.Value.Year;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var expenses = from ex in db.Expenses
                                       join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                       where ex.UserId == IdUser && ex.Date.Month == m && ex.Date.Year == y
                                       select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                        var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in expenses)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    listView1.Items.Add(lvi);
                                }
                            }
                        }

                        SeriesCollection series = new SeriesCollection();

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in expensesCategory)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    pieChart1.Series = series;
                                }
                            }
                        }
                    }
                }
            }
            else if (flagIncome == true)
            {
                if (dateTimePicker1.Visible == true)
                {
                    date = dateTimePicker1.Value.Date;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var incomes = from inc in db.Incomes
                                      join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                      where inc.UserId == IdUser && inc.Date == date
                                      select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                        var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in incomes)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    listView1.Items.Add(lvi);
                                }
                            }
                        }

                        SeriesCollection series = new SeriesCollection();

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in incomesCategory)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    pieChart1.Series = series;
                                }
                            }
                        }
                    }
                }
                else if (dateTimePickerYearMonth1.Visible == true)
                {
                    int m = dateTimePickerYearMonth1.Value.Month;
                    int y = dateTimePickerYearMonth1.Value.Year;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var incomes = from inc in db.Incomes
                                      join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                      where inc.UserId == IdUser && inc.Date.Month == m && inc.Date.Year == y
                                      select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                        var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in incomes)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                                    lvi.SubItems.Add(item.Category);
                                    lvi.SubItems.Add(item.Sum.ToString());
                                    lvi.SubItems.Add(item.Description.ToString());
                                    lvi.SubItems.Add(item.Date.ToShortDateString());

                                    listView1.Items.Add(lvi);
                                }
                            }
                        }

                        SeriesCollection series = new SeriesCollection();

                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            foreach (var item in incomesCategory)
                            {
                                if (checkedListBox1.CheckedItems[i].ToString() == item.Category)
                                {
                                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                                    pieChart1.Series = series;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.Font = new Font(label1.Font, FontStyle.Underline);
            label1.ForeColor = Color.FromArgb(67, 160, 71);
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.Font = new Font(label1.Font, FontStyle.Regular);
            if (dayClick == false)
                label1.ForeColor = Color.White;
            else
                label1.ForeColor = Color.MediumPurple;
        }

        private void label2_MouseEnter(object sender, EventArgs e)
        {
            label2.Font = new Font(label2.Font, FontStyle.Underline);
            label2.ForeColor = Color.FromArgb(67, 160, 71);
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            label2.Font = new Font(label2.Font, FontStyle.Regular);
            if (monthClick == false)
                label2.ForeColor = Color.White;
            else
                label2.ForeColor = Color.MediumPurple;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            monthClick = false;
            dayClick = true;
            dateTimePicker1.Visible = true;
            dateTimePickerYearMonth1.Visible = false;
            label1.ForeColor = Color.MediumPurple;
            label2.ForeColor = Color.White;

            date = dateTimePicker1.Value.Date;

            listView1.Items.Clear();
            pieChart1.Series = null;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            if (flagExpense == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expenses = from ex in db.Expenses
                                   join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                   where ex.UserId == IdUser && ex.Date == date
                                   select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                    var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in expenses)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in expensesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in expensesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
            else if (flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomes = from inc in db.Incomes
                                  join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                  where inc.UserId == IdUser && inc.Date == date
                                  select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                    var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in incomes)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in incomesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in incomesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            monthClick = true;
            dayClick = false;
            dateTimePicker1.Visible = false;
            dateTimePickerYearMonth1.Visible = true;
            label2.ForeColor = Color.MediumPurple;
            label1.ForeColor = Color.White;

            int m = dateTimePickerYearMonth1.Value.Month;
            int y = dateTimePickerYearMonth1.Value.Year;

            listView1.Items.Clear();
            pieChart1.Series = null;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            if (flagExpense == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expenses = from ex in db.Expenses
                                   join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                   where ex.UserId == IdUser && ex.Date.Month == m && ex.Date.Year == y
                                   select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                    var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d => (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in expenses)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }
                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in expensesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;

                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in expensesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
            else if (flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomes = from inc in db.Incomes
                                  join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                  where inc.UserId == IdUser && inc.Date.Month == m && inc.Date.Year == y
                                  select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                    var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in incomes)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in incomesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;

                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in incomesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IdUser = log.userId;

            dayClick = true;
            label1.ForeColor = Color.MediumPurple;
            button4.BackColor = Color.MediumPurple;
            flagExpense = true;

            using (BudgetEntities db = new BudgetEntities())
            {
                var titles = db.CategoriesExpense;

                foreach (var item in titles)
                {
                    checkedListBox1.Items.Add(item.Title);
                }
            }

            date = dateTimePicker1.Value.Date;

            listView1.Items.Clear();
            pieChart1.Series = null;

            using (BudgetEntities db = new BudgetEntities())
            {
                var expenses = from ex in db.Expenses
                               join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                               where ex.UserId == IdUser && ex.Date == date
                               select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                foreach (var item in expenses)
                {
                    ListViewItem lvi = new ListViewItem(item.Id.ToString());

                    lvi.SubItems.Add(item.Category);
                    lvi.SubItems.Add(item.Sum.ToString());
                    lvi.SubItems.Add(item.Description.ToString());
                    lvi.SubItems.Add(item.Date.ToShortDateString());

                    listView1.Items.Add(lvi);
                }

                SeriesCollection series = new SeriesCollection();

                foreach (var item in expensesCategory)
                {
                    series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                    pieChart1.Series = series;
                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    foreach (var item in expensesCategory)
                    {
                        if (checkedListBox1.Items[i].ToString() == item.Category)
                            checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            date = dateTimePicker1.Value.Date;

            listView1.Items.Clear();
            pieChart1.Series = null;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            if (flagExpense == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expenses = from ex in db.Expenses
                                   join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                   where ex.UserId == IdUser && ex.Date == date
                                   select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                    var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in expenses)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in expensesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in expensesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
            else if (flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomes = from inc in db.Incomes
                                  join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                  where inc.UserId == IdUser && inc.Date == date
                                  select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                    var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in incomes)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in incomesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in incomesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addItem = new AddItem(this);
            addItem.Show();

            button2.BackColor = Color.MediumPurple;
            button3.BackColor = Color.FromArgb(128, 255, 128);
            button4.BackColor = Color.FromArgb(255, 128, 128);
            button5.BackColor = Color.SandyBrown;
            button1.BackColor = Color.FromArgb(255,255,128);
            button1.BackColor = Color.FromArgb(255, 255, 128);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flagIncome = true;
            flagExpense = false;

            button3.BackColor = Color.MediumPurple;
            button4.BackColor = Color.FromArgb(255, 128, 128);
            button5.BackColor = Color.SandyBrown;
            button2.BackColor = Color.DeepSkyBlue;

            listView1.Items.Clear();
            pieChart1.Series = null;
            checkedListBox1.Items.Clear();

            using (BudgetEntities db = new BudgetEntities())
            {
                var titles = db.CategoriesIncome;

                foreach (var item in titles)
                {
                    checkedListBox1.Items.Add(item.Title);
                }
            }

            if (dateTimePicker1.Visible == true)
            {
                date = dateTimePicker1.Value.Date;

                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomes = from inc in db.Incomes
                                  join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                  where inc.UserId == IdUser && inc.Date == date
                                  select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                    var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in incomes)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in incomesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in incomesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
            else if (dateTimePickerYearMonth1.Visible == true)
            {
                int m = dateTimePickerYearMonth1.Value.Month;
                int y = dateTimePickerYearMonth1.Value.Year;

                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomes = from inc in db.Incomes
                                  join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                  where inc.UserId == IdUser && inc.Date.Month == m && inc.Date.Year == y
                                  select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                    var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in incomes)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in incomesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in incomesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            flagExpense = true;
            flagIncome = true;

            button4.BackColor = Color.MediumPurple;
            button3.BackColor = Color.FromArgb(128, 255, 128);
            button5.BackColor = Color.SandyBrown;
            button2.BackColor = Color.DeepSkyBlue;
            button1.BackColor = Color.FromArgb(255, 255, 128);

            listView1.Items.Clear();
            pieChart1.Series = null;
            checkedListBox1.Items.Clear();

            using (BudgetEntities db = new BudgetEntities())
            {
                var titles = db.CategoriesExpense;

                foreach (var item in titles)
                {
                    checkedListBox1.Items.Add(item.Title);
                }
            }

            if (dateTimePicker1.Visible == true)
            {
                date = dateTimePicker1.Value.Date;

                using (BudgetEntities db = new BudgetEntities())
                {
                    var expenses = from ex in db.Expenses
                                   join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                   where ex.UserId == IdUser && ex.Date == date
                                   select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                    var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in expenses)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in expensesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in expensesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
            else if (dateTimePickerYearMonth1.Visible == true)
            {
                int m = dateTimePickerYearMonth1.Value.Month;
                int y = dateTimePickerYearMonth1.Value.Year;

                using (BudgetEntities db = new BudgetEntities())
                {
                    var expenses = from ex in db.Expenses
                                   join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                   where ex.UserId == IdUser && ex.Date.Month == m && ex.Date.Year == y
                                   select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                    var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                    foreach (var item in expenses)
                    {
                        ListViewItem lvi = new ListViewItem(item.Id.ToString());

                        lvi.SubItems.Add(item.Category);
                        lvi.SubItems.Add(item.Sum.ToString());
                        lvi.SubItems.Add(item.Description.ToString());
                        lvi.SubItems.Add(item.Date.ToShortDateString());

                        listView1.Items.Add(lvi);
                    }

                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in expensesCategory)
                    {
                        series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                        pieChart1.Series = series;

                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        foreach (var item in expensesCategory)
                        {
                            if (checkedListBox1.Items[i].ToString() == item.Category)
                                checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.BackColor = Color.MediumPurple;
            button3.BackColor = Color.FromArgb(128, 255, 128);
            button4.BackColor = Color.FromArgb(255, 128, 128);
            button2.BackColor = Color.DeepSkyBlue;
            button1.BackColor = Color.FromArgb(255, 255, 128);

            charts = new Charts(this);
            charts.Show();
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button5, "Статистика");
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editClick = true;

            addItem = new AddItem(this);
            addItem.button3.Text = "Изменить";

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                Id = Convert.ToInt32(item.SubItems[0].Text);
                addItem.textBox1.Text = item.SubItems[2].Text;
                addItem.textBox2.Text = item.SubItems[3].Text;
                addItem.dateTimePicker1.Value = Convert.ToDateTime(item.SubItems[4].Text);
                addItem.date = Convert.ToDateTime(item.SubItems[4].Text);

                if (flagExpense == true)
                {
                    addItem.flagExpense = true;
                    addItem.button1.BackColor = Color.MediumPurple;
                    addItem.button2.Enabled = false;
                    addItem.panel2.Visible = true;
                    addItem.panel1.Visible = false;

                    foreach (KeyValuePair<Button, string> pair in addItem.tooltipCollectionEx)
                    {
                        if (item.SubItems[1].Text == pair.Value)
                        {
                            pair.Key.BackColor = Color.LightBlue;
                            addItem.categoryId = Convert.ToInt32(pair.Key.Tag);
                        }

                    }
                }
                else if (flagIncome == true)
                {
                    addItem.flagIncome = true;
                    addItem.button2.BackColor = Color.MediumPurple;
                    addItem.button1.Enabled = false;
                    addItem.panel1.Visible = true;
                    addItem.panel2.Visible = false;

                    foreach (KeyValuePair<Button, string> pair in addItem.tooltipCollectionInc)
                    {
                        if (item.SubItems[1].Text == pair.Value)
                        {
                            pair.Key.BackColor = Color.LightBlue;
                            addItem.categoryId = Convert.ToInt32(pair.Key.Tag);
                        }
                    }
                }
            }

            addItem.Show();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                Id = Convert.ToInt32(item.SubItems[0].Text);
            }

            listView1.Items.Clear();
            pieChart1.Series = null;
            checkedListBox1.Items.Clear();

            if (flagExpense == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var titles = db.CategoriesExpense;

                    foreach (var item in titles)
                    {
                        checkedListBox1.Items.Add(item.Title);
                    }
                }

                using (BudgetEntities db = new BudgetEntities())
                {
                    var expense = db.Expenses.Where(i => i.Id == Id).FirstOrDefault();

                    db.Expenses.Remove(expense);
                    db.SaveChanges();

                    var user = db.Users.Where(i => i.Id == IdUser).FirstOrDefault();
                    user.Cash += expense.Sum;
                    db.SaveChanges();

                    var userUpdate = db.Users.Where(i => i.Id == IdUser).FirstOrDefault();
                    textBox1.Text = userUpdate.Cash.ToString();
                }
                if (dateTimePicker1.Visible == true)
                {
                    date = dateTimePicker1.Value.Date;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var expenses = from ex in db.Expenses
                                       join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                       where ex.UserId == IdUser && ex.Date == date
                                       select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                        var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        foreach (var item in expenses)
                        {
                            ListViewItem lvi = new ListViewItem(item.Id.ToString());

                            lvi.SubItems.Add(item.Category);
                            lvi.SubItems.Add(item.Sum.ToString());
                            lvi.SubItems.Add(item.Description.ToString());
                            lvi.SubItems.Add(item.Date.ToShortDateString());

                            listView1.Items.Add(lvi);
                        }

                        SeriesCollection series = new SeriesCollection();

                        foreach (var item in expensesCategory)
                        {
                            series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                            pieChart1.Series = series;
                        }

                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            foreach (var item in expensesCategory)
                            {
                                if (checkedListBox1.Items[i].ToString() == item.Category)
                                    checkedListBox1.SetItemChecked(i, true);
                            }
                        }
                    }
                }
                else if (dateTimePickerYearMonth1.Visible == true)
                {
                    int m = dateTimePickerYearMonth1.Value.Month;
                    int y = dateTimePickerYearMonth1.Value.Year;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var expenses = from ex in db.Expenses
                                       join c in db.CategoriesExpense on ex.CategoryId equals c.Id
                                       where ex.UserId == IdUser && ex.Date.Month == m && ex.Date.Year == y
                                       select new { Id = ex.Id, Category = c.Title, Sum = ex.Sum, Date = ex.Date, Description = ex.Description };

                        var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, UserId = ex.UserId, Sum = ex.Sum, Date = ex.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        foreach (var item in expenses)
                        {
                            ListViewItem lvi = new ListViewItem(item.Id.ToString());

                            lvi.SubItems.Add(item.Category);
                            lvi.SubItems.Add(item.Sum.ToString());
                            lvi.SubItems.Add(item.Description.ToString());
                            lvi.SubItems.Add(item.Date.ToShortDateString());

                            listView1.Items.Add(lvi);
                        }

                        SeriesCollection series = new SeriesCollection();

                        foreach (var item in expensesCategory)
                        {
                            series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                            pieChart1.Series = series;

                        }

                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            foreach (var item in expensesCategory)
                            {
                                if (checkedListBox1.Items[i].ToString() == item.Category)
                                    checkedListBox1.SetItemChecked(i, true);
                            }
                        }
                    }
                }
            }
            else if (flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var income = db.Incomes.Where(i => i.Id == Id).FirstOrDefault();

                    db.Incomes.Remove(income);
                    db.SaveChanges();

                    var user = db.Users.Where(i => i.Id == IdUser).FirstOrDefault();
                    user.Cash -= income.Sum;
                    db.SaveChanges();

                    var userUpdate = db.Users.Where(i => i.Id == IdUser).FirstOrDefault();
                    textBox1.Text = userUpdate.Cash.ToString();
                }

                using (BudgetEntities db = new BudgetEntities())
                {
                    var titles = db.CategoriesIncome;

                    foreach (var item in titles)
                    {
                        checkedListBox1.Items.Add(item.Title);
                    }
                }

                if (dateTimePicker1.Visible == true)
                {
                    date = dateTimePicker1.Value.Date;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var incomes = from inc in db.Incomes
                                      join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                      where inc.UserId == IdUser && inc.Date == date
                                      select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                        var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && d.Date == date).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        foreach (var item in incomes)
                        {
                            ListViewItem lvi = new ListViewItem(item.Id.ToString());

                            lvi.SubItems.Add(item.Category);
                            lvi.SubItems.Add(item.Sum.ToString());
                            lvi.SubItems.Add(item.Description.ToString());
                            lvi.SubItems.Add(item.Date.ToShortDateString());

                            listView1.Items.Add(lvi);
                        }

                        SeriesCollection series = new SeriesCollection();

                        foreach (var item in incomesCategory)
                        {
                            series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                            pieChart1.Series = series;
                        }

                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            foreach (var item in incomesCategory)
                            {
                                if (checkedListBox1.Items[i].ToString() == item.Category)
                                    checkedListBox1.SetItemChecked(i, true);
                            }
                        }
                    }
                }
                else if (dateTimePickerYearMonth1.Visible == true)
                {
                    int m = dateTimePickerYearMonth1.Value.Month;
                    int y = dateTimePickerYearMonth1.Value.Year;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var incomes = from inc in db.Incomes
                                      join c in db.CategoriesIncome on inc.CategoryId equals c.Id
                                      where inc.UserId == IdUser && inc.Date.Month == m && inc.Date.Year == y
                                      select new { Id = inc.Id, Category = c.Title, Sum = inc.Sum, Date = inc.Date, Description = inc.Description };

                        var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, UserId = inc.UserId, Sum = inc.Sum, Date = inc.Date }).Where(d=> (d.UserId == IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(c => c.Category, s => s.Sum).Select(g => new { Category = g.Key, Sum = g.Sum() });

                        foreach (var item in incomes)
                        {
                            ListViewItem lvi = new ListViewItem(item.Id.ToString());

                            lvi.SubItems.Add(item.Category);
                            lvi.SubItems.Add(item.Sum.ToString());
                            lvi.SubItems.Add(item.Description.ToString());
                            lvi.SubItems.Add(item.Date.ToShortDateString());

                            listView1.Items.Add(lvi);
                        }

                        SeriesCollection series = new SeriesCollection();

                        foreach (var item in incomesCategory)
                        {
                            series.Add(new PieSeries() { Title = item.Category.ToString(), Values = new ChartValues<int> { Convert.ToInt32(item.Sum) }, DataLabels = true, LabelPoint = labelPoint });
                            pieChart1.Series = series;
                        }

                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            foreach (var item in incomesCategory)
                            {
                                if (checkedListBox1.Items[i].ToString() == item.Category)
                                    checkedListBox1.SetItemChecked(i, true);
                            }
                        }
                    }
                }
            }        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Registration registration = new Registration(this);
            registration.Show();

            button1.BackColor = Color.MediumPurple;
            button5.BackColor = Color.SandyBrown;
            button3.BackColor = Color.FromArgb(128, 255, 128);
            button4.BackColor = Color.FromArgb(255, 128, 128);
            button2.BackColor = Color.DeepSkyBlue;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
            
            log.textBox1.Text = null;
            log.textBox2.Text = null;
        }
    }
}

//http://helpexe.ru/programmirovanie/kak-sozdat-krugovuju-diagrammu-s-pomoshhju

//https://question-it.com/questions/896910/datetimepicker-dlja-izmenenija-tolko-mesjatsa-i-goda


//CREATE TABLE[dbo].[CategoriesExpense]
//(

//   [Id] INT IDENTITY(1, 1) NOT NULL,


//[Title] NVARCHAR(50) NOT NULL,
//PRIMARY KEY CLUSTERED([Id] ASC)
//);


//CREATE TABLE[dbo].[CategoriesIncome]
//(

//   [Id] INT IDENTITY(1, 1) NOT NULL,


//[Title] NVARCHAR(50) NOT NULL,
//PRIMARY KEY CLUSTERED([Id] ASC)
//);


//CREATE TABLE[dbo].[Expenses]
//(

//   [Id] INT IDENTITY(1, 1) NOT NULL,


//[CategoryId]  INT NOT NULL,

//[Date] DATE NOT NULL,

//[Sum] MONEY NOT NULL,

//[Description] NVARCHAR(100) NULL,
//    PRIMARY KEY CLUSTERED([Id] ASC),
//    FOREIGN KEY([CategoryId]) REFERENCES[dbo].[CategoriesExpense] ([Id])
//);


//CREATE TABLE[dbo].[Incomes]
//(

//   [Id] INT IDENTITY(1, 1) NOT NULL,


//[CategoryId]  INT NOT NULL,

//[Date] DATE NOT NULL,

//[Sum] MONEY NOT NULL,

//[Description] NVARCHAR(100) NULL,
//    PRIMARY KEY CLUSTERED([Id] ASC),
//    FOREIGN KEY([CategoryId]) REFERENCES[dbo].[CategoriesIncome] ([Id])
//);


//CREATE TABLE[dbo].[Users]
//(

//   [Id] INT IDENTITY(1, 1) NOT NULL,


//[Login]       NVARCHAR(50)   NOT NULL,


//[Password]    NVARCHAR(8)    NOT NULL,


//[Name]        NCHAR(50)      NULL,
//    [Picture] VARBINARY(MAX) NULL,
//    [Cash]
//MONEY NULL,
//    [PictureName] VARCHAR(100)   NULL,
//    PRIMARY KEY CLUSTERED([Id] ASC)
//);
