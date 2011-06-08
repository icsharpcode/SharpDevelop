// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class AddPackageReferenceCommand : AbstractMenuCommand
	{
		IPackageManagementOutputMessagesView outputMessagesView;
		
		public AddPackageReferenceCommand()
			: this(PackageManagementServices.OutputMessagesView)
		{
		}
		
		public AddPackageReferenceCommand(IPackageManagementOutputMessagesView outputMessagesView)
		{
			this.outputMessagesView = outputMessagesView;
		}
		
		public override void Run()
		{
			outputMessagesView.Clear();
			
			using (IAddPackageReferenceView view = CreateAddPackageReferenceView()) {
				view.ShowDialog();
			}
		}
		
		protected virtual IAddPackageReferenceView CreateAddPackageReferenceView()
		{
			return new AddPackageReferenceView() {
				Owner = WorkbenchSingleton.MainWindow
			};
		}
	}
}
