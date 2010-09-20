// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows.Input;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common
{
    public static class KeyboardUtil
    {
        public static bool CtrlDown
        {
            get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); }
        }
        public static bool ShiftDown
        {
            get { return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift); }
        }
    }
}
