// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using NoGoop.ObjBrowser;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class ComponentInspectorView : AbstractViewContent
	{
		bool disposed;
		static ObjectBrowser objectBrowser;
		bool showStatusPanel = false;
		bool tabbedLayout = true;
		static ComponentInspectorView instance;
		
		public ComponentInspectorView()
		{
			instance = this;
	
			// HACK: Due to various static members in the ComponentInspector
			// the ObjectBrowser does not like being re-used after being disposed. 
			// Workaround this by keeping a reference to the ObjectBrowser.
			if (objectBrowser == null) {
				objectBrowser = new ObjectBrowser(showStatusPanel, tabbedLayout);
			}
			AssemblySupport.AddCurrentAssemblies();
			ComSupport.RestoreComEnvironment();
			AssemblySupport.RestoreAssemblies();
			Application.Idle += IdleHandler;
		}
		
		public static ComponentInspectorView Instance {
			get {
				return instance;
			}
		}
		
		public override Control Control {
			get {
				return objectBrowser;
			}
		}
		
		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		public override void Dispose()
		{
			if (!disposed) {
				Application.Idle -= IdleHandler;
				disposed = true;
//				objectBrowser.Dispose();
//				objectBrowser = null;
			}
			instance = null;
		}
		
		public override string TitleName {
			get {
				return "${res:ComponentInspector.ToolsMenu.ShowComponentInspectorMenuItem}";
			}
			set {
			}
		}
		
		public void OpenFile(string fileName)
		{
			objectBrowser.OpenFile(fileName);
		}
		
		/// <summary>
		/// Closes the selected file.
		/// </summary>
		public void CloseSelectedFile()
		{
			objectBrowser.CloseSelectedFile();
		}		
		
		/// <summary>
		/// Component Inspector uses the idle handler to do various things,
		/// for now we hook the idle event too. Look into changing this later.
		/// </summary>
		void IdleHandler(object sender, EventArgs e)
		{
			EventLogList.NewIncarnation();
			ObjectCreator.CheckOutstandingCreation();
		}
	}
}
