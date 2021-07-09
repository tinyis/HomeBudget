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
using LiveCharts;
using LiveCharts.Wpf;

namespace HomeBudget
{
    public partial class Charts : MaterialForm
    {
        Form1 form1;
        bool flagIncome = false;
        bool flagExpense = false;
        bool dayClick = false;
        bool monthClick = false;

        string checkedCategory = null;
        Log log;

        DateTime date;

        public Charts(Form1 obj)
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

            checkedListBox1.BackColor = Color.FromArgb(51, 51, 51);
            checkedListBox1.ColumnWidth = 190;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            flagIncome = true;
            flagExpense = false;

            button2.BackColor = Color.MediumPurple;
            button1.BackColor = Color.FromArgb(255, 128, 128);

            checkedListBox1.Items.Clear();
            cartesianChart1.Series.Clear();

            using (BudgetEntities db = new BudgetEntities())
            {
                var titles = db.CategoriesIncome;

                foreach (var item in titles)
                {
                    checkedListBox1.Items.Add(item.Title);
                }
            }

            if (dateTimePickerYearMonth1.Visible == true)
            {
                int m = dateTimePickerYearMonth1.Value.Month;
                int y = dateTimePickerYearMonth1.Value.Year;

                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomesDate = db.Incomes.Where(d => (d.UserId == form1.IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in incomesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToShortDateString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Дата",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
            else if (comboBox1.Visible == true)
            {
                int y = Convert.ToInt32(comboBox1.Text);

                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomesDate = db.Incomes.Where(d => (d.UserId == form1.IdUser) && ( d.Date.Year == y)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in incomesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Месяц",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flagExpense = true;
            flagIncome = true;

            button1.BackColor = Color.MediumPurple;
            button2.BackColor = Color.FromArgb(128, 255, 128);

            checkedListBox1.Items.Clear();
            cartesianChart1.Series.Clear();

            using (BudgetEntities db = new BudgetEntities())
            {
                var titles = db.CategoriesExpense;

                foreach (var item in titles)
                {
                    checkedListBox1.Items.Add(item.Title);
                }
            }

            if (dateTimePickerYearMonth1.Visible == true)
            {
                int m = dateTimePickerYearMonth1.Value.Month;
                int y = dateTimePickerYearMonth1.Value.Year;

                using (BudgetEntities db = new BudgetEntities())
                {
                    var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in expensesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToShortDateString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Дата",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
            else if (comboBox1.Visible == true)
            {
                int y = Convert.ToInt32(comboBox1.Text);

                using (BudgetEntities db = new BudgetEntities())
                {
                    var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) && (d.Date.Year == y)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in expensesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Месяц",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
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

        private void Charts_Load(object sender, EventArgs e)
        {
            flagExpense = true;

            using (BudgetEntities db = new BudgetEntities())
            {
                var titles = db.CategoriesExpense;

                foreach (var item in titles)
                {
                    checkedListBox1.Items.Add(item.Title);
                }
            }

            int m = dateTimePickerYearMonth1.Value.Month;
            int y = dateTimePickerYearMonth1.Value.Year;

            dayClick = true;
            label1.ForeColor = Color.MediumPurple;
            button1.BackColor = Color.MediumPurple;
            comboBox1.Visible = false;

            cartesianChart1.Series.Clear();

            using (BudgetEntities db = new BudgetEntities())
            {
                var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                foreach (var item in expensesDate) //Заполняем коллекции
                {
                    zp.Add(Convert.ToInt32(item.Sum));
                    date.Add(item.Date.ToShortDateString());
                }
                cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                {
                    Title = "Дата",
                    Labels = date
                });

                LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                line.Title = "";
                line.Values = zp;              

                series.Add(line); //Добавляем линию на график
                cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
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
            dateTimePickerYearMonth1.Visible = true;
            comboBox1.Visible = false;
            label1.ForeColor = Color.MediumPurple;
            label2.ForeColor = Color.White;

            int m = dateTimePickerYearMonth1.Value.Month;
            int y = dateTimePickerYearMonth1.Value.Year;

            cartesianChart1.Series.Clear();

            if(flagExpense == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in expensesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToShortDateString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Дата",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
            else if(flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomesDate = db.Incomes.Where(d => (d.UserId == form1.IdUser) && (d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in incomesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToShortDateString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Дата",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }          
        }

        private void label2_Click(object sender, EventArgs e)
        {
            monthClick = true;
            dayClick = false;
            dateTimePickerYearMonth1.Visible = false;
            comboBox1.Visible = true;
            label2.ForeColor = Color.MediumPurple;
            label1.ForeColor = Color.White;

            int y = Convert.ToInt32(comboBox1.Text);

            cartesianChart1.Series.Clear();

            if(flagExpense==true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) && (d.Date.Year == y)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in expensesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Месяц",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
            else if(flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomesDate = db.Incomes.Where(d => (d.UserId == form1.IdUser) && (d.Date.Year == y)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in incomesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Месяц",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }          
         }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked && checkedListBox1.CheckedItems.Count > 0)
            {
                checkedListBox1.ItemCheck -= checkedListBox1_ItemCheck;
                checkedListBox1.SetItemChecked(checkedListBox1.CheckedIndices[0], false);
                checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            }
        }

        private void dateTimePickerYearMonth1_ValueChanged(object sender, EventArgs e)
        {
            int m = dateTimePickerYearMonth1.Value.Month;
            int y = dateTimePickerYearMonth1.Value.Year;

            cartesianChart1.Series.Clear();

            if(flagExpense==true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) &&(d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in expensesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToShortDateString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Дата",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
            else if(flagIncome == true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomesDate = db.Incomes.Where(d => (d.UserId == form1.IdUser) &&(d.Date.Month == m) && (d.Date.Year == y)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in incomesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToShortDateString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Дата",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }         
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {    
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                checkedCategory = checkedListBox1.CheckedItems[i].ToString();
            }         

            cartesianChart1.Series.Clear();

            if(flagExpense==true)
            {
                if (dateTimePickerYearMonth1.Visible == true)
                {
                    int m = dateTimePickerYearMonth1.Value.Month;
                    int y = dateTimePickerYearMonth1.Value.Year;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, Sum = ex.Sum, Date = ex.Date, UserId = ex.UserId }).Where(c => (c.Category == checkedCategory) && (c.Date.Month == m) && (c.Date.Year == y) && (c.UserId == form1.IdUser)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                        SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                        ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                        List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                        foreach (var item in expensesCategory) //Заполняем коллекции
                        {
                            zp.Add(Convert.ToInt32(item.Sum));
                            date.Add(item.Date.ToShortDateString());
                        }
                        cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                        cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                        {
                            Title = "Дата",
                            Labels = date
                        });

                        LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                        line.Title = "";
                        line.Values = zp;

                        series.Add(line); //Добавляем линию на график
                        cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                    }
                }
                else if (comboBox1.Visible == true)
                {
                    int y = Convert.ToInt32(comboBox1.Text);

                    cartesianChart1.Series.Clear();

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var expensesCategory = db.Expenses.Join(db.CategoriesExpense, ex => ex.CategoryId, c => c.Id, (ex, c) => new { Category = c.Title, Sum = ex.Sum, Date = ex.Date, UserId = ex.UserId }).Where(c => (c.Category == checkedCategory) && (c.Date.Year == y) && (c.UserId == form1.IdUser)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                        SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                        ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                        List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                        foreach (var item in expensesCategory) //Заполняем коллекции
                        {
                            zp.Add(Convert.ToInt32(item.Sum));
                            date.Add(item.Date.ToString());
                        }
                        cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                        cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                        {
                            Title = "Месяц",
                            Labels = date
                        });

                        LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                        line.Title = "";
                        line.Values = zp;

                        series.Add(line); //Добавляем линию на график
                        cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                    }
                }
            }
            else if(flagIncome==true)
            {
                if (dateTimePickerYearMonth1.Visible == true)
                {
                    int m = dateTimePickerYearMonth1.Value.Month;
                    int y = dateTimePickerYearMonth1.Value.Year;

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, Sum = inc.Sum, Date = inc.Date, UserId = inc.UserId }).Where(c => (c.Category == checkedCategory) && (c.Date.Month == m) && (c.Date.Year == y) && (c.UserId == form1.IdUser)).GroupBy(d => d.Date, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                        SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                        ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                        List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                        foreach (var item in incomesCategory) //Заполняем коллекции
                        {
                            zp.Add(Convert.ToInt32(item.Sum));
                            date.Add(item.Date.ToShortDateString());
                        }
                        cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                        cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                        {
                            Title = "Дата",
                            Labels = date
                        });

                        LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                        line.Title = "";
                        line.Values = zp;

                        series.Add(line); //Добавляем линию на график
                        cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                    }
                }
                else if (comboBox1.Visible == true)
                {
                    int y = Convert.ToInt32(comboBox1.Text);

                    cartesianChart1.Series.Clear();

                    using (BudgetEntities db = new BudgetEntities())
                    {
                        var incomesCategory = db.Incomes.Join(db.CategoriesIncome, inc => inc.CategoryId, c => c.Id, (inc, c) => new { Category = c.Title, Sum = inc.Sum, Date = inc.Date, UserId = inc.UserId }).Where(c => (c.Category == checkedCategory) && (c.Date.Year == y) && (c.UserId == form1.IdUser)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                        SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                        ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                        List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                        foreach (var item in incomesCategory) //Заполняем коллекции
                        {
                            zp.Add(Convert.ToInt32(item.Sum));
                            date.Add(item.Date.ToString());
                        }
                        cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                        cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                        {
                            Title = "Месяц",
                            Labels = date
                        });

                        LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                        line.Title = "";
                        line.Values = zp;

                        series.Add(line); //Добавляем линию на график
                        cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                    }
                }
            }         
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int y = Convert.ToInt32(comboBox1.Text);

            cartesianChart1.Series.Clear();

            if(flagExpense==true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var expensesDate = db.Expenses.Where(d => (d.UserId == form1.IdUser) && (d.Date.Year == y)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in expensesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Месяц",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }
            else if(flagIncome==true)
            {
                using (BudgetEntities db = new BudgetEntities())
                {
                    var incomesDate = db.Incomes.Where(d => (d.UserId == form1.IdUser) && (d.Date.Year == y)).GroupBy(d => d.Date.Month, s => s.Sum).Select(g => new { Date = g.Key, Sum = g.Sum() });

                    SeriesCollection series = new SeriesCollection(); //отображение данных на график. Линии и т.д.
                    ChartValues<int> zp = new ChartValues<int>(); //Значения которые будут на линии, будет создания чуть позже.
                    List<string> date = new List<string>(); //здесь будут храниться значения для оси X
                    foreach (var item in incomesDate) //Заполняем коллекции
                    {
                        zp.Add(Convert.ToInt32(item.Sum));
                        date.Add(item.Date.ToString());
                    }
                    cartesianChart1.AxisX.Clear(); //Очищаем ось X от значений по умолчанию
                    cartesianChart1.AxisX.Add(new Axis //Добавляем на ось X значения, через блок инициализатора.
                    {
                        Title = "Месяц",
                        Labels = date
                    });

                    LineSeries line = new LineSeries(); //Создаем линию, задаем ей значения из коллекции
                    line.Title = "";
                    line.Values = zp;

                    series.Add(line); //Добавляем линию на график
                    cartesianChart1.Series = series; //Отрисовываем график в интерфейсе
                }
            }         
        }

        private void Charts_FormClosed(object sender, FormClosedEventArgs e)
        {
            form1.button5.BackColor = Color.SandyBrown;
            //form1.button3.BackColor = Color.FromArgb(128, 255, 128);
            //form1.button4.BackColor = Color.MediumPurple;
            //form1.button2.BackColor = Color.DeepSkyBlue;          
        }
    }
}
