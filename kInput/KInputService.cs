using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KInputService : Microsoft.Xna.Framework.GameComponent
    {
        private List<KInputEventType> _noBubbleUp;
     
        private Dictionary<string, IKInputComponent> _components;
        /// <summary>
        /// Gets a collection containing all of the currently loaded input component
        /// modules.
        /// </summary>
        public Dictionary<string, IKInputComponent> Components
        {
            get { return _components; }
        }

        internal Dictionary<KInputEventBinding, EventHandler> EventBindings;

        /// <summary>
        /// Retrieves the duration of a specific object event.  
        /// </summary>
        /// <param name="anObject">The object to retrieve the duration for.  ie, Keys.A.</param>
        /// <returns>A timespan representing the duration of the object's state.</returns>
        public TimeSpan GetDuration(object anObject)
        {
            foreach (KeyValuePair<string, IKInputComponent> kv in _components)
            {
                TimeSpan t = kv.Value.GetDuration(anObject);
                if(t.TotalMilliseconds>0)
                    return t;
            }

            return TimeSpan.FromMilliseconds(0);
        }

        /// <summary>
        /// Gets whether the object is currently in a "pressed" state.
        /// </summary>
        /// <param name="anObject">The object to check, ie Keys.A</param>
        /// <returns>True if the object is currently in a pressed state.</returns>
        public bool IsDown(object anObject)
        {
            foreach (KeyValuePair<string, IKInputComponent> kv in _components)
            {
                bool t = kv.Value.IsDown(anObject);
                if (t == true)
                    return t;
            }

            return false;

        }


        /// <summary>
        /// Binds an input event to an input object.  If the object is not yet in the object collection,
        /// the object is added to the collection autmatically.
        /// </summary>
        /// <param name="anObject">The object to bind the event to</param>
        /// <param name="anEventType">The event type to bind</param>
        /// <param name="anEvent">The event handler to bind to the object</param>
        public void BindEvent(IKInputObject anObject, KInputEventType anEventType, EventHandler anEvent)
        {

            KInputEventBinding kb = new KInputEventBinding(anObject, anEventType);
            anObject.GetHashCode();
            if (EventBindings.ContainsKey(kb))
                EventBindings[kb] += anEvent;
            else
                EventBindings.Add(kb, anEvent);

        }

        /// <summary>
        /// removes an event binding from an object
        /// </summary>
        /// <param name="anObject">The object to remove the event from</param>
        /// <param name="anEventType">The event type to remove the binding for</param>
        /// <param name="anEvent">The event to remove from the object</param>
        public void UnBindEvent(IKInputObject anObject, KInputEventType anEventType, EventHandler anEvent)
        {
            KInputEventBinding kb = new KInputEventBinding(anObject, anEventType);

            if (!EventBindings.ContainsKey(kb))
                return;

            if (EventBindings.ContainsKey(kb))
                EventBindings[kb] -= anEvent;
        }

        /// <summary>
        /// Removes an input object and all associated event bindings.
        /// </summary>
        /// <param name="anObject">The object to remove from the collection</param>
        public void RemoveInputObject(IKInputObject anObject)
        {
            var bindings = from binding in EventBindings where binding.Key.InputObject == anObject select binding;
            List<KeyValuePair<KInputEventBinding, EventHandler>> tbind = new List<KeyValuePair<KInputEventBinding, EventHandler>>(bindings);

            foreach (KeyValuePair<KInputEventBinding, EventHandler> kv in tbind)
                EventBindings.Remove(kv.Key);
        }

        /// <summary>
        /// Wrapper to set component property on all components that contain the property
        /// </summary>
        /// <param name="aProperty">The property to set the value of</param>
        /// <param name="aPropertyValue">The value to set on the property</param>
        public void SetComponentProperty(string aProperty, object aPropertyValue)
        {
            foreach (KeyValuePair<string, IKInputComponent> kv in _components)
            {
                SetComponentProperty(kv.Key, aProperty, aPropertyValue, false);
            }
        }

        /// <summary>
        /// Sets a property of the specified input component.  
        /// </summary>
        /// <param name="aComponent">The input component name to set a property for</param>
        /// <param name="aProperty">The name of the public property to set on aComponent</param>
        /// <param name="aPropertyValue">The value to set on the public property</param>
        public void SetComponentProperty(string aComponent, string aProperty, object aPropertyValue)
        {

            SetComponentProperty(aComponent, aProperty, aPropertyValue, true);
        }

        /// <summary>
        /// Sets a property of the specified input component.  
        /// </summary>
        /// <param name="aComponent">The input component name to set a property for</param>
        /// <param name="aProperty">The name of the public property to set on aComponent</param>
        /// <param name="aPropertyValue">The value to set on the public property</param>
        /// <param name="throwException">Throw an exception on invalid parameters, or just return</param>
        internal void SetComponentProperty(string aComponent, string aProperty, object aPropertyValue, bool throwException)
        {

            if (!Components.ContainsKey(aComponent))
            {
                if (throwException)
                    throw new Exception("A component with the name " + aComponent + " could not be found.");
                else
                    return;
            }

            IKInputComponent k = Components[aComponent];
            System.Reflection.PropertyInfo p = k.GetType().GetProperty(aProperty);

            if (p == null)
            {
                if (throwException)
                    throw new Exception(aProperty + " is not a property of the component.");
                else
                    return;
            }

            p.SetValue(k, aPropertyValue, null);

        }

        /// <summary>
        /// Wrapper to get component property on all components that contain the property
        /// </summary>
        /// <param name="aProperty">The property to get the value of</param>
        /// <returns>An object representing the value of the component property, or null if the property does not exist.</returns>
        public object GetComponentProperty(string aProperty)
        {
            object ret = null;

            foreach (KeyValuePair<string, IKInputComponent> kv in _components)
            {
                ret = GetComponentProperty(kv.Key, aProperty, false);
                if (ret != null)
                    return ret;
            }

            return ret;
        }

        /// <summary>
        /// Gets a property of the specified input component.  
        /// </summary>
        /// <param name="aComponent">The input component name to get a property for</param>
        /// <param name="aProperty">The name of the public property to get on aComponent</param>
        /// <returns>An object representing the value of the component property, or null if the property does not exist.</returns>
        public object GetComponentProperty(string aComponent, string aProperty)
        {

            return GetComponentProperty(aComponent, aProperty, true);
        }

        internal object GetReflectedProperty(object anObject, string aPropertyName, object valIfNotFound)
        {
            PropertyInfo pi = anObject.GetType().GetProperty(aPropertyName);
            if (pi == null)
                return valIfNotFound;
            
            return pi.GetValue(anObject, null);
        }

        internal bool HasReflectedProperty(object anObject, string aPropertyName)
        {
            PropertyInfo pi = anObject.GetType().GetProperty(aPropertyName);
            if (pi == null)
                return false;
            else
                return true;
            
        }
        /// <summary>
        /// Gets a property of the specified input component.  
        /// </summary>
        /// <param name="aComponent">The input component name to get a property for</param>
        /// <param name="aProperty">The name of the public property to get on aComponent</param>
        /// <param name="throwException">Throw an exception on invalid parameters, or just return</param>
        internal object GetComponentProperty(string aComponent, string aProperty, bool throwException)
        {

            if (!Components.ContainsKey(aComponent))
            {
                if (throwException)
                    throw new Exception("A component with the name " + aComponent + " could not be found.");
                else
                    return null;
            }

            IKInputComponent k = Components[aComponent];
            System.Reflection.PropertyInfo p = k.GetType().GetProperty(aProperty);

            if (p == null)
            {
                if (throwException)
                    throw new Exception(aProperty + " is not a property of the component.");
                else
                    return null;
            }

            return p.GetValue(k, null);
            

        }

        /// <summary>
        /// Creates an instance of a KInputService component
        /// </summary>
        /// <param name="game">The XNA game that will contain the component</param>
        public KInputService(Game game)
            : base(game)
        {
            if (Game.Services.GetService(this.GetType()) != null)
                throw new Exception("An input service has already been added.");

            Game.Services.AddService(this.GetType(), this);
            // TODO: Construct any child components here
            _components = new Dictionary<string, IKInputComponent>(StringComparer.CurrentCultureIgnoreCase);
            _components.Add("keyboard", new KKeyboardInputComponent(this));
            _components.Add("mouse", new KMouseInputComponent(this));
            _components.Add("player1", new KGamePadInputComponent(this, PlayerIndex.One));
            _components.Add("player2", new KGamePadInputComponent(this, PlayerIndex.Two));
            _components.Add("player3", new KGamePadInputComponent(this, PlayerIndex.Three));
            _components.Add("player4", new KGamePadInputComponent(this, PlayerIndex.Four));

            EventBindings = new Dictionary<KInputEventBinding, EventHandler>();
            _noBubbleUp = new List<KInputEventType>();

            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();

            foreach (KeyValuePair<string, IKInputComponent> kv in _components)
                kv.Value.Initialize();
       }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            foreach (KeyValuePair<string, IKInputComponent> kv in Components)
                if (kv.Value.Enabled)
                    kv.Value.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Fires an input event.
        /// </summary>
        /// <param name="args">The event args to send to the subject event</param>
        /// <param name="alreadyFired">Represents whether or not the event was already fired by a child input component</param>
        internal void FireEvent(KInputEventArgs args, bool alreadyFired)
        {
            if (_noBubbleUp.Contains(args.EventType) && alreadyFired)
                return;

            System.Reflection.EventInfo ei;

            ei = this.GetType().GetEvent(args.EventType.ToString());
            if (ei != null)
            {
                
                System.Reflection.MethodInfo mi = this.GetType().GetMethod("on" + args.EventType.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
                if (mi != null)
                    mi.Invoke(this, new object[] { this, args });
            }
        }
        
        #region Keyboard Events
        /// <summary>
        /// An event that occurs when a key is released.
        /// </summary>
        public event EventHandler KeyUp;
        internal void onKeyUp(object Sender, EventArgs e)
        {
            if (KeyUp != null)
                KeyUp(Sender, e);
        }

        /// <summary>
        /// an event that occurs when a key is pressed.
        /// </summary>
        public event EventHandler KeyDown;
        internal void onKeyDown(object Sender, EventArgs e)
        {
            if (KeyDown != null)
                KeyDown(Sender, e);
        }
        #endregion

        #region Mouse Events
        /// <summary>
        /// Event that occurs when the left mouse button is pressed.
        /// </summary>
        public event EventHandler LeftMouseDown;
        internal void onLeftMouseDown(object Sender, EventArgs e)
        {
            if (LeftMouseDown != null)
                LeftMouseDown(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the left mouse is released.
        /// </summary>
        public event EventHandler LeftMouseUp;
        internal void onLeftMouseUp(object Sender, EventArgs e)
        {
            if (LeftMouseUp != null)
                LeftMouseUp(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the right mouse button is pressed.
        /// </summary>
        public event EventHandler RightMouseDown;
        internal void onRightMouseDown(object Sender, EventArgs e)
        {
            if (RightMouseDown != null)
                RightMouseDown(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the right mouse button is released.
        /// </summary>
        public event EventHandler RightMouseUp;
        internal void onRightMouseUp(object Sender, EventArgs e)
        {
            if (RightMouseUp != null)
                RightMouseUp(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the middle mouse button is pressed.
        /// </summary>
        public event EventHandler MiddleMouseDown;
        internal void onMiddleMouseDown(object Sender, EventArgs e)
        {
            if (MiddleMouseDown != null)
                MiddleMouseDown(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the middle mouse button is released.
        /// </summary>
        public event EventHandler MiddleMouseUp;
        internal void onMiddleMouseUp(object Sender, EventArgs e)
        {
            if (MiddleMouseUp != null)
                MiddleMouseUp(Sender, e);
        }
        
        /// <summary>
        /// Event that occurs when the XButton1 button is pressed.
        /// </summary>
        public event EventHandler XButton1MouseDown;
        internal void onXButton1MouseDown(object Sender, EventArgs e)
        {
            if (XButton1MouseDown != null)
                XButton1MouseDown(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the XButton1 button is released.
        /// </summary>
        public event EventHandler XButton1MouseUp;
        internal void onXButton1MouseUp(object Sender, EventArgs e)
        {
            if (XButton1MouseUp != null)
                XButton1MouseUp(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the XButton2 button is pressed.
        /// </summary>
        public event EventHandler XButton2MouseDown;
        internal void onXButton2MouseDown(object Sender, EventArgs e)
        {
            if (XButton2MouseDown != null)
                XButton2MouseDown(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the XButton2 button is released
        /// </summary>
        public event EventHandler XButton2MouseUp;
        internal void onXButton2MouseUp(object Sender, EventArgs e)
        {
            if (XButton2MouseUp != null)
                XButton2MouseUp(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the mouse is moved.
        /// </summary>
        public event EventHandler MouseMove;
        internal void onMouseMove(object Sender, EventArgs e)
        {
            if (MouseMove != null)
                MouseMove(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event is started via the left mouse button.
        /// </summary>
        public event EventHandler LeftDragStart;
        internal void onLeftDragStart(object Sender, EventArgs e)
        {
            if (LeftDragStart != null)
                LeftDragStart(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event is ended using the left mouse button.
        /// </summary>
        public event EventHandler LeftDragEnd;
        internal void onLeftDragEnd(object Sender, EventArgs e)
        {
            if (LeftDragEnd != null)
                LeftDragEnd(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event is started using the right mouse button
        /// </summary>
        public event EventHandler RightDragStart;
        internal void onRightDragStart(object Sender, EventArgs e)
        {
            if (RightDragStart != null)
                RightDragStart(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event that was started with the right mouse button is ended.
        /// </summary>
        public event EventHandler RightDragEnd;
        internal void onRightDragEnd(object Sender, EventArgs e)
        {
            if (RightDragEnd != null)
                RightDragEnd(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event is started with the middle mouse button.
        /// </summary>
        public event EventHandler MiddleDragStart;
        internal void onMiddleDragStart(object Sender, EventArgs e)
        {
            if (MiddleDragStart != null)
                MiddleDragStart(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event that was started with the middle mouse button is ended.
        /// </summary>
        public event EventHandler MiddleDragEnd;
        internal void onMiddleDragEnd(object Sender, EventArgs e)
        {
            if (MiddleDragEnd != null)
                MiddleDragEnd(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event is started with the XButton1 mouse button.
        /// </summary>
        public event EventHandler XButton1DragStart;
        internal void onXButton1DragStart(object Sender, EventArgs e)
        {
            if (XButton1DragStart != null)
                XButton1DragStart(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event that was started with the XButton1 mouse button is ended.
        /// </summary>
        public event EventHandler XButton1DragEnd;
        internal void onXButton1DragEnd(object Sender, EventArgs e)
        {
            if (XButton1DragEnd != null)
                XButton1DragEnd(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event is started with the XButton2 mouse button.
        /// </summary>
        public event EventHandler XButton2DragStart;
        internal void onXButton2DragStart(object Sender, EventArgs e)
        {
            if (XButton2DragStart != null)
                XButton2DragStart(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a drag event that was started with the XButton2 mouse button is ended.
        /// </summary>
        public event EventHandler XButton2DragEnd;
        internal void onXButton2DragEnd(object Sender, EventArgs e)
        {
            if (XButton2DragEnd != null)
                XButton2DragEnd(Sender, e);
        }
        #endregion

        #region GamePad Events
        /// <summary>
        /// Event that occurs when a Game pad button is pressed.
        /// </summary>
        public event EventHandler GamePadButtonDown;
        internal void onGamePadButtonDown(object Sender, EventArgs e)
        {
            if (GamePadButtonDown != null)
                GamePadButtonDown(Sender, e);
        }

        /// <summary>
        /// Event that occurs when a game pad button is released.
        /// </summary>
        public event EventHandler GamePadButtonUp;
        internal void onGamePadButtonUp(object Sender, EventArgs e)
        {
            if (GamePadButtonUp != null)
                GamePadButtonUp(Sender, e);
        }

        /// <summary>
        /// Event that occurs when the left game pad stick is moved.
        /// </summary>
        public event EventHandler GamePadLeftStickMoved;
        internal void onGamePadLeftStickMoved(object sender, EventArgs e)
        {
            if (GamePadLeftStickMoved != null)
                GamePadLeftStickMoved(sender, e);
        }

        /// <summary>
        /// Event that occurs when the right game pad stick is moved.
        /// </summary>
        public event EventHandler GamePadRightStickMoved;
        internal void onGamePadRightStickMoved(object sender, EventArgs e)
        {
            if (GamePadRightStickMoved != null)
                GamePadRightStickMoved(sender, e);
        }

        /// <summary>
        /// Event that occurs when the left game pad trigger value changed.
        /// </summary>
        public event EventHandler GamePadLeftTriggerChanged;
        internal void onGamePadLeftTriggerChanged(object sender, EventArgs e)
        {
            if (GamePadLeftTriggerChanged != null)
                GamePadLeftTriggerChanged(sender, e);
        }

        /// <summary>
        /// Event that occurs when the right game pad trigger value changed.
        /// </summary>
        public event EventHandler GamePadRightTriggerChanged;
        
        internal void onGamePadRightTriggerChanged(object sender, EventArgs e)
        {
            if (GamePadRightTriggerChanged != null)
                GamePadRightTriggerChanged(sender, e);
        }
        #endregion

        /// <summary>
        /// returns whether or not the specified key is in the alpha range.  Is it a letter?
        /// </summary>
        /// <param name="aKey">The key to check</param>
        /// <returns>True if the key is alpha</returns>
        public static bool IsAlpha(Keys aKey)
        {
            if (GetAlphaKey(aKey) == "")
                return false;
            else
                return true;
        }

        /// <summary>
        /// returns the string representation of the Keys enum value passed in.
        /// </summary>
        /// <param name="aKey">The key to check</param>
        /// <returns>The string representation of the string passed in.</returns>
        public static string GetAlphaKey(Keys aKey)
        {
            if (aKey == Keys.A ||
                aKey == Keys.C ||
                aKey == Keys.D ||
                aKey == Keys.E ||
                aKey == Keys.F ||
                aKey == Keys.G ||
                aKey == Keys.H ||
                aKey == Keys.I ||
                aKey == Keys.J ||
                aKey == Keys.K ||
                aKey == Keys.L ||
                aKey == Keys.M ||
                aKey == Keys.N ||
                aKey == Keys.O ||
                aKey == Keys.P ||
                aKey == Keys.Q ||
                aKey == Keys.R ||
                aKey == Keys.S ||
                aKey == Keys.T ||
                aKey == Keys.U ||
                aKey == Keys.V ||
                aKey == Keys.W ||
                aKey == Keys.X ||
                aKey == Keys.Y ||
                aKey == Keys.Z ||
                aKey == Keys.B)
                return aKey.ToString();
            else
                return "";
        }

        /// <summary>
        /// returns whether or not the specified key is in the numeric range.  Is it a number?
        /// </summary>
        /// <param name="aKey">The key to check</param>
        /// <returns>True if the key is a number</returns>
        public static bool IsNumeric(Keys aKey)
        {
            if (GetNumericKey(aKey) >= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// returns the integer representation of a numeric key.
        /// </summary>
        /// <param name="aKey">The key to return the integer for.</param>
        /// <returns>The integer value of the key, or if it is not an integer key, -1</returns>
        public static int GetNumericKey(Keys aKey)
        {
            if (aKey == Keys.D0 ||
                aKey == Keys.NumPad0)
                return 0;
            else if (aKey == Keys.D1 ||
                aKey == Keys.NumPad1)
                return 1;
            else if (aKey == Keys.D2 ||
                aKey == Keys.NumPad2)
                return 2;
            else if (aKey == Keys.D3 ||
                aKey == Keys.NumPad3)
                return 3;
            else if (aKey == Keys.D4 ||
                aKey == Keys.NumPad4)
                return 4;
            else if (aKey == Keys.D5 ||
                aKey == Keys.NumPad5)
                return 5;
            else if (aKey == Keys.D6 ||
                aKey == Keys.NumPad6)
                return 6;
            else if (aKey == Keys.D7 ||
                aKey == Keys.NumPad7)
                return 7;
            else if (aKey == Keys.D8 ||
                aKey == Keys.NumPad8)
                return 8;
            else if (aKey == Keys.D9 ||
                aKey == Keys.NumPad9)
                return 9;
            else
                return -1;

        }
    }
}