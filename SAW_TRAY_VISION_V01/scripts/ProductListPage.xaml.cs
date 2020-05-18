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

namespace SAW_TRAY_VISION_V01
{
    /// <summary>
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductListPage : Page
    {
        public ProductListPage()
        {
            InitializeComponent();
            Show_Table();
        }
        public void Show_Table()
        {
            dg_product.ItemsSource = MyGlobals.Prods.ProductsTable.DefaultView;
        }

        private void Bt_Refesh_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Prods.FromXml();
            Show_Table();
        }

        private void Bt_Save_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Prods.ToXml();
        }

        private void Bt_Default_Click(object sender, RoutedEventArgs e)
        {
            MyGlobals.Prods.FromDefault();
            Show_Table();

        }
    }
}
