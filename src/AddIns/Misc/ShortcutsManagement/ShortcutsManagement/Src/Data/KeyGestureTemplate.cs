using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement
{
    public class KeyGestureTemplate
    {
        public Key Key
        {
            get; set;
        }

        public ModifierKeys Modifiers
        {
            get; set;
        }

        public KeyGestureTemplate(KeyGesture gesture)
        {
            Key = gesture.Key;
            Modifiers = gesture.Modifiers;
        }

        public KeyGestureTemplate(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public KeyGestureTemplate(Key key)
        {
            Key = key;
            Modifiers = ModifierKeys.None;
        }

        public bool Matches(KeyGesture gesture, bool strictMatch)
        {
            if(strictMatch)
            {
                return Key == gesture.Key && Modifiers == gesture.Modifiers;
            }

            var modifierMatches = gesture.Modifiers - (gesture.Modifiers ^ Modifiers) >= 0;
            var keyMatches = Key == gesture.Key;

            if (Modifiers == ModifierKeys.None)
            {
                return keyMatches;
            }

            if (Array.IndexOf(new[] { Key.LeftAlt, Key.RightAlt, 
                                       Key.LeftShift, Key.RightShift,
                                       Key.LeftCtrl, Key.RightCtrl,
                                       Key.LWin, Key.RWin,
                                       Key.System}, Key) >= 0)
            {
                return modifierMatches;
            }
            
            return modifierMatches && keyMatches;
        }
    }
}
