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
using System.ServiceModel;

namespace CitiesChainClient
{
    /// <summary>
    /// Interaction logic for GameField.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class GameField : Window, ICallback
    {
        private ICitiesChain icc = null;
        public int userID;
        public string userName = "";
        private int dontbreak = 9999;
        public GameField(int id)
        {
            InitializeComponent();
            userID = id;
            foreach (Player player in CitiesChain.playerList)
            {
                if (player.PlayerId == id)
                {
                    try
                    {
                        userName = player.Name;

                        // Configure the ABCs of using the CitiesChain service
                        DuplexChannelFactory<ICitiesChain> channel = new DuplexChannelFactory<ICitiesChain>(this, "CitiesChainService");

                        // Activate a CitiesChain object
                        icc = channel.CreateChannel();

                        if (icc.Join(player))
                        {
                            icc.PostMessage($"\n{userName} joined the game!");
                            icc.GetMessage();
                        }
                        else
                        {
                            // Alias rejected by the service so nullify service proxies
                            icc = null;
                            MessageBox.Show("ERROR: Alias in use. Please try again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            icc.Leave(userName);
            icc.PostMessage($"\n{userName} left the game!");
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
                icc.PostMessage($"\n{userName}: {ChatTextField.Text}");
                ChatTextField.Text = "";
                if (comand == "/start" && dontbreak == 9999)
                {
                    dontbreak = 0;
                    for (int i = 3; i > 0; i--)
                    {
                        icc.PostMessage($"\nThe game will start in {i}");
                        await Task.Delay(1000);
                    }
                }
                else
                {
                    icc.MakeATurn(comand);
                }
            }
        }

        // Implements the callback logic that will be triggered by a callback event in the service
        private delegate void GuiUpdateDelegate(string message);

        public void SendAllMessages(string message)
        {   
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                try
                {
                    GameField_RTB.AppendText(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                this.Dispatcher.BeginInvoke(new GuiUpdateDelegate(SendAllMessages), new object[] { message });
        }
    }
}
