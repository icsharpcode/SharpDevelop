// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using NIgnore = NUnit.Framework.IgnoreAttribute;

namespace Debugger.Tests
{
	[TestFixture]
	[NIgnore("Debugger is broken on .NET 4.0 Beta 2")]
	public partial class DebuggerTests: DebuggerTestsBase
	{
		
	}
}
