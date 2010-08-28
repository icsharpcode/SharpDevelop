// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Windows.Input;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Custom implementation of ICommand based on the RelayCommand model by Josh Smith for the designer.
	/// </summary>
	public class DesignCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;
        
        public DesignCommand(Action<object> action,Func<object,bool> canExecute)
        {
            Debug.Assert(action != null);
            this._action = action;
            this._canExecute = canExecute;
        }
        
        public void Execute(object parameter)
        {
            _action(parameter);
        }
        
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);
            return true;
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
