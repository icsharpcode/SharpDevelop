using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer.XamlBackend;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.AddIn
{
	public static class WpfTools
	{
		static WpfTools()
		{
			CreateToolbox();
			CreateOutline();
			CreatePropertyGrid();
			CreateDesigner();

			WorkbenchSingleton.Workbench.ActiveViewContentChanged += new EventHandler(Workbench_ActiveViewContentChanged);
		}

		public static Toolbox Toolbox;
		public static Outline Outline;
		public static PropertyGridView PropertyGridView;
		public static DesignSurface Designer;

		public static SharpDevelopElementHost ToolboxHost;
		public static SharpDevelopElementHost OutlineHost;
		public static SharpDevelopElementHost PropertyGridViewHost;
		public static SharpDevelopElementHost DesignerHost;

		public static PropertyContainer PropertyContainer;

		public static DesignContext ActiveContext
		{
			get
			{
				var wpfViewContent = WorkbenchSingleton.Workbench.ActiveViewContent as IWpfViewContent;
				if (wpfViewContent != null) {
					return wpfViewContent.Context;
				}
				return null;
			}
		}

		static void CreateToolbox()
		{
			Toolbox = new Toolbox();
			ToolboxHost = new SharpDevelopElementHost(Toolbox);
		}

		static void CreateOutline()
		{
			Outline = new Outline();
			OutlineHost = new SharpDevelopElementHost(Outline);

			// see 3522
			Outline.AddCommandHandler(ApplicationCommands.Delete,
				() => ApplicationCommands.Delete.Execute(null, Designer));
		}

		static void CreatePropertyGrid()
		{
			PropertyGridView = new PropertyGridView();
			PropertyGridViewHost = new SharpDevelopElementHost(PropertyGridView);
			PropertyContainer = new PropertyContainer();
			PropertyContainer.PropertyGridReplacementControl = PropertyGridViewHost;
		}

		static void CreateDesigner()
		{
			Designer = new DesignSurface();
			DesignerHost = new SharpDevelopElementHost(Designer);
			DragDropExceptionHandler.HandleException = ICSharpCode.Core.MessageService.ShowError;
		}

		static void Workbench_ActiveViewContentChanged(object sender, EventArgs e)
		{
			Toolbox.Context = ActiveContext;
			Outline.Context = ActiveContext;
			PropertyGridView.Context = ActiveContext;
			Designer.Context = ActiveContext;
		}
	}

	public interface IWpfViewContent
	{
		XamlDesignContext Context { get; }
	}
}
