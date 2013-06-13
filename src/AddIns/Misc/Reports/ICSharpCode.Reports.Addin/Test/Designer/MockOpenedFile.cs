// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
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
		
		public MockOpenedFile(string fileName):base()
		{
			this.FileName = ICSharpCode.Core.FileName.Create(fileName);
		}
		
		public override IList<IViewContent> RegisteredViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		protected override void ChangeFileName(ICSharpCode.Core.FileName newValue)
		{
		
		}
		public override void RegisterView(IViewContent view)
		{
		}
		
		public override void UnregisterView(IViewContent view)
		{
		}
		
		public override event EventHandler FileClosed { add {} remove {} }
		

	}
}
