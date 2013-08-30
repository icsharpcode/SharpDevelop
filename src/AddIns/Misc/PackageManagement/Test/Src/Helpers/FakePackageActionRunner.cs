// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageActionRunner : IPackageActionRunner
	{
		public IPackageAction ActionPassedToRun;
		public bool IsRunCalled;
		
		public virtual void Run(IPackageAction action)
		{
			IsRunCalled = true;
			ActionPassedToRun = action;
			ActionsPassedToRun.Add(action);
		}
		
		public List<IPackageAction> ActionsPassedToRun = 
			new List<IPackageAction>();
		
		public void Run(IEnumerable<IPackageAction> actions)
		{
			IsRunCalled = true;
			ActionsRunInOneCall = actions;
		}
		
		public IEnumerable<IPackageAction> ActionsRunInOneCall;
		
		public List<IPackageAction> GetActionsRunInOneCallAsList()
		{
			return new List<IPackageAction>(ActionsRunInOneCall);
		}
	}
}
