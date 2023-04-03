using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace CitiesChainLibrary
{
    /// <summary>
    /// A callback contract.
    /// </summary>
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void SendAllMessages(string message);
    }

    /// <summary>
    /// A service contract.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface ICitiesChain
    {
        [OperationContract]
        bool Join(Player player);
        [OperationContract(IsOneWay = true)]
        void Leave(string name);
        [OperationContract]
        bool MakeATurn(string city);
        [OperationContract]
        void PostMessage(string msg);
        [OperationContract]
        int GetPlayerId(Player player);
        [OperationContract]
        string GetHostName();
        [OperationContract]
        int GetPlayersCount();
        [OperationContract]
        void SetDontBreak();
        [OperationContract]
        bool GetDontBreak();
        [OperationContract]
        int GetCurrentPlayer();
        [OperationContract]
        void SetCurrentPlayer();
        [OperationContract]
        void ResetPlayerTurn();
        [OperationContract]
        void GameOver(int userID);
        [OperationContract]
        bool isLoser(int userID);
    }

    /// <summary>
    /// Defines the CitiesChain game logic.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CitiesChain : ICitiesChain
    {
        private readonly string[] cities_data = File.ReadAllLines("../../../CitiesNames.csv");
        private readonly Dictionary<Player, ICallback> players = new Dictionary<Player, ICallback>();
        private string lastPlayedCity = "";
        private int Id = 0;
        private bool dontbreak = true;
        private int currentPlayer = 0;
        private List<int> outplayers = new List<int>();



        /// <summary>
        /// Stores unique username and subscribes the user's client to the callbacks.
        /// </summary>
        /// <param name="name">Username to add.</param>
        /// <returns><c>true</c> if the player successfully joins the game, <c>false</c> otherwise.</returns>
        public bool Join(Player player)
        {
            if (players.ContainsKey(player))
                return false;
            else
            {
                player.PlayerId = Id++;
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                players.Add(player, callback);

                return true;
            }
        }

        /// <summary>
        /// Removes the player from the storage and unsubscribes the client from the callbacks.
        /// </summary>
        /// <param name="name">Username to remove.</param>
        public void Leave(string name)
        {
            if (players.Keys.Any(key => key.Name == name))
            {
                players.Remove(players.Keys.First(key => key.Name == name));
            }
        }

        /// <summary>
        /// Validates and accepts or rejects the player's turn.
        /// </summary>
        /// <param name="city">Name of the city that the player entered.</param>
        /// <returns><c>true</c> if the entered name of the city was accepted, <c>false</c> otherwise.</returns>
        public bool MakeATurn(string city)
        {
            if (cities_data.Contains(city))
            {
                if (lastPlayedCity.Length == 0 || !city.ToLower().Equals(lastPlayedCity.ToLower()) && lastPlayedCity.Last().Equals(city.ToLower().First()))
                {
                    lastPlayedCity = city;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Posts a general message to all users.
        /// </summary>
        /// <param name="msg">Message to post</param>
        /// <returns></returns>
        public void PostMessage(string msg)
        {
            UpdateAllUsers(msg);
        }

        /// <summary>
        /// Returns the Server-assigned id of the specified player.
        /// </summary>
        /// <param name="player">Player object to get id of.</param>
        /// <returns>Id of the specified player.</returns>
        public int GetPlayerId(Player player)
        {
            return players.Keys.First(p => p.Name == player.Name).PlayerId;
        }

        /// <summary>
        /// Returns the name of the current game's host.
        /// </summary>
        /// <returns>The name of the host.</returns>
        public string GetHostName()
        {
            return players.Keys.First().Name;
        }

        /// <summary>
        /// Returns the current number of connected players.
        /// </summary>
        /// <returns>Number of connected players.</returns>
        public int GetPlayersCount()
        {
            return players.Count;
        }

        /// <summary>
        /// Updates the displays of all players.
        /// </summary>
        private void UpdateAllUsers(string msg)
        {
            foreach (ICallback c in players.Values)
                c.SendAllMessages(msg);
        }

        /// <summary>
        /// Sets dontbreak server variable to false.
        /// </summary>
        public void SetDontBreak()
        {
            dontbreak = false;
        }

        /// <summary>
        /// Get dontbreak server variable.
        /// </summary>
        /// <returns><c>dontbreak</c> server variable.</returns>
        public bool GetDontBreak()
        {
            return dontbreak;
        }

        /// <summary>
        /// Returns the current player id.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentPlayer()
        {
            return currentPlayer;
        }

        /// <summary>
        /// Increases the current player id.
        /// </summary>
        public void SetCurrentPlayer()
        {
            currentPlayer++;
        }

        /// <summary>
        /// Resets the current player id.
        /// </summary>
        public void ResetPlayerTurn()
        {
            currentPlayer = 0;
        }

        public void GameOver(int userId)
        {
            outplayers.Add(userId);
        }

        public bool isLoser(int userId)
        {
            if (outplayers.Contains(userId))
                return true;
            else
                return false;
        }
    }
}
