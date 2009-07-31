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
    	[TestFixtureSetUp]
    	public void TestFixtureSetUp()
    	{
    		PropertyService.InitializeServiceForUnitTests();
    	}
    	
    	[SetUp]
    	public void SetuUp()
    	{
    	}
    	
    	[TearDown]
    	public void TearDown()
    	{
    		SDCommandManager.Reset();
    	}
    	
		[Test]
		public void InvokeInputBindingUpdateHandlersManually()
		{
			var testResult = false;
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResult = true; });
			
			SDCommandManager.InvokeInputBindingUpdateHandlers(
				null, 
				new BindingsUpdatedHandlerArgs(),
				BindingInfoMatchType.SubSet, 
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" });
			
			Assert.IsTrue(testResult);
		}
		
		[Test]
		public void InvokeInputBindingUpdateHandlersWithTwoParamsManually()
		{
			var testResults = new List<string>();
			
			System.IO.File.AppendAllText("C:/test.txt", "Add" + Environment.NewLine);
					
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "InvokeOwner" }, 
				delegate { testResults.Add("TwoExactAttributes"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("LessMatchingAttributes"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest2" }, 
				delegate { testResults.Add("LessNotMatchingAttributes"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate(), 
				delegate { testResults.Add("NoAttributes"); });
			
			SDCommandManager.InvokeInputBindingUpdateHandlers(
				null, 
				new BindingsUpdatedHandlerArgs(),
				BindingInfoMatchType.SubSet, 
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "InvokeOwner" });
		
			Assert.IsTrue(testResults.Contains("TwoExactAttributes"));
			Assert.IsTrue(testResults.Contains("LessMatchingAttributes"));
			Assert.IsFalse(testResults.Contains("LessNotMatchingAttributes"));
			Assert.IsTrue(testResults.Contains("NoAttributes"));
		}
		
		[Test]
		public void InvokeInputBindingsOnRoutedCommandRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("test1"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, 
				delegate { testResults.Add("test2"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "ApplicationCommands.Copy" }, 
				delegate { testResults.Add("ApplicationCommands.Copy"); });
			
			
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "text");
			
			Assert.IsTrue(testResults.Contains("test1"));
			Assert.IsTrue(testResults.Contains("test2"));
			Assert.IsFalse(testResults.Contains("ApplicationCommands.Copy"));
			
			SDCommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
			
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnRoutedCommandRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("test1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, 
				delegate { testResults.Add("test2"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "ApplicationCommands.Copy" }, 
				delegate { testResults.Add("ApplicationCommands.Copy"); });
			
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "text");
			
			Assert.IsTrue(testResults.Contains("test1"));
			Assert.IsTrue(testResults.Contains("test2"));
			Assert.IsFalse(testResults.Contains("ApplicationCommands.Copy"));
			
			SDCommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
			
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy"));
		}
		
		[Test]
		public void InvokeInputBindingsOnOwnerTypeRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { OwnerTypeName = "TestOwnerType" }, 
				delegate { testResults.Add("TestOwnerType1"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerTypeName = "TestOwnerType" }, 
				delegate { testResults.Add("TestOwnerType2"); });
			
			SDCommandManager.RegisterNamedUIType("TestOwnerType", typeof(UserControl));
			
			Assert.IsTrue(testResults.Contains("TestOwnerType1"));
			Assert.IsTrue(testResults.Contains("TestOwnerType2"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnOwnerTypeRegistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { OwnerTypeName = "TestOwnerType" }, 
				delegate { testResults.Add("TestOwnerType1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerTypeName = "TestOwnerType" }, 
				delegate { testResults.Add("TestOwnerType2"); });
			
			SDCommandManager.RegisterNamedUIType("TestOwnerType", typeof(UserControl));
			
			Assert.IsTrue(testResults.Contains("TestOwnerType1"));
			Assert.IsTrue(testResults.Contains("TestOwnerType2"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnOwnerTypeUnregistration()
		{
			var testResults = new HashSet<string>();
			
			SDCommandManager.RegisterNamedUIType("TestOwnerType", typeof(UserControl));
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { OwnerTypeName = "TestOwnerType" }, 
				delegate { testResults.Add("TestOwnerType1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerTypeName = "TestOwnerType" }, 
				delegate { testResults.Add("TestOwnerType2"); });
			
			SDCommandManager.UnregisterNamedUIType("TestOwnerType", typeof(UserControl));
			
			Assert.IsTrue(testResults.Contains("TestOwnerType1"));
			Assert.IsTrue(testResults.Contains("TestOwnerType2"));
		}
		
		[Test]
		public void InvokeInputBindingsOnOwnerInstanceRegistration()
		{
			var testResults = new HashSet<string>();
			
			var uiElement = new UIElement();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("TestOwnerInstance1"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("TestOwnerInstance2"); });
			
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", new UIElement());
			
			Assert.IsTrue(testResults.Contains("TestOwnerInstance1"));
			Assert.IsTrue(testResults.Contains("TestOwnerInstance2"));
		}
		
		[Test]
		public void InvokeCommandBindingsOnOwnerInstanceRegistration()
		{
			var testResults = new HashSet<string>();
			
			var uiElement = new UIElement();
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("TestOwnerInstance1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("TestOwnerInstance2"); });
			
			
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", new UIElement());
			
			Assert.IsTrue(testResults.Contains("TestOwnerInstance1"));
			Assert.IsTrue(testResults.Contains("TestOwnerInstance2"));
		}
		
		
		[Test]
		public void InvokeCommandBindingsOnOwnerInstanceUnregistration()
		{
			var testResults = new HashSet<string>();
			var uiElement = new UIElement();
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", uiElement);
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", new UIElement());
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("TestOwnerInstance1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommand", OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("TestOwnerInstance2"); });
			
			SDCommandManager.UnregisterNamedUIElement("TestOwnerInstance", uiElement);
			
			Assert.IsTrue(testResults.Contains("TestOwnerInstance1"));
			Assert.IsTrue(testResults.Contains("TestOwnerInstance2"));
		}
		
		[Test]
		public void InvokeBindingsUpdateHandlersOnInputBindingInfoRegistration()
		{
			var testResults = new HashSet<string>();
			 
			// Update handles is interested about changes in routed command
			SDCommandManager.RegisterRoutedUICommand("RoutedCommandName", "RoutedCommandText");
			var routedCommand = SDCommandManager.GetRoutedUICommand("RoutedCommandName");
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, 
				delegate { testResults.Add("commandTest0"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName" }, 
				delegate { testResults.Add("commandTest1"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName", OwnerTypeName = "TestOwner" }, 
				delegate { testResults.Add("commandTest2"); });
			
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner"});
			
			Assert.IsFalse(testResults.Contains("commandTest0"));
			Assert.IsTrue(testResults.Contains("commandTest1"));
			Assert.IsFalse(testResults.Contains("commandTest2"));
			
			
			// Update handles is interested about changes in owner type
			var testType = typeof(UserControl);
			SDCommandManager.RegisterNamedUIType("TestOwner", testType);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, 
				delegate { testResults.Add("typeTest0"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("typeTest1"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, 
				delegate { testResults.Add("typeTest2"); });
			
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerTypeName = "TestOwner"});
		
			Assert.IsFalse(testResults.Contains("typeTest0"));
			Assert.IsTrue(testResults.Contains("typeTest1"));
			Assert.IsTrue(testResults.Contains("typeTest2"));
			
			
			// Update handles is interested about changes in owner instance
			var testInstance = new UIElement();
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", testInstance);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, 
				delegate { testResults.Add("instanceTest0"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("instanceTest1"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("instanceTest2"); });
			
			SDCommandManager.RegisterInputBinding(new InputBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerInstanceName = "TestOwnerInstance"});
		
			Assert.IsFalse(testResults.Contains("instanceTest0"));
			Assert.IsTrue(testResults.Contains("instanceTest1"));
			Assert.IsTrue(testResults.Contains("instanceTest2"));
		}
		
		
		[Test]
		public void InvokeBindingsUpdateHandlersOnCommandBindingInfoRegistration()
		{
			var testResults = new HashSet<string>();
			 
			// Update handles is interested about changes in routed command
			SDCommandManager.RegisterRoutedUICommand("RoutedCommandName", "RoutedCommandText");
			var routedCommand = SDCommandManager.GetRoutedUICommand("RoutedCommandName");
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, 
				delegate { testResults.Add("commandTest0"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName" }, 
				delegate { testResults.Add("commandTest1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "RoutedCommandName", OwnerTypeName = "TestOwner" }, delegate { testResults.Add("commandTest2"); });
			
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner"});
			
			Assert.IsFalse(testResults.Contains("commandTest0"));
			Assert.IsTrue(testResults.Contains("commandTest1"));
			Assert.IsFalse(testResults.Contains("commandTest2"));
			
			
			// Update handles is interested about changes in owner type
			var testType = typeof(UserControl);
			SDCommandManager.RegisterNamedUIType("TestOwner", testType);
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, 
				delegate { testResults.Add("typeTest0"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("typeTest1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, 
				delegate { testResults.Add("typeTest2"); });
			
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerTypeName = "TestOwner"});
		
			Assert.IsFalse(testResults.Contains("typeTest0"));
			Assert.IsTrue(testResults.Contains("typeTest1"));
			Assert.IsTrue(testResults.Contains("typeTest2"));
			
			
			// Update handles is interested about changes in owner instance
			var testInstance = new UIElement();
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", testInstance);
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeSomethingUnrelatedTest" }, 
				delegate { testResults.Add("instanceTest0"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, 
				delegate { testResults.Add("instanceTest1"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerInstanceName = "TestOwnerInstance" }, 
				delegate { testResults.Add("instanceTest2"); });
			
			SDCommandManager.RegisterCommandBinding(new CommandBindingInfo {
				RoutedCommandName = "InvokeTest",
				OwnerInstanceName = "TestOwnerInstance"});
		
			Assert.IsFalse(testResults.Contains("instanceTest0"));
			Assert.IsTrue(testResults.Contains("instanceTest1"));
			Assert.IsTrue(testResults.Contains("instanceTest2"));
		}
		
		[Test]
		public void InvokeUpdateHandlersOnGroupsSetTest()
		{
			var testResults = new HashSet<string>();
			
			var removedGroup = new BindingGroup { Name = "Removed" };
			var addedGroup = new BindingGroup { Name = "Added" };
			
			var bindingInfo = new CommandBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner",
				Groups = new BindingGroupCollection { removedGroup }
			};
			
			SDCommandManager.RegisterCommandBinding(bindingInfo);
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { Groups = new BindingGroupCollection { removedGroup } },
				delegate { testResults.Add("removedGroup"); });
			
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { Groups = new BindingGroupCollection { addedGroup } },
				delegate { testResults.Add("addedGroup"); });
			
			bindingInfo.Groups = new BindingGroupCollection { addedGroup };
			
			Assert.IsTrue(testResults.Contains("addedGroup"));
			Assert.IsTrue(testResults.Contains("removedGroup"));
		}
		
		[Test]
		public void InvokeUpdateHandlersOnGroupsClearTest()
		{
			var testResults = new HashSet<string>();
			
			var removedGroup = new BindingGroup();
			var addedGroup = new BindingGroup();
			
			var bindingInfo = new CommandBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner",
				Groups = new BindingGroupCollection { removedGroup }
			};
			
			SDCommandManager.RegisterCommandBinding(bindingInfo);
			SDCommandManager.RegisterCommandBindingsUpdateHandler(
				new BindingInfoTemplate { Groups = new BindingGroupCollection { removedGroup } },
				delegate { testResults.Add("removedGroup"); });
			
			bindingInfo.Groups.Clear();
			
			Assert.IsTrue(testResults.Contains("removedGroup"));
		}
		
		[Test]
		public void NamedUIElementOperationsTest()
		{
        	var uiElementName = "test";
        	var uiElement = new UIElement();
        	SDCommandManager.RegisterNamedUIElement(uiElementName, uiElement);
        	
        	// Map forward
        	var retrievedInstances = SDCommandManager.GetNamedUIElementCollection(uiElementName);
        	Assert.AreEqual(1, retrievedInstances.Count);
        	Assert.AreSame(retrievedInstances.First(), uiElement);
        	retrievedInstances = null;
        		
        	// Map backward
        	var retrievedNames = SDCommandManager.GetUIElementNameCollection(uiElement);
        	Assert.AreEqual(1, retrievedNames.Count);
        	Assert.AreSame(retrievedNames.First(), uiElementName);
        	retrievedNames = null;
        	
        	uiElement = null;        	
        	GC.Collect();
        	
        	// Map forward (after GC)
        	var retrievedAfterGCInstances = SDCommandManager.GetNamedUIElementCollection(uiElementName);
        	Assert.AreEqual(0, retrievedAfterGCInstances.Count);
        	
			// Map backward (after GC)
			Assert.Throws(typeof(ArgumentNullException), delegate { SDCommandManager.GetUIElementNameCollection(uiElement); });
		}
	}
}
