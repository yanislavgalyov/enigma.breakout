using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kraken.Xna.Input
{
    internal struct KInputEventBinding
    {
        public IKInputObject InputObject;
        public KInputEventType EventType;
        

        public KInputEventBinding(IKInputObject anObject, KInputEventType anEventType)
        {
            
            InputObject = anObject;
            EventType = anEventType;
        }

    }
}