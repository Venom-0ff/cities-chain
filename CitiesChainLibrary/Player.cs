using System;
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
    internal class Player
    {
        public string Name { get; }
        public string Answer { get; set; }
        public bool IsPlaying { get; set; }

        public Player(string Name)
        {
            this.Name = Name;
        }
    }
}
