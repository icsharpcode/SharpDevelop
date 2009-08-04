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
			var result = false;
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {  
				if(args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified 
				   && args.AttachedInstances.Contains(uiElement)
				   && args.Groups.Contains(bindingGroup)) {
					result = true;
				}
			};
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			                                    
			Assert.IsEmpty(uiElement.InputBindings);
			
			bindingGroup.AttachTo(uiElement);
			Assert.AreEqual(1, uiElement.InputBindings.Count);
			Assert.IsTrue(result);
		}
        
		[Test]
		public void DetachGroupMethod()
		{
			var result = false;
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {  
				if(args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified 
				   && args.AttachedInstances.Contains(uiElement)
				   && args.Groups.Contains(bindingGroup)) {
					result = true;
				}
			};
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			bindingInfo.Groups.Add(bindingGroup);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			bindingGroup.AttachTo(uiElement);
			Assert.AreEqual(1, uiElement.InputBindings.Count);
			Assert.IsTrue(result);
			
			result = false;
			bindingGroup.DetachFrom(uiElement);
			Assert.IsEmpty(uiElement.InputBindings);
			Assert.IsTrue(result);
		}
		
		[Test]
		public void AddAttachedGroupTest()
		{
			var result = false;
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {  
				if(args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified 
				   && args.AttachedInstances.Contains(uiElement)
				   && args.Groups.Contains(bindingGroup)) {
					result = true;
				}
			};
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.RoutedCommandName = "ApplicationCommands.Copy";
			bindingInfo.OwnerInstanceName = "NamedInstance";
			bindingInfo.DefaultGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			Assert.IsFalse(result);
			
			bindingGroup.AttachTo(uiElement);
			bindingInfo.Groups.Add(bindingGroup);
			Assert.IsTrue(result);
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
			
			bindingGroup.AttachTo(otherUIElement);
			Assert.IsEmpty(otherUIElement.InputBindings);
			
        	SDCommandManager.RegisterNamedUIElement("OtherNamedInstance", otherUIElement);
        	Assert.AreEqual(1, otherUIElement.InputBindings.Count);
        	
        	SDCommandManager.UnregisterNamedUIElement("OtherNamedInstance", otherUIElement);
        	Assert.IsEmpty(otherUIElement.InputBindings);
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
