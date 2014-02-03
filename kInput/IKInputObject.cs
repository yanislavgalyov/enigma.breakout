using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// Interface defining members that the KInput library needs to handle input
    /// events on bound objects.
    /// </summary>
    public interface IKInputObject
    {
        /// <summary>
        /// Gets whether or not the object currently has focus.  Keyboard objects do not receive
        /// input if they do not have focus.
        /// </summary>
        bool HasFocus { get; }

        /// <summary>
        /// Gets whether or not the object is visible.  Objects that are not visible do not receive
        /// input events.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets whether input is currently enabled on an object.  Objects will not receive
        /// input events if InputEnabled = false;
        /// </summary>
        bool InputEnabled { get; }

        /// <summary>
        /// Gets the current position of the input object.
        /// </summary>
        Microsoft.Xna.Framework.Vector3 Position { get; }

        /// <summary>
        /// Gets the current screen rectangle of the input object.
        /// </summary>
        Microsoft.Xna.Framework.Rectangle Rectangle { get; }
    }
}
