// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
