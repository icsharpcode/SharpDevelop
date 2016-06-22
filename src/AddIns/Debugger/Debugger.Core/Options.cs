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
using System.Collections.Generic;
using System.ComponentModel;

namespace Debugger
{
	public class Options
	{
		public virtual bool EnableJustMyCode { get; set; }
		public virtual bool SuppressJITOptimization { get; set; }
		public virtual bool SuppressNGENOptimization { get; set; }
		public virtual bool StepOverDebuggerAttributes { get; set; }
		public virtual bool StepOverAllProperties { get; set; }
		public virtual bool StepOverFieldAccessProperties { get; set; }
		public virtual IEnumerable<string> SymbolsSearchPaths { get; set; }
		public virtual bool PauseOnHandledExceptions { get; set; }
		public virtual IEnumerable<ExceptionFilterEntry> ExceptionFilterList { get; set; }
	}
	
	[Serializable]
	public class ExceptionFilterEntry : INotifyPropertyChanged
	{
		string expression;
		bool isActive;
		
		public ExceptionFilterEntry()
		{
			this.IsActive = true;
		}
		
		public ExceptionFilterEntry(string expression)
		{
			this.IsActive = true;
			this.Expression = expression;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var propertyChanged = PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public string Expression {
			get {
				return expression;
			}
			set {
				if (expression != value) {
					expression = value;
					OnPropertyChanged("Expression");
				}
			}
		}

		public bool IsActive {
			get {
				return isActive;
			}
			set {
				if (isActive != value) {
					isActive = value;
					OnPropertyChanged("IsActive");
				}
			}
		}
	}
}
