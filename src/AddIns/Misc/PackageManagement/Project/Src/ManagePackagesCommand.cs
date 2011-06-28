// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class ManagePackagesCommand : AbstractMenuCommand
	{
		IPackageManagementOutputMessagesView outputMessagesView;
		
		public ManagePackagesCommand()
			: this(PackageManagementServices.OutputMessagesView)
		{
		}
		
		public ManagePackagesCommand(IPackageManagementOutputMessagesView outputMessagesView)
		{
			this.outputMessagesView = outputMessagesView;
		}
		
		public override void Run()
		{
			outputMessagesView.Clear();
			
			using (IManagePackagesView view = CreateManagePackagesView()) {
				view.ShowDialog();
			}
		}
		
		protected virtual IManagePackagesView CreateManagePackagesView()
		{
			return new ManagePackagesView() {
				Owner = WorkbenchSingleton.MainWindow
			};
		}
	}
}
