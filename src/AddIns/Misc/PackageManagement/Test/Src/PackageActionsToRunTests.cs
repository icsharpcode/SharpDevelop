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

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageActionsToRunTests
	{
		PackageActionsToRun actions;
		
		void CreateActions()
		{
			actions = new PackageActionsToRun();
		}
		
		InstallPackageAction AddAction()
		{
			var project = new FakePackageManagementProject();
			var events = new FakePackageManagementEvents();
			var action = new InstallPackageAction(project, events);
			actions.AddAction(action);
			return action;
		}
		
		[Test]
		public void GetNextAction_NewInstance_ReturnsFalse()
		{
			CreateActions();
			IPackageAction action = null;
			bool result = actions.GetNextAction(out action);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetNextAction_OneActionAdded_ReturnsActionInOutParameter()
		{
			CreateActions();
			ProcessPackageAction expectedAction = AddAction();
			
			IPackageAction action = null;
			actions.GetNextAction(out action);
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void GetNextAction_OneActionAdded_ReturnsTrue()
		{
			CreateActions();
			ProcessPackageAction expectedAction = AddAction();
			
			IPackageAction action = null;
			bool result = actions.GetNextAction(out action);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithOneActionAdded_ReturnsNullActionInOutParameterOnSecondCall()
		{
			CreateActions();
			IPackageAction expectedAction = AddAction();
			IPackageAction action = null;
			actions.GetNextAction(out action);
			actions.GetNextAction(out action);
			
			Assert.IsNull(action);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithOneActionAdded_ReturnsFalseOnSecondCall()
		{
			CreateActions();
			IPackageAction expectedAction = AddAction();
			
			IPackageAction action = null;
			actions.GetNextAction(out action);
			bool result = actions.GetNextAction(out action);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithTwoActionsAdded_ReturnsSecondActionAddedInOutParameter()
		{
			CreateActions();
			AddAction();
			IPackageAction expectedAction = AddAction();
			IPackageAction action = null;
			actions.GetNextAction(out action);
			actions.GetNextAction(out action);
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithTwoActionsAdded_ReturnsTrueOnSecondCall()
		{
			CreateActions();
			AddAction();
			IPackageAction expectedAction = AddAction();
			IPackageAction action = null;
			actions.GetNextAction(out action);
			bool result = actions.GetNextAction(out action);
			
			Assert.IsTrue(result);
		}
	}
}
