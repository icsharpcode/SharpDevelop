// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop
{
	public static class TestExtensions
	{
		public static void AddStrictMockService<T>(this IServiceContainer container) where T : class
		{
			container.AddService(typeof(T), MockRepository.GenerateStrictMock<T>());
		}
	}
}
