// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
