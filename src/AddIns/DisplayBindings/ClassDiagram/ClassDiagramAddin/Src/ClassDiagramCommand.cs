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
				ToolStrip ts = ((ToolBarComboBox)this.ComboBox).Owner;
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
			ToolBarComboBox box1 = (ToolBarComboBox) this.ComboBox;
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
