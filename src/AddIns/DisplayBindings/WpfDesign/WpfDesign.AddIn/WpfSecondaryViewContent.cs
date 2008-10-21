// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3528 $</version>
// </file>

using ICSharpCode.WpfDesign.Designer.OutlineView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.PropertyGrid;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Designer.XamlBackend;
using ICSharpCode.Xaml;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// IViewContent implementation that hosts the WPF designer.
	/// </summary>
	public class WpfSecondaryViewContent : AbstractViewContent, IWpfViewContent, IHasPropertyContainer, IUndoHandler, IToolsHost
	{
		public WpfSecondaryViewContent(WpfPrimaryViewContent primaryViewContent)
		{
			this.primaryViewContent = primaryViewContent;
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
		}

		WpfPrimaryViewContent primaryViewContent;

		public override Control Control
		{
			get { return WpfTools.DesignerHost; }
		}

		public override void Load(OpenedFile file, Stream stream)
		{
			Context.Parse(new StreamReader(stream).ReadToEnd());
		}

		public override void Save(OpenedFile file, Stream stream)
		{
			if (Context.CanSave) {
				new StreamWriter(stream).Write(Context.Save());
			}
			else {
				primaryViewContent.Save(file, stream);
			}
		}

		public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return newView == primaryViewContent && !Context.CanSave;
		}

		#region WpfViewContent Implementation

		public XamlDesignContext Context
		{
			get { return primaryViewContent.Context; }
		}

		public PropertyContainer PropertyContainer
		{
			get { return WpfTools.PropertyContainer; }
		}

		public System.Windows.Forms.Control ToolsControl
		{
			get { return WpfTools.ToolboxHost; }
		}

		public bool EnableRedo
		{
			get { return Context.UndoService.CanRedo; }
		}

		public bool EnableUndo
		{
			get { return Context.UndoService.CanUndo; }
		}

		public void Redo()
		{
			Context.UndoService.Redo();
		}

		public void Undo()
		{
			Context.UndoService.Undo();
		}

		#endregion
	}
}