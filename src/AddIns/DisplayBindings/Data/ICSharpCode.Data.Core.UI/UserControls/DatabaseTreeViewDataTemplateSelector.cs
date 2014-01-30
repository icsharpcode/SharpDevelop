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
using System.Windows.Controls;
using System.Windows;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.DatabaseObjects;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    public class DatabaseTreeViewDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element == null)
                return null;
            
            if (item is DatabaseObjectBase)
            { 
                if (element != null)
                {
                    if (item is IDatabase)
                        return element.FindResource("DatabaseDataTemplate") as DataTemplate;
                    else if (item is ITable || item is IView)
                        return element.FindResource("TableDataTemplate") as DataTemplate;
                    else if (item is IColumn)
                        return element.FindResource("ColumnDataTemplate") as DataTemplate;
                    else if (item is IProcedure)
                        return element.FindResource("ProcedureDataTemplate") as DataTemplate;
                    else if (item is IProcedureParameter)
                        return element.FindResource("ProcedureParameterDataTemplate") as DataTemplate;
                }
            }

            return element.FindResource("StandardDataTemplate") as DataTemplate;
        }
    }
}
