// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests basic operation of the PythonDesignerLoader.
	/// </summary>
	[TestFixture]
	public class PythonDesignerLoaderTestFixture
	{
		FormsDesignerViewContent view;
		DerivedPythonDesignerLoader loader;
		MockDesignerGenerator generator;
		MockTypeResolutionService mockTypeResolutionService;
		MockDesignerLoaderHost mockDesignerLoaderHost;
		MockExtenderProviderService mockExtenderProviderService;
		IComponent rootComponent;
		Form designedForm;
		MockEventBindingService mockEventBindingService;
		MockResourceService mockResourceService;
		DesignerSerializationManager serializationManager;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			generator = new MockDesignerGenerator();
			view = new FormsDesignerViewContent(null, new MockOpenedFile("Test.py"));
			generator.Attach(view);
			
			view.DesignerCodeFileContent = GetFormCode();
			loader = new DerivedPythonDesignerLoader(generator);
	
			// Begin load.
			mockDesignerLoaderHost = new MockDesignerLoaderHost();
			mockResourceService = new MockResourceService();
			mockDesignerLoaderHost.AddService(typeof(IResourceService), mockResourceService);

			mockTypeResolutionService = mockDesignerLoaderHost.TypeResolutionService;

			mockExtenderProviderService = new MockExtenderProviderService();
			mockDesignerLoaderHost.AddService(typeof(IExtenderProviderService), mockExtenderProviderService);
			mockDesignerLoaderHost.AddService(typeof(ProjectResourceService), new ProjectResourceService(new MockProjectContent()));
			
			mockEventBindingService = new MockEventBindingService();
			mockDesignerLoaderHost.AddService(typeof(IEventBindingService), mockEventBindingService);
			
			serializationManager = new DesignerSerializationManager(mockDesignerLoaderHost);
			
			System.Console.WriteLine("Before BeginLoad");
			loader.BeginLoad(mockDesignerLoaderHost);
			System.Console.WriteLine("After BeginLoad");
			rootComponent = mockDesignerLoaderHost.RootComponent;
			
			designedForm = new Form();
			designedForm.Name = "NewMainForm";
			mockDesignerLoaderHost.RootComponent = designedForm;	
			loader.CallPerformFlush(serializationManager);
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			loader.Dispose();
			designedForm.Dispose();
		}
		
		[Test]
		public void IsDerivedFromBasicDesignerLoader()
		{
			BasicDesignerLoader basicLoader = loader as BasicDesignerLoader;
			Assert.IsNotNull(basicLoader);
		}
		
		[Test]
		public void CreateComponent()
		{			
			List<CreatedComponent> expectedCreatedComponents = new List<CreatedComponent>();
			expectedCreatedComponents.Add(new CreatedComponent(typeof(Form).FullName, "MainForm", null));
			
			Assert.AreEqual(expectedCreatedComponents, mockDesignerLoaderHost.CreatedComponents);
		}
				
		[Test]
		public void ComponentSerializationServiceCreated()
		{
			CodeDomComponentSerializationService service = mockDesignerLoaderHost.GetService(typeof(ComponentSerializationService)) as CodeDomComponentSerializationService;
			Assert.IsNotNull(service);
		}

		[Test]
		public void NameCreationServiceCreated()
		{
			PythonNameCreationService service = mockDesignerLoaderHost.GetService(typeof(INameCreationService)) as PythonNameCreationService;
			Assert.IsNotNull(service);
		}
		
		[Test]
		public void DesignerSerializationServiceCreated()
		{
			DesignerSerializationService service = mockDesignerLoaderHost.GetService(typeof(IDesignerSerializationService)) as DesignerSerializationService;
			Assert.IsNotNull(service);
		}

		[Test]
		public void ProjectResourceServiceDesignerDoesNotSupportProjectResources()
		{
			ProjectResourceService service = mockDesignerLoaderHost.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
			Assert.IsFalse(service.DesignerSupportsProjectResources);
		}
		
		[Test]
		public void RootDesignerComponentNameIsMainForm()
		{
			Form form = rootComponent as Form;
			Assert.AreEqual("MainForm", form.Name);
		}
		
		[Test]
		public void PerformFlushUsesDesignerHost()
		{
			Assert.AreEqual(mockDesignerLoaderHost, generator.MergeChangesHost);
		}
		
		[Test]
		public void GetEventPropertyUsesEventBindingService()
		{
			IEventBindingService eventBindingService = (IEventBindingService)mockEventBindingService;
			EventDescriptor e = TypeDescriptor.GetEvents(typeof(Form)).Find("Load", true);
			PropertyDescriptor expectedProperty = eventBindingService.GetEventProperty(e);
			Assert.AreEqual(expectedProperty, loader.GetEventProperty(e));
		}
		
		[Test]
		public void GetRootComponentFromLoader()
		{
			Assert.AreEqual(designedForm, loader.RootComponent);
		}
		
		[Test]
		public void SerializationManagerPassedToMergeRootComponentMethod()
		{
			Assert.IsTrue(Object.ReferenceEquals(serializationManager, generator.MergeChangesSerializationManager));
		}
				
		/// <summary>
		/// The code that the designer loader will parse.
		/// </summary>
		string GetFormCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents(self):\r\n" +
					"\t\tself.Name = 'MainForm'\r\n"; 
		}
	}
}
