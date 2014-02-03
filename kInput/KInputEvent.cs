using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Kraken.Xna.Input
{
    /// <summary>
    /// An enum containing the available event types for input processing
    /// </summary>
    public enum KInputEventType
    {
        /// <summary>
        /// occurs when a key is pressed
        /// </summary>
        KeyDown,
        /// <summary>
        /// Occurs when a key is released
        /// </summary>
        KeyUp,
        /// <summary>
        /// Occurs when the mouse is moved over an object
        /// </summary>
        MouseOver,
        /// <summary>
        /// Occurs when the mouse is moved off an object
        /// </summary>
        MouseOut,
        /// <summary>
        /// Occurs when the mouse is moved on an object
        /// </summary>
        MouseMove,
        /// <summary>
        /// occurs when the left mouse button is pressed
        /// </summary>
        LeftMouseDown,
        /// <summary>
        /// occurs when the left mouse button is released
        /// </summary>
        LeftMouseUp,
        /// <summary>
        /// occurs when the right mouse button is pressed
        /// </summary>
        RightMouseDown,
        /// <summary>
        /// occurs when the right mouse button is released
        /// </summary>
        RightMouseUp,
        /// <summary>
        /// Occurs when the middle mouse button is pressed
        /// </summary>
        MiddleMouseDown,
        /// <summary>
        /// Occurs when the middle mouse button is released
        /// </summary>
        MiddleMouseUp,
        /// <summary>
        /// Occurs when the XButton1 mouse button is pressed
        /// </summary>
        XButton1MouseDown,
        /// <summary>
        /// Occurs when the XButton1 mouse button is released
        /// </summary>
        XButton1MouseUp,
        /// <summary>
        /// Occurs when the XButton2 mouse button is pressed
        /// </summary>
        XButton2MouseDown,
        /// <summary>
        /// Occurs when the XButton2 mouse button is release
        /// </summary>
        XButton2MouseUp,
        /// <summary>
        /// occurs when the mouse begins dragging with the left mouse button
        /// </summary>
        LeftDragStart,
        /// <summary>
        /// occurs when the mouse is done dragging with the left mouse button
        /// </summary>
        LeftDragEnd,
        /// <summary>
        /// occurs when the mouse begins dragging with the right mouse button
        /// </summary>
        RightDragStart,
        /// <summary>
        /// occurs when the mouse is done dragging with the right mouse button
        /// </summary>
        RightDragEnd,
        /// <summary>
        /// occurs when the mouse begins dragging with the middle mouse button
        /// </summary>
        MiddleDragStart,
        /// <summary>
        /// occurs when the mouse is done dragging with the middle mouse button
        /// </summary>
        MiddleDragEnd,
        /// <summary>
        /// occurs when the mouse begins dragging with the XButton1 mouse button
        /// </summary>
        XButton1DragStart,
        /// <summary>
        /// occurs when the mouse is done dragging with the XButton1 mouse button
        /// </summary>
        XButton1DragEnd,
        /// <summary>
        /// occurs when the mouse begins dragging with the xbutton2 mouse button
        /// </summary>
        XButton2DragStart,
        /// <summary>
        /// occurs when the mouse is done dragging with the xbutton2 mouse button
        /// </summary>
        XButton2DragEnd,
        /// <summary>
        /// occurs when the mouse has hovered over the object for a specified period of time
        /// </summary>
        HoverDelay,
        /// <summary>
        /// occurs when the hover over has timed out
        /// </summary>
        HoverTimeout,
        /// <summary>
        /// Occurs when a game pad button is pressed
        /// </summary>
        GamePadButtonDown,
        /// <summary>
        /// Occurs when a game pad button is released
        /// </summary>
        GamePadButtonUp,
        /// <summary>
        /// Occurs when the left thubstick is moved
        /// </summary>
        GamePadLeftStickMoved,
        /// <summary>
        /// Occurs when the right thumbstick is moved
        /// </summary>
        GamePadRightStickMoved,
        /// <summary>
        /// Occurs when the left trigger's value is changed
        /// </summary>
        GamePadLeftTriggerChanged,
        /// <summary>
        /// Occurs when the right trigger's value is changed
        /// </summary>
        GamePadRightTriggerChanged,
        

    }

    /// <summary>
    /// Base input event argument class.
    /// </summary>
    public class KInputEventArgs : EventArgs
    {
        /// <summary>
        /// Contains the KInputEventType that is being fired
        /// </summary>
        public KInputEventType EventType { get; internal set; }

        /// <summary>
        /// Gets the duration of the event, if applicable.
        /// </summary>
        public TimeSpan Duration { get; internal set; }
    }

    /// <summary>
    /// Derived KInputEventArgs containing specific properties used within a key press event
    /// </summary>
    public class KKeyboardEventArgs : KInputEventArgs
    {
        // Keyboard event args
        /// <summary>
        /// The Key that was pressed or released
        /// </summary>
        public Keys Key;

        /// <summary>
        /// The state of the keyboard at the time of the event
        /// </summary>
        public KeyboardState KeyboardState;

        /// <summary>
        /// The shiftstate at the time of the event.
        /// </summary>
        public KShiftState ShiftState;

        /// <summary>
        /// Returns whether the current event is a repeat key event
        /// </summary>
        public bool Repeating;

    }

    /// <summary>
    /// Derived KInputEventArgs with specialized properties used in mouse event handlers
    /// </summary>
    public class KMouseEventArgs : KInputEventArgs
    {
        // mouse event args

        /// <summary>
        /// The mouse button that is the subject of the event
        /// </summary>
        public KMouseButtons MouseButton;

        /// <summary>
        /// The mouse state at the time the event was fired
        /// </summary>
        public MouseState MouseState;

        /// <summary>
        /// The starting mouse position of the event
        /// </summary>
        public Vector2 StartPos;

        /// <summary>
        /// The ending mouse position of the event
        /// </summary>
        public Vector2 EndPos;

        /// <summary>
        /// whether or not the mouse is currently dragging
        /// </summary>
        public bool Dragging;

        /// <summary>
        /// The object that the mouse is currently being dragged over
        /// </summary>
        public object DragObject;

        /// <summary>
        /// The offset from the top left corner of the mouse object where the mouse cursor was when the event started.
        /// </summary>
        public Vector2 Offset;
    }

    /// <summary>
    /// Derived KInputEventArgs with specialized properties for handling game pad events
    /// </summary>
    public class KGamePadEventArgs : KInputEventArgs
    {
        // gamepad event args

        /// <summary>
        /// The game pad button that is the subject of the event
        /// </summary>
        public Buttons GamePadButton;

        /// <summary>
        /// The gamepad state at the time of the event
        /// </summary>
        public GamePadState GamePadState;

        /// <summary>
        /// The playerindex of the gamepad that triggered the event
        /// </summary>
        public PlayerIndex PlayerIndex;
    }


    /// <summary>
    /// Enum to identify mouse buttons.  XNA does not provide a class containing mouse
    /// button definitions
    /// </summary>
    public enum KMouseButtons
    {
        /// <summary>
        /// No button is pressed, or a button being pressed isn't applicable to this event.
        /// </summary>
        NoButton,
        /// <summary>
        /// The left mouse button is the subject of the event
        /// </summary>
        LeftButton,
        /// <summary>
        /// The middle mouse button is the subject of the event.
        /// </summary>
        MiddleButton,
        /// <summary>
        /// The Right mouse button is the subject of the event.
        /// </summary>
        RightButton,
        /// <summary>
        /// The XButton1 mouse button is the subject of the event.
        /// </summary>
        XButton1,
        /// <summary>
        /// The XButton2 mouse button is the subject of the event.
        /// </summary>
        XButton2
    }
    /// <summary>
    /// A enum containing definitions for the current shift state of a keypress event
    /// </summary>
    public enum KShiftState
    {
        /// <summary>
        /// shift control and alt are not pressed
        /// </summary>
        None,
        /// <summary>
        /// shift is pressed
        /// </summary>
        Shift,
        /// <summary>
        /// alt is pressed
        /// </summary>
        Alt,
        /// <summary>
        /// control is pressed
        /// </summary>
        Control
    }



}