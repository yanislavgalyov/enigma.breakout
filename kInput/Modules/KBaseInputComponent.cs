using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// abstract implementation of a IKInputComponent.  Encapsulates common functionality that will
    /// be used within inheriting classes.
    /// </summary>
    public abstract class KBaseInputComponent : IKInputComponent
    {
        internal Dictionary<object, TimeSpan> _startTimes;
        internal KInputService _parent;

        /// <summary>
        /// Gets whether or not this component is currently enabled.  It will not
        /// be updated by the manager if Enabled=false.  You can use this to avoid 
        /// doing un-needed operations if you know that your game does not support
        /// a particular input type
        /// </summary>
        public Boolean Enabled { get; set; }

        /// <summary>
        /// creates an instance of KBaseInputComponent
        /// </summary>
        /// <param name="aParent">KInputService parent</param>
        public KBaseInputComponent(KInputService aParent)
        {
            Enabled = true;
            _parent = aParent;
            _startTimes = new Dictionary<object, TimeSpan>();
        }

        /// <summary>
        /// Sets the start time of an object.
        /// </summary>
        /// <param name="anObject">The object to set the start time for</param>
        internal void SetStartTime(object anObject)
        {
            if (_startTimes.ContainsKey(anObject))
                _startTimes[anObject] = DateTime.Now.TimeOfDay;
            else
                _startTimes.Add(anObject, DateTime.Now.TimeOfDay);
        }

        /// <summary>
        /// Clears the start time of an object.
        /// </summary>
        /// <param name="anObject">The object to clear the start time for.</param>
        internal TimeSpan ClearStartTime(object anObject)
        {
            if (_startTimes.ContainsKey(anObject))
            {
                TimeSpan t = GetDuration(anObject);
                _startTimes.Remove(anObject);
                return t;
            }
            else return TimeSpan.FromMilliseconds(0);

        }

        /// <summary>
        /// Gets the duration of an object.
        /// </summary>
        /// <param name="anObject">The object to return the start time for</param>
        /// <returns>A timespan representing the duration of the object</returns>
        public TimeSpan GetDuration(object anObject)
        {

            if (_startTimes.ContainsKey(anObject))
            {
                TimeSpan current = DateTime.Now.TimeOfDay;
                TimeSpan start = _startTimes[anObject];
                return current - start;
            }
            else
                return TimeSpan.FromMilliseconds(0);
        }

        /// <summary>
        /// Returns whether the object is in a "pressed" state or not.
        /// </summary>
        /// <param name="anObject">The object to check, ie Keys.A</param>
        /// <returns>True if the object is in a pressed state.</returns>
        public abstract bool IsDown(object anObject);


        /// <summary>
        /// This method must be implemented by an inheriting class.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of the timing values</param>
        public abstract void Update(Microsoft.Xna.Framework.GameTime gameTime);

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public abstract void Initialize();
    }
}