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

using System;
using System.Windows.Input;

namespace ICSharpCode.Core
{
	/// <summary>
	/// For compatibility with SD 4.x.
	/// New code should use SimpleCommand instead.
	/// 
	/// TODO: make obsolete
	/// </summary>
	public abstract class AbstractCommand : ICommand
	{
		public object Owner { get; set; }
		
		public abstract void Run();
		
		void ICommand.Execute(object parameter)
		{
			this.Owner = parameter;
			Run();
		}
		
		event EventHandler ICommand.CanExecuteChanged {
			add { }
			remove { }
		}
		
		bool ICommand.CanExecute(object parameter)
		{
			return true;
		}
	}
	
	/// <summary>
	/// For compatibility with SD 4.x.
	/// New code should use SimpleCommand instead.
	/// 
	/// TODO: make obsolete
	/// </summary>
	public abstract class AbstractMenuCommand : ICommand
	{
		bool isEnabled = true;
		
		public virtual bool IsEnabled {
			get { return isEnabled; }
			set { isEnabled = value; }
		}
		
		public object Owner { get; set; }
		
		public abstract void Run();
		
		void ICommand.Execute(object parameter)
		{
			this.Owner = parameter;
			Run();
		}
		
		event EventHandler ICommand.CanExecuteChanged {
			add { CommandWrapper.RegisterConditionRequerySuggestedHandler(value); }
			remove { CommandWrapper.UnregisterConditionRequerySuggestedHandler(value); }
		}
		
		bool ICommand.CanExecute(object parameter)
		{
			return this.IsEnabled;
		}
	}
	
	
	/// <summary>
	/// For compatibility with SD 4.x.
	/// 
	/// TODO: make obsolete
	/// </summary>
	public abstract class AbstractCheckableMenuCommand : AbstractMenuCommand, ICheckableMenuCommand
	{
		public virtual bool IsChecked { get; set; }
		
		public override void Run()
		{
			IsChecked = !IsChecked;
		}
		
		event EventHandler ICheckableMenuCommand.IsCheckedChanged { 
			add { CommandWrapper.RegisterConditionRequerySuggestedHandler(value); }
			remove { CommandWrapper.UnregisterConditionRequerySuggestedHandler(value); }
		}
		
		bool ICheckableMenuCommand.IsChecked(object parameter)
		{
			this.Owner = parameter;
			return this.IsChecked;
		}
	}
}
