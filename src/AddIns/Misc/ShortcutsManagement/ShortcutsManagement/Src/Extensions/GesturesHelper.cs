using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement
{
    public static class GesturesHelper
    {
        public static string GetGestureFromKeyEventArgument(KeyEventArgs args)
        {
            var pressedButton = new StringBuilder(); 

            // If gesture is not supported that means it was not entered fully
            var keyboardDevice = (KeyboardDevice) args.Device;
            if ((keyboardDevice.Modifiers & ModifierKeys.Control) > 0)
            {
                pressedButton.Append("Ctrl + ");
            }
            if ((keyboardDevice.Modifiers & ModifierKeys.Windows) > 0)
            {
                pressedButton.Append("Windows + ");
            }
            if ((keyboardDevice.Modifiers & ModifierKeys.Alt) > 0)
            {
                pressedButton.Append("Alt + ");
            }
            if ((keyboardDevice.Modifiers & ModifierKeys.Shift) > 0)
            {
                pressedButton.Append("Shift + ");
            }

            
            // Filter modifier keys from being displayed twice (example: Control + LeftCtrl)
            
            if (Array.IndexOf(new [] { Key.LeftAlt, Key.RightAlt, 
                                       Key.LeftShift, Key.RightShift,
                                       Key.LeftCtrl, Key.RightCtrl,
                                       Key.LWin, Key.RWin,
                                       Key.System}, args.Key) < 0)
            {
                pressedButton.Append(new KeyConverter().ConvertToInvariantString(args.Key));
            }

            return pressedButton.ToString();
        }
    }
}
