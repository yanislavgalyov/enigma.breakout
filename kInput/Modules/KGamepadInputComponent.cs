using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// Class to handle gamepad input events.
    /// </summary>
    public class KGamePadInputComponent : KBaseInputComponent
    {
        PlayerIndex _playerIndex;
        GamePadState _currentState;
        GamePadState _lastState;
        KGamePadEventArgs _gamePadArgs;

        /// <summary>
        /// The amount of time in milliseconds to wait after a button has been pressed, to send another button
        /// press event.
        /// </summary>
        public int RepeatDelay { get; set; }

        /// <summary>
        /// Creates a new instance of a KGamePadInputComponent
        /// </summary>
        /// <param name="aParent">The parent input service</param>
        /// <param name="aPlayerIndex">The player index of this game pad object</param>
        public KGamePadInputComponent(KInputService aParent, PlayerIndex aPlayerIndex)
            : base(aParent)
        {
            RepeatDelay = 500;
            _playerIndex = aPlayerIndex;
            _gamePadArgs = new KGamePadEventArgs();
            _lastState = GamePad.GetState(_playerIndex);
            _gamePadArgs.PlayerIndex = _playerIndex;
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Performs checks to determine which gamepad events have occurred.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _currentState = GamePad.GetState(_playerIndex);

            if (!_currentState.IsConnected)
                return;

            CheckButton(Buttons.A);
            CheckButton(Buttons.B);
            CheckButton(Buttons.Back);
            CheckButton(Buttons.BigButton);
            CheckButton(Buttons.Start);
            CheckButton(Buttons.X);
            CheckButton(Buttons.Y);
            CheckButton(Buttons.LeftShoulder);
            CheckButton(Buttons.RightShoulder);
            CheckButton(Buttons.DPadDown);
            CheckButton(Buttons.DPadLeft);
            CheckButton(Buttons.DPadRight);
            CheckButton(Buttons.DPadUp);
            CheckButton(Buttons.LeftStick);
            CheckButton(Buttons.RightStick);
            CheckButton(Buttons.LeftThumbstickDown);
            CheckButton(Buttons.LeftThumbstickLeft);
            CheckButton(Buttons.LeftThumbstickRight);
            CheckButton(Buttons.LeftThumbstickUp);
            CheckButton(Buttons.LeftTrigger);
            CheckButton(Buttons.RightThumbstickDown);
            CheckButton(Buttons.RightThumbstickLeft);
            CheckButton(Buttons.RightThumbstickRight);
            CheckButton(Buttons.RightThumbstickUp);
            CheckButton(Buttons.RightTrigger);

            CheckLeftStick();
            CheckRightStick();
            CheckLeftTrigger();
            CheckRightTrigger();

            _lastState = _currentState;
        }

        /// <summary>
        /// Checks the left stick to determine whether the value has changed or not, and fires the 
        /// GamePadLeftStickMoved Event
        /// </summary>
        internal void CheckLeftStick()
        {
            if (_currentState.ThumbSticks.Left.X != _lastState.ThumbSticks.Left.X ||
                _currentState.ThumbSticks.Left.Y != _lastState.ThumbSticks.Left.Y)
            {
                SetStartTime(KInputEventType.GamePadLeftStickMoved);
                SetupEventArgs(ref _gamePadArgs, Buttons.LeftStick, KInputEventType.GamePadLeftStickMoved);
                FireEvent(_gamePadArgs);
            }

            if (_currentState.ThumbSticks.Left.X == 0 &&
                _currentState.ThumbSticks.Left.Y == 0)
            {
                ClearStartTime(KInputEventType.GamePadLeftStickMoved);
            }
        
        }

        /// <summary>
        /// Checks the right stick to determine whether the value has changed or not, and fires the 
        /// GamePadRightStickMoved Event
        /// </summary>
        internal void CheckRightStick()
        {
            if (_currentState.ThumbSticks.Right.X != _lastState.ThumbSticks.Right.X ||
                _currentState.ThumbSticks.Right.Y != _lastState.ThumbSticks.Right.Y)
            {
                SetStartTime(KInputEventType.GamePadRightStickMoved);
                SetupEventArgs(ref _gamePadArgs, Buttons.RightStick, KInputEventType.GamePadRightStickMoved);
                FireEvent(_gamePadArgs);
            }

            if (_currentState.ThumbSticks.Right.X == 0 &&
                _currentState.ThumbSticks.Right.Y == 0)
            {
                ClearStartTime(KInputEventType.GamePadRightStickMoved);
            }


        }

        /// <summary>
        /// Checks whether or not a button press event is being repeated because the elapsed milliseconds
        /// from the start time has exceeded the repeat delay
        /// </summary>
        /// <param name="aButton"></param>
        /// <returns></returns>
        internal bool CheckRepeat(Buttons aButton)
        {
            TimeSpan t;
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (_startTimes.ContainsKey(aButton))
                t = _startTimes[aButton];
            else return false;

            if (now.Subtract(t).Milliseconds > RepeatDelay)
            {
                _startTimes[aButton] = DateTime.Now.TimeOfDay;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks whether a button has been pressed or released.
        /// </summary>
        /// <param name="aButton">The button to check</param>
        internal void CheckButton(Buttons aButton)
        {
            if ((_currentState.IsButtonDown(aButton) && !_lastState.IsButtonDown(aButton))
                || CheckRepeat(aButton))
            {
                SetStartTime(aButton);
                SetupEventArgs(ref _gamePadArgs, aButton, KInputEventType.GamePadButtonDown);
                FireEvent(_gamePadArgs);
            }
            else if (_lastState.IsButtonDown(aButton) && !_currentState.IsButtonDown(aButton))
            {
                
                SetupEventArgs(ref _gamePadArgs, aButton, KInputEventType.GamePadButtonUp);
                
                _gamePadArgs.Duration = ClearStartTime(aButton);
                FireEvent(_gamePadArgs);
            }

        }

        /// <summary>
        /// cheks whether the left trigger value has changed and fires the GamePadLeftTriggerChanged event if it has
        /// </summary>
        internal void CheckLeftTrigger()
        {
            if (_currentState.Triggers.Left != _lastState.Triggers.Left)
            {
                SetStartTime(KInputEventType.GamePadLeftTriggerChanged);
                SetupEventArgs(ref _gamePadArgs, Buttons.LeftTrigger, KInputEventType.GamePadLeftTriggerChanged);
                FireEvent(_gamePadArgs);

            }

            if (_currentState.Triggers.Left == 0)
                ClearStartTime(KInputEventType.GamePadLeftTriggerChanged);
        }

        /// <summary>
        /// cheks whether the right trigger value has changed and fires the GamePadRightTriggerChanged event if it has
        /// </summary>
        internal void CheckRightTrigger()
        {
            if (_currentState.Triggers.Right != _lastState.Triggers.Right)
            {
                SetStartTime(KInputEventType.GamePadRightTriggerChanged);
                SetupEventArgs(ref _gamePadArgs, Buttons.RightTrigger, KInputEventType.GamePadRightTriggerChanged);
                FireEvent(_gamePadArgs);
            }

            if (_currentState.Triggers.Right == 0)
                ClearStartTime(KInputEventType.GamePadRightTriggerChanged);
        }

        /// <summary>
        /// sets up the event args with event specific information.
        /// </summary>
        /// <param name="aGamePadArgs">The game pad event args to set up</param>
        /// <param name="aGamePadButtons">The game pad button causing the event</param>
        /// <param name="anEventType">The event that is about to be fired.</param>
        public void SetupEventArgs(ref KGamePadEventArgs aGamePadArgs, Buttons aGamePadButtons, KInputEventType anEventType)
        {
            aGamePadArgs.EventType = anEventType;
            aGamePadArgs.GamePadButton = aGamePadButtons;
            aGamePadArgs.GamePadState = _currentState;
            aGamePadArgs.Duration = TimeSpan.FromMilliseconds(0);
        }

        /// <summary>
        /// Gets whether a game pad button is currently pressed.
        /// </summary>
        /// <param name="anObject">The game pad button to check if pressed.</param>
        /// <returns>True if the game pad button is pressed.</returns>
        public override bool IsDown(object anObject)
        {
            if (anObject.GetType() != typeof(Buttons))
                return false;
            else return _currentState.IsButtonDown((Buttons)anObject);
        }
        /// <summary>
        /// Fires an event.
        /// </summary>
        /// <param name="args"></param>
        internal void FireEvent(KInputEventArgs args)
        {
            _parent.FireEvent(args, false);
        }
    }
}
