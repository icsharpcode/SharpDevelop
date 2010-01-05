using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public interface ISet
		: ICollection
	{
		void Add(object o);
		bool Contains(object o);
		void Remove(object o);
	}
}
