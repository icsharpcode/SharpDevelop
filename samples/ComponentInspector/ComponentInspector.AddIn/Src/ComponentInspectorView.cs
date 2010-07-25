// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using NoGoop.ObjBrowser;

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
	
			SetLocalizedTitle("${res:ComponentInspector.ToolsMenu.ShowComponentInspectorMenuItem}");
			
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
		
		public override object Control {
			get {
				return objectBrowser;
			}
		}
		
		public override bool IsDirty {
			get {
				return false;
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
			base.Dispose();
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
