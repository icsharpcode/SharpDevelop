// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Base class for return types that wrap around other return types.
	/// </summary>
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
		
		public virtual bool IsArrayReturnType {
			get {
				IReturnType baseType = BaseType;
				bool tmp = (baseType != null && TryEnter()) ? baseType.IsArrayReturnType : false;
				busy = false;
				return tmp;
			}
		}
		public virtual ArrayReturnType CastToArrayReturnType()
		{
			IReturnType baseType = BaseType;
			ArrayReturnType temp;
			if (baseType != null && TryEnter())
				temp = baseType.CastToArrayReturnType();
			else
				throw new InvalidCastException("Cannot cast " + ToString() + " to expected type.");
			busy = false;
			return temp;
		}
		
		public virtual bool IsGenericReturnType {
			get {
				IReturnType baseType = BaseType;
				bool tmp = (baseType != null && TryEnter()) ? baseType.IsGenericReturnType : false;
				busy = false;
				return tmp;
			}
		}
		public virtual GenericReturnType CastToGenericReturnType()
		{
			IReturnType baseType = BaseType;
			GenericReturnType temp;
			if (baseType != null && TryEnter())
				temp = baseType.CastToGenericReturnType();
			else
				throw new InvalidCastException("Cannot cast " + ToString() + " to expected type.");
			busy = false;
			return temp;
		}
		
		public virtual bool IsConstructedReturnType {
			get {
				IReturnType baseType = BaseType;
				bool tmp = (baseType != null && TryEnter()) ? baseType.IsConstructedReturnType : false;
				busy = false;
				return tmp;
			}
		}
		public virtual ConstructedReturnType CastToConstructedReturnType()
		{
			IReturnType baseType = BaseType;
			ConstructedReturnType temp;
			if (baseType != null && TryEnter())
				temp = baseType.CastToConstructedReturnType();
			else
				throw new InvalidCastException("Cannot cast " + ToString() + " to expected type.");
			busy = false;
			return temp;
		}
	}
}
