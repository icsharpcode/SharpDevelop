// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

//Namespace adapted to 
namespace ICSharpCode.Reports.Addin.Test.Designer
{
	/// <summary>
	/// Mock OpenedFile class.
	/// </summary>
	public class MockOpenedFile : OpenedFile
	{
		public MockOpenedFile()
		{
		}
		
		public MockOpenedFile(string fileName)
		{
			this.FileName = fileName;
		}
		
		public override IList<IViewContent> RegisteredViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override void RegisterView(IViewContent view)
		{
		}
		
		public override void UnregisterView(IViewContent view)
		{
		}
		
//		public override bool IsDirty {
//			get { return base.IsDirty; }
//			set { base.IsDirty = value; }
//		}
	}
}
