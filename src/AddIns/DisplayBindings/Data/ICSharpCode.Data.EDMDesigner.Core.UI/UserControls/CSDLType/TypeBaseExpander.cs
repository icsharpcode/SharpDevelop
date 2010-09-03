// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Windows.Input;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType
{
    public class TypeBaseExpander : Expander
    {
        static TypeBaseExpander()
        {
            ResourceDictionaryLoader.LoadResourceDictionary("/UserControls/CSDLType/TypeBaseExpanderResourceDictionary.xaml");
        }
        
        protected override void ClickOnTheHeader()
        {
            base.ClickOnTheHeader();
            OnClick();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            OnClick();
        }

        protected virtual void OnClick()
        {
            if (Click != null)
                Click();
        }

        public event Action Click;
    }
}
