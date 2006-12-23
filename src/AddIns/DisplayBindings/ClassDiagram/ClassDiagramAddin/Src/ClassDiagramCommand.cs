// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Globalization;

using ClassDiagram;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Dom;

namespace ClassDiagramAddin
{
	public abstract class ClassDiagramAddinCommand : AbstractMenuCommand
	{
		protected ClassCanvas Canvas
		{
			get { return (ClassCanvas)((ClassDiagramViewContent)this.Owner).Control; }
		}
	}
	
	public class AutoArrangeDiagramCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.AutoArrange();
		}
	}
	
	public class ExpandAllCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.ExpandAll();
		}
	}

	public class CollapseAllCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.CollapseAll();
		}
	}

	public class MatchAllWidthsCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.MatchAllWidths();
		}
	}

	public class ShrinkAllWidthsCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.ShrinkAllWidths();
		}
	}
	
	public class SetDiagramZoomCommand : AbstractComboBoxCommand
	{
		protected ClassCanvas Canvas
		{
			get
			{
				return (ClassCanvas)((ToolBarComboBox)this.Owner).Owner.Parent;
			}
		}
		
		public override void Run()
		{
			Canvas.Zoom = zoom;
		}
		
		private void ComboBoxTextChanged(object sender, EventArgs e)
		{
			float zoomPercent = 100.0f;
			string s = comboBox.Text.Trim().Trim('%');
			if (float.TryParse (s, out zoomPercent))
			{
				zoom = zoomPercent / 100.0f;
				this.Run();
			}
		}

		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			ToolBarComboBox box1 = (ToolBarComboBox) this.Owner;
			comboBox = box1.ComboBox;
			comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox.Items.Add("10%");
			comboBox.Items.Add("25%");
			comboBox.Items.Add("50%");
			comboBox.Items.Add("75%");
			comboBox.Items.Add("100%");
			comboBox.Items.Add("125%");
			comboBox.Items.Add("150%");
			comboBox.Items.Add("175%");
			comboBox.Items.Add("200%");
			comboBox.Items.Add("250%");
			comboBox.Items.Add("300%");
			comboBox.Items.Add("350%");
			comboBox.Items.Add("400%");
			comboBox.TextChanged += new EventHandler(this.ComboBoxTextChanged);
		}

		ComboBox comboBox;
		float zoom = 1.0f;
	}
}
