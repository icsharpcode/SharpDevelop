// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

using Ast = ICSharpCode.NRefactory.Ast;

namespace Debugger
{
	/// <summary>
	/// Provides information about a property in a class
	/// </summary>
	public class PropertyInfo: MemberInfo
	{
		MethodInfo getMethod;
		MethodInfo setMethod;
		
		/// <summary> Gets a value indicating whether this property is private </summary>
		public override bool IsPrivate {
			get {
				return !(getMethod ?? setMethod).IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this property is public </summary>
		public override bool IsPublic {
			get {
				return (getMethod ?? setMethod).IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this property is static </summary>
		public override bool IsStatic {
			get {
				return (getMethod ?? setMethod).IsStatic;
			}
		}
		
		/// <summary> Gets the metadata token associated with getter (or setter)
		/// of this property </summary>
		[Debugger.Tests.Ignore]
		public override uint MetadataToken {
			get {
				return (getMethod ?? setMethod).MetadataToken;
			}
		}
		
		/// <summary> Gets the name of this property </summary>
		public override string Name {
			get {
				return (getMethod ?? setMethod).Name.Remove(0,4);
			}
		}
		
		internal PropertyInfo(DebugType declaringType, MethodInfo getMethod, MethodInfo setMethod): base(declaringType)
		{
			if (getMethod == null && setMethod == null) throw new ArgumentNullException("Both getter and setter can not be null.");
			
			this.getMethod = getMethod;
			this.setMethod = setMethod;
		}
		
		/// <summary> Get the get accessor of the property </summary>
		public MethodInfo GetGetMethod()
		{
			return getMethod;
		}
		
		/// <summary> Get the set accessor of the property </summary>
		public MethodInfo GetSetMethod()
		{
			return setMethod;
		}
		
		/// <summary> Get the value of the property using the get accessor </summary>
		public Value GetValue(Value objectInstance)
		{
			return GetValue(objectInstance, null);
		}
		
		/// <summary> Get the value of indexer property </summary>
		public Value GetValue(Value objectInstance, Value[] parameters)
		{
			if (getMethod == null) throw new CannotGetValueException("Property does not have a get method");
			parameters = parameters ?? new Value[0];
			
			List<Value> dependencies = new List<Value>();
			dependencies.Add(objectInstance);
			dependencies.AddRange(parameters);
			
			return new Value(
				this.Process,
				this.Name,
				new Ast.MemberReferenceExpression(
					new Ast.IdentifierExpression("parent"), // TODO
					this.Name
				),
				dependencies.ToArray(),
				dependencies.ToArray(),
				delegate { return getMethod.Invoke(objectInstance, parameters).RawCorValue; }
			);
		}
		
		/// <summary> Set the value of the property using the set accessor </summary>
		public Value SetValue(Value objectInstance, Value newValue)
		{
			return SetValue(objectInstance, newValue, null);
		}
		
		/// <summary> Set the value of indexer property </summary>
		public Value SetValue(Value objectInstance, Value newValue, Value[] parameters)
		{
			if (setMethod == null) throw new CannotGetValueException("Property does not have a set method");
			
			parameters = parameters ?? new Value[0];
			Value[] allParams = new Value[1 + parameters.Length];
			allParams[0] = newValue;
			parameters.CopyTo(allParams, 1);
			
			return setMethod.Invoke(objectInstance, allParams);
		}
	}
}
