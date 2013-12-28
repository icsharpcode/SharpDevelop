// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public sealed class ShowZoomControlCommand : AbstractCheckableMenuCommand
	{
		ParallelStackPad pad;
		
		public override object Owner {
			get { return base.Owner; }
			set {
				if (!(value is ParallelStackPad))
					throw new Exception("Owner has to be a ParallelStackPad");
				pad = value as ParallelStackPad;
				base.Owner = value;
			}
		}
		
		public override bool IsChecked {
			get { return pad.IsZoomControlVisible; }
			set { pad.IsZoomControlVisible = value; }
		}
	}
	
	public sealed class ToggleMethodViewCommand : AbstractCheckableMenuCommand
	{
		ParallelStackPad pad;
		
		public override object Owner {
			get { return base.Owner; }
			set {
				if (!(value is ParallelStackPad))
					throw new Exception("Owner has to be a AbstractConsolePad");
				pad = value as ParallelStackPad;
				base.Owner = value;
			}
		}
		
		public override bool IsChecked {
			get { return pad.IsMethodView; }
			set { pad.IsMethodView = value; }
		}
	}
	
	public sealed class ParallelStacksViewCommand : AbstractComboBoxCommand
	{
		ParallelStackPad pad;
		ComboBox box;		
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			this.pad = this.Owner as ParallelStackPad;
			if (this.pad == null)
				return;
			
			box = this.ComboBox as ComboBox;
			
			if (this.box == null)
				return;
			
			foreach (var name in Enum.GetNames(typeof(ParallelStacksView)))
				box.Items.Add(name);
			
			box.SelectedIndex = 0;
			
			base.OnOwnerChanged(e);
		}
		
		public override void Run()
		{
			if (this.pad != null && this.box != null) {
				pad.ParallelStacksView = (ParallelStacksView)Enum.Parse(typeof(ParallelStacksView), box.SelectedValue.ToString());
			}
			base.Run();
		}
	}
}