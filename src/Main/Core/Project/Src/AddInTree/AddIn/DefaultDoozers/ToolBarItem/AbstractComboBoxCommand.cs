// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
