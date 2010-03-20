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
using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests basic operation of the RubyDesignerLoader.
	/// </summary>
	[TestFixture]
	public class RubyDesignerLoaderTestFixture
	{
		FormsDesignerViewContent view;
		DerivedRubyDesignerLoader loader;
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
			view = new FormsDesignerViewContent(null, new MockOpenedFile("Test.rb"));
			generator.Attach(view);
			
			view.DesignerCodeFileContent = GetFormCode();
			loader = new DerivedRubyDesignerLoader(generator);
	
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
			RubyNameCreationService service = mockDesignerLoaderHost.GetService(typeof(INameCreationService)) as RubyNameCreationService;
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
			return "require \"mscorlib\"\r\n" +
					"require \"System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n" +
					"require \"System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
					"\r\n" +
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents()\r\n" +
					"\t\tself.Name = 'MainForm'\r\n" +
					"\tend\r\n" +
					"end";
		}
	}
}
