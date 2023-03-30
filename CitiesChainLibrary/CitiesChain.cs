using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;

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
        string GetMessage();
    }

    /// <summary>
    /// Defines the CitiesChain game logic.
    /// </summary>
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CitiesChain : ICitiesChain
    {
        public static List<Player> playerList = new List<Player>();
        private readonly string[] cities_data = File.ReadAllLines("../../../CitiesNames.csv");
        private readonly Dictionary<Player, ICallback> players = new Dictionary<Player, ICallback>();
        private string lastPlayedCity;


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
                if (lastPlayedCity.Last().Equals(city.ToLower().First()))
                {
                    lastPlayedCity = city;
                    UpdateAllUsers(city);
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
        /// Returns the last posted message.
        /// </summary>
        /// <returns>The last posted message.</returns>
        public string GetMessage()
        {
            return lastPlayedCity;
        }

        /// <summary>
        /// Updates the displays of all players.
        /// </summary>
        private void UpdateAllUsers(string msg)
        {
            foreach (ICallback c in players.Values)
                c.SendAllMessages(msg);
        }
    }
}
