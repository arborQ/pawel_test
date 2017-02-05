using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using LiteDB;

namespace WindowsFormsApplication1
{
    public class SavedNumberEntity
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public DateTime ChangeTime { get; set; }
    }

    public partial class Form1 : Form
    {
        private const string DbFileName = "ApplicationDataBase.db";

        private IEnumerable<SavedNumberEntity> GetSavedNumbers(string tableName = "CheckedNumbers")
        {
            using (var db = new LiteDatabase(DbFileName))
            {
                var dbColletion = db.GetCollection<SavedNumberEntity>(tableName);

                return dbColletion.FindAll().ToList();
            }
        }

        private void Check(IEnumerable<int> valuse, string tableName = "CheckedNumbers")
        {
            using (var db = new LiteDatabase(DbFileName))
            {
                var dbColletion = db.GetCollection<SavedNumberEntity>(tableName);
                foreach (var value in valuse)
                {
                    dbColletion.EnsureIndex(a => a.Value, true);
                    dbColletion.Insert(new SavedNumberEntity
                    {
                        Value = value,
                        ChangeTime = DateTime.Now
                    });
                }
            }
        }

        private bool IsChecked(int value, string tableName = "CheckedNumbers")
        {
            using (var db = new LiteDatabase(DbFileName))
            {
                var dbColletion = db.GetCollection<SavedNumberEntity>(tableName);
                return dbColletion.Exists(a => a.Value == value);
            }
        }

        public Form1()
        {
            InitializeComponent();
            label2.Text = "";
            label3.Text = "";
            toolStripStatusLabel1.Text = "Copyright © 2017 Paweł Wójcik. All rights reserved. ";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string odczyt = System.IO.File.ReadAllText(@"C:\1111.txt");
            //Thread.Sleep(100);
            textBox1.Text = odczyt + "\r\n";
            label3.Text = "";
            textBox1.ScrollBars = ScrollBars.Vertical;
            label2.Text = "Wszystkie pozycje:";
        }

        private async Task<IEnumerable<int>> RandomNumbers()
        {
            var alreadyChecked = GetSavedNumbers().Select(c => c.Value).ToList();
            var toSelect = Enumerable.Range(0, 3600).Except(alreadyChecked).ToList();

            var numbers = new List<int>();
            var random = new Random();

            while (numbers.Count < 10)
            {
                await Task.Factory.StartNew(() =>
                {
                    int oneInt = random.Next(0, toSelect.Count);

                    if (!IsChecked(oneInt))
                    {
                        numbers.Add(oneInt);
                        toSelect = toSelect.Except(numbers).ToList();
                    }
                });
            }

            return numbers;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private IList<int> Numbers = new List<int>();
        private async void button3_Click(object sender, EventArgs e)
        {
            label3.Text = "do check (01):\r\ndo check (02):\r\ndo check (03):\r\ndo check (04):\r\ndo check (05):\r\ndo check (06):\r\ndo check (07):\r\ndo check (08):\r\ndo check (09):\r\ndo check (10):\r\n";
            textBox1.Text = "Wyliczam...";

            var numbers = await RandomNumbers();
            Numbers = numbers.ToList();
            textBox1.Text = string.Empty;

            foreach (var number in numbers)
            //for (int i = 1; i <= 10; i++)
            {
                textBox1.Text += number.ToString("D4") + "\r\n";
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (Numbers.Any())
            {
                try
                {
                    Check(Numbers);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            else
            {
                MessageBox.Show("Brak wczytancyh wartości.");
            }

            loaded(null, null);
        }

        private void loaded(object sender, EventArgs e)
        {
            Text = "Wczytane: " + GetSavedNumbers().Count();
        }
    }
}
