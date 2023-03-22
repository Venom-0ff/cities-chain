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
        void SendAllMessages(string[] messages);
    }

    /// <summary>
    /// A service contract.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface ICitiesChain
    {
        [OperationContract]
        bool Join(string name);
        [OperationContract(IsOneWay = true)]
        void Leave(string name);
        [OperationContract]
        bool MakeATurn(string city);
        [OperationContract]
        string[] GetAllMessages();
    }

    /// <summary>
    /// Defines the CitiesChain game logic.
    /// </summary>
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CitiesChain
    {
        private readonly string[] cities_data = File.ReadAllLines("../../../CitiesNames.csv");
        private readonly Dictionary<string, ICallback> players = new Dictionary<string, ICallback>();
        private readonly List<string> playedCities;

        public CitiesChain()
        {
            playedCities = new List<string>();
        }

        /// <summary>
        /// Stores unique username and subscribes the user's client to the callbacks.
        /// </summary>
        /// <param name="name">Username to add.</param>
        /// <returns><c>true</c> if the player successfully joins the game, <c>false</c> otherwise.</returns>
        public bool Join(string name)
        {
            if (players.ContainsKey(name.ToLower()))
                return false;
            else
            {
                ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                players.Add(name.ToLower(), callback);

                return true;
            }
        }

        /// <summary>
        /// Removes the player from the storage and unsubscribes the client from the callbacks.
        /// </summary>
        /// <param name="name">Username to remove.</param>
        public void Leave(string name)
        {
            if (players.ContainsKey(name.ToLower()))
            {
                players.Remove(name.ToLower());
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
                if (playedCities.Count == 0 || playedCities.Last().Last().Equals(city.ToLower().First()))
                {
                    playedCities.Add(city);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the displays of all players.
        /// </summary>
        private void UpdateAllUsers()
        {
            string[] msgs = playedCities.ToArray<string>();
            foreach (ICallback c in players.Values)
                c.SendAllMessages(msgs);
        }
    }
}
