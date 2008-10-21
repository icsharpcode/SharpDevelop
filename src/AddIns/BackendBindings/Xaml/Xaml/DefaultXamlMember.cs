using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Xaml
{
	public class DefaultXamlMember : XamlMember
	{
		public override string Name
		{
			get { throw new NotImplementedException(); }
		}

		public override XamlType OwnerType
		{
			get { throw new NotImplementedException(); }
		}

		public override XamlType ValueType
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsStatic
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsAttachable
		{
			get { throw new NotImplementedException(); }
		}

		public override XamlType TargetType
		{
			get { throw new NotImplementedException(); }
		}

		public override AllowedLocation AllowedLocation
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsEvent
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsDirective
		{
			get { throw new NotImplementedException(); }
		}

		public override T GetAttribute<T>()
		{
			throw new NotImplementedException();
		}

		public override bool HasTextSyntax
		{
			get { throw new NotImplementedException(); }
		}
	}
}
