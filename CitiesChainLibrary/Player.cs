using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [DataMember]
        public string Answer { get; set; }
        [DataMember]
        public bool IsPlaying { get; set; }
        [DataMember]
        public bool IsHost { get; set; }
        public Player(int Id, string Name)
        {
            this.Name = Name;
            this.PlayerId = Id;
            CitiesChain.playerList.Add(this);
        }

    }
}
