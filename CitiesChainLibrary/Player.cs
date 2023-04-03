using System.Runtime.Serialization;

namespace CitiesChainLibrary
{
    /// <summary>
    /// A class that represents a player.
    /// </summary>
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int PlayerId { get; set; }
        public Player(string Name)
        {
            this.Name = Name;
        }
    }
}
