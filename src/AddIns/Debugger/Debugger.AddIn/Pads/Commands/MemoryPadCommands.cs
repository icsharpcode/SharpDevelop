// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public sealed class JumpToAddressCommand : AbstractComboBoxCommand
	{
		MemoryPad pad;
		ComboBox comboBox;		
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			this.pad = this.Owner as MemoryPad;
			if (this.pad == null)
				return;
			
			comboBox = this.ComboBox as ComboBox;
			
			if (this.comboBox == null)
				return;
			
			comboBox.KeyUp += (s, ea) => { if (ea.Key == Key.Enter) Run(); };
			comboBox.IsEditable = true;
			comboBox.Width = 130;
			
			base.OnOwnerChanged(e);
		}
		
		public override void Run()
		{
			if (this.pad != null && this.comboBox != null) {
				pad.JumpToAddress(comboBox.Text);
			}
			base.Run();
		}
	}
	
	public abstract class ItemMemoryCommand : AbstractCommand
	{
		protected MemoryPad pad;
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			this.pad = this.Owner as MemoryPad;
			if (this.pad == null)
				return;
			
			base.OnOwnerChanged(e);
		}
	}
	
	public sealed class RefreshAddressCommand : ItemMemoryCommand
	{
		public override void Run()
		{
			if (this.pad == null)
				return;
			
			this.pad.Refresh();
		}
	}
	
	public sealed class NextAddressCommand : ItemMemoryCommand
	{
		public override void Run()
		{
			if (this.pad == null)
				return;
			
			this.pad.MoveToNextAddress();
		}
	}
	
	public sealed class PreviousAddressCommand : ItemMemoryCommand
	{
		public override void Run()
		{
			if (this.pad == null)
				return;
			
			this.pad.MoveToPreviousAddress();
		}
	}
	
	public sealed class DisplayByteSizeCommand : AbstractComboBoxCommand
	{
		MemoryPad pad;
		ComboBox comboBox;		
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			this.pad = this.Owner as MemoryPad;
			if (this.pad == null)
				return;
			
			comboBox = this.ComboBox as ComboBox;
			
			if (this.comboBox == null)
				return;
			
			comboBox.SelectionChanged += (s, ea) => { Run(); };
			
			comboBox.Items.Add(1);
			comboBox.Items.Add(2);
			comboBox.Items.Add(4);
			comboBox.Text = "1";
			comboBox.Width = 30;
			comboBox.IsEditable = false;
			
			base.OnOwnerChanged(e);
		}
		
		public override void Run()
		{
			if (this.pad != null && this.comboBox != null) {
				pad.DisplayByteSize = Convert.ToByte(this.comboBox.SelectedValue);
				pad.DisplayMemory();
			}
			base.Run();
		}
	}
}
