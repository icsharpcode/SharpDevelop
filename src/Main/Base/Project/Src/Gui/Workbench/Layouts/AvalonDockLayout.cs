// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of AvalonDockLayout.
	/// </summary>
	public class AvalonDockLayout : IWorkbenchLayout
	{
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object ActiveContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			throw new NotImplementedException();
		}
		
		public void Detach()
		{
			throw new NotImplementedException();
		}
		
		public void ShowPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void ActivatePad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void ActivatePad(string fullyQualifiedTypeName)
		{
			throw new NotImplementedException();
		}
		
		public void HidePad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void UnloadPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public bool IsVisible(PadDescriptor padContent)
		{
			throw new NotImplementedException();
		}
		
		public void RedrawAllComponents()
		{
			throw new NotImplementedException();
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			throw new NotImplementedException();
		}
		
		public void LoadConfiguration()
		{
			throw new NotImplementedException();
		}
		
		public void StoreConfiguration()
		{
			throw new NotImplementedException();
		}
	}
}
