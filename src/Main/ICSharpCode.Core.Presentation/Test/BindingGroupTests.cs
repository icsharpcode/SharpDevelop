using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using SDCommandManager = ICSharpCode.Core.Presentation.CommandManager;
using NUnit.Framework;

namespace ICSharpCode.Core.Presentation.Tests
{
    [TestFixture]
    public class BindingGroupTests
    {
		BindingGroup bindingGroup;
		UIElement uiElement;
    	
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			PropertyService.InitializeServiceForUnitTests();
		}
		
		[SetUp]
		public void SetUp()
		{
        	bindingGroup = new BindingGroup();
        	uiElement = new UIElement();
        	
        	SDCommandManager.RegisterNamedUIElement("NamedInstance", uiElement);
        	SDCommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
		}
		
		[TearDown]
		public void TearDown()
		{
			SDCommandManager.Reset();
		}
        
		[Test]
		public void AttachGroupMethod()
		{
			var results = new List<string>();
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate(), 
				delegate() { results.Add("SubSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "NamedInstance",
					Group = bindingGroup },
				delegate() { results.Add("SuperSetTest"); });
			                                      
			Assert.AreEqual(0, uiElement.InputBindings.Count);
			
			bindingGroup.AttachTo(uiElement);
			Assert.AreEqual(1, uiElement.InputBindings.Count);
			Assert.Contains("SubSetTest", results);
			Assert.Contains("SuperSetTest", results);
		}
        
		[Test]
		public void DetachGroupMethod()
		{
			var results = new List<string>();
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			bindingGroup.AttachTo(uiElement);
			Assert.AreEqual(1, uiElement.InputBindings.Count);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate(), 
				delegate() { results.Add("SuperSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "NamedInstance",
					Group = bindingGroup },
				delegate() { results.Add("SubSetTest"); });
			
			bindingGroup.DetachFrom(uiElement);
			Assert.AreEqual(0, uiElement.InputBindings.Count);
			Assert.Contains("SubSetTest", results);
			Assert.Contains("SuperSetTest", results);	
		}
		
		[Test]
		public void AddGroupTest()
		{
			var results = new List<string>();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { Group = bindingGroup },
				delegate() { results.Add("SuperSetTest"); });
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			Assert.Contains("SuperSetTest", results);
		}
    }
}
