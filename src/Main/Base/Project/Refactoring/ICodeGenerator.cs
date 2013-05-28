// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ICodeGenerator.
	/// </summary>
	public interface ICodeGenerator
	{
		void AddAttribute(IEntity target, IAttribute attribute);
		void AddAssemblyAttribute(IProject targetProject, IAttribute attribute);
		void AddReturnTypeAttribute(IMethod target, IAttribute attribute);
		void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo);
		
		string GetPropertyName(string fieldName);
		string GetParameterName(string fieldName);
		string GetFieldName(string propertyName);
		
		/// <summary>
		/// Determines whether the given identifier is valid in the language or can be used if escaped properly.
		/// </summary>
		bool IsValidIdentifier(string identifier);
		
		/// <summary>
		/// Escapes the identifier if needed.
		/// </summary>
		string EscapeIdentifier(string identifier);
		
		/// <summary>
		/// Returns whether the given string is a keyword or not.
		/// </summary>
		bool IsKeyword(string identifier, bool treatContextualKeywordsAsIdentifiers = true);
		
		void RenameSymbol(RenameReferenceContext context, string newName);
		IEnumerable<Conflict> FindRenamingConflicts(Reference symbol, string newName);
	}
	
	public abstract class Conflict
	{
		public IVariable ConflictingVariable { get; private set; }
		public IEntity ConflictingEntity { get; private set; }
		
		// TODO : Please add something like ISymbol => (IVariable, IEntity)
		public bool IsLocalConflict {
			get {
				return ConflictingVariable != null;
			}
		}
		
		public abstract bool IsSolvableConflict {
			get;
		}
		
		public abstract void Solve();
		
		protected Conflict()
		{
		}
		
		protected Conflict(IVariable variable)
		{
			this.ConflictingVariable = variable;
		}
		
		protected Conflict(IEntity entity)
		{
			this.ConflictingEntity = entity;
		}
	}
	
	public class UnknownConflict : Conflict
	{
		public override bool IsSolvableConflict {
			get {
				return false;
			}
		}
		
		public override void Solve()
		{
			throw new NotSupportedException();
		}
	}
	
	public class RenameReferenceContext
	{
		public IReadOnlyList<Conflict> Conflicts { get; private set; }
		public Reference RenameTarget { get; private set; }
		
		// TODO : Please add something like ISymbol => (IVariable, IEntity)
		public IVariable OldVariable { get; set; }
		public IEntity OldEntity { get; set; }
		
		public bool IsLocal {
			get { return OldVariable != null; }
		}
		
		public bool HasConflicts {
			get { return Conflicts.Any(); }
		}
		
		public RenameReferenceContext(Reference target, IList<Conflict> conflicts)
		{
			this.RenameTarget = target;
			this.Conflicts = conflicts.AsReadOnly();
		}
	}
	
	public class DefaultCodeGenerator : ICodeGenerator
	{
		public static readonly DefaultCodeGenerator DefaultInstance = new DefaultCodeGenerator();
		
		public virtual void AddAttribute(IEntity target, IAttribute attribute)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddAssemblyAttribute(IProject targetProject, IAttribute attribute)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void AddReturnTypeAttribute(IMethod target, IAttribute attribute)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual string GetPropertyName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual string GetParameterName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToLower(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToLower(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToLower(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual string GetFieldName(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return propertyName;
			string newName = Char.ToLower(propertyName[0]) + propertyName.Substring(1);
			if (newName == propertyName)
				return "_" + newName;
			else
				return newName;
		}
		
		public virtual bool IsValidIdentifier(string identifier)
		{
			return false;
		}

		public virtual string EscapeIdentifier(string identifier)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual bool IsKeyword(string identifier, bool treatContextualKeywordsAsIdentifiers = true)
		{
			return false;
		}
		
		public virtual void RenameSymbol(RenameReferenceContext context, string newName)
		{
			throw new NotSupportedException("Feature not supported!");
		}
		
		public virtual IEnumerable<Conflict> FindRenamingConflicts(Reference context, string newName)
		{
			throw new NotSupportedException("Feature not supported!");
		}
	}
}
