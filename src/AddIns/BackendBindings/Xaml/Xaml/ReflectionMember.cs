using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace ICSharpCode.Xaml
{
	public class ReflectionMember : XamlMember
	{
		internal ReflectionMember(ReflectionMemberInfo info)
		{
			this.info = info;
		}

		ReflectionMemberInfo info;

		public ReflectionMemberInfo Info
		{
			get { return info; }
		}

		public override string Name
		{
			get { return info.Name; }
		}

		public override XamlType OwnerType
		{
			get { return ReflectionMapper.GetXamlType(info.OwnerType); }
		}

		public override XamlType ValueType
		{
			get
			{
				return ReflectionMapper.GetXamlType(info.ValueType);
			}
		}

		public override XamlType TargetType
		{
			get { throw new NotImplementedException(); }
		}

		public override AllowedLocation AllowedLocation
		{
			get { return AllowedLocation.Any; }
		}

		public override bool IsEvent
		{
			get { return Info is ReflectionEventInfo; }
		}

		public override bool IsDirective
		{
			get { return false; }
		}

		public override bool IsReadOnly
		{
			get { return Info.IsReadOnly; }
		}

		public override bool IsStatic
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsAttachable
		{
			get { return Info is ReflectionAttachedPropertyInfo || Info is DependencyPropertyInfo; }
		}

		public override T GetAttribute<T>()
		{
			return null;
		}

		public override bool HasTextSyntax
		{
			get { return Runtime.GetValueSerializer(this) != null; }
		}
	}
}
