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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Metoda pro čtení player1
        /// </summary>
        /// <returns></returns>
        public int GetPlayer1()
        {
            if ((bool)WHuman.IsChecked)
            {
                return 0;
            }
            if (WsliderPC.Value == 1)
            {
                return 1;
            }
            if (WsliderPC.Value == 2)
            {
                return 2;
            }
            if (WsliderPC.Value == 3)
            {
                return 3;
            }
            if (WsliderPC.Value == 4)
            {
                return 4;
            }
            return 0;
        }
        /// <summary>
        /// Metoda pro čtení player2
        /// </summary>
        /// <returns></returns>
        public int GetPlayer2()
        {
            if ((bool)BHuman.IsChecked)
            {
                return 0;
            }
            if (BsliderPC.Value == 1)
            {
                return 1;
            }
            if (BsliderPC.Value == 2)
            {
                return 2;
            }
            if (BsliderPC.Value == 3)
            {
                return 3;
            }
            if (BsliderPC.Value == 4)
            {
                return 4;
            }
            return 0;
        }

        private void BPC_Checked(object sender, RoutedEventArgs e)
        {
            BPCsettings.Visibility = Visibility.Visible;
        }

        private void BPC_Unchecked(object sender, RoutedEventArgs e)
        {
            BPCsettings.Visibility = Visibility.Hidden;
        }
    }
}
