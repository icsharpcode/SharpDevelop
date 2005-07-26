// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Interface for classes that can evaluate conditions defined in the addin tree.
	/// </summary>
	public interface IConditionEvaluator
	{
		bool IsValid(object caller, Condition condition);
	}
}
