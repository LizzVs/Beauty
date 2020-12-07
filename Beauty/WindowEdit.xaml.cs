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
    /// Логика взаимодействия для WindowEdit.xaml
    /// </summary>
    public partial class WindowEdit : Window
    {
        public WindowEdit()
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
            Product = MySql.QueryToDB("Select * from Product where id="+MainWindow.get_id());
            textBoxTitle.Text = Product.Rows[0]["title"].ToString();
            textBoxCost.Text = Product.Rows[0]["cost"].ToString();
            textBoxDescription.Text = Product.Rows[0]["description"].ToString();
            if (Product.Rows[0]["isactive"].ToString() == "True")
                checkBoxActive.IsChecked = true;
            comboBoxManufacturer.SelectedIndex = int.Parse(Product.Rows[0]["manufacturerid"].ToString())-1;
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
                    Product.Rows[0]["title"] = textBoxTitle.Text;
                    Product.Rows[0]["cost"] = textBoxCost.Text;
                    Product.Rows[0]["description"] = textBoxDescription.Text;
                    Product.Rows[0]["isactive"] = checkBoxActive.IsChecked;
                    Product.Rows[0]["manufacturerid"] = comboBoxManufacturer.SelectedIndex+1;
                    MySql.EditDB(Product);
                    MessageBox.Show("Товар успешно отредактирован!");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверный формат данных!");
                }
            }
        }
    }
}
