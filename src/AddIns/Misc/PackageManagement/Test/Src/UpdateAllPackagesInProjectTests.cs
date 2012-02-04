// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class UpdateAllPackagesInProjectTests
	{
		UpdateAllPackagesInProject updateAllPackagesInProject;
		FakePackageManagementProject fakeProject;
		List<UpdatePackageAction> updateActions;
		
		void CreateUpdateAllPackagesInProject()
		{
			fakeProject = new FakePackageManagementProject();
			updateAllPackagesInProject = new UpdateAllPackagesInProject(fakeProject);
		}
		
		void AddPackageToProject(string packageId)
		{
			var package = new FakePackage(packageId, "1.0");
			fakeProject.FakePackagesInReverseDependencyOrder.Add(package);
		}
		
		void CallCreateActions()
		{
			IEnumerable<UpdatePackageAction> actions = updateAllPackagesInProject.CreateActions();
			updateActions = actions.ToList();
		}
		
		UpdatePackageAction FirstUpdateAction {
			get { return updateActions[0]; }
		}
		
		[Test]
		public void CreateActions_OnePackageInProject_ReturnsOneAction()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			CallCreateActions();
			
			Assert.AreEqual(1, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_OnePackageInProject_ActionCreatedFromProject()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			CallCreateActions();
			
			UpdatePackageAction action = updateActions[0];
			UpdatePackageAction expectedAction = fakeProject.FirstFakeUpdatePackageActionCreated;
			
			Assert.AreEqual(expectedAction, action);
		}
		
		[Test]
		public void CreateActions_OnePackageInProject_PackageIdSpecifiedInAction()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			CallCreateActions();
			
			string id = FirstUpdateAction.PackageId;
			
			Assert.AreEqual("Test", id);
		}
		
		[Test]
		public void CreateActions_OnePackageInProject_PackageVersionSpecifiedInActionIsNull()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			CallCreateActions();
			
			SemanticVersion version = FirstUpdateAction.PackageVersion;
			
			Assert.IsNull(version);
		}
		
		[Test]
		public void CreateActions_NoPackagesInProject_ReturnsNoActions()
		{
			CreateUpdateAllPackagesInProject();
			CallCreateActions();
						
			Assert.AreEqual(0, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_TwoPackagesInProject_TwoUpdateActionsCreated()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test1");
			AddPackageToProject("Test2");
			CallCreateActions();
						
			Assert.AreEqual(2, updateActions.Count);
		}
		
		[Test]
		public void CreateActions_TwoPackagesInProject_TwoPackageIdsSpecifiedInActions()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test1");
			AddPackageToProject("Test2");
			CallCreateActions();
						
			Assert.AreEqual("Test1", updateActions[0].PackageId);
			Assert.AreEqual("Test2", updateActions[1].PackageId);
		}
		
		[Test]
		public void CreateActions_OnePackageInProject_UpdateIfPackageDoesNotExistInProjectIsFalse()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			CallCreateActions();
			
			bool update = FirstUpdateAction.UpdateIfPackageDoesNotExistInProject;
			
			Assert.IsFalse(update);
		}
		
		[Test]
		public void CreateActions_OnePackageInProjectAndUpdateDependenciesSetToFalse_ActionUpdateDependenciesIsFalse()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			updateAllPackagesInProject.UpdateDependencies = false;
			
			CallCreateActions();
			
			bool update = FirstUpdateAction.UpdateDependencies;
			
			Assert.IsFalse(update);
		}
		
		[Test]
		public void UpdateDependencies_NewInstance_ReturnsTrue()
		{
			CreateUpdateAllPackagesInProject();
			bool update = updateAllPackagesInProject.UpdateDependencies;
			
			Assert.IsTrue(update);
		}
		
		[Test]
		public void CreateActions_OnePackageInProjectAndAllowPrereleaseVersionsSetToFalse_ActionPrereleaseVersionsIsFalse()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			updateAllPackagesInProject.AllowPrereleaseVersions = false;
			
			CallCreateActions();
			
			bool allow = FirstUpdateAction.AllowPrereleaseVersions;
			
			Assert.IsFalse(allow);
		}
		
		[Test]
		public void CreateActions_OnePackageInProjectAndAllowPrereleaseVersionsSetToTrue_ActionPrereleaseVersionsIsTrue()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			updateAllPackagesInProject.AllowPrereleaseVersions = true;
			
			CallCreateActions();
			
			bool allow = FirstUpdateAction.AllowPrereleaseVersions;
			
			Assert.IsTrue(allow);
		}
		
		[Test]
		public void AllowPrereleaseVersions_NewInstance_ReturnsFalse()
		{
			CreateUpdateAllPackagesInProject();
			bool allow = updateAllPackagesInProject.AllowPrereleaseVersions;
			
			Assert.IsFalse(allow);
		}
		
		[Test]
		public void CreateActions_OnePackageInProjectAndUpdateDependenciesSetToTrue_ActionUpdateDependenciesIsTrue()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			updateAllPackagesInProject.UpdateDependencies = true;
			
			CallCreateActions();
			
			bool update = FirstUpdateAction.UpdateDependencies;
			
			Assert.IsTrue(update);
		}
		
		[Test]
		public void CreateActions_OnePackageInProjectAndPackageScriptRunnerSet_ActionPackageScriptRunnerIsSet()
		{
			CreateUpdateAllPackagesInProject();
			AddPackageToProject("Test");
			var expectedRunner = new FakePackageScriptRunner();
			updateAllPackagesInProject.PackageScriptRunner = expectedRunner;
			
			CallCreateActions();
			
			IPackageScriptRunner runner = FirstUpdateAction.PackageScriptRunner;
			
			Assert.AreEqual(expectedRunner, runner);
		}
	}
}
