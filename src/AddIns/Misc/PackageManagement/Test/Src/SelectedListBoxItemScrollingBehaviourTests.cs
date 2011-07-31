// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.PackageManagement;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SelectedListBoxItemScrollingBehaviourTests
	{
		DependencyPropertyChangedEventArgs eventArgs;
		ListBox listBox;
		TestableSelectedListBoxItemScrollingBehaviour behaviour;
		
		[SetUp]
		public void Init()
		{
			listBox = new ListBox();
		}
		
		void CreateEventArgs(object oldValue, object newValue)
		{
			eventArgs = new DependencyPropertyChangedEventArgs(ListBoxBehaviour.IsSelectedItemScrolledIntoViewProperty, oldValue, newValue);
		}
		
		void CreateBehaviour()
		{
			CreateBehaviour(listBox);
		}
		
		void CreateBehaviour(DependencyObject dependencyObject)
		{
			behaviour = new TestableSelectedListBoxItemScrollingBehaviour(dependencyObject, eventArgs);
		}
		
		[Test]
		public void Update_DependencyPropertyChangedFromFalseToTrue_SelectionChangedEventRegistered()
		{
			CreateEventArgs(false, true);
			CreateBehaviour();
			
			behaviour.Update();
			
			Assert.IsTrue(behaviour.IsRegisterSelectionChangedHandlerCalled);
		}
		
		[Test]
		public void Update_DependencyPropertyChangedFromFalseToTrue_ListBoxPassedToRegisterSelectionChangedHandler()
		{
			CreateEventArgs(false, true);
			CreateBehaviour();
			
			behaviour.Update();
			
			Assert.AreEqual(listBox, behaviour.ListBoxPassedToRegisterSelectionChangedHandler);
		}
		
		[Test]
		public void Update_DependencyPropertyChangedFromTrueToFalse_ListBoxPassedToUnregisterSelectionChangedHandler()
		{
			CreateEventArgs(true, false);
			CreateBehaviour();
			
			behaviour.Update();
			
			Assert.AreEqual(listBox, behaviour.ListBoxPassedToUnregisterSelectionChangedHandler);
		}
		
		[Test]
		public void Update_DependencyPropertyNewValueIsNotBoolean_SelectionChangedEventHandlerNotRegisteredOrUnregistered()
		{
			CreateEventArgs(true, new object());
			CreateBehaviour();
			behaviour.Update();
			
			Assert.IsFalse(behaviour.IsRegisterSelectionChangedHandlerCalled);
			Assert.IsFalse(behaviour.IsUnregisterSelectionChangedHandlerCalled);
		}
		
		[Test]
		public void Update_DependencyObjectIsNotListBox_SelectionChangedEventHandlerNotRegisteredOrUnregistered()
		{
			CreateEventArgs(true, false);
			CreateBehaviour(new DependencyObject());
			behaviour.Update();
			
			Assert.IsFalse(behaviour.IsRegisterSelectionChangedHandlerCalled);
			Assert.IsFalse(behaviour.IsUnregisterSelectionChangedHandlerCalled);
		}
		
		[Test]
		public void OnListBoxSelectionChanged_OneItemInSelection_ScrolledIntoViewIsCalled()
		{
			behaviour.CallOnListBoxSelectionChanged(listBox);
			
			Assert.IsTrue(behaviour.IsScrollListBoxItemIntoViewCalled);
		}
		
		[Test]
		public void OnListBoxSelectionChanged_OneItemInSelection_ListBoxPassedToScrolledIntoView()
		{
			CreateBehaviour();
			behaviour.CallOnListBoxSelectionChanged(listBox);
			
			Assert.AreEqual(listBox, behaviour.ListBoxPassedToScrollListBoxItemIntoView);
		}
		
		[Test]
		public void OnListBoxSelectionChanged_OneItemInSelection_SelectedItemPassedToScrolledIntoView()
		{
			CreateBehaviour();
			string selectedItem = "a";
			behaviour.CallOnListBoxSelectionChanged(listBox, selectedItem);
			
			Assert.AreEqual(selectedItem, behaviour.ItemPassedToScrollListBoxItemIntoView);
		}
		
		[Test]
		public void OnListBoxSelectionChanged_SenderAndOriginalSourceAreDifferentObjects_ScrolledIntoViewNotCalled()
		{
			CreateBehaviour();
			ListBox sender = new ListBox();
			ListBox originalSource = new ListBox();
			string[] addedItems = new string[] { "a" };
			behaviour.CallOnListBoxSelectionChanged(sender, originalSource, addedItems);
			
			Assert.IsFalse(behaviour.IsScrollListBoxItemIntoViewCalled);
		}
		
		[Test]
		public void OnListBoxSelectionChanged_NoItemsSelected_ScrolledIntoViewNotCalled()
		{
			CreateBehaviour();
			string[] addedItems = new string[0];
			behaviour.CallOnListBoxSelectionChanged(listBox, listBox, addedItems);
			
			Assert.IsFalse(behaviour.IsScrollListBoxItemIntoViewCalled);
		}
		
		[Test]
		public void OnListBoxSelectionChanged_SenderAndOriginalSourceAreSameObjectButNotListBoxes_ScrolledIntoViewNotCalled()
		{
			CreateBehaviour();
			object sender = new object();
			string[] addedItems = new string[] { "a" };
			behaviour.CallOnListBoxSelectionChanged(sender, sender, addedItems);
			
			Assert.IsFalse(behaviour.IsScrollListBoxItemIntoViewCalled);
		}
	}
}
