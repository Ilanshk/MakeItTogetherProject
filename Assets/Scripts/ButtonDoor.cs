using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Game
{
    public class ButtonDoor : NetworkBehaviour
    {
        public delegate void ButtonPressed(ButtonDoor buttonDoor);
        public event ButtonPressed OnButtonPressed;

        public void Activate()
        {
            if (IsServer)
            {
                OnButtonPressed?.Invoke(this);
            }
        }

    }
}
