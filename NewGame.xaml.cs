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

namespace Dama_WPF
{
    /// <summary>
    /// Interaction logic for NewGame.xaml
    /// </summary>
    public partial class NewGame : Window
    {
        public bool IsCreated = false;
        public NewGame()
        {
            InitializeComponent();
        }

        private void WPC_Checked(object sender, RoutedEventArgs e)
        {
            WPCsettings.Visibility = Visibility.Visible;
        }

        private void WPC_Unchecked(object sender, RoutedEventArgs e)
        {
            WPCsettings.Visibility = Visibility.Hidden;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            IsCreated = true;
            this.Close();
        }

        private void CancleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
