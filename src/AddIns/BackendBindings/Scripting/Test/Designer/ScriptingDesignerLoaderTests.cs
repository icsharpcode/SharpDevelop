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

//using System;
//using System.Collections;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.ComponentModel.Design.Serialization;
//using System.Windows.Forms;
//
//using ICSharpCode.FormsDesigner;
//using ICSharpCode.FormsDesigner.Services;
//using ICSharpCode.Scripting.Tests.Utils;
//using NUnit.Framework;
//
//namespace ICSharpCode.Scripting.Tests.Designer
//{
//	[TestFixture]
//	[RequiresSTA]
//	public class ScriptingDesignerLoaderTests
//	{
//		MockDesignerGenerator fakeGenerator;
//		TestableScriptingDesignerLoader loader;
//		MockDesignerLoaderHost fakeDesignerLoaderHost;
//		MockEventBindingService fakeEventBindingService;
//		FormsDesignerViewContent formsDesignerView;
//		FakeDesignerSerializationManager fakeSerializationManager;
//
//		[SetUp]
//		public void Init()
//		{
//			CreateScriptingDesignerLoader();
//		}
//		
//		void CreateScriptingDesignerLoader()
//		{
//			fakeGenerator = new MockDesignerGenerator();
//			loader = new TestableScriptingDesignerLoader(fakeGenerator);
//			
//			formsDesignerView = new FormsDesignerViewContent(new MockViewContent(), new MockOpenedFile("Test.py"));
//			fakeGenerator.Attach(formsDesignerView);
//		}
//		
//		[TearDown]
//		public void TearDown()
//		{
//			loader.Dispose();
//		}
//		
//		[Test]
//		public void ScriptingDesignerLoaderClass_NewInstance_IsBasicDesignerLoader()
//		{
//			BasicDesignerLoader basicLoader = loader as BasicDesignerLoader;
//			Assert.IsNotNull(basicLoader);
//		}
//		
//		[Test]
//		public void BeginLoad_PassedFakeDesignerLoaderHost_ComponentSerializationServiceAddedToDesignerLoaderHost()
//		{
//			BeginLoad();
//			CodeDomComponentSerializationService service = fakeDesignerLoaderHost.GetService(typeof(ComponentSerializationService)) as CodeDomComponentSerializationService;
//			Assert.IsNotNull(service);
//		}
//		
//		void BeginLoad()
//		{
//			CreateDesignerLoaderHostWithoutProjectResourceService();
//			fakeDesignerLoaderHost.AddService(typeof(ProjectResourceService), new ProjectResourceService(new MockProjectContent()));
//			loader.BeginLoad(fakeDesignerLoaderHost);
//		}
//		
//		void CreateDesignerLoaderHostWithoutProjectResourceService()
//		{
//			fakeDesignerLoaderHost = new MockDesignerLoaderHost();
//			fakeEventBindingService = new MockEventBindingService();
//			fakeDesignerLoaderHost.AddService(typeof(IEventBindingService), fakeEventBindingService);
//		}
//		
//		[Test]
//		public void BeginLoad_PassedFakeDesignerLoaderHost_NameCreationServiceAddedToDesignerLoaderHost()
//		{
//			BeginLoad();
//			ScriptingNameCreationService service = fakeDesignerLoaderHost.GetService(typeof(INameCreationService)) as ScriptingNameCreationService;
//			Assert.IsNotNull(service);
//		}
//		
//		[Test]
//		public void BeginLoad_PassedFakeDesignerLoaderHost_DesignerSerializationServiceAddedToDesignerLoaderHost()
//		{
//			BeginLoad();
//			DesignerSerializationService service = fakeDesignerLoaderHost.GetService(typeof(IDesignerSerializationService)) as DesignerSerializationService;
//			Assert.IsNotNull(service);
//		}
//		
//		[Test]
//		public void BeginLoad_PassedFakeDesignerLoaderHost_ProjectResourceServiceDesignerAddedToDesignerLoaderHostDoesNotSupportProjectResources()
//		{
//			BeginLoad();
//			ProjectResourceService service = fakeDesignerLoaderHost.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
//			Assert.IsFalse(service.DesignerSupportsProjectResources);
//		}
//		
//		[Test]
//		public void GetEventProperty_PassedFormLoadEventDescriptor_ReturnsPropertyDescriptorFromEventBindingService()
//		{
//			BeginLoad();
//			IEventBindingService eventBindingService = (IEventBindingService)fakeEventBindingService;
//			EventDescriptor e = TypeDescriptor.GetEvents(typeof(Form)).Find("Load", true);
//			
//			PropertyDescriptor propertyDescriptor = loader.GetEventProperty(e);
//			PropertyDescriptor expectedPropertyDescriptor = eventBindingService.GetEventProperty(e);
//			
//			Assert.AreEqual(expectedPropertyDescriptor, propertyDescriptor);
//		}
//		
//		[Test]
//		public void ScriptingDesignerLoaderConstructor_PassedNullGenerator_ThrowsArgumentException()
//		{
//			ArgumentException ex = Assert.Throws<ArgumentException>(delegate { 
//				loader = new TestableScriptingDesignerLoader(null); 
//			});
//			string paramName = ex.ParamName;
//			string expectedParamName = "generator";
//			Assert.AreEqual(expectedParamName, paramName);
//		}
//		
//		[Test]
//		public void PerformFlush_PassedDesignerSerializationManager_DesignerLoaderHostPassedToMergeRootComponentChangesMethod()
//		{
//			BeginLoad();
//			DesignerSerializationManager serializationManager = new DesignerSerializationManager();
//			loader.CallPerformFlush(serializationManager);
//			
//			IDesignerHost host = fakeGenerator.MergeChangesHost;
//			Assert.AreEqual(fakeDesignerLoaderHost, host);
//		}
//		
//		[Test]
//		public void PerformFlush_PassedDesignerSerializationManager_SerializationManagerPassedToMergeRootComponentMethod()
//		{
//			BeginLoad();
//			DesignerSerializationManager expectedSerializationManager = new DesignerSerializationManager();
//			loader.CallPerformFlush(expectedSerializationManager);
//			
//			IDesignerSerializationManager serializationManager = fakeGenerator.MergeChangesSerializationManager;
//			Assert.AreEqual(expectedSerializationManager, serializationManager);
//		}
//		
//		[Test]
//		public void RootComponent_DesignerLoaderHostRootComponentIsForm_ReturnsDesignerLoaderHostRootComponent()
//		{
//			BeginLoad();
//			using (Form form = new Form()) {
//				fakeDesignerLoaderHost.RootComponent = form;
//				IComponent rootComponent = loader.RootComponent;
//				Assert.AreEqual(form, rootComponent);
//			}
//		}
//		
//		
//		[Test]
//		public void BeginLoad_PassedFakeDesignerLoaderHost_CallsCreatesComponentWalkerPassingNonNullComponentCreator()
//		{
//			BeginLoad();
//			IComponentCreator componentCreator = loader.ComponentCreatorPassedToCreateComponentWalker;
//			Assert.IsNotNull(componentCreator);
//		}
//		
//		[Test]
//		public void BeginLoad_PassedFakeDesignerLoaderHost_CallsComponentWalkerCreateComponentMethodPassingFormCode()
//		{
//			string expectedCode = 
//				"MyForm(Form):\r\n" +
//				"    pass";
//			
//			formsDesignerView.DesignerCodeFileContent = expectedCode;
//			
//			BeginLoad();
//			string code = loader.FakeComponentWalker.CodePassedToCreateComponent;
//			Assert.AreEqual(expectedCode, code);
//		}
//		
//		[Test]
//		public void BeginLoad_NoProjectResourceService_NullReferenceExceptionIsNotThrown()
//		{
//			CreateDesignerLoaderHostWithoutProjectResourceService();
//			
//			Assert.DoesNotThrow(delegate { loader.BeginLoad(fakeDesignerLoaderHost); });
//		}
//		
//		[Test]
//		public void CreateComponent_CreateTextBox_TextBoxTypePassedToDesignerLoaderHostCreateComponentMethod()
//		{
//			BeginLoad();
//			loader.CreateComponent(typeof(TextBox), "MyTextBox");
//			CreatedComponent createdComponent = fakeDesignerLoaderHost.CreatedComponents[0];
//			CreatedComponent expectedCreatedComponent = new CreatedComponent("System.Windows.Forms.TextBox", "MyTextBox");
//			
//			Assert.AreEqual(expectedCreatedComponent, createdComponent);
//		}
//		
//		[Test]
//		public void CreateComponent_CreateTextBox_TextBoxInstanceReturned()
//		{
//			BeginLoad();
//			IComponent component = loader.CreateComponent(typeof(TextBox), "MyTextBox");
//			bool result = component is TextBox;
//			
//			Assert.IsTrue(result);
//		}
//		
//		[Test]
//		public void Add_AddTextBox_AddsTextBoxToDesignerLoaderHostContainer()
//		{
//			BeginLoad();
//			using (TextBox textBox = new TextBox()) {
//				loader.Add(textBox, "MyTextBox");
//				IComponent component = fakeDesignerLoaderHost.Container.Components["MyTextBox"];
//				Assert.AreEqual(textBox, component);
//			}
//		}
//		
//		[Test]
//		public void GetComponent_TextBoxAddedToLoader_ReturnsTextBox()
//		{
//			BeginLoad();
//			using (TextBox textBox = new TextBox()) {
//				loader.Add(textBox, "MyTextBox");
//				IComponent component = loader.GetComponent("MyTextBox");
//				Assert.AreEqual(textBox, component);
//			}
//		}
//		
//		[Test]
//		public void GetComponent_NoComponentsAddedToLoader_ReturnsNull()
//		{
//			BeginLoad();
//			IComponent component = loader.GetComponent("MyTextBox");
//			Assert.IsNull(component);
//		}
//		
//		[Test]
//		public void GetType_PassedTypeName_ReturnsTypeFromDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			Type expectedType = typeof(string);
//			fakeSerializationManager.TypeToReturnFromGetType = expectedType;
//			Type type = loader.GetType("string");
//			
//			Assert.AreEqual(expectedType, type);
//		}
//		
//		void CreateDesignerSerializationManager()
//		{
//			fakeSerializationManager = new FakeDesignerSerializationManager();
//		}
//		
//		[Test]
//		public void GetType_PassedTypeName_TypeNamePassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			string expectedTypeName = "test";
//			loader.GetType(expectedTypeName);
//			
//			string typeName = fakeSerializationManager.TypeNamePassedToGetType;
//			Assert.AreEqual(expectedTypeName, typeName);			
//		}
//		
//		[Test]
//		public void GetInstance_PassedName_ReturnsInstanceFromDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			object expectedInstance = new object();
//			fakeSerializationManager.InstanceToReturnFromGetInstance = expectedInstance;
//			object instance = loader.GetInstance("test");
//			
//			Assert.AreEqual(expectedInstance, instance);
//		}
//		
//		[Test]
//		public void GetInstance_PassedName_InstanceNamePassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			string expectedName = "test";
//			loader.GetInstance(expectedName);
//			
//			string name = fakeSerializationManager.NamePassedToGetInstance;
//			Assert.AreEqual(expectedName, name);			
//		}
//		
//		[Test]
//		public void CreateInstance_PassedType_ReturnsInstanceFromDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			object expectedInstance = new object();
//			fakeSerializationManager.InstanceToReturnFromCreateInstance = expectedInstance;
//			object instance = LoaderCreateInstance(typeof(string));
//			
//			Assert.AreEqual(expectedInstance, instance);
//		}
//		
//		object LoaderCreateInstance(Type type)
//		{
//			return LoaderCreateInstance(type, null, null, false);
//		}
//		
//		object LoaderCreateInstance(string name)
//		{
//			return LoaderCreateInstance(null, null, name, false);
//		}
//		
//		object LoaderCreateInstance(ICollection arguments)
//		{
//			return LoaderCreateInstance(null, arguments, null, false);
//		}
//		
//		object LoaderCreateInstance(bool addToContainer)
//		{
//			return LoaderCreateInstance(null, null, null, addToContainer);
//		}
//		
//		object LoaderCreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
//		{
//			return loader.CreateInstance(type, arguments, name, addToContainer);
//		}
//		
//		[Test]
//		public void CreateInstance_PassedType_TypePassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			Type expectedType = typeof(string);
//			LoaderCreateInstance(expectedType);
//			Type type = fakeSerializationManager.TypePassedToCreateInstance;
//			
//			Assert.AreEqual(expectedType, type);
//		}
//		
//		[Test]
//		public void CreateInstance_PassedName_NamePassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			string expectedName = "test";
//			LoaderCreateInstance(expectedName);
//			string name = fakeSerializationManager.NamePassedToCreateInstance;
//			
//			Assert.AreEqual(expectedName, name);
//		}
//		
//		[Test]
//		public void CreateInstance_PassedTrueAddToContainer_AddToContainerPassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			LoaderCreateInstance(true);
//			bool addToContainer = fakeSerializationManager.AddToContainerPassedToCreateInstance;
//			
//			Assert.IsTrue(addToContainer);
//		}
//		
//		[Test]
//		public void CreateInstance_PassedFalseAddToContainer_AddToContainerPassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			LoaderCreateInstance(false);
//			bool addToContainer = fakeSerializationManager.AddToContainerPassedToCreateInstance;
//			
//			Assert.IsFalse(addToContainer);
//		}
//		
//		[Test]
//		public void CreateInstance_PassedArguments_ArgumentsPassedToDesignerSerializationManager()
//		{
//			CreateDesignerSerializationManager();
//			loader.CallPerformLoad(fakeSerializationManager);
//			
//			string[] expectedArguments = new string[] { "a", "b" };
//			LoaderCreateInstance(expectedArguments);
//			ICollection arguments = fakeSerializationManager.ArgumentsPassedToCreateInstance;
//			
//			Assert.AreEqual(expectedArguments, arguments);
//		}
//	}
//}
