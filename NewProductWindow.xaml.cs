using EX1_EF_DAL;
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

namespace EX1_EF_View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        short OrderID;
        public Window1(short orderID)
        {
            InitializeComponent();
            OrderID = orderID;
            ProducttGrid.ItemsSource = DAL.GetAllProducts();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (ProducttGrid.SelectedItem != null)
            {
                DAL.AddProductToOrder(OrderID, ProducttGrid.SelectedItem as Product);
                this.Close();
            }
        }
    }
}
