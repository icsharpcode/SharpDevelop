// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
