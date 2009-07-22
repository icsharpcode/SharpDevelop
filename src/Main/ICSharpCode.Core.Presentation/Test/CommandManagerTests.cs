using ICSharpCode.Core.Presentation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation.Tests
{
	[TestFixture]
    public class CommandManagerTests
    {
    	UIElement namedInstanceScope;
    	BindingGroup bindingGroup;
    	BindingGroup bindingGroup2;
    	
    	[SetUp]
    	public void SetuUp()
    	{
    		namedInstanceScope = new UIElement();
    		bindingGroup = new BindingGroup();
    		bindingGroup2 = new BindingGroup();
    		
    		CommandManager.RegisterNamedUIType("Global", typeof(UIElement));
    		CommandManager.RegisterNamedUIElement("NamedInstanceScope", namedInstanceScope);
    		
			CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomething" });
			
    		// Register binding with same values
			CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
			CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Global",
				RoutedCommandName = "TestCommands.DoSomethingOther" });
    		
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "Local",
				RoutedCommandName = "TestCommands.DoSomething" });
			
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "UnrelatedScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther" });
    		
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "NamedScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyElse" });
    		
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerInstanceName = "NamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerInstanceName = "OtherNamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomething" });
    		
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerInstanceName = "OtherNamedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomethingElse" ,
				Groups = new BindingGroupCollection { bindingGroup2 }});
    					
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
				OwnerTypeName = "UnrelatedInstanceScope",
				RoutedCommandName = "TestCommands.DoSomethingCompletelyOther" ,
				Groups = new BindingGroupCollection { bindingGroup }});
    		
    		CommandManager.RegisterInputBinding(new InputBindingInfo {
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
    		
    		CommandManager.UnregisterInputBinding(BindingInfoMatchType.SuperSet, new BindingInfoTemplate());
    		CommandManager.UnregisterCommandBinding(BindingInfoMatchType.SuperSet, new BindingInfoTemplate());
    		
    		CommandManager.UnregisterNamedUIType("Global", typeof(UIElement));
    		CommandManager.UnregisterNamedUIElement("NamedInstanceScope", namedInstanceScope);
    	}
    	
    	
    	[Test]
		public void FindInputBindingInfosTests()
		{
			var doSomethingBindingInfos = CommandManager.FindInputBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" });
			Assert.AreEqual(4, doSomethingBindingInfos.Count());
			
			var namedInstanceBindingInfos = CommandManager.FindInputBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { OwnerInstances = new[] { namedInstanceScope },  RoutedCommandName = "TestCommands.DoSomething" });
			Assert.AreEqual(1, namedInstanceBindingInfos.Count());
			
			var groupBindingInfos = CommandManager.FindInputBindingInfos(
									BindingInfoMatchType.SuperSet, 
									new BindingInfoTemplate { Groups = new BindingGroupCollection { bindingGroup }});
			Assert.AreEqual(2, groupBindingInfos.Count());
		}
		
		[Test]
		public void InvokeBindingsOnRoutedCommandRegistration()
		{
			var testResults = new HashSet<string>();
			
			CommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest" }, () => testResults.Add("test1"));
			CommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "InvokeTest", OwnerTypeName = "TestOwner" }, () => testResults.Add("test2"));
			CommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommand = ApplicationCommands.Copy }, () => testResults.Add("ApplicationCommands.Copy1"));
			CommandManager.RegisterInputBindingsUpdateHandler(new BindingInfoTemplate { RoutedCommandName = "ApplicationCommands.Copy" }, () => testResults.Add("ApplicationCommands.Copy2"));
			
			CommandManager.RegisterRoutedUICommand("InvokeTest", "text");
			
			Assert.IsTrue(testResults.Contains("test1"));
			Assert.IsTrue(testResults.Contains("test2"));
			Assert.IsFalse(testResults.Contains("ApplicationCommands.Copy1"));
			
			CommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
			
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy1"));
			Assert.IsTrue(testResults.Contains("ApplicationCommands.Copy2"));
		}
	}
}
