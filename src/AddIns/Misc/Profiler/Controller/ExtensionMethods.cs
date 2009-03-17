using ICSharpCode.Profiler.Controller.Data;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Extension Methods for the Profiler.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Returns a string containing all errors.
		/// </summary>
		public static string GetErrorString(this CompilerErrorCollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			StringBuilder builder = new StringBuilder();

			foreach (CompilerError err in collection)
				builder.AppendLine(err.ToString());

			return builder.ToString();
		}

		/// <summary>
		/// Returns a  CallTreeNode with all merged items.
		/// </summary>
		public static CallTreeNode Merge(this IEnumerable<CallTreeNode> items)
		{
			return items.First().Merge(items);
		}
	}
}
