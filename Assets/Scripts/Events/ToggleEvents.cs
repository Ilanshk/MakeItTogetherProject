using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Events
{
    public static class ToggleEvents {
        public delegate void SetToggle(String category, int value,int state);
        public static SetToggle setToggle;
    }
}
