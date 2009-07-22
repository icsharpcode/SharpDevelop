using ICSharpCode.Core.Presentation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation.Tests
{
	[TestFixture]
    public class CommandManagerTests
    {
    	UIElement namedInstanceScope;
    	BindingGroup bindingGroup;
    	BindingGroup bindingGroup2;
    	
    	[TestFixtureSetUp]
    	public void TestFixtureSetUp()
    	{
    		PropertyService.InitializeServiceForUnitTests();
    	}
    	
    	[SetUp]
    	public void SetuUp()
    	{
    		namedInstanceScope = new UIElement();
    		bindingGroup = new BindingGroup();
    		bindingGroup2 = new BindingGroup();
    		
    		SDCommandManager.RegisterNamedUIType("Global", typeof(UIElement));
    		SDCommandManager.RegisterNamedUIElement("NamedInstanceScope", namedInstanceScope);
    		
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomething" });
			
    		
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomethingOther" });
    		
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomethingOther" });
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Local",
				RoutedCommandName = "TestCommands.DoSomething" });
			
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "Local",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "UnrelatedScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther" });
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "UnrelatedScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther" });
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "NamedScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyElse" });
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "NamedScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyElse" });
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerInstanceName = "NamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerInstanceName = "NamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerInstanceName = "OtherNamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerInstanceName = "OtherNamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerInstanceName = "OtherNamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomethingElse" ,
				Groups = new BindingGroupCollection { bindingGroup2 }});
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerInstanceName = "OtherNamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomethingElse" ,
				Groups = new BindingGroupCollection { bindingGroup2 }});
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "UnrelatedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther" ,
				Groups = new BindingGroupCollection { bindingGroup }});
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "UnrelatedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther" ,
				Groups = new BindingGroupCollection { bindingGroup }});
    		
    		
    		SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "UnrelatedInstanceScope2",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther",
				Groups = new BindingGroupCollection { bindingGroup }});
    		
    		SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				OwnerTypeName = "UnrelatedInstanceScope2",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther",
				Groups = new BindingGroupCollection { bindingGroup }});
    	}
    	
    	[TearDown]
    	public void TearDown()
    	{
    		bindingGroup = null;
    		bindingGroup2 = null;
    		namedInstanceScope = null;
    		
    		SDCommandManager.Reset();
    	}
    	
    	[Test]
		public void FindCommandBindingInfosTests()
		{
			var doSomethingBindingInfos = SDCommandManager.FindCommandBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" });
			Assert.AreEqual(5, doSomethingBindingInfos.Count());
			
			var namedInstanceBindingInfos = SDCommandManager.FindCommandBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { OwnerInstances = new[] { namedInstanceScope },  RoutedCommandName = "TestCommands.DoSomething" });
			Assert.AreEqual(1, namedInstanceBindingInfos.Count());
			
			var groupBindingInfos = SDCommandManager.FindCommandBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { Groups = new BindingGroupCollection { bindingGroup }});
			Assert.AreEqual(2, groupBindingInfos.Count());
		}    	

    	[Test]
		public void FindInputBindingInfosTests()
		{
			var doSomethingBindingInfos = SDCommandManager.FindInputBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" });
			Assert.AreEqual(4, doSomethingBindingInfos.Count());
			
			var namedInstanceBindingInfos = SDCommandManager.FindInputBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { OwnerInstances = new[] { namedInstanceScope },  RoutedCommandName = "TestCommands.DoSomething" });
			Assert.AreEqual(1, namedInstanceBindingInfos.Count());
			
			var groupBindingInfos = SDCommandManager.FindInputBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { Groups = new BindingGroupCollection { bindingGroup }});
			Assert.AreEqual(2, groupBindingInfos.Count());
		}
		
		[Test]
		public void InvokeInputBindingsOnRoutedCommandRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("test1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, () => testResults.Add("test2"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommand = ApplicationCommands.Copy }, () => testResults.Add("ApplicationCommands.Copy1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "ApplicationCommands.Copy" }, () => testResults.Add("ApplicationCommands.Copy2"));
			
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "text");
			
			Assert.IsTrue(testResults.Contains("test1"));
			Assert.IsTrue(testResults.Contains("test2"));
			Assert.IsFalse(testResults.Contains("ApplicationCommands.Copy1"));
			
			SDCommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
			
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy1"));
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy2"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnRoutedCommandRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("test1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, () => testResults.Add("test2"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommand = ApplicationCommands.Copy }, () => testResults.Add("ApplicationCommands.Copy1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "ApplicationCommands.Copy" }, () => testResults.Add("ApplicationCommands.Copy2"));
			
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "text");
			
			Assert.IsTrue(testResults.Contains("test1"));
			Assert.IsTrue(testResults.Contains("test2"));
			Assert.IsFalse(testResults.Contains("ApplicationCommands.Copy1"));
			
			SDCommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
			
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy1"));
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy2"));
		}
		
		[Test]
		public void InvokeInputBindingsOnOwnerTypeRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypeName = "TestOwnerType" }, () => testResults.Add("TestOwnerType1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerTypeName = "TestOwnerType" }, () => testResults.Add("TestOwnerType2"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypes = new[] { typeof(UIElement) }}, () => testResults.Add("UIElement1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypeName = "UIElement" }, () => testResults.Add("UIElement2"));
			
			SDCommandManager.RegisterNamedUIType("TestOwnerType", typeof(UserControl));
			
			Assert.IsTrue(testResults.Contains("TestOwnerType1"));
			Assert.IsTrue(testResults.Contains("TestOwnerType2"));
			Assert.IsFalse(testResults.Contains("UIElement1"));
			Assert.IsFalse(testResults.Contains("UIElement2"));
			
			SDCommandManager.RegisterNamedUIType("UIElement", typeof(UIElement));
			
			Assert.IsTrue(testResults.Contains("UIElement1"));
			Assert.IsTrue(testResults.Contains("UIElement2"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnOwnerTypeRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypeName = "TestOwnerType" }, () => testResults.Add("TestOwnerType1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerTypeName = "TestOwnerType" }, () => testResults.Add("TestOwnerType2"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypes = new[] { typeof(UIElement) }}, () => testResults.Add("UIElement1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypeName = "UIElement" }, () => testResults.Add("UIElement2"));
			
			SDCommandManager.RegisterNamedUIType("TestOwnerType", typeof(UserControl));
			
			Assert.IsTrue(testResults.Contains("TestOwnerType1"));
			Assert.IsTrue(testResults.Contains("TestOwnerType2"));
			Assert.IsFalse(testResults.Contains("UIElement1"));
			Assert.IsFalse(testResults.Contains("UIElement2"));
			
			SDCommandManager.RegisterNamedUIType("UIElement", typeof(UIElement));
			
			Assert.IsTrue(testResults.Contains("UIElement1"));
			Assert.IsTrue(testResults.Contains("UIElement2"));
		}
		
		[Test]
		public void InvokeInputBindingsOnOwnerInstanceRegistration()
		{
			var testResults = new HashSet<string>();
			
			var uiElement = new UIElement();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstanceName = "TestOwnerInstance" }, () => testResults.Add("TestOwnerInstance1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerInstanceName = "TestOwnerInstance" }, () => testResults.Add("TestOwnerInstance2"));
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstances = new[] { uiElement }}, () => testResults.Add("UIElement1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstances = new[] { new UIElement() }}, () => testResults.Add("UIElement2"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstanceName = "UIElement" }, () => testResults.Add("UIElement3"));
			
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", new UIElement());
			
			Assert.IsTrue(testResults.Contains("TestOwnerInstance1"));
			Assert.IsTrue(testResults.Contains("TestOwnerInstance2"));
			Assert.IsFalse(testResults.Contains("UIElement1"));
			Assert.IsFalse(testResults.Contains("UIElement2"));
			Assert.IsFalse(testResults.Contains("UIElement3"));
			
			SDCommandManager.RegisterNamedUIElement("UIElement", uiElement);
			
			Assert.IsTrue(testResults.Contains("UIElement1"));
			Assert.IsFalse(testResults.Contains("UIElement2"));
			Assert.IsTrue(testResults.Contains("UIElement3"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnOwnerInstanceRegistration()
		{
			var testResults = new HashSet<string>();
			
			var uiElement = new UIElement();
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstanceName = "TestOwnerInstance" }, () => testResults.Add("TestOwnerInstance1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerInstanceName = "TestOwnerInstance" }, () => testResults.Add("TestOwnerInstance2"));
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstances = new[] { uiElement }}, () => testResults.Add("UIElement1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstances = new[] { new UIElement() }}, () => testResults.Add("UIElement2"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstanceName = "UIElement" }, () => testResults.Add("UIElement3"));
			
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", new UIElement());
			
			Assert.IsTrue(testResults.Contains("TestOwnerInstance1"));
			Assert.IsTrue(testResults.Contains("TestOwnerInstance2"));
			Assert.IsFalse(testResults.Contains("UIElement1"));
			Assert.IsFalse(testResults.Contains("UIElement2"));
			Assert.IsFalse(testResults.Contains("UIElement3"));
			
			SDCommandManager.RegisterNamedUIElement("UIElement", uiElement);
			
			Assert.IsTrue(testResults.Contains("UIElement1"));
			Assert.IsFalse(testResults.Contains("UIElement2"));
			Assert.IsTrue(testResults.Contains("UIElement3"));
		}
		
		[Test]
		public void InvokeBindingsUpdateHandlersOnInputBindingInfoRegistration()
		{
			var testResults = new HashSet<string>();
			 
			// Update handles is interested about changes in routed command
			SDCommandManager.RegisterRoutedUICommand("RoutedCommandName", "RoutedCommandText");
			var routedCommand = SDCommandManager.GetRoutedUICommand("RoutedCommandName");
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, () => testResults.Add("commandTest0"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName" }, () => testResults.Add("commandTest1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName", OwnerTypeName = "TestOwner" }, () => testResults.Add("commandTest2"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommand = routedCommand}, () => testResults.Add("commandTest3"));
			
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner"});
			
			Assert.IsFalse(testResults.Contains("commandTest0"));
			Assert.IsTrue(testResults.Contains("commandTest1"));
			Assert.IsTrue(testResults.Contains("commandTest2"));
			Assert.IsTrue(testResults.Contains("commandTest3"));
			
			
			// Update handles is interested about changes in owner type
			var testType = typeof(UserControl);
			SDCommandManager.RegisterNamedUIType("TestOwner", testType);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, () => testResults.Add("typeTest0"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("typeTest1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, () => testResults.Add("typeTest2"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypes = new[] { testType }}, () => testResults.Add("typeTest3"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypes = new[] { testType }}, () => testResults.Add("typeTest4"));
			
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerTypeName = "TestOwner"});
		
			Assert.IsFalse(testResults.Contains("typeTest0"));
			Assert.IsTrue(testResults.Contains("typeTest1"));
			Assert.IsTrue(testResults.Contains("typeTest2"));
			Assert.IsTrue(testResults.Contains("typeTest3"));
			Assert.IsTrue(testResults.Contains("typeTest4"));
			
			
			// Update handles is interested about changes in owner instance
			var testInstance = new UIElement();
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", testInstance);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, () => testResults.Add("instanceTest0"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("instanceTest1"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerInstanceName = "TestOwnerInstance" }, () => testResults.Add("instanceTest2"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerInstances = new[] { testInstance }}, () => testResults.Add("instanceTest3"));
			SDCommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstances = new[] { testInstance }}, () => testResults.Add("instanceTest4"));
			
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerInstanceName = "TestOwnerInstance"});
		
			Assert.IsFalse(testResults.Contains("instanceTest0"));
			Assert.IsTrue(testResults.Contains("instanceTest1"));
			Assert.IsTrue(testResults.Contains("instanceTest2"));
			Assert.IsTrue(testResults.Contains("instanceTest3"));
			Assert.IsTrue(testResults.Contains("instanceTest4"));
		}
		
		
		[Test]
		public void InvokeBindingsUpdateHandlersOnCommandBindingInfoRegistration()
		{
			var testResults = new HashSet<string>();
			 
			// Update handles is interested about changes in routed command
			SDCommandManager.RegisterRoutedUICommand("RoutedCommandName", "RoutedCommandText");
			var routedCommand = SDCommandManager.GetRoutedUICommand("RoutedCommandName");
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, () => testResults.Add("commandTest0"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName" }, () => testResults.Add("commandTest1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName", OwnerTypeName = "TestOwner" }, () => testResults.Add("commandTest2"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommand = routedCommand}, () => testResults.Add("commandTest3"));
			
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner"});
			
			Assert.IsFalse(testResults.Contains("commandTest0"));
			Assert.IsTrue(testResults.Contains("commandTest1"));
			Assert.IsTrue(testResults.Contains("commandTest2"));
			Assert.IsTrue(testResults.Contains("commandTest3"));
			
			
			// Update handles is interested about changes in owner type
			var testType = typeof(UserControl);
			SDCommandManager.RegisterNamedUIType("TestOwner", testType);
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, () => testResults.Add("typeTest0"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("typeTest1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, () => testResults.Add("typeTest2"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypes = new[] { testType }}, () => testResults.Add("typeTest3"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerTypes = new[] { testType }}, () => testResults.Add("typeTest4"));
			
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerTypeName = "TestOwner"});
		
			Assert.IsFalse(testResults.Contains("typeTest0"));
			Assert.IsTrue(testResults.Contains("typeTest1"));
			Assert.IsTrue(testResults.Contains("typeTest2"));
			Assert.IsTrue(testResults.Contains("typeTest3"));
			Assert.IsTrue(testResults.Contains("typeTest4"));
			
			
			// Update handles is interested about changes in owner instance
			var testInstance = new UIElement();
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", testInstance);
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, () => testResults.Add("instanceTest0"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("instanceTest1"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerInstanceName = "TestOwnerInstance" }, () => testResults.Add("instanceTest2"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerInstances = new[] { testInstance }}, () => testResults.Add("instanceTest3"));
			SDCommandManager.RegisterCommandBindingsUpdateHandler(new BindingInfoTemplate { OwnerInstances = new[] { testInstance }}, () => testResults.Add("instanceTest4"));
			
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerInstanceName = "TestOwnerInstance"});
		
			Assert.IsFalse(testResults.Contains("instanceTest0"));
			Assert.IsTrue(testResults.Contains("instanceTest1"));
			Assert.IsTrue(testResults.Contains("instanceTest2"));
			Assert.IsTrue(testResults.Contains("instanceTest3"));
			Assert.IsTrue(testResults.Contains("instanceTest4"));
		}
	}
}
