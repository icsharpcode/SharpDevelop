// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			ProcessPackageAction action = null;
			bool result = actions.GetNextAction(out action);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetNextAction_OneActionAdded_ReturnsActionInOutParameter()
		{
			CreateActions();
			ProcessPackageAction expectedAction = AddAction();
			
			ProcessPackageAction action = null;
			actions.GetNextAction(out action);
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void GetNextAction_OneActionAdded_ReturnsTrue()
		{
			CreateActions();
			ProcessPackageAction expectedAction = AddAction();
			
			ProcessPackageAction action = null;
			bool result = actions.GetNextAction(out action);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithOneActionAdded_ReturnsNullActionInOutParameterOnSecondCall()
		{
			CreateActions();
			ProcessPackageAction expectedAction = AddAction();
			ProcessPackageAction action = null;
			actions.GetNextAction(out action);
			actions.GetNextAction(out action);
			
			Assert.IsNull(action);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithOneActionAdded_ReturnsFalseOnSecondCall()
		{
			CreateActions();
			ProcessPackageAction expectedAction = AddAction();
			
			ProcessPackageAction action = null;
			actions.GetNextAction(out action);
			bool result = actions.GetNextAction(out action);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithTwoActionsAdded_ReturnsSecondActionAddedInOutParameter()
		{
			CreateActions();
			AddAction();
			ProcessPackageAction expectedAction = AddAction();
			ProcessPackageAction action = null;
			actions.GetNextAction(out action);
			actions.GetNextAction(out action);
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void GetNextAction_CalledTwiceWithTwoActionsAdded_ReturnsTrueOnSecondCall()
		{
			CreateActions();
			AddAction();
			ProcessPackageAction expectedAction = AddAction();
			ProcessPackageAction action = null;
			actions.GetNextAction(out action);
			bool result = actions.GetNextAction(out action);
			
			Assert.IsTrue(result);
		}
	}
}
