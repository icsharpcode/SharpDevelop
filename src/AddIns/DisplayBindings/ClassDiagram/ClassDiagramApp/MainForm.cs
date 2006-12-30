/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 23/09/2006
 * Time: 14:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ClassDiagram;

using ICSharpCode.SharpDevelop.Dom;

namespace ClassDiagramApp
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());	
		}
		
		ClassCanvas classcanvas = new ClassCanvas();
		ClassEditor classeditor = new ClassEditor();
		ProjectContentRegistry registry = new ProjectContentRegistry();
		IProjectContent pc;
		ICompilationUnit cu;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			splitContainer1.Panel1.Controls.Add (classcanvas);
			classcanvas.Dock = DockStyle.Fill;
			splitContainer2.Panel2.Controls.Add (classeditor);
			classeditor.Dock = DockStyle.Fill;

			pc = new ReflectionProjectContent(Assembly.LoadFrom("ClassCanvas.dll"), registry);
			cu = new DefaultCompilationUnit(pc);
			
			classcanvas.CanvasItemSelected += OnItemSelected;
			classcanvas.LayoutChanged += HandleLayoutChange;
		}
		
		void MainFormFormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			Application.Exit();
		}
		
		void ToolStripMenuItem1Click(object sender, System.EventArgs e)
		{
			ClassCanvasItem item = ClassCanvas.CreateItemFromType(new DefaultClass(cu, "ClassDiagram.ClassCanvasItem"));
			item.X = 20;
			item.Y = 20;
			classcanvas.AddCanvasItem(item);
		}
		
		void ZoomValueChanged(object sender, System.EventArgs e)
		{
			classcanvas.Zoom = zoom.Value / 100f;
		}
		
		void ColExpBtnClick(object sender, System.EventArgs e)
		{
			if (colExpBtn.Text == "Collapse All")
			{
				classcanvas.CollapseAll();
				colExpBtn.Text = "Expand All";
			}
			else
			{
				classcanvas.ExpandAll();
				colExpBtn.Text = "Collapse All";
			}
		}
		
		void OnItemSelected (object sender, CanvasItemEventArgs e)
		{
			if (e.CanvasItem is ClassCanvasItem)
				classeditor.SetClass(((ClassCanvasItem)e.CanvasItem).RepresentedClassType);
			else
				classeditor.SetClass (null);
		}
		
		void SaveBtnClick(object sender, System.EventArgs e)
		{
			classcanvas.WriteToXml().Save(@"C:\Documents and Settings\itai\My Documents\test.cd");
		}
		
		void InitBtnClick(object sender, System.EventArgs e)
		{
			float x=20, y=20;
			float max_h = 0;
			
			foreach (IClass ct in pc.Classes)
			{
				ClassCanvasItem classitem = ClassCanvas.CreateItemFromType(ct);
				classitem.X = x;
				classitem.Y = y;
				classcanvas.AddCanvasItem(classitem);
				x += classitem.Width + 20;
				if (classitem.Height > max_h)
					max_h = classitem.Height;
				if (x > 1000)
				{
					x = 20;
					y += max_h + 20;
					max_h = 0;
				}
			}
		}
		
		void LoadBtnClick(object sender, System.EventArgs e)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(@"C:\Documents and Settings\itai\My Documents\test.cd");
			classcanvas.LoadFromXml(doc, pc);
		}
		
		void ModifiedBtnClick(object sender, System.EventArgs e)
		{
			modifiedBtn.Enabled = false;
		}

		void HandleLayoutChange(object sender, System.EventArgs e)
		{
			modifiedBtn.Enabled = true;
		}
		
		void LayoutBtnClick(object sender, System.EventArgs e)
		{
			classcanvas.AutoArrange();
		}
		
		void MatchBtnClick(object sender, System.EventArgs e)
		{
			classcanvas.MatchAllWidths();
		}
		
		void ShrinkBtnClick(object sender, System.EventArgs e)
		{
			classcanvas.ShrinkAllWidths();
		}
		
		void ComboBox1TextChanged(object sender, System.EventArgs e)
		{
			float zoomPercent = 100.0f;
			string s = comboBox1.Text.Trim().Trim('%');
			if (float.TryParse (s, out zoomPercent))
			{
				classcanvas.Zoom = zoomPercent / 100.0f;
			}
		}
		
		void AddNoteBtnClick(object sender, EventArgs e)
		{
			NoteCanvasItem note = new NoteCanvasItem();
			note.X = 40;
			note.Y = 40;
			note.Width = 100;
			note.Height = 100;
			classcanvas.AddCanvasItem(note);
		}
		
		void SaveImgBtnClick(object sender, EventArgs e)
		{
			classcanvas.SaveToImage(@"C:\Documents and Settings\itai\My Documents\test.png");
		}
	}
}
