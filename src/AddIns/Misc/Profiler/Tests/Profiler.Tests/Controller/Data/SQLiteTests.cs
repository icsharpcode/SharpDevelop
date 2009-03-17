// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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