// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Base class for return types that wrap around other return types.
	/// </summary>
	[Serializable]
	public abstract class ProxyReturnType : IReturnType
	{
		public abstract IReturnType BaseType {
			get;
		}
		
		// Required to prevent stack overflow on inferrence cycles
		bool busy = false;
		
		// keep this method as small as possible, it should be inlined!
		bool TryEnter()
		{
			if (busy) {
				PrintTryEnterWarning();
				return false;
			} else {
				busy = true;
				return true;
			}
		}
		
		void PrintTryEnterWarning()
		{
			LoggingService.Info("TryEnter failed on " + ToString());
		}
		
		public virtual string FullyQualifiedName {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.FullyQualifiedName : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual string Name {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.Name : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual string Namespace {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.Namespace : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual string DotNetName {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.DotNetName : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual int TypeParameterCount {
			get {
				IReturnType baseType = BaseType;
				int tmp = (baseType != null && TryEnter()) ? baseType.TypeParameterCount : 0;
				busy = false;
				return tmp;
			}
		}
		
		public virtual int ArrayDimensions {
			get {
				IReturnType baseType = BaseType;
				int tmp = (baseType != null && TryEnter()) ? baseType.ArrayDimensions : 0;
				busy = false;
				return tmp;
			}
		}
		
		public virtual IReturnType ArrayElementType {
			get {
				IReturnType baseType = BaseType;
				IReturnType tmp = (baseType != null && TryEnter()) ? baseType.ArrayElementType : null;
				busy = false;
				return tmp;
			}
		}
		
		public virtual IReturnType UnboundType {
			get {
				IReturnType baseType = BaseType;
				IReturnType tmp = (baseType != null && TryEnter()) ? baseType.UnboundType : null;
				busy = false;
				return tmp;
			}
		}
		
		public virtual IList<IReturnType> TypeArguments {
			get {
				IReturnType baseType = BaseType;
				IList<IReturnType> tmp = (baseType != null && TryEnter()) ? baseType.TypeArguments : null;
				busy = false;
				return tmp;
			}
		}
		
		public virtual IClass GetUnderlyingClass()
		{
			IReturnType baseType = BaseType;
			IClass tmp = (baseType != null && TryEnter()) ? baseType.GetUnderlyingClass() : null;
			busy = false;
			return tmp;
		}
		
		public virtual List<IMethod> GetMethods()
		{
			IReturnType baseType = BaseType;
			List<IMethod> tmp = (baseType != null && TryEnter()) ? baseType.GetMethods() : new List<IMethod>();
			busy = false;
			return tmp;
		}
		
		public virtual List<IProperty> GetProperties()
		{
			IReturnType baseType = BaseType;
			List<IProperty> tmp = (baseType != null && TryEnter()) ? baseType.GetProperties() : new List<IProperty>();
			busy = false;
			return tmp;
		}
		
		public virtual List<IField> GetFields()
		{
			IReturnType baseType = BaseType;
			List<IField> tmp = (baseType != null && TryEnter()) ? baseType.GetFields() : new List<IField>();
			busy = false;
			return tmp;
		}
		
		public virtual List<IEvent> GetEvents()
		{
			IReturnType baseType = BaseType;
			List<IEvent> tmp = (baseType != null && TryEnter()) ? baseType.GetEvents() : new List<IEvent>();
			busy = false;
			return tmp;
		}
		
		public virtual bool IsDefaultReturnType {
			get {
				IReturnType baseType = BaseType;
				bool tmp = (baseType != null && TryEnter()) ? baseType.IsDefaultReturnType : false;
				busy = false;
				return tmp;
			}
		}
	}
}
