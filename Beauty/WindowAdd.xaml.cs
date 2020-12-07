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
using System.Windows.Shapes;
using System.Data;


namespace Beauty
{
    /// <summary>
    /// Логика взаимодействия для WindowAdd.xaml
    /// </summary>
    public partial class WindowAdd : Window
    {
        public WindowAdd()
        {
            InitializeComponent();
        }
        ClassMySQL MySql = new ClassMySQL();
        DataTable Product = new DataTable();
        MainWindow main = new MainWindow();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable Man = MySql.QueryToDB("Select id, name from manufacturer");
            for (int i = 0; i < Man.Rows.Count; i++)
                comboBoxManufacturer.Items.Add(Man.Rows[i]["name"]);            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.Show();
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxTitle.Text == "" | textBoxCost.Text == "")
                MessageBox.Show("Заполнены не все необходимые поля!");
            else
            {
                try
                {
                    Product = MySql.QueryToDB("Select * from Product");
                    DataRow NewProduct = Product.NewRow();
                    NewProduct["title"] = textBoxTitle.Text;
                    NewProduct["cost"] = textBoxCost.Text;
                    NewProduct["description"] = textBoxDescription.Text;
                    NewProduct["isactive"] = checkBoxActive.IsChecked;
                    NewProduct["manufacturerid"] = comboBoxManufacturer.SelectedIndex+1;
                    Product.Rows.Add(NewProduct);
                    MySql.AddToDB(Product);
                    MessageBox.Show("Товар успешно добавлен!");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверный формат данных!");
                }
            }
        }
    }
}
