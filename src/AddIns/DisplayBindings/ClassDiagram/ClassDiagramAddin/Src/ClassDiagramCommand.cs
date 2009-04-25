/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 23/09/2006
 * Time: 12:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using ClassDiagram;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

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
	
	public class ZoomInCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.Zoom *= 1.1f;
		}
	}
	
	public class ZoomOutCommand : ClassDiagramAddinCommand
	{
		public override void Run()
		{
			Canvas.Zoom *= 0.9f;
		}
	}
	
	public class SetDiagramZoomCommand : AbstractComboBoxCommand
	{
		bool dontModifyCanvas;

		private void CanvasZoomChanged (object sender, EventArgs e)
		{
			dontModifyCanvas = true;
			comboBox.Text = Canvas.Zoom.ToString() + "%"; 
			dontModifyCanvas = false;
		}
		
		protected ClassCanvas Canvas
		{
			get
			{
				ToolStrip ts = ((ToolBarComboBox)this.Owner).Owner;
				if (ts != null)
					return (ClassCanvas)ts.Parent;
				return null;
			}
		}
		
		public override void Run()
		{
			Canvas.Zoom = zoom;
		}
		
		private void ComboBoxTextChanged(object sender, EventArgs e)
		{
			if (dontModifyCanvas) return;
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
			comboBox.Items.AddRange(new object[] {"10%", "25%", "50%", "75%",
			                        	"100%", "125%", "150%", "175%", "200%",
			                        	"250%", "300%", "350%", "400%"});
			
			ClassCanvas canvas = Canvas;
			if (canvas != null)
				canvas.ZoomChanged += CanvasZoomChanged;
			comboBox.TextChanged += ComboBoxTextChanged;
		}

		ComboBox comboBox;
		float zoom = 1.0f;
	}
}
