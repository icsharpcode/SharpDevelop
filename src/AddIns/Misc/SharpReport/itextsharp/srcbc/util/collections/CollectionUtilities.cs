using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.Utilities.Collections
{
	public sealed class CollectionUtilities
	{
		private CollectionUtilities()
		{
		}

		public static bool CheckElementsAreOfType(
			IEnumerable e,
			Type		t)
		{
			foreach (object o in e)
			{
				if (!t.IsInstanceOfType(o))
					return false;
			}
			return true;
		}

		public static string ToString(
			IEnumerable c)
		{
			StringBuilder sb = new StringBuilder("[");

			IEnumerator e = c.GetEnumerator();

			if (e.MoveNext())
			{
				sb.Append(e.Current.ToString());

				while (e.MoveNext())
				{
					sb.Append(", ");
					sb.Append(e.Current.ToString());
				}
			}

			sb.Append(']');

			return sb.ToString();
		}
	}
}
