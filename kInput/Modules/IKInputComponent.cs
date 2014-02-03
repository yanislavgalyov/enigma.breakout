using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// Interface that defines the common properties for a KInputComponent.  KInputService uses an 
    /// observer pattern to manage strategy pattern objects.  The IKInputComponent is the stategy interface. 
    /// </summary>
    public interface IKInputComponent
    {
        /// <summary>
        /// Gets whether or not this component is currently enabled.  It will not
        /// be updated by the manager if Enabled=false.  You can use this to avoid 
        /// doing un-needed operations if you know that your game does not support
        /// a particular input type
        /// </summary>
        Boolean Enabled { get; set; }

        /// <summary>
        /// Method used to return the duration of a specific input object.
        /// </summary>
        /// <param name="anObject">The object to return the duration of.  This could be a mouse button, or a gamepad button, or a keyboard key, etc depending on the implementing class</param>
        /// <returns>The timespan representing the duration of the object.</returns>
        TimeSpan GetDuration(object anObject);

        /// <summary>
        /// Returns whether the specified object is currently down(in a pressed state)
        /// </summary>
        /// <param name="anObject">The object to check for</param>
        /// <returns>True if the object is pressed</returns>
        Boolean IsDown(object anObject);
        /// <summary>
        /// Updates the component.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of the timing values.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Initializes the component
        /// </summary>
        void Initialize();
    }
}