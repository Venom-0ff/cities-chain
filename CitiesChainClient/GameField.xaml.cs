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
        private int dontbreak = 9999;

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
                    icc.PostMessage($"\n{userName} joined the game!");

                    if (userID == 0)
                        GameField_RTB.AppendText("\nYou're the game host!");
                    else
                        GameField_RTB.AppendText($"\n{icc.GetHostName()} is the game host!");
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
            if (e.Key == Key.Enter)
            {
                string comand = ChatTextField.Text;

                if (comand == "/start" && userID == 0 && dontbreak == 9999)
                {
                    icc.PostMessage($"\n{userName}: {ChatTextField.Text}");
                    ChatTextField.Text = "";
                    dontbreak = 0;
                    for (int i = 3; i > 0; i--)
                    {
                        icc.PostMessage($"\nThe game will start in {i}");
                        await Task.Delay(1000);
                    }
                    GameField_RTB.Document.Blocks.Clear();
                    icc.PostMessage("The game has started! Every message from now on will be treated as an answer.");
                    return;
                }

                if (comand == "/start" && userID != 0 && dontbreak == 9999)
                {
                    GameField_RTB.AppendText("\nOnly host can start the game!");
                    ChatTextField.Text = "";
                    return;
                }
                
                if (dontbreak != 9999)
                {
                    if (icc.MakeATurn(comand))
                    {
                        icc.PostMessage($"\n{userName}'s answer '{comand}' was accepted!\nNext player has to name a city that starts with '{comand.First()}'.");
                        ChatTextField.Text = "";
                    }
                    else
                    {
                        icc.PostMessage($"\n{userName}'s answer '{comand}' was not accepted and they're out of the game!");
                        ChatTextField.Text = "";
                    }
                }
                else
                {
                    icc.PostMessage($"\n{userName}: {ChatTextField.Text}");
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
