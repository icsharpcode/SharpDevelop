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
		public void InvokeBindingUpdateHandlersManually()
		{
			var testResult = false;
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				if(args.Action == NotifyBindingsChangedAction.NamedTypeModified && args.OldNamedTypes.Contains(typeof(UIElement))) {
					testResult = true;
				}
			};
			
			SDCommandManager.InvokeBindingsChanged(
				null, 
				new NotifyBindingsChangedEventArgs(
					NotifyBindingsChangedAction.NamedTypeModified,
					"SomeType",
					new [] { typeof(UIElement) },
					new Type[0]));
			
			Assert.IsTrue(testResult);
		}
		
		[Test]
		public void InvokeInputBindingsOnRoutedCommandRegistration()
		{
			var testResult = false;
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				if(args.Action == NotifyBindingsChangedAction.RoutedUICommandModified && args.RoutedCommandName == "InvokeTest") {
					testResult = true;
				}
			};
			
			var existingCommandTestResult = false;
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				if(args.Action == NotifyBindingsChangedAction.RoutedUICommandModified && args.RoutedCommandName == "ApplicationCommands.Copy") {
					existingCommandTestResult = true;
				}
			};
			
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "text");
			Assert.IsTrue(testResult);
			Assert.IsFalse(existingCommandTestResult);
			
			SDCommandManager.RegisterRoutedUICommand(ApplicationCommands.Copy);
			Assert.IsTrue(existingCommandTestResult);
		}
		
		[Test]
		public void InvokeInputBindingsOnOwnerTypeRegistration()
		{
			var results = new HashSet<string>();
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				if(args.Action == NotifyBindingsChangedAction.NamedTypeModified && args.TypeName == "TestOwnerType") {
					if(args.NewNamedTypes.Contains(typeof(UserControl)) && !args.OldNamedTypes.Contains(typeof(UserControl))) {
						results.Add("UserControlAdded");
					}
					
					if(args.OldNamedTypes.Contains(typeof(UserControl)) && !args.NewNamedTypes.Contains(typeof(UserControl))) {
						results.Add("UserControlRemoved");
					}
				}
			};
			
			results.Clear();
			SDCommandManager.RegisterNamedUIType("TestOwnerType", typeof(UserControl));
			Assert.IsTrue(results.Contains("UserControlAdded"));
			Assert.IsFalse(results.Contains("UserControlRemoved"));
			
			results.Clear();
			SDCommandManager.UnregisterNamedUIType("TestOwnerType", typeof(UserControl));
			Assert.IsFalse(results.Contains("UserControlAdded"));
			Assert.IsTrue(results.Contains("UserControlRemoved"));
		}
		
		[Test]
		public void InvokeInputBindingsOnOwnerInstanceRegistration()
		{
			var results = new HashSet<string>();
			
			var uiElement = new UIElement();
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				if(args.Action == NotifyBindingsChangedAction.NamedInstanceModified && args.UIElementName == "TestOwner") {
					if(args.NewNamedUIElements.Contains(uiElement) && !args.OldNamedUIElements.Contains(uiElement)) {
						results.Add("UIElementAdded");
					}
					
					if(!args.NewNamedUIElements.Contains(uiElement) && args.OldNamedUIElements.Contains(uiElement)) {
						results.Add("UIElementRemoved");
					}
				}
			};
			
			results.Clear();
			SDCommandManager.RegisterNamedUIElement("TestOwner", uiElement);
			Assert.IsTrue(results.Contains("UIElementAdded"));
			Assert.IsFalse(results.Contains("UIElementRemoved"));
			
			results.Clear();
			SDCommandManager.UnregisterNamedUIElement("TestOwner", uiElement);
			Assert.IsFalse(results.Contains("UIElementAdded"));
			Assert.IsTrue(results.Contains("UIElementRemoved"));
		}
		
		[Test]
		public void InvokeBindingsUpdateHandlersOnInputBindingInfoRegistration()
		{
			var testResult = false;
			
			var uiElement = new UIElement();
			var bindingInfo = new InputBindingInfo { RoutedCommandName = "InvokeTest", OwnerInstanceName = "TestOwnerInstance", DefaultGestures = new ObservableInputGestureCollection { new KeyGesture(Key.K, ModifierKeys.Control) } };
			 
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "RoutedCommandText");
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", uiElement);
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				var template = new BindingInfoTemplate(bindingInfo, false);
				var contains = args.ModifiedBindingInfoTemplates.Contains(template);
				if(args.Action == NotifyBindingsChangedAction.BindingInfoModified && contains) {
					testResult = true;
				}
			};
			
			Assert.IsEmpty(uiElement.InputBindings);
			
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			Assert.IsTrue(testResult);
			Assert.AreEqual(1, uiElement.InputBindings.Count);
			Assert.AreEqual("InvokeTest", ((RoutedUICommand)uiElement.InputBindings[0].Command).Name);
		}
		
		[Test]
		public void InvokeBindingsUpdateHandlersOnCommandBindingInfoRegistration()
		{
			var testResult = false;
			
			var uiElement = new UIElement();
			var bindingInfo = new CommandBindingInfo { RoutedCommandName = "InvokeTest", OwnerInstanceName = "TestOwnerInstance"};
			 
			SDCommandManager.RegisterRoutedUICommand("InvokeTest", "RoutedCommandText");
			SDCommandManager.RegisterNamedUIElement("TestOwnerInstance", uiElement);
			
			SDCommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) {
				if(args.Action == NotifyBindingsChangedAction.BindingInfoModified && args.ModifiedBindingInfoTemplates.Contains(bindingInfo)) {
					testResult = true;
				}
			};
			
			Assert.IsEmpty(uiElement.InputBindings);
			
			SDCommandManager.RegisterCommandBinding(bindingInfo);
			
			Assert.IsTrue(testResult);
			Assert.AreEqual(1, uiElement.CommandBindings.Count);
			Assert.AreEqual("InvokeTest", ((RoutedUICommand)uiElement.CommandBindings[0].Command).Name);
		}
		
		[Test]
		public void GroupAddRemoveTests()
		{
			var testResults = new HashSet<string>();
			
			var removedGroup = new BindingGroup { Name = "Removed" };
			var addedGroup = new BindingGroup { Name = "Added" };
			
			var bindingInfo = new CommandBindingInfo {
				RoutedCommandName = "RoutedCommandName",
				OwnerTypeName = "SomeOwner",
				Groups = new BindingGroupCollection { removedGroup }
			};
			
			CommandManager.BindingsChanged += delegate(object sender, NotifyBindingsChangedEventArgs args) { 
				if(args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified && args.Groups.Contains(addedGroup)) {
					testResults.Add("GroupAdded");
				}
				
				if(args.Action == NotifyBindingsChangedAction.GroupAttachmendsModified && args.Groups.Contains(removedGroup)) {
					testResults.Add("GroupRemoved");
				}
			};
			
			SDCommandManager.RegisterCommandBinding(bindingInfo);
			
			testResults.Clear();
			bindingInfo.Groups = new BindingGroupCollection { addedGroup };
			Assert.IsTrue(testResults.Contains("GroupAdded"));
			Assert.IsTrue(testResults.Contains("GroupRemoved"));
			bindingInfo.Groups.Clear();
				
			testResults.Clear();
			bindingInfo.Groups.Add(addedGroup);
			Assert.IsTrue(testResults.Contains("GroupAdded"));
			
			testResults.Clear();
			bindingInfo.Groups.Remove(addedGroup);
			Assert.IsTrue(testResults.Contains("GroupAdded"));
			
			bindingInfo.Groups.Add(removedGroup);
			testResults.Clear();
			bindingInfo.Groups.Clear();
			Assert.IsTrue(testResults.Contains("GroupRemoved"));
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
		
		[Test]
		public void InvokeGesturesChangedManualyTest()
		{
			var result = false;
			
			var oldGestures = new InputGestureCollection();
			var newGestures = new InputGestureCollection();
			
			var identifier = new InputBindingIdentifier { OwnerInstanceName = "SomeOwner", RoutedCommandName = "SomeCommand" };
			
			SDCommandManager.GesturesChanged += delegate(object sender, NotifyGesturesChangedEventArgs args) { 
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.All(
				   	d => d.InputBindingIdentifier.Equals(identifier)
				   		&& d.OldGestures == oldGestures
						&& d.NewGestures == newGestures)) {
					result = true;
				}
			};
			
			SDCommandManager.InvokeGesturesChanged(
				null, 
				new NotifyGesturesChangedEventArgs(
					new GesturesModificationDescription(
						identifier,
						oldGestures,
						newGestures
					)));
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void InvokeGesturesChangedOnCurrentProfileSetTest()
		{
			var results = new HashSet<string>();
			
			var newGestures = new InputGestureCollection();
			newGestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			newGestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt));
			
			var identifier = new InputBindingIdentifier { OwnerInstanceName = "SomeOwner", RoutedCommandName = "SomeCommand" };
			
			SDCommandManager.GesturesChanged += delegate(object sender, NotifyGesturesChangedEventArgs args) { 
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.Any(
				   	d => d.InputBindingIdentifier.Equals(identifier)
				   		&& d.OldGestures.Count == 0
						&& d.NewGestures.Count == 2)) {
					results.Add("SetResult");
				}
				
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.Any(
				   	d => d.InputBindingIdentifier.Equals(identifier)
				   		&& d.OldGestures.Count == 2
						&& d.NewGestures.Count == 0)) {
					results.Add("ResetResult");
				}
			};
			
			var profile = new UserGestureProfile();
			profile[identifier] = newGestures;
			
			results.Clear();
			UserGestureManager.CurrentProfile = profile;
			Assert.IsTrue(results.Contains("SetResult"));
			Assert.IsFalse(results.Contains("ResetResult"));
			
			results.Clear();
			UserGestureManager.CurrentProfile = null;
			Assert.IsFalse(results.Contains("SetResult"));
			Assert.IsTrue(results.Contains("ResetResult"));
		}
		
		[Test]
		public void InvokeGesturesChangedOnDefaultGesturesSetTest()
		{
			var results = new HashSet<string>();
			
			var gestures = new InputGestureCollection();
			gestures.Add(new KeyGesture(Key.C, ModifierKeys.Alt));
			gestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt));
			
			var profileGestures = new InputGestureCollection();
			profileGestures.Add(new KeyGesture(Key.T, ModifierKeys.Alt));
			
			var bindingInfo = new InputBindingInfo();
			bindingInfo.OwnerInstanceName = "SomeOwner";
			bindingInfo.RoutedCommandName = "SomeCommand";
			bindingInfo.DefaultGestures.AddRange(gestures);
			SDCommandManager.RegisterInputBinding(bindingInfo);
			
			SDCommandManager.GesturesChanged += delegate(object sender, NotifyGesturesChangedEventArgs args) { 
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.Any(
				   	d => d.InputBindingIdentifier.Equals(bindingInfo.Identifier)
				   		&& d.OldGestures.Count == 2
						&& d.NewGestures.Count == 3)) {
					results.Add("GestureAdded");
				}
				
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.Any(
				   	d => d.InputBindingIdentifier.Equals(bindingInfo.Identifier)
				   		&& d.OldGestures.Count == 3
						&& d.NewGestures.Count == 2)) {
					results.Add("GestureRemoved");
				}
				
				
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.Any(
				   	d => d.InputBindingIdentifier.Equals(bindingInfo.Identifier)
				   		&& d.OldGestures.Count == 0
						&& d.NewGestures.Count == 2)) {
					results.Add("GesturesSet");
				}
				
				if(args.ModificationDescriptions.Count == 1 
				   && args.ModificationDescriptions.Any(
				   	d => d.InputBindingIdentifier.Equals(bindingInfo.Identifier)
				   		&& d.OldGestures.Count == 2
						&& d.NewGestures.Count == 0)) {
					results.Add("GesturesReset");
				}
			};
			
			var key = new KeyGesture(Key.M, ModifierKeys.Control | ModifierKeys.Shift);
			
			results.Clear();
			var c = bindingInfo.DefaultGestures.Count;
			bindingInfo.DefaultGestures.Add(key);
			Assert.IsTrue(results.Contains("GestureAdded"));
			Assert.IsFalse(results.Contains("GestureRemoved"));
			
			results.Clear();
			bindingInfo.DefaultGestures.Remove(key);
			Assert.IsTrue(results.Contains("GestureRemoved"));
			Assert.IsFalse(results.Contains("GestureAdded"));
			
			var profile = new UserGestureProfile();
			profile[bindingInfo.Identifier] = new InputGestureCollection();
			UserGestureManager.CurrentProfile = profile;
			
			// User defined gestures are used
			results.Clear();
			bindingInfo.DefaultGestures.Add(key);
			bindingInfo.DefaultGestures.Remove(key);
			Assert.IsFalse(results.Contains("GestureRemoved"));
			Assert.IsFalse(results.Contains("GestureAdded"));
			
			profile[bindingInfo.Identifier] = null;
			results.Clear();
			var backupDefaultGestures = bindingInfo.DefaultGestures;
			bindingInfo.DefaultGestures = null;
			Assert.IsTrue(results.Contains("GesturesReset"));
			
			bindingInfo.DefaultGestures = backupDefaultGestures;
			Assert.IsTrue(results.Contains("GesturesSet"));
		}
	}
}
