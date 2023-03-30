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

namespace CitiesChainClient
{
    /// <summary>
    /// Interaction logic for GameField.xaml
    /// </summary>
    public partial class GameField : Window
    {
        public int userID;
        public GameField(int id)
        {
            InitializeComponent();
            userID = id;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.users.TryGetValue(userID, out var tmp);
                string comand = ChatTextField.Text;
                GameField_RTB.AppendText($"\n{tmp}: {ChatTextField.Text}");
                ChatTextField.Text = "";
                if (comand == "/start")
                {
                    for (int i = 5; i > 0; i--)
                    {
                        GameField_RTB.AppendText($"\nThe game will start in {i}");
                        await Task.Delay(1000);
                    }
                }
            }
        }
    }
}
