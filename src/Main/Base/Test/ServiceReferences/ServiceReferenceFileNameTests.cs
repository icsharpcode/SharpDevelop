// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceFileNameTests
	{
		ServiceReferenceFileName fileName;
		
		void CreateServiceReferenceFileName(string serviceReferencesFolder, string serviceName)
		{
			fileName = new ServiceReferenceFileName(serviceReferencesFolder, serviceName);
		}
		
		[Test]
		public void Path_NewInstanceCreated_ReturnsFullPathToFile()
		{
			CreateServiceReferenceFileName(@"d:\projects\MyProject\Service References", "MyService");
			
			string path = fileName.Path;
			
			string expectedPath = @"d:\projects\MyProject\Service References\MyService\Reference.cs";
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void ServiceName_NewInstanceCreated_ReturnsFullPathToFile()
		{
			CreateServiceReferenceFileName(@"d:\projects\MyProject\Service References", "MyService");
			
			string serviceName = fileName.ServiceName;
			
			Assert.AreEqual("MyService", serviceName);
		}
	}
}
