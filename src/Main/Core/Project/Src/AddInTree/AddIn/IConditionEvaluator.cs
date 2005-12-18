// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
