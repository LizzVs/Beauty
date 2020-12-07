using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Word = Microsoft.Office.Interop.Word;

namespace Beauty
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        DataTable Products = new DataTable();
        DataTable jsonProducts = new DataTable();
        ClassMySQL MySql = new ClassMySQL();
        static public int id;
        static public int get_id()
        {
            return id;
        }
        String answer;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            WebRequest connect = WebRequest.Create("http://localhost/beauty/getproducts.php");
            WebResponse res = await connect.GetResponseAsync();
            using (Stream stream = res.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                    answer = await reader.ReadToEndAsync();
            }
            //  десериализация
            Products = JsonConvert.DeserializeObject<DataTable>(answer);
            Products.Columns.Remove(Products.Columns[0]);
            Products.Columns.Remove(Products.Columns[1]);
            Products.Columns.Remove(Products.Columns[2]);
            Products.Columns.Remove(Products.Columns[3]);
            Products.Columns.Remove(Products.Columns[4]);
            Products.Columns.Remove(Products.Columns[5]);
            Products.Columns.Remove(Products.Columns[6]);


            // MySql.AsyncRequest(Products).Wait(3);
            // MessageBox.Show(Products.Rows.Count.ToString());
            // Products = MySql.QueryToDB("Select title,mainimagepath,cost,isactive from Product");
            dataGridProduct.ItemsSource = Products.DefaultView;
            DataTable Man = MySql.QueryToDB("Select id, name from manufacturer");
            for (int i = 0; i < Man.Rows.Count; i++)
                comboBoxManufacturer.Items.Add(Man.Rows[i]["name"]);
        }

        private void textBoxTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            Products = MySql.QueryToDB("Select title,mainimagepath,cost,isactive from Product where title like '" + textBoxTitle.Text + "%'");
            dataGridProduct.ItemsSource = Products.DefaultView;
        }

        private void comboBoxManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxManufacturer.SelectedIndex != 0)
                Products = MySql.QueryToDB("Select title,mainimagepath,cost,isactive from Product inner join manufacturer on product.manufacturerid=manufacturer.id where manufacturer.name='" + comboBoxManufacturer.SelectedItem + "' and title like '" + textBoxTitle.Text + "%'");
            else
                MySql.AsyncRequest(Products).Wait(3);
            dataGridProduct.ItemsSource = Products.DefaultView;
        }

        private void buttonIncrease_Click(object sender, RoutedEventArgs e)
        {
            Products = MySql.QueryToDB("Select title,mainimagepath,cost,isactive from Product order by cost");
            dataGridProduct.ItemsSource = Products.DefaultView;
        }

        private void buttonDecrease_Click(object sender, RoutedEventArgs e)
        {
            Products = MySql.QueryToDB("Select title,mainimagepath,cost,isactive from Product order by cost desc");
            dataGridProduct.ItemsSource = Products.DefaultView;
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProduct.SelectedIndex == -1)
                MessageBox.Show("Товар не выбран!");
            else
            {
                DataTable tableId = MySql.QueryToDB("Select id From product where title='" + Products.Rows[dataGridProduct.SelectedIndex]["title"].ToString() + "'");
                id = int.Parse(tableId.Rows[0]["id"].ToString());
                WindowEdit Edit = new WindowEdit();
                Edit.Show();
                this.Hide();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowAdd add = new WindowAdd();
            add.Show();
            this.Hide();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProduct.SelectedIndex == -1)
                MessageBox.Show("Товар не выбран!");
            else
            {
                DataTable tableId = MySql.QueryToDB("Select id From product where title='" + Products.Rows[dataGridProduct.SelectedIndex]["title"].ToString() + "'");
                id = int.Parse(tableId.Rows[0]["id"].ToString());
                MySql.DeleteFromDB(id);
                MessageBox.Show("Товар успешно удалён!");
                Products = MySql.QueryToDB("Select title,mainimagepath,cost,isactive from Product");
                dataGridProduct.ItemsSource = Products.DefaultView;
            }
        }

        private void buttonWord_Click(object sender, RoutedEventArgs e)
        {
            var application = new Word.Application();
            Word.Document doc = application.Documents.Add();

            Word.Paragraph tableParagraph = doc.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table prodTable = doc.Tables.Add(tableRange, Products.Rows.Count + 1, 3);
            prodTable.Borders.InsideLineStyle = prodTable.Borders.OutsideLineStyle
                = Word.WdLineStyle.wdLineStyleSingle;
            prodTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            cellRange = prodTable.Cell(1, 1).Range;
            cellRange.Text = "id";
            cellRange = prodTable.Cell(1, 2).Range;
            cellRange.Text = "Название товара";
            cellRange = prodTable.Cell(1, 3).Range;
            cellRange.Text = "Стоимость";
            /*    cellRange = champTable.Cell(1, 4).Range;
                cellRange.Text = "Место проведения";
                cellRange = champTable.Cell(1, 5).Range;
                cellRange.Text = "Начало";
                cellRange = champTable.Cell(1, 6).Range;
                cellRange.Text = "Конец";*/

            prodTable.Rows[1].Range.Bold = 1;
            for (int i = 0; i < Products.Rows.Count; i++)
            {
                cellRange = prodTable.Cell(i + 2, 1).Range;
                cellRange.Text = Products.Rows[i]["id"].ToString();
                cellRange = prodTable.Cell(i + 2, 2).Range;
                cellRange.Text = Products.Rows[i]["title"].ToString();
                cellRange = prodTable.Cell(i + 2, 3).Range;
                cellRange.Text = Products.Rows[i]["cost"].ToString();;
            }

            application.Visible = true;
            
            doc.SaveAs2(Directory.GetCurrentDirectory()+@"Products.docx");
            doc.Close();
        }
    }
}
