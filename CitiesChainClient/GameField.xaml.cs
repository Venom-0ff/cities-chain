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
using CitiesChainLibrary;

namespace CitiesChainClient
{
    /// <summary>
    /// Interaction logic for GameField.xaml
    /// </summary>
    public partial class GameField : Window
    {
        public int userID;
        public string userName = "";
        private int dontbreak = 9999;
        public GameField(int id)
        {
            InitializeComponent();
            userID = id;
            foreach (Player player in CitiesChain.playerList)
            {
                if(player.PlayerId == id)
                {
                    userName = player.Name;
                    break;
                }
            }
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
            if (e.Key == Key.Enter && userID == 1)
            {
                string comand = ChatTextField.Text;
                GameField_RTB.AppendText($"\n{userName}: {ChatTextField.Text}");
                ChatTextField.Text = "";
                if (comand == "/start" && dontbreak == 9999)
                {
                    dontbreak = 0;
                    for (int i = 3; i > 0; i--)
                    {
                        GameField_RTB.AppendText($"\nThe game will start in {i}");
                        await Task.Delay(1000);
                    }
                }
            }
        }
    }
}
