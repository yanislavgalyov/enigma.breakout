using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// A derived KBaseInputComponent to handle mouse input.
    /// </summary>
    public class KMouseInputComponent : KBaseInputComponent
    {
        private MouseState _lastMouseState;
        private MouseState _currentMouseState;
        private KMouseEventArgs _mouseArgs;
        private Dictionary<KInputEventType, MouseState> _eventStates;
        private Rectangle _viewPortRectangle;
        private Rectangle _mouseRect;
        private bool Dragging = false;
        private bool WasDragging = false;

        private object DragObject = null;

        /// <summary>
        /// Defines ending events matching beginning events.
        /// </summary>
        private Dictionary<KInputEventType, KInputEventType> _eventMatches;
        private IKInputObject _lastMouseOverObject;
        private IKInputObject _mouseOverObject;

        /// <summary>
        /// The amount of time to wait after a mouseover event to start a hoverdelay event
        /// </summary>
        public int HoverDelay { get; set; }

        /// <summary>
        /// The amount of time to wait after a hoverdelay event to fire a timeout event.
        /// </summary>
        public int HoverTimeout { get; set; }
        bool HoverOn = false;

        /// <summary>
        /// Creates a new instance of a KMouseInputcomponent
        /// </summary>
        /// <param name="aParent">The KInputService parent</param>
        public KMouseInputComponent(KInputService aParent) : base(aParent)
        {
            HoverDelay = 1000;
            HoverTimeout = 2000;

            _lastMouseState = Mouse.GetState();
            _eventStates = new Dictionary<KInputEventType, MouseState>();
            _mouseArgs = new KMouseEventArgs();
        }

        /// <summary>
        /// Initializes the Mouse input component.
        /// </summary>
        public override void Initialize()
        {
            _viewPortRectangle = new Rectangle(_parent.Game.GraphicsDevice.Viewport.X,
                                               _parent.Game.GraphicsDevice.Viewport.Y,
                                               _parent.Game.GraphicsDevice.Viewport.Width,
                                               _parent.Game.GraphicsDevice.Viewport.Height);

            _eventMatches = new Dictionary<KInputEventType, KInputEventType>();
            _eventMatches.Add(KInputEventType.LeftMouseUp, KInputEventType.LeftMouseDown);
            _eventMatches.Add(KInputEventType.RightMouseUp, KInputEventType.RightMouseDown);
            _eventMatches.Add(KInputEventType.MiddleMouseUp, KInputEventType.MiddleMouseDown);
            _eventMatches.Add(KInputEventType.XButton1MouseUp, KInputEventType.XButton1MouseDown);
            _eventMatches.Add(KInputEventType.XButton2MouseUp, KInputEventType.XButton2MouseDown);
            _eventMatches.Add(KInputEventType.LeftDragEnd, KInputEventType.LeftDragStart);
            _eventMatches.Add(KInputEventType.MouseOut, KInputEventType.MouseOver);
            _eventMatches.Add(KInputEventType.HoverTimeout, KInputEventType.HoverDelay);

        }

        /// <summary>
        /// Inserts or updates the mouse state as it existed when the event started.
        /// </summary>
        /// <param name="anEvent">The event to save the mouse state for</param>
        /// <param name="aMouseState">The mousestate associated with the event</param>
        private void SetEventState(KInputEventType anEvent, MouseState aMouseState)
        {
            if (_eventMatches.ContainsKey(anEvent))
                anEvent = _eventMatches[anEvent];

            if (_eventStates.ContainsKey(anEvent))
                _eventStates[anEvent] = aMouseState;
            else
                _eventStates.Add(anEvent, aMouseState);
        }

        /// <summary>
        /// Clears the event state for an event.
        /// </summary>
        /// <param name="anEvent">The event to clear the state for</param>
        private void ClearEventState(KInputEventType anEvent)
        {
            if (_eventMatches.ContainsKey(anEvent))
                anEvent = _eventMatches[anEvent];

            if (_eventStates.ContainsKey(anEvent))
                _eventStates.Remove(anEvent);
        }

        /// <summary>
        /// Sets up the event args for a mouse event.
        /// </summary>
        /// <param name="args">The args to set up</param>
        /// <param name="aButton">The Button to set up the args for</param>
        /// <param name="anEvent">The event to set up the args for</param>
        private void SetupMouseEvent(ref KMouseEventArgs args, KMouseButtons aButton, KInputEventType anEvent)
        {
            MouseState tmpState;
            KInputEventType preEvent;

            if (_eventMatches.ContainsKey(anEvent))
                preEvent = _eventMatches[anEvent];
            else
                preEvent = anEvent;

            if (_eventStates.ContainsKey(preEvent))
                tmpState = _eventStates[preEvent];
            else
                tmpState = _currentMouseState;

            args.EventType = anEvent;
            args.MouseState = _currentMouseState;
            args.StartPos = new Microsoft.Xna.Framework.Vector2(tmpState.X, tmpState.Y);
            args.EndPos = new Microsoft.Xna.Framework.Vector2(_currentMouseState.X, _currentMouseState.Y);
            args.MouseButton = aButton;
            

        }
        #region IKInputComponent Members

        /// <summary>
        /// Checks to see if the current mouse button is down or not.
        /// </summary>
        /// <param name="anObject">The mouse button to check if pressed.</param>
        /// <returns>True if the mouse button is currently pressed.</returns>
        public override bool IsDown(object anObject)
        {
            if (GetDuration(anObject).TotalMilliseconds > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// invokes methods to determine which mouse events have occurred.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();
            _mouseRect = new Rectangle(_currentMouseState.X, _currentMouseState.Y, 1, 1);

            if (!_mouseRect.Intersects(_viewPortRectangle))
                return;

            WasDragging = Dragging;
            GetMouseObjects();
            ProcessMouseDown();
            ProcessMouseUp();
            ProcessMouseMove();
            

            _lastMouseState = _currentMouseState;
        }

        #endregion
        /// <summary>
        /// Determines whether a mouse down event has occurred.
        /// </summary>
        internal void ProcessMouseDown()
        {
            if (_lastMouseState.LeftButton != ButtonState.Pressed && _currentMouseState.LeftButton == ButtonState.Pressed)
            {
                SetEventState(KInputEventType.LeftMouseDown, _currentMouseState);
                SetStartTime(KMouseButtons.LeftButton);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.LeftButton, KInputEventType.LeftMouseDown);
                FireEvent(_mouseArgs);
            }

            if (_lastMouseState.RightButton != ButtonState.Pressed && _currentMouseState.RightButton == ButtonState.Pressed)
            {
                SetEventState(KInputEventType.RightMouseDown, _currentMouseState);
                SetStartTime(KMouseButtons.RightButton);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.RightButton, KInputEventType.RightMouseDown);
                FireEvent(_mouseArgs);
            }

            if (_lastMouseState.MiddleButton != ButtonState.Pressed && _currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                SetEventState(KInputEventType.MiddleMouseDown, _currentMouseState);
                SetStartTime(KMouseButtons.MiddleButton);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.MiddleButton, KInputEventType.MiddleMouseDown);
                FireEvent(_mouseArgs);
            }

            if (_lastMouseState.XButton1 != ButtonState.Pressed && _currentMouseState.XButton1 == ButtonState.Pressed)
            {
                SetEventState(KInputEventType.XButton1MouseDown, _currentMouseState);
                SetStartTime(KMouseButtons.XButton1);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton1, KInputEventType.XButton1MouseDown);
                FireEvent(_mouseArgs);
            }

            if (_lastMouseState.XButton2 != ButtonState.Pressed && _currentMouseState.XButton2 == ButtonState.Pressed)
            {
                SetEventState(KInputEventType.XButton2MouseDown, _currentMouseState);
                SetStartTime(KMouseButtons.XButton2);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton2, KInputEventType.XButton2MouseDown);
                FireEvent(_mouseArgs);
            }
        }

        /// <summary>
        /// Determines whether a mouse up event has occurred.
        /// </summary>
        internal void ProcessMouseUp()
        {
            if (_lastMouseState.LeftButton == ButtonState.Pressed && _currentMouseState.LeftButton != ButtonState.Pressed)
            {
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.LeftButton, KInputEventType.LeftMouseUp);
                _mouseArgs.Duration = ClearStartTime(KMouseButtons.LeftButton);
                FireEvent(_mouseArgs);
                
                ClearEventState(KInputEventType.LeftMouseUp);
                ClearEventState(KInputEventType.MouseMove);

                if (_eventStates.ContainsKey(KInputEventType.LeftDragStart))
                {
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.LeftButton, KInputEventType.LeftDragEnd);
                    _mouseArgs.Duration = ClearStartTime(KInputEventType.LeftDragStart);
                    FireEvent(_mouseArgs);
                    Dragging = false;
                    DragObject = null;
                    ClearEventState(KInputEventType.LeftDragEnd);
                }
            }

            if (_lastMouseState.RightButton == ButtonState.Pressed && _currentMouseState.RightButton != ButtonState.Pressed)
            {
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.RightButton, KInputEventType.RightMouseUp);
                _mouseArgs.Duration= ClearStartTime(KMouseButtons.RightButton);
                FireEvent(_mouseArgs);
                
                ClearEventState(KInputEventType.RightMouseUp);
                ClearEventState(KInputEventType.MouseMove);

                if (_eventStates.ContainsKey(KInputEventType.RightDragStart))
                {
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.RightButton, KInputEventType.RightDragEnd);
                    _mouseArgs.Duration = ClearStartTime(KInputEventType.RightDragStart);
                    FireEvent(_mouseArgs);
                    
                    ClearEventState(KInputEventType.RightDragEnd);
                    Dragging = false;
                    DragObject = null;
                }
            }

            if (_lastMouseState.MiddleButton == ButtonState.Pressed && _currentMouseState.MiddleButton != ButtonState.Pressed)
            {
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.MiddleButton, KInputEventType.MiddleMouseUp);
                _mouseArgs.Duration = ClearStartTime(KMouseButtons.MiddleButton);
                FireEvent(_mouseArgs);
                
                ClearEventState(KInputEventType.MiddleMouseUp);
                ClearEventState(KInputEventType.MouseMove);

                if (_eventStates.ContainsKey(KInputEventType.MiddleDragStart))
                {
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.MiddleButton, KInputEventType.MiddleDragEnd);
                    _mouseArgs.Duration = ClearStartTime(KInputEventType.MiddleDragStart);
                    FireEvent(_mouseArgs);
                    Dragging = false;
                    DragObject = null;
                    ClearEventState(KInputEventType.MiddleDragEnd);
                }
            }

            if (_lastMouseState.XButton1 == ButtonState.Pressed && _currentMouseState.XButton1 != ButtonState.Pressed)
            {
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton1, KInputEventType.XButton1MouseUp);
                _mouseArgs.Duration = ClearStartTime(KMouseButtons.XButton1);
                FireEvent(_mouseArgs);
                
                ClearEventState(KInputEventType.XButton1MouseUp);
                ClearEventState(KInputEventType.MouseMove);

                if (_eventStates.ContainsKey(KInputEventType.XButton1DragStart))
                {
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton1, KInputEventType.XButton1DragEnd);
                    _mouseArgs .Duration = ClearStartTime(KInputEventType.XButton1DragStart);
                    FireEvent(_mouseArgs);
                    Dragging = false;
                    DragObject = null;
                    ClearEventState(KInputEventType.XButton1DragEnd);
                }
            }

            if (_lastMouseState.XButton2 == ButtonState.Pressed && _currentMouseState.XButton2 != ButtonState.Pressed)
            {
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton2, KInputEventType.XButton2MouseUp);
                _mouseArgs.Duration = ClearStartTime(KMouseButtons.XButton2);
                FireEvent(_mouseArgs);
                
                ClearEventState(KInputEventType.XButton2MouseUp);
                ClearEventState(KInputEventType.MouseMove);

                if (_eventStates.ContainsKey(KInputEventType.XButton2DragStart))
                {
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton2, KInputEventType.XButton2DragEnd);

                    _mouseArgs.Duration = ClearStartTime(KInputEventType.XButton2DragStart); 
                    FireEvent(_mouseArgs);
                    Dragging = false;
                    DragObject = null;
                    ClearEventState(KInputEventType.XButton2DragEnd);
                }
            }
        }

        /// <summary>
        /// Determines whether a mouse move event has occurred.
        /// </summary>
        internal void ProcessMouseMove()
        {
            Vector2 anEndPos = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            Vector2 aStartPos = new Vector2(_lastMouseState.X, _lastMouseState.Y);

            if (aStartPos != anEndPos)
            {

                SetEventState(KInputEventType.MouseMove, _currentMouseState);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.NoButton, KInputEventType.MouseMove);
                _mouseArgs.StartPos = aStartPos;
                _mouseArgs.EndPos = anEndPos;
                FireEvent(_mouseArgs);
                ClearEventState(KInputEventType.MouseMove);

                if (_currentMouseState.LeftButton == ButtonState.Pressed && !_eventStates.ContainsKey(KInputEventType.LeftDragStart))
                {
                    SetEventState(KInputEventType.LeftDragStart, _currentMouseState);
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.LeftButton, KInputEventType.LeftDragStart);
                    Dragging = true;
                    DragObject = _mouseOverObject;
                    FireEvent(_mouseArgs);
                }

                if (_currentMouseState.RightButton == ButtonState.Pressed && !_eventStates.ContainsKey(KInputEventType.RightDragStart))
                {
                    SetEventState(KInputEventType.RightDragStart, _currentMouseState);
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.RightButton, KInputEventType.RightDragStart);
                    Dragging = true;
                    DragObject = _mouseOverObject;
                    FireEvent(_mouseArgs);
                }

                if (_currentMouseState.MiddleButton == ButtonState.Pressed && !_eventStates.ContainsKey(KInputEventType.MiddleDragStart))
                {
                    SetEventState(KInputEventType.MiddleDragStart, _currentMouseState);
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.MiddleButton, KInputEventType.MiddleDragStart);
                    Dragging = true;
                    DragObject = _mouseOverObject;
                    FireEvent(_mouseArgs);
                }

                if (_currentMouseState.XButton1 == ButtonState.Pressed && !_eventStates.ContainsKey(KInputEventType.XButton1DragStart))
                {
                    SetEventState(KInputEventType.XButton1DragStart, _currentMouseState);
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton1, KInputEventType.XButton1DragStart);
                    Dragging = true;
                    DragObject = _mouseOverObject;
                    FireEvent(_mouseArgs);
                }

                if (_currentMouseState.XButton2 == ButtonState.Pressed && !_eventStates.ContainsKey(KInputEventType.XButton2DragStart))
                {
                    SetEventState(KInputEventType.XButton2DragStart, _currentMouseState);
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.XButton2, KInputEventType.XButton2DragStart);
                    Dragging = true;
                    DragObject = _mouseOverObject;
                    FireEvent(_mouseArgs);
                }

            }
        }

        /// <summary>
        /// Determines which mouse object should currently receive events.
        /// </summary>
        internal void GetMouseObjects()
        {
            if (Dragging) return;

            _lastMouseOverObject = _mouseOverObject;
            _mouseOverObject = null;

            var objects = 
                from obj 
                    in _parent.EventBindings 
                    where 
                       obj.Key.InputObject.Visible
                       && obj.Key.InputObject.InputEnabled
                       && _mouseRect.Intersects(new Rectangle((int)obj.Key.InputObject.Position.X, (int)obj.Key.InputObject.Position.Y, (int)obj.Key.InputObject.Rectangle.Width, (int)obj.Key.InputObject.Rectangle.Height))
                select obj.Key.InputObject;

            float maxZ = -2;
            foreach (IKInputObject obj in objects)
            {
                Vector3 position = obj.Position;
                if (position.Z > maxZ)
                {
                    maxZ = position.Z;
                    _mouseOverObject = obj;
                }
            }
            
            if (_lastMouseOverObject == null && _mouseOverObject == null)
                return;

            if (_lastMouseOverObject != _mouseOverObject)
            {
                HoverOn = false;
                if (_lastMouseOverObject != null)
                    DoHoverOff(true, _lastMouseOverObject);

                if (_lastMouseOverObject != null)
                {
                    ClearEventState(KInputEventType.MouseOut);
                    
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.NoButton, KInputEventType.MouseOut);
                    _mouseArgs.Duration = ClearStartTime(KInputEventType.MouseOut);
                    FireEvent(_mouseArgs, _lastMouseOverObject);
                }

                if (_mouseOverObject != null)
                {
                    SetEventState(KInputEventType.MouseOver, _currentMouseState);
                    SetStartTime(KInputEventType.MouseOver);
                    SetupMouseEvent(ref _mouseArgs, KMouseButtons.NoButton, KInputEventType.MouseOver);
                    FireEvent(_mouseArgs);
                }
            }
            else if (_lastMouseOverObject == _mouseOverObject)
            {
                DoHoverOn();
                DoHoverOff(false, _mouseOverObject);
            }

        }

        /// <summary>
        /// Determines whether a hovertimeout event should fire
        /// </summary>
        /// <param name="forceOff">Force the event to fire</param>
        /// <param name="anObject">The object to fire the event for</param>
        private void DoHoverOff(bool forceOff, IKInputObject anObject)
        {
            if (!_startTimes.ContainsKey(KInputEventType.HoverDelay) ||
                (DateTime.Now.TimeOfDay.Subtract(_startTimes[KInputEventType.HoverDelay]).TotalMilliseconds < HoverTimeout
                && !forceOff))
                return;
            
            ClearEventState(KInputEventType.HoverDelay);
            
            SetupMouseEvent(ref _mouseArgs, KMouseButtons.NoButton, KInputEventType.HoverTimeout);
            _mouseArgs.Duration = ClearStartTime(KInputEventType.HoverDelay);
            FireEvent(_mouseArgs, anObject);
        }

        /// <summary>
        /// Determines whether a hoverdelay event should fire
        /// </summary>
        internal void DoHoverOn()
        {
            if (!_startTimes.ContainsKey(KInputEventType.MouseOver) || _startTimes.ContainsKey(KInputEventType.HoverDelay) || HoverOn)
                return;

            TimeSpan mouseOver = _startTimes[KInputEventType.MouseOver];
            
            if (DateTime.Now.TimeOfDay.Subtract(mouseOver).TotalMilliseconds > HoverDelay)
            {
                HoverOn = true;
                SetStartTime(KInputEventType.HoverDelay);
                SetEventState(KInputEventType.HoverDelay, _currentMouseState);
                SetupMouseEvent(ref _mouseArgs, KMouseButtons.NoButton, KInputEventType.HoverDelay);
                FireEvent(_mouseArgs);
            }
        }

        /// <summary>
        /// Fires a mouse event specefied by the args
        /// </summary>
        /// <param name="args">The mouse event args to pass to the event</param>
        /// <param name="anEventObject">The object to fire the event for.  This could be the currently selected mouse object, or the last mouse object</param>
        internal void FireEvent(KMouseEventArgs args, IKInputObject anEventObject)
        {
            bool fired = false;
            args.Dragging = Dragging;
            args.DragObject = DragObject;

            if (anEventObject != null && !WasDragging)
            {
                Vector3 position = (Vector3)_parent.GetReflectedProperty(anEventObject, "Position", Vector3.Zero);

                args.Offset = new Vector2(args.MouseState.X, args.MouseState.Y) - new Vector2(position.X, position.Y);
            }

            if (anEventObject != null)
            {
                KInputEventBinding kb = new KInputEventBinding(anEventObject, args.EventType);
                if(_parent.EventBindings.ContainsKey(kb))
                {
                    fired = true;
                    _parent.EventBindings[kb](anEventObject, args);
                }
            }

            _parent.FireEvent(args, fired);

        }

        /// <summary>
        /// Fires an event on the current mouse object.
        /// </summary>
        /// <param name="args">The mouse event args to pass to the event</param>
        internal void FireEvent(KMouseEventArgs args)
        {
            FireEvent(args, _mouseOverObject);
        }
    }
}
