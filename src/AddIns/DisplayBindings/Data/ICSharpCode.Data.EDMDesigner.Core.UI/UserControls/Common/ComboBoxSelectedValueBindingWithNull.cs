// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common
{
    public class ComboBoxSelectedValueBindingWithNull<T> where T : class
    {
        public ComboBoxSelectedValueBindingWithNull(object defaultValue, Action<T> setProperty)
        {
            if (defaultValue != null)
                ComboSelectedValue = defaultValue;
            SetProperty = setProperty;
        }

        public Action<T> SetProperty { get; private set; }

        private object _comboSelectedValue;
        public object ComboSelectedValue
        {
            get { return _comboSelectedValue; }
            set 
            { 
                _comboSelectedValue = value;
                if (SetProperty != null)
                {
                    T t = value as T;
                    if (t != null)
                        SetProperty(t);
                    else if (value is NullValue)
                        SetProperty(null);
                }
            }
        }
    }
}
