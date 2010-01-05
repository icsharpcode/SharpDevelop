using System;

namespace Org.BouncyCastle.Cms
{
    public class CmsException
        : Exception
    {
		public CmsException()
		{
		}

		public CmsException(
			string name)
			: base(name)
        {
        }

		public CmsException(
			string		name,
			Exception	e)
			: base(name, e)
        {
        }
    }
}
