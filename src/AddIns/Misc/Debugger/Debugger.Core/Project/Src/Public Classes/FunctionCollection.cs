// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public class FunctionCollection: ReadOnlyCollectionBase
	{
		internal void Add(Function function)
		{
			System.Diagnostics.Trace.Assert(function != null);
            if (function != null)		
				InnerList.Add(function);
		}

		public Function this[int index] {
			get {
				return (Function) InnerList[index];
			}
		}

		public Function this[string functionName]
		{
			get {
				foreach (Function f in InnerList) 
					if (f.Name == functionName)
						return f;

				throw new UnableToGetPropertyException(this, "this[string]", "Function \"" + functionName + "\" is not in collection");
			}
		}
	}
}
