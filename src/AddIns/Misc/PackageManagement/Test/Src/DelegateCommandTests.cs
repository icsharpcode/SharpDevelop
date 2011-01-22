// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class DelegateCommandTests
	{	
		[Test]
		public void CanExecute_NoCanExecuteDelegateDefined_ReturnsTrue()
		{
			Action<object> execute = delegate { };
			var command = new DelegateCommand(execute);
			
			bool result = command.CanExecute(null);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void CanExecute_CanExecuteDelegateDefinedToReturnFalse_ReturnsFalse()
		{
			Action<object> execute = delegate { };
			Predicate<object> canExecute = delegate { return false; };
			var command = new DelegateCommand(execute, canExecute);
			
			bool result = command.CanExecute(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void CanExecute_CanExecuteDelegateDefined_ParameterPassedToCanExecuteDelegate()
		{
			Action<object> execute = delegate { };
			
			object parameterPassed = null;
			Predicate<object> canExecute = param => {
				parameterPassed = param;
				return true;
			};
			var command = new DelegateCommand(execute, canExecute);
			
			object expectedParameter = new object();
			bool result = command.CanExecute(expectedParameter);
			
			Assert.AreEqual(expectedParameter, parameterPassed);
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_ObjectPassedAsParameter_ParameterPassedToExecuteDelegate()
		{
			object parameterPassed = null;
			Action<object> execute = param => parameterPassed = param;
			var command = new DelegateCommand(execute);
			
			object expectedParameter = new object();
			command.Execute(expectedParameter);
			
			Assert.AreEqual(expectedParameter, parameterPassed);
		}
	}
}
