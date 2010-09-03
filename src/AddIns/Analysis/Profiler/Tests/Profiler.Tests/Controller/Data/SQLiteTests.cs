// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace Profiler.Tests.Controller.Data
{
	[TestFixture]
	public class SQLiteTests
	{
		[DllImport("System.Data.SQLite.dll", EntryPoint="sqlite3_threadsafe")]
		static extern int IsThreadSafe();
		
		[Test]
		public void MultiThreadingSQLiteTest()
		{
			int value = IsThreadSafe();
			Assert.AreEqual(1, value, "SQLite3 is not thread-safe!");
		}
	}
}
