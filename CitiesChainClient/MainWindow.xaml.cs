using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CitiesChainLibrary;

namespace CitiesChainClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player player;
        int userID = 1;
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
                txtUser.Text = "Wrong name";
            }
            else
            {
                player = new Player(userID, txtUser.Text);
                GameField game = new GameField(userID);
                Hide();
                userID++;
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
