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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EX1_EF_View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DAL dal;
        // for the Update Proccess
        Product NewelySelectedProduct;
        short NewelySelectedOrderID;
        public MainWindow()
        {
            InitializeComponent();
            dal = new DAL();
            // Init
            Orders_DataGrid.ItemsSource = dal.GetAllOrders();
            Date_Picker.SelectedDateChanged += DatePicker_SelectedDateChanged;
            OrderIDTextBox.TextChanged += OrderIDTextBox_TextChanged;
            NewQuantity.TextChanged += NewQuantity_TextChanged;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Orders_DataGrid.SelectionChanged -= Orders_DataGrid_SelectionChanged;
            Orders_DataGrid.ItemsSource = dal.GetByMonth(Date_Picker.SelectedDate.Value);
            RemoveButton.Visibility = NewQuantity.Visibility = Visibility.Collapsed;
        }

        private void Orders_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                NewelySelectedProduct = GetProductFromItem((sender as DataGrid).SelectedItem.ToString());
                NewelySelectedOrderID = GetOrderIDFromItem((sender as DataGrid).SelectedItem.ToString());
                RemoveButton.Visibility = NewQuantity.Visibility = Visibility.Visible;
            }
        }

        public short GetOrderIDFromItem(string item)
        {
            string[] row_splited = item.Split(',');
            string[] OrderIDSplitted = row_splited[0].Split('=');
            return short.Parse(OrderIDSplitted[1].Trim());
        }

        public Product GetProductFromItem(string item)
        {
            string[] row_splited = item.Split(',');
            string[] productNameSplitted = row_splited[3].Split('=');
            return dal.GetProductByName(productNameSplitted[1].Trim());
        }
        private void Get_All_Orders_Click(object sender, RoutedEventArgs e)
        {
            Orders_DataGrid.SelectionChanged -= Orders_DataGrid_SelectionChanged;
            Orders_DataGrid.ItemsSource = dal.GetAllOrders();
            RemoveButton.Visibility = NewQuantity.Visibility = Visibility.Collapsed;
        }

        private void OrderIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num;
            if (int.TryParse(OrderIDTextBox.Text, out num))
            {
                Orders_DataGrid.ItemsSource = dal.GetByOrderID(num);

                if (Orders_DataGrid.Items.Count > 0)
                    AddProduct.Visibility = Visibility.Visible;
                else
                    AddProduct.Visibility = Visibility.Collapsed;

                Orders_DataGrid.SelectionChanged += Orders_DataGrid_SelectionChanged;
            }
            else
            {
                OrderIDTextBox.Text = "";
                RemoveButton.Visibility = NewQuantity.Visibility = Visibility.Collapsed;
            }
        }

        private void NewQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            short num;
            if (short.TryParse(NewQuantity.Text, out num))
            {
                dal.UpdateProductByID(NewelySelectedOrderID, NewelySelectedProduct, num);
                Orders_DataGrid.ItemsSource = dal.GetByOrderID(int.Parse(OrderIDTextBox.Text));
            }
            else
                NewQuantity.Text = "";
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            dal.RemoveProductFromOrderDB(NewelySelectedOrderID, NewelySelectedProduct);
            Orders_DataGrid.ItemsSource = dal.GetByOrderID(int.Parse(OrderIDTextBox.Text));
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Window1 newproductwindow = new Window1(short.Parse(OrderIDTextBox.Text));
            newproductwindow.Show();
        }
    }
}

