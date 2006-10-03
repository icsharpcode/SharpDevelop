// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

using Hornung.ResourceToolkit.ResourceFileContent;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Detects and resolves resource references using the standard .NET
	/// framework provided resource access methods (ResourceManager and derived
	/// classes).
	/// </summary>
	public class BclNRefactoryResourceResolver : INRefactoryResourceResolver
	{
		
		/// <summary>
		/// Tries to find a resource reference in the specified expression.
		/// </summary>
		/// <param name="expressionResult">The ExpressionResult for the expression.</param>
		/// <param name="expr">The AST representation of the expression.</param>
		/// <param name="resolveResult">SharpDevelop's ResolveResult for the expression.</param>
		/// <param name="caretLine">The line where the expression is located.</param>
		/// <param name="caretColumn">The column where the expression is located.</param>
		/// <param name="fileName">The name of the source file where the expression is located.</param>
		/// <param name="fileContent">The content of the source file where the expression is located.</param>
		/// <returns>A ResourceResolveResult describing the referenced resource, or <c>null</c>, if this expression does not reference a resource using the standard .NET framework classes.</returns>
		public ResourceResolveResult Resolve(ExpressionResult expressionResult, Expression expr, ResolveResult resolveResult, int caretLine, int caretColumn, string fileName, string fileContent)
		{
			IResourceFileContent rfc = null;
			
			MemberResolveResult mrr = resolveResult as MemberResolveResult;
			if (mrr != null) {
				rfc = ResolveResourceFileContent(mrr.ResolvedMember);
			}
			
			LocalResolveResult lrr = resolveResult as LocalResolveResult;
			if (lrr != null) {
				if (!lrr.IsParameter) {
					rfc = ResolveResourceFileContent(lrr.Field);
				}
			}
			
			
			if (rfc != null) {
				string key = GetKeyFromExpression(expr);
				
				// TODO: Add information about return type (of the resource, if present).
				return new ResourceResolveResult(resolveResult.CallingClass, resolveResult.CallingMember, null, rfc, key);
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to determine the resource file content which is referenced by the
		/// resource manager which is assigned to the specified member.
		/// </summary>
		/// <returns>
		/// The IResourceFileContent, if successful, or a null reference, if the
		/// specified member is not a resource manager or if the
		/// resource file cannot be determined.
		/// </returns>
		static IResourceFileContent ResolveResourceFileContent(IMember member)
		{
			if (member != null && member.ReturnType != null) {
				if (IsResourceManager(member.ReturnType) && member.DeclaringType != null && member.DeclaringType.CompilationUnit != null) {
					
					string declaringFileName = member.DeclaringType.CompilationUnit.FileName;
					if (declaringFileName != null) {
						
						SupportedLanguage? language = NRefactoryResourceResolver.GetFileLanguage(declaringFileName);
						if (language == null) {
							return null;
						}
						
						CompilationUnit cu = NRefactoryAstCacheService.GetFullAst(language.Value, declaringFileName);
						if (cu != null) {
							
							ResourceManagerInitializationFindVisitor visitor = new ResourceManagerInitializationFindVisitor(member);
							cu.AcceptVisitor(visitor, null);
							if (visitor.FoundFileName != null) {
								
								return ResourceFileContentRegistry.GetResourceFileContent(visitor.FoundFileName);
								
							}
							
						}
						
					}
					
				}
			}
			return null;
		}
		
		/// <summary>
		/// Determines if the specified type is a ResourceManager type that can
		/// be handled by this resolver.
		/// </summary>
		static bool IsResourceManager(IReturnType type)
		{
			IClass resourceManager = ParserService.CurrentProjectContent.GetClass("System.Resources.ResourceManager");
			if (resourceManager == null) {
				return false;
			}
			
			IClass c = type.GetUnderlyingClass();
			if (c == null) {
				return false;
			}
			
			return (c.CompareTo(resourceManager) == 0 || c.IsTypeInInheritanceTree(resourceManager));
		}
		
		// ********************************************************************************************************************************
		
		#region ResourceManagerInitializationFindVisitor
		
		/// <summary>
		/// Finds an initialization statement for a resource manager member and
		/// tries to infer the referenced resource file from the parameters
		/// given to the resource manager constructor.
		/// </summary>
		class ResourceManagerInitializationFindVisitor : PositionTrackingAstVisitor
		{
			
			readonly IMember resourceManagerMember;
			readonly bool isLocalVariable;
			
			CompilationUnit compilationUnit;
			
			string foundFileName;
			
			/// <summary>
			/// Gets the resource file name which the resource manager accesses, or a null reference if the file could not be determined or does not exist.
			/// </summary>
			public string FoundFileName {
				get {
					return this.foundFileName;
				}
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="ResourceManagerInitializationFindVisitor" /> class.
			/// </summary>
			/// <param name="resourceManagerMember">The member which the resource manager to be found is assigned to.</param>
			public ResourceManagerInitializationFindVisitor(IMember resourceManagerMember) : base()
			{
				this.resourceManagerMember = resourceManagerMember;
				IField resourceManagerField = resourceManagerMember as IField;
				if (resourceManagerField != null && resourceManagerField.IsLocalVariable) {
					this.isLocalVariable = true;
				}
			}
			
			public override object TrackedVisit(CompilationUnit compilationUnit, object data)
			{
				this.compilationUnit = compilationUnit;
				return base.TrackedVisit(compilationUnit, data);
			}
			
			public override object TrackedVisit(LocalVariableDeclaration localVariableDeclaration, object data)
			{
				return base.TrackedVisit(localVariableDeclaration, localVariableDeclaration);
			}
			
			public override object TrackedVisit(FieldDeclaration fieldDeclaration, object data)
			{
				return base.TrackedVisit(fieldDeclaration, fieldDeclaration);
			}
			
			public override object TrackedVisit(VariableDeclaration variableDeclaration, object data)
			{
				LocalVariableDeclaration localVariableDeclaration = data as LocalVariableDeclaration;
				if (this.isLocalVariable && localVariableDeclaration != null) {
					if (variableDeclaration.Name == this.resourceManagerMember.Name) {
						// Make sure we got the right declaration by comparing the positions.
						// Both must have the same start position.
						if (localVariableDeclaration.StartLocation.X == this.resourceManagerMember.Region.BeginColumn && localVariableDeclaration.StartLocation.Y == this.resourceManagerMember.Region.BeginLine) {
							#if DEBUG
							LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found local variable declaration: "+localVariableDeclaration.ToString()+" at "+localVariableDeclaration.StartLocation.ToString());
							#endif
							data = true;
						}
						
					}
				}
				FieldDeclaration fieldDeclaration = data as FieldDeclaration;
				if (!this.isLocalVariable && fieldDeclaration != null) {
					if (variableDeclaration.Name == this.resourceManagerMember.Name) {
						// Make sure we got the right declaration by comparing the positions.
						// Both must have the same start position.
						if (fieldDeclaration.StartLocation.X == this.resourceManagerMember.Region.BeginColumn && fieldDeclaration.StartLocation.Y == this.resourceManagerMember.Region.BeginLine) {
							#if DEBUG
							LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found field declaration: "+fieldDeclaration.ToString()+" at "+fieldDeclaration.StartLocation.ToString());
							#endif
							data = true;
						}
						
					}
				}
				return base.TrackedVisit(variableDeclaration, data);
			}
			
			public override object TrackedVisit(AssignmentExpression assignmentExpression, object data)
			{
				if (this.FoundFileName == null &&	// skip if already found to improve performance
				    assignmentExpression.Op == AssignmentOperatorType.Assign && this.PositionAvailable &&
				    (!this.isLocalVariable || this.resourceManagerMember.Region.IsInside(this.CurrentNodeStartLocation.Y, this.CurrentNodeStartLocation.X))	// skip if local variable is out of scope
				   ) {
					
					MemberResolveResult mrr = this.Resolve(assignmentExpression.Left, this.resourceManagerMember) as MemberResolveResult;
					if (mrr != null) {
						
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver: Resolved member: "+mrr.ResolvedMember.ToString());
						#endif
						
						// HACK: The GetType()s are necessary because the DOM IComparable implementations try to cast the parameter object to their own interface type which may fail.
						if (mrr.ResolvedMember.GetType().Equals(this.resourceManagerMember.GetType()) && mrr.ResolvedMember.CompareTo(this.resourceManagerMember) == 0) {
							
							#if DEBUG
							LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found assignment to field: "+assignmentExpression.ToString());
							#endif
							data = true;
							
							// Resolving the property association only makes sense if
							// there is a possible relationship between the return types
							// of the resolved member and the member we are looking for.
						} else if (this.compilationUnit != null && !this.isLocalVariable &&
						           (
						            mrr.ResolvedMember.ReturnType.Equals(this.resourceManagerMember.ReturnType) ||
						            (
						             mrr.ResolvedMember.ReturnType.GetUnderlyingClass() != null && this.resourceManagerMember.ReturnType.GetUnderlyingClass() != null &&
						             (
						              mrr.ResolvedMember.ReturnType.GetUnderlyingClass().IsTypeInInheritanceTree(this.resourceManagerMember.ReturnType.GetUnderlyingClass()) ||
						              this.resourceManagerMember.ReturnType.GetUnderlyingClass().IsTypeInInheritanceTree(mrr.ResolvedMember.ReturnType.GetUnderlyingClass())
						             )
						            )
						           )) {
							
							if (this.resourceManagerMember is IProperty && mrr.ResolvedMember is IField) {
								// Find out if the resourceManagerMember is a property whose get block returns the value of the resolved member.
								
								PropertyFieldAssociationVisitor visitor = new PropertyFieldAssociationVisitor((IProperty)this.resourceManagerMember);
								this.compilationUnit.AcceptVisitor(visitor, null);
								if (visitor.AssociatedField != null && visitor.AssociatedField.CompareTo(mrr.ResolvedMember) == 0) {
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found assignment to field: "+assignmentExpression.ToString());
									#endif
									data = true;
								}
								
							} else if (this.resourceManagerMember is IField && mrr.ResolvedMember is IProperty) {
								// Find out if the resolved member is a property whose set block assigns the value to the resourceManagerMember.
								
								PropertyFieldAssociationVisitor visitor = new PropertyFieldAssociationVisitor((IField)this.resourceManagerMember);
								this.compilationUnit.AcceptVisitor(visitor, null);
								if (visitor.AssociatedProperty != null && visitor.AssociatedProperty.CompareTo(mrr.ResolvedMember) == 0) {
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found assignment to property: "+assignmentExpression.ToString());
									#endif
									data = true;
								}
								
							}
							
						}
						
					}
					
				}
				return base.TrackedVisit(assignmentExpression, data);
			}
			
			public override object TrackedVisit(ObjectCreateExpression objectCreateExpression, object data)
			{
				if (data as bool? ?? false) {
					
					#if DEBUG
					LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found object initialization: "+objectCreateExpression.ToString());
					#endif
					
					// Resolve the constructor.
					// A type derived from the declaration type is also allowed.
					MemberResolveResult mrr = this.Resolve(objectCreateExpression, this.resourceManagerMember) as MemberResolveResult;
					
					#if DEBUG
					if (mrr != null) {
						LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver: resolved constructor: "+mrr.ResolvedMember.ToString());
					}
					#endif
					
					if (mrr != null &&
					    mrr.ResolvedMember is IMethod &&
					    (mrr.ResolvedMember.DeclaringType.CompareTo(this.resourceManagerMember.ReturnType.GetUnderlyingClass()) == 0 ||
					     mrr.ResolvedMember.DeclaringType.IsTypeInInheritanceTree(this.resourceManagerMember.ReturnType.GetUnderlyingClass()))
					   ) {
						
						// This most probably is the resource manager initialization we are looking for.
						// Find a parameter that indicates the resources being referenced.
						
						foreach (Expression param in objectCreateExpression.Parameters) {
							
							PrimitiveExpression p = param as PrimitiveExpression;
							if (p != null) {
								string pValue = p.Value as string;
								if (pValue != null) {
									
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found string parameter: '"+pValue+"'");
									#endif
									
									string fileName = NRefactoryResourceResolver.GetResourceFileNameByResourceName(this.resourceManagerMember.DeclaringType.CompilationUnit.FileName, pValue);
									if (fileName != null) {
										#if DEBUG
										LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found resource file: "+fileName);
										#endif
										this.foundFileName = fileName;
										break;
									}
									
								}
								
								continue;
							}
							
							// Support typeof(...)
							TypeOfExpression t = param as TypeOfExpression;
							if (t != null && this.PositionAvailable) {
								TypeResolveResult trr = this.Resolve(new TypeReferenceExpression(t.TypeReference), this.resourceManagerMember) as TypeResolveResult;
								if (trr != null) {
									
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found typeof(...) parameter, type: '"+trr.ResolvedType.ToString()+"'");
									#endif
									
									string fileName = NRefactoryResourceResolver.GetResourceFileNameByResourceName(this.resourceManagerMember.DeclaringType.CompilationUnit.FileName, trr.ResolvedType.FullyQualifiedName);
									if (fileName != null) {
										#if DEBUG
										LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found resource file: "+fileName);
										#endif
										this.foundFileName = fileName;
										break;
									}
									
								}
							}
							
						}
						
					}
					
				}
				
				return base.TrackedVisit(objectCreateExpression, data);
			}
			
		}
		
		#endregion
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to infer the resource key being referenced from the given expression.
		/// </summary>
		static string GetKeyFromExpression(Expression expr)
		{
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver trying to get key from expression: "+expr.ToString());
			#endif
			
			IndexerExpression indexer = expr as IndexerExpression;
			if (indexer != null) {
				foreach (Expression index in indexer.Indexes) {
					PrimitiveExpression p = index as PrimitiveExpression;
					if (p != null) {
						string key = p.Value as string;
						if (key != null) {
							#if DEBUG
							LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found key: "+key);
							#endif
							return key;
						}
					}
				}
			}
			
			InvocationExpression invocation = expr as InvocationExpression;
			if (invocation != null) {
				FieldReferenceExpression fre = invocation.TargetObject as FieldReferenceExpression;
				if (fre != null) {
					if (fre.FieldName == "GetString" || fre.FieldName == "GetObject" || fre.FieldName == "GetStream") {
						if (invocation.Arguments.Count > 0) {
							PrimitiveExpression p = invocation.Arguments[0] as PrimitiveExpression;
							if (p != null) {
								string key = p.Value as string;
								if (key != null) {
									#if DEBUG
									LoggingService.Debug("ResourceToolkit: BclNRefactoryResourceResolver found key: "+key);
									#endif
									return key;
								}
							}
						}
					}
				}
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by this
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		public IEnumerable<string> GetPossiblePatternsForFile(string fileName)
		{
			return new string[] {
				"GetString",
				"GetObject",
				"GetStream",
				(NRefactoryResourceResolver.GetLanguagePropertiesForFile(fileName) ?? LanguageProperties.None).IndexerExpressionStartToken
			};
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="BclNRefactoryResourceResolver"/> class.
		/// </summary>
		public BclNRefactoryResourceResolver()
		{
		}
	}
}
