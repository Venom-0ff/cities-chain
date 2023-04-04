// Authors: Bhavin Patel, Kieran Primeau, Stanislav Kovalenko, Stepan Kostyukov (Group 2)
using CitiesChainLibrary;
using System;
using System.Windows;
using System.Windows.Input;

namespace CitiesChainClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player player;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUser.Text == "")
            {
                MessageBox.Show("Wrong name");
            }
            else
            {
                player = new Player(txtUser.Text);
                GameField game = new GameField(player);
                Hide();
                game.Show();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
