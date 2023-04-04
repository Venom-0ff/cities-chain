// Authors: Bhavin Patel, Kieran Primeau, Stanislav Kovalenko, Stepan Kostyukov (Group 2)
using CitiesChainLibrary;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        private bool hosttrue = false;

        public GameField(Player player)
        {
            InitializeComponent();
            try
            {
                userName = player.Name;
                // Configure the ABCs of using the CitiesChain service
                DuplexChannelFactory<ICitiesChain> channel = new DuplexChannelFactory<ICitiesChain>(this, "CitiesChainService");

                // Activate a CitiesChain object
                icc = channel.CreateChannel();

                if (icc.Join(player))
                {
                    userID = icc.GetPlayerId(player);
                    icc.PostMessage($"\n{userName} joined the game.");

                    if (userID == 0)
                    {
                        hosttrue = true;
                        GameField_RTB.AppendText("\nYou're the game host.");
                    }
                    else
                        GameField_RTB.AppendText($"\n{icc.GetHostName()} is the game host.");
                }
                else
                {
                    // Name rejected by the service so nullify service proxies
                    icc = null;
                    MessageBox.Show("ERROR: Name is already in use. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            icc.Leave(userName);
            icc.PostMessage($"\n{userName} left the game.");
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
            if (icc.isLoser(userID))
            {
                MessageBox.Show("You are out of the game!");
                ChatTextField.Text = "";
                return;
            }

            if (!icc.GetDontBreak() && icc.GetPlayersCount() <= 1)
            {
                ChatTextField.Text = "";
                MessageBox.Show("Congrats! You're the winner!\nThe game is over, you can close all windows now.");
                return;
            }

            if (e.Key == Key.Enter && userID == icc.GetCurrentPlayer())
            {
                string command = ChatTextField.Text;

                if (command != "/start" && hosttrue && icc.GetDontBreak())
                {
                    MessageBox.Show("Type \"/start\" to start the game.");
                    ChatTextField.Text = "";
                }

                if (command == "/start" && hosttrue && icc.GetDontBreak())
                {
                    if (icc.GetPlayersCount() >= 2)
                    {
                        icc.PostMessage($"\n{userName}: {ChatTextField.Text}");
                        ChatTextField.Text = "";
                        icc.SetDontBreak();
                        for (int i = 3; i > 0; i--)
                        {
                            icc.PostMessage($"\nThe game will start in {i}");
                            await Task.Delay(1000);
                        }
                        icc.PostMessage("\nThe game has started!\nEvery message from now on will be treated as an answer.");
                        GameField_RTB.ScrollToEnd();
                        return;
                    }
                    else
                    {
                        ChatTextField.Text = "";
                        GameField_RTB.AppendText("\nCan't start the game: not enough players.");
                        GameField_RTB.ScrollToEnd();
                        return;
                    }
                }

                if (command == "/start" && !hosttrue && !icc.GetDontBreak())
                {
                    GameField_RTB.AppendText("\nOnly host can start the game.");
                    GameField_RTB.ScrollToEnd();
                    ChatTextField.Text = "";
                    return;
                }

                if (!icc.GetDontBreak())
                {
                    if (icc.MakeATurn(command))
                    {
                        string temp = command.Last() + "";
                        icc.PostMessage($"\n{userName}'s answer '{command}' was accepted!\nNext player has to name a city that starts with '{temp.ToUpper()}'.");
                        GameField_RTB.ScrollToEnd();
                        ChatTextField.Text = "";
                        icc.SetCurrentPlayer();
                        if (userID >= icc.GetPlayersCount() - 1)
                        {
                            icc.ResetPlayerTurn();
                        }
                    }
                    else
                    {
                        icc.PostMessage($"\n{userName}'s answer '{command}' was not accepted and they're out of the game!");
                        GameField_RTB.ScrollToEnd();
                        ChatTextField.Text = "";
                        icc.GameOver(userID);
                        icc.SetCurrentPlayer();
                        if (userID >= icc.GetPlayersCount() - 1)
                        {
                            icc.ResetPlayerTurn();
                        }

                        if (icc.GetPlayersCount() <= 1)
                        {
                            icc.PostMessage($"\nCongrats {icc.GetWinner()}! You're the winner!");
                            return;
                        }
                    }
                }
                
                return;
            }

            if (e.Key == Key.Enter && userID != icc.GetCurrentPlayer())
            {
                if (icc.GetDontBreak())
                {
                    MessageBox.Show("Wait for the host to start the game!");
                    ChatTextField.Text = "";
                }
                else
                {
                    MessageBox.Show("It's not your turn yet!");
                    ChatTextField.Text = "";
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
                    GameField_RTB.ScrollToEnd();
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
