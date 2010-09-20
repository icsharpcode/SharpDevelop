// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	public abstract class AbstractComboBoxCommand : AbstractCommand, IComboBoxCommand
	{
		bool isEnabled = true;
		object comboBox;
		
		public virtual bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
			}
		}
		
		public virtual object ComboBox {
			get { return comboBox; }
			set { 
				comboBox = value;
				OnComboBoxChanged();
			}
		}
		
		protected virtual void OnComboBoxChanged()
		{
		}
		
		public override void Run()
		{
			
		}
	}
}
