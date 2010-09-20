// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class MockPackageFilesViewFactory : IPackageFilesViewFactory
	{
		PackageFilesView packageFilesViewCreated;
		WixProject createMethodProjectParameter;
		IWorkbench createMethodWorkbenchParameter;
		MockWixPackageFilesControl packageFilesControl;
		
		public MockPackageFilesViewFactory()
		{
		}
		
		public PackageFilesView Create(WixProject project, IWorkbench workbench)
		{
			createMethodProjectParameter = project;
			createMethodWorkbenchParameter = workbench;
			packageFilesControl = new MockWixPackageFilesControl();
			
			packageFilesViewCreated = new PackageFilesView(project, workbench, packageFilesControl);
			return packageFilesViewCreated;
		}
		
		public PackageFilesView PackageFilesViewCreated {
			get { return packageFilesViewCreated; }
		}
		
		public WixProject CreateMethodProjectParameter {
			get { return createMethodProjectParameter; }
		}
		
		public IWorkbench CreateMethodWorkbenchParameter {
			get { return createMethodWorkbenchParameter; }
		}
		
		public MockWixPackageFilesControl PackageFilesControlCreated {
			get { return packageFilesControl; }
		}
	}
}
