using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	public class InnerClassEnumerator : IEnumerable<IClass>
	{
		readonly IClass _c;

		public InnerClassEnumerator(IClass c)
		{
			_c = c;
		}

		#region IEnumerable<IClass> Members

		public IEnumerator<IClass> GetEnumerator()
		{
			var result = new List<IClass>();
			Visit(_c, result);
			return result.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private void Visit(IClass c, List<IClass> resultList)
		{
			foreach (var innerClass in c.InnerClasses)
			{
				resultList.Add(innerClass);
				Visit(innerClass, resultList);
			}
		}
	}
}
