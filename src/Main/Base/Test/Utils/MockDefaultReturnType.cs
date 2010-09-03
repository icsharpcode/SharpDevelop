// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockDefaultReturnType : IReturnType
	{
		List<IMethod> methods = new List<IMethod>();
		List<IProperty> properties = new List<IProperty>();
		
		public MockDefaultReturnType()
		{
		}
		
		/// <summary>
		/// Gets the method list directly. Only available in the
		/// mock default return type class.
		/// </summary>
		public List<IMethod> Methods {
			get {
				return methods;
			}
		}
		
		/// <summary>
		/// Gets the property list directly. Only available in the
		/// mock default return type class.
		/// </summary>
		public List<IProperty> Properties {
			get {
				return properties;
			}
		}
		
		public bool Equals(IReturnType other)
		{
			throw new NotImplementedException();
		}
		
		public string FullyQualifiedName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Namespace {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string DotNetName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TypeArgumentCount {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDefaultReturnType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IClass GetUnderlyingClass()
		{
			throw new NotImplementedException();
		}
		
		public List<IMethod> GetMethods()
		{
			return methods;
		}
		
		public List<IProperty> GetProperties()
		{
			return properties;
		}
		
		public List<IField> GetFields()
		{
			return new List<IField>();
		}
		
		public List<IEvent> GetEvents()
		{
			return new List<IEvent>();
		}
		
		public ArrayReturnType CastToArrayReturnType()
		{
			throw new NotImplementedException();
		}
		
		public GenericReturnType CastToGenericReturnType()
		{
			throw new NotImplementedException();
		}
		
		public ConstructedReturnType CastToConstructedReturnType()
		{
			throw new NotImplementedException();
		}
		
		public bool IsArrayReturnType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsGenericReturnType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsConstructedReturnType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDecoratingReturnType<T>() where T : DecoratingReturnType
		{
			throw new NotImplementedException();
		}
		
		public T CastToDecoratingReturnType<T>() where T : DecoratingReturnType
		{
			throw new NotImplementedException();
		}
		
		public Nullable<bool> IsReferenceType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType GetDirectReturnType()
		{
			return this;
		}
	}
}
