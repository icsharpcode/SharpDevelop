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
			bindingGroup = new BindingGroup { Name = "BindingGroupTests" };
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
				delegate { results.Add("SubSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "NamedInstance",
					Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("SuperSetTest"); });
			                                      
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
				delegate { results.Add("SuperSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "NamedInstance",
					Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("SubSetTest"); });
			
			bindingGroup.DetachFrom(uiElement);
			Assert.AreEqual(0, uiElement.InputBindings.Count);
			Assert.Contains("SubSetTest", results);
			Assert.Contains("SuperSetTest", results);	
		}
		
		[Test]
		public void AddAttachedGroupTest()
		{
			var results = new List<string>();
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate { Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("SuperSetTest"); });
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			Assert.IsFalse(results.Contains("SuperSetTest"));
			
			bindingGroup.AttachTo(uiElement);
			
			bindingInfo.Groups.Add(bindingGroup);
			Assert.Contains("SuperSetTest", results);
			Assert.IsTrue(uiElement.InputBindings[0].Command == ApplicationCommands.Copy);
		}
        
		[Test]
		public void RegisterNamedInstanceAfterGroupAttachTest()
		{
			var results = new List<string>();
			
			var otherUIElement = new UIElement();
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "OtherNamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate(), 
				delegate { results.Add("SubSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "OtherNamedInstance",
					Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("OtherSuperSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "NamedInstance",
					Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("SuperSetTest"); });
			                                     
			bindingGroup.AttachTo(otherUIElement);
			Assert.AreEqual(0, uiElement.InputBindings.Count);
			Assert.IsFalse(results.Contains("SubSetTest"));
			Assert.IsFalse(results.Contains("OtherSuperSetTest"));
			Assert.IsFalse(results.Contains("SuperSetTest"));
			
        	SDCommandManager.RegisterNamedUIElement("OtherNamedInstance", otherUIElement);
			Assert.AreEqual(1, otherUIElement.InputBindings.Count);
			Assert.IsTrue(results.Contains("SubSetTest"));
			Assert.IsTrue(results.Contains("OtherSuperSetTest"));
			Assert.IsFalse(results.Contains("SuperSetTest"));
		}
		[Test]
		public void UnregisterNamedInstanceAttachedToGroupTest()
		{
			var results = new List<string>();
			
			var otherUIElement = new UIElement();
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "OtherNamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			bindingGroup.AttachTo(otherUIElement);
        	SDCommandManager.RegisterNamedUIElement("OtherNamedInstance", otherUIElement);
			Assert.AreEqual(1, otherUIElement.InputBindings.Count);
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate(), 
				delegate { results.Add("SubSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "OtherNamedInstance",
					Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("OtherSuperSetTest"); });
			
			SDCommandManager.RegisterInputBindingsUpdateHandler(
				new BindingInfoTemplate {                                                
					RoutedCommandName = "ApplicationCommands.Copy",
					OwnerInstanceName = "NamedInstance",
					Groups = new BindingGroupCollection { bindingGroup } },
				delegate { results.Add("SuperSetTest"); });
			
			SDCommandManager.UnregisterNamedUIElement("OtherNamedInstance", otherUIElement);
			
			Assert.AreEqual(0, otherUIElement.InputBindings.Count);
			Assert.IsTrue(results.Contains("SubSetTest"));
			Assert.IsTrue(results.Contains("OtherSuperSetTest"));
			Assert.IsFalse(results.Contains("SuperSetTest"));
		}
		
    	[Test]
		public void BindingGroupCollectionClearTest()
		{
			var group = new BindingGroup();
			var groups = new BindingGroupCollection{ group };
			
			var result = false;
			groups.CollectionChanged += delegate { result = true; };
			
			Assert.IsFalse(result);
			groups.Clear();
			Assert.IsTrue(result);
		}
    }
}
