// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Martin Koníček"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ICSharpCode.SharpDevelop;

namespace Debugger.AddIn.Visualizers
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
        	var handler = PropertyChanged;
           if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
