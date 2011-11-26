// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Reflection;

using AspNet.Mvc.Tests.CodeTemplates.Models;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateHostTests
	{
		TestableMvcTextTemplateHost host;
		
		void CreateHost()
		{
			host = new TestableMvcTextTemplateHost();
		}
		
		[Test]
		public void ViewName_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ViewName = null;
			string viewName = host.ViewName;
			
			Assert.AreEqual(String.Empty, viewName);
		}
		
		[Test]
		public void ControllerName_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ControllerName = null;
			string controllerName = host.ControllerName;
			
			Assert.AreEqual(String.Empty, controllerName);
		}
		
		[Test]
		public void Namespace_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.Namespace = null;
			string ns = host.Namespace;
			
			Assert.AreEqual(String.Empty, ns);
		}
		
		[Test]
		public void ControllerRootName_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ControllerRootName = null;
			string controllerRootName = host.ControllerRootName;
			
			Assert.AreEqual(String.Empty, controllerRootName);
		}
		
		[Test]
		public void ControllerRootName_ControllerNameSetToAboutController_ControllerRootNameUpdatedToAbout()
		{
			CreateHost();
			host.ControllerName = "AboutController";
			string controllerRootName = host.ControllerRootName;
			
			Assert.AreEqual("About", controllerRootName);
		}
		
		[Test]
		public void ControllerRootName_ControllerNameSetToHome_ControllerRootNameUpdatedToHome()
		{
			CreateHost();
			host.ControllerName = "Home";
			string controllerRootName = host.ControllerRootName;
			
			Assert.AreEqual("Home", controllerRootName);
		}
		
		[Test]
		public void ControllerRootName_ControllerNameSetToHomeControllerWithControllerInUpperCase_ControllerRootNameUpdatedToHome()
		{
			CreateHost();
			host.ControllerName = "HomeCONTROLLER";
			string controllerRootName = host.ControllerRootName;
			
			Assert.AreEqual("Home", controllerRootName);
		}
		
		[Test]
		public void ViewDataTypeName_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ViewDataTypeName = null;
			string typeName = host.ViewDataTypeName;
			
			Assert.AreEqual(String.Empty, typeName);
		}
		
		[Test]
		public void ViewDataTypeAssemblyLocation_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ViewDataTypeAssemblyLocation = null;
			string location = host.ViewDataTypeAssemblyLocation;
			
			Assert.AreEqual(String.Empty, location);
		}
		
		[Test]
		public void MasterPageFile_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.MasterPageFile = null;
			string masterPage = host.MasterPageFile;
			
			Assert.AreEqual(String.Empty, masterPage);
		}
		
		[Test]
		public void PrimaryContentPlaceHolderID_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.PrimaryContentPlaceHolderID = null;
			string id = host.PrimaryContentPlaceHolderID;
			
			Assert.AreEqual(String.Empty, id);
		}
		
		[Test]
		public void ViewDataType_ViewDataTypeAssemblyLocationIsSet_ViewDataTypeAssemblyLocationUsedToLoadAssembly()
		{
			CreateHost();
			host.AssemblyToReturnFromLoadAssemblyFrom = typeof(String).Assembly;
			string expectedFileName = @"d:\test\bin\test.dll";
			host.ViewDataTypeAssemblyLocation = expectedFileName;
			host.ViewDataTypeName = "System.String";
			
			Type type = host.ViewDataType;
			
			string fileName = host.FileNamePassedToLoadAssemblyFrom;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void ViewDataType_ViewDataTypeNameIsSystemString_StringTypeReturned()
		{
			CreateHost();
			host.AssemblyToReturnFromLoadAssemblyFrom = typeof(String).Assembly;
			host.ViewDataTypeName = "System.String";
			
			Type type = host.ViewDataType;
			
			Assert.AreEqual("System.String", type.FullName);
		}
		
		[Test]
		public void ViewDataType_CalledTwice_AssemblyLoadedOnlyOnce()
		{
			CreateHost();
			host.AssemblyToReturnFromLoadAssemblyFrom = typeof(String).Assembly;
			host.ViewDataTypeName = "System.String";
			
			host.ViewDataTypeAssemblyLocation = @"d:\test\bin\test.dll";
			
			Type type = host.ViewDataType;
			
			host.FileNamePassedToLoadAssemblyFrom = null;
			type = host.ViewDataType;
			
			Assert.IsNull(host.FileNamePassedToLoadAssemblyFrom);
		}
		
		[Test]
		public void GetViewDataTypeProperties_ViewDataTypeIsModelWithOnePropertyCalledName_ReturnsOnePropertyCalledName()
		{
			CreateHost();
			host.ViewDataType = typeof(ModelWithOneProperty);
			
			PropertyInfo propertyInfo = host.GetViewDataTypeProperties().First();
			
			Assert.AreEqual("Name", propertyInfo.Name);
		}
	}
}
