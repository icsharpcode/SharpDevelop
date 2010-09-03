// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Can deserialize project resources used in a component definition.
	/// </summary>
	internal sealed class ProjectResourcesComponentCodeDomSerializer : CodeDomSerializer
	{
		readonly CodeDomSerializer baseSerializer;
		
		internal ProjectResourcesComponentCodeDomSerializer(CodeDomSerializer baseSerializer)
		{
			if (baseSerializer == null)
				throw new ArgumentNullException("baseSerializer");
			this.baseSerializer = baseSerializer;
		}
		
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			if (manager == null)
				throw new ArgumentNullException("manager");
			if (codeObject == null)
				throw new ArgumentNullException("codeObject");
			
			object instance = null;
			
			CodeStatementCollection statements = codeObject as CodeStatementCollection;
			if (statements != null) {
				foreach (CodeStatement statement in statements) {
					
					if (!this.DeserializeProjectResourceStatement(manager, statement)) {
						object result = this.baseSerializer.Deserialize(manager, new CodeStatementCollection(new [] {statement}));
						if (instance == null) {
							instance = result;
						}
					}
					
				}
				return instance;
			}
			
			return this.baseSerializer.Deserialize(manager, codeObject);
		}
		
		/// <summary>
		/// Attempts to deserialize the specified statement as
		/// a statement which accesses a project resource.
		/// </summary>
		/// <returns><c>true</c> if the statement was deserialized successfully, otherwise <c>false</c>.</returns>
		bool DeserializeProjectResourceStatement(IDesignerSerializationManager manager, CodeStatement statement)
		{
			var assignStatement = statement as CodeAssignStatement;
			if (assignStatement != null) {
				return this.DeserializeProjectResourceAssignStatement(manager, assignStatement);
			}
			
			var exprStatement = statement as CodeExpressionStatement;
			if (exprStatement != null) {
				var invokeExpr = exprStatement.Expression as CodeMethodInvokeExpression;
				if (invokeExpr != null) {
					return this.DeserializeProjectResourceMethodInvokeExpression(manager, invokeExpr);
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Deserializes an assignment.
		/// This is used for standard properties, e.g.
		/// <code>myPictureBox.Image = value</code>
		/// </summary>
		bool DeserializeProjectResourceAssignStatement(IDesignerSerializationManager manager, CodeAssignStatement assignStatement)
		{
			var propRefTarget = assignStatement.Left as CodePropertyReferenceExpression;
			if (propRefTarget == null) {
				return false;
			}
			
			var propRefSource = assignStatement.Right as CodePropertyReferenceExpression;
			if (propRefSource == null) {
				return false;
			}
			
			LoggingService.Debug("Forms designer: deserializing a property assignment:");
			LoggingService.Debug("-> " + CodeStatementToString(assignStatement));
			
			IComponent component = this.baseSerializer.Deserialize(manager, propRefTarget.TargetObject) as IComponent;
			if (component == null) {
				LoggingService.Info("Forms designer: ProjectResourcesComponentCodeDomSerializer could not deserialze the target object to IComponent");
				return false;
			}
			if (component.Site == null) {
				LoggingService.Info("Forms designer: ProjectResourcesComponentCodeDomSerializer: The deserialized component '" + component.ToString() + "' does not have a Site.");
				return false;
			}
			
			PropertyDescriptor propDescTarget = TypeDescriptor.GetProperties(component).Find(propRefTarget.PropertyName, false);
			if (propDescTarget == null) {
				throw new InvalidOperationException("Could not find the property descriptor for property '" + propRefTarget.PropertyName + "' on object '" + component.ToString() + "'.");
			}
			
			ProjectResourceInfo resourceInfo = GetProjectResourceFromPropertyReference(manager, propRefSource);
			if (resourceInfo == null) {
				return false;
			}
			
			// Set the property value of the target component to the value from the resource
			propDescTarget.SetValue(component, resourceInfo.OriginalValue);
			
			// Store our resource info in the dictionary service.
			// This is needed for the serializer so that it can
			// serialize this value as a project resource reference again.
			StoreResourceInfo(component, propDescTarget.Name, resourceInfo);
			
			return true;
		}
		
		/// <summary>
		/// Deserializes a method invocation expression.
		/// This is used for extender providers, e.g.
		/// <code>myProvider.SetSomeProperty(targetComponent, value)</code>
		/// </summary>
		bool DeserializeProjectResourceMethodInvokeExpression(IDesignerSerializationManager manager, CodeMethodInvokeExpression invokeExpression)
		{
			if (!invokeExpression.Method.MethodName.StartsWith("Set", StringComparison.OrdinalIgnoreCase) ||
			    invokeExpression.Parameters.Count != 2) {
				return false;
			}
			
			var propRefSource = invokeExpression.Parameters[1] as CodePropertyReferenceExpression;
			if (propRefSource == null) {
				return false;
			}
			
			LoggingService.Debug("Forms designer: deserializing a method invocation:");
			LoggingService.Debug("-> " + CodeStatementToString(new CodeExpressionStatement(invokeExpression)));
			
			object extenderProvider = this.baseSerializer.Deserialize(manager, invokeExpression.Method.TargetObject);
			if (extenderProvider == null) {
				return false;
			}
			
			IComponent targetComponent = this.baseSerializer.Deserialize(manager, invokeExpression.Parameters[0]) as IComponent;
			if (targetComponent == null) {
				LoggingService.Info("Forms designer: ProjectResourcesComponentCodeDomSerializer could not deserialze the target object to IComponent");
				return false;
			}
			if (targetComponent.Site == null) {
				LoggingService.Info("Forms designer: ProjectResourcesComponentCodeDomSerializer: The deserialized component '" + targetComponent.ToString() + "' does not have a Site.");
				return false;
			}
			
			ProjectResourceInfo resourceInfo = GetProjectResourceFromPropertyReference(manager, propRefSource);
			if (resourceInfo == null) {
				return false;
			}
			
			// Set the property value of the target component to the value from the resource
			extenderProvider.GetType()
				.InvokeMember(invokeExpression.Method.MethodName,
				              BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
				              null,
				              extenderProvider,
				              new [] {targetComponent, resourceInfo.OriginalValue},
				              System.Globalization.CultureInfo.InvariantCulture);
			
			// Store our resource info in the dictionary service.
			// This is needed for the serializer so that it can
			// serialize this value as a project resource reference again.
			StoreResourceInfo(targetComponent, invokeExpression.Method.MethodName.Substring(3), resourceInfo);
			
			return true;
		}
		
		static ProjectResourceInfo GetProjectResourceFromPropertyReference(IDesignerSerializationManager manager, CodePropertyReferenceExpression propRefSource)
		{
			ProjectResourceService prs = manager.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
			if (prs == null) {
				throw new InvalidOperationException("The required ProjectResourceService is not available.");
			}
			
			return prs.GetProjectResource(propRefSource);
		}
		
		static void StoreResourceInfo(IComponent component, string propertyName, ProjectResourceInfo resourceInfo)
		{
			var dictService = component.Site.GetService(typeof(IDictionaryService)) as IDictionaryService;
			if (dictService == null) {
				throw new InvalidOperationException("The required IDictionaryService is not available on component '" + component.ToString() + "'.");
			}
			dictService.SetValue(ProjectResourceService.ProjectResourceKey + propertyName, resourceInfo);
		}
		
		#region Default overrides redirecting to baseSerializer
		
		protected override object DeserializeInstance(IDesignerSerializationManager manager, Type type, object[] parameters, string name, bool addToContainer)
		{
			// Because this method is protected, we cannot call into the baseSerializer.
			// However, as we do not use any serialization of this serializer,
			// this is should never be called.
			throw new NotImplementedException();
		}
		
		public override string GetTargetComponentName(CodeStatement statement, CodeExpression expression, Type targetType)
		{
			return this.baseSerializer.GetTargetComponentName(statement, expression, targetType);
		}
		
		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			return this.baseSerializer.Serialize(manager, value);
		}
		
		public override object SerializeAbsolute(IDesignerSerializationManager manager, object value)
		{
			return this.baseSerializer.SerializeAbsolute(manager, value);
		}
		
		public override CodeStatementCollection SerializeMember(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
		{
			return this.baseSerializer.SerializeMember(manager, owningObject, member);
		}
		
		public override CodeStatementCollection SerializeMemberAbsolute(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
		{
			return this.baseSerializer.SerializeMemberAbsolute(manager, owningObject, member);
		}
		
		#endregion
		
		static string CodeStatementToString(CodeStatement statement)
		{
			CodeDomVerboseOutputGenerator outputGenerator = new CodeDomVerboseOutputGenerator();
			using(StringWriter sw = new StringWriter(System.Globalization.CultureInfo.InvariantCulture)) {
				outputGenerator.PublicGenerateCodeFromStatement(statement, sw, null);
				return sw.ToString().TrimEnd();
			}
		}
	}
}
