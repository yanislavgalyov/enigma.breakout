using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// a derived KBaseInputComponent to handle keyboard input.
    /// </summary>
    public class KKeyboardInputComponent : KBaseInputComponent
    {
        private List<Keys> _lastKeys;
        private List<Keys> _pressedKeys;
        private KKeyboardEventArgs KeyArgs;

        /// <summary>
        /// the amount of time in milliseconds to wait before repeating an keydown event if the key is still pressed.
        /// </summary>
        public int RepeatDelay { get; set; }

        /// <summary>
        /// Gets or sets wheter keyboard repeat events are currently enabled or not.
        /// </summary>
        public bool RepeatEnabled { get; set; }

        /// <summary>
        /// Creates a new instance of a KKeyboard component
        /// </summary>
        /// <param name="aParent">The KInputService parent</param>
        public KKeyboardInputComponent(KInputService aParent) : base(aParent)
        {
            _lastKeys = new List<Keys>();
            _pressedKeys = new List<Keys>();
            KeyArgs = new KKeyboardEventArgs();
            RepeatDelay = 1000;
            RepeatEnabled = false;
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        public override void Initialize()
        {

        }
        #region IKInputComponent Members

        /// <summary>
        /// Checks for key press events.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            processKeys(gameTime);
        }

        #endregion

        private List<Keys> tk = new List<Keys>();
        /// <summary>
        /// Checks the pressed keys and determines which keyboard events should fire.
        /// </summary>
        /// <param name="aGameTime"></param>
        private void processKeys(GameTime aGameTime)
        {
            
            foreach (Keys k in Keyboard.GetState().GetPressedKeys())
            {
                if (!_pressedKeys.Contains(k))
                {
                    _pressedKeys.Add(k);
                }

                
                if (!_lastKeys.Contains(k) || (RepeatEnabled && GetDuration(k).TotalMilliseconds > RepeatDelay))
                {
                    SetStartTime(k);
                    
                    KeyArgs.Key = k;

                    KeyArgs.EventType = KInputEventType.KeyDown; 

                    KeyArgs.KeyboardState = Keyboard.GetState();
                    KeyArgs.Duration = TimeSpan.FromMilliseconds(0);
                    if (RepeatEnabled && GetDuration(k).TotalMilliseconds > RepeatDelay)
                        KeyArgs.Repeating = true;
                    else
                        KeyArgs.Repeating = false;
                    if (!_lastKeys.Contains(k))
                        _lastKeys.Add(k);

                    FireEvent(KeyArgs);
                    
                }
                
            }

            tk.Clear();
            tk.AddRange(_lastKeys);
            tk.ForEach(delegate(Keys k)
            {
                if (!_pressedKeys.Contains(k))
                {
                    
                    KeyArgs.Key = k;
                    KeyArgs.EventType = KInputEventType.KeyUp;
                    KeyArgs.KeyboardState = Keyboard.GetState();
                    KeyArgs.Duration = ClearStartTime(k);
                    _lastKeys.Remove(k);


                    FireEvent(KeyArgs);
                    
                }
            });

            _pressedKeys.Clear();
        }

        /// <summary>
        /// Returns whether the specified key is currently pressed or not.
        /// </summary>
        /// <param name="anObject">The key to check.</param>
        /// <returns>True if the key is currently pressed</returns>
        public override bool IsDown(object anObject)
        {
            if (anObject.GetType() != typeof(Keys))
                return false;
            else
                return _lastKeys.Contains((Keys)anObject);
        }

        /// <summary>
        /// Prepares and fires a keyboard event.
        /// </summary>
        /// <param name="args">The keyboard event args to fire.</param>
        internal void FireEvent(KKeyboardEventArgs args)
        {
            args.ShiftState = KShiftState.None;
            
            if (args.KeyboardState.IsKeyDown(Keys.LeftAlt) ||
                args.KeyboardState.IsKeyDown(Keys.RightAlt))
                args.ShiftState = KShiftState.Alt;

            if (args.KeyboardState.IsKeyDown(Keys.LeftControl) ||
                args.KeyboardState.IsKeyDown(Keys.RightControl))
                args.ShiftState = KShiftState.Control;

            if (args.KeyboardState.IsKeyDown(Keys.LeftShift) ||
                args.KeyboardState.IsKeyDown(Keys.RightShift))
                args.ShiftState = KShiftState.Shift;
            
            bool fired = false;
            var objects = from obj in _parent.EventBindings 
                          where
                            ((IKInputObject)obj.Key.InputObject).Visible
                            && ((IKInputObject)obj.Key.InputObject).HasFocus
                            && ((IKInputObject)obj.Key.InputObject).InputEnabled
                          select obj.Key;

            foreach (KInputEventBinding k in objects)
            {
                if (k.EventType == args.EventType && _parent.EventBindings.ContainsKey(k))
                {
                    _parent.EventBindings[k](k.InputObject, args);
                    fired = true;
                }
            }

            _parent.FireEvent(args, fired);
        }

    }
}