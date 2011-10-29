/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-06-18
 * Time: 13:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of MSpecTestFramework.
	/// </summary>
	public class MSpecTestFramework : ITestFramework
	{
		public bool IsTestMember(IMember member) {
			return member is IField 
				&& HasItReturnType(member as IField);
		}
		
		public bool IsTestClass(IClass c) {
			return HasSpecificationMembers(c) && !HasBehaviorAttribute(c);
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass @class) {
			return GetTestMembers(@class, @class.Fields);
		}

		private IEnumerable<TestMember> GetTestMembers(IClass testClass, IList<IField> fields)
		{
			var result = fields.Where(HasItReturnType).Select(field => new TestMember(field)).ToList();
			foreach (var field in fields)
				if (HasBehavesLikeReturnType(field))
				{
					var behaviorFields = ResolveBehaviorFieldsOf(field);
					var behaviorMembers = behaviorFields.Where(HasItReturnType);
					var testMembersFromBehavior = behaviorMembers.Select(testField => 
						new TestMember(testField.DeclaringType, new BehaviorImportedTestMember(testClass, testField)));
					result.AddRange(testMembersFromBehavior);
				}
			return result;
		}
		
		public bool IsTestProject(IProject project) {
			if (project != null) {
				foreach (ProjectItem item in project.Items) 
					if (IsMSpecAssemblyReference(item))
						return true;
			}
			return false;
		}
		
		public ITestRunner CreateTestRunner() {
			return new MSpecTestRunner();
			
		}
		
		public ITestRunner CreateTestDebugger() {
			return new MSpecTestDebugger();
		}
		
		public bool IsBuildNeededBeforeTestRun {
			get {return true;}
		}

		private IList<IField> ResolveBehaviorFieldsOf(IField field)
		{
			var fieldReturnType = field.ReturnType.CastToConstructedReturnType();
			if (fieldReturnType == null) return new List<IField>();
			if (fieldReturnType.TypeArguments.Count != 1)
				LoggingService.Error(string.Format("Expected behavior specification {0} to have one type argument but {1} found.", field.FullyQualifiedName, fieldReturnType.TypeArgumentCount));
			var behaviorClassType = fieldReturnType.TypeArguments.FirstOrDefault();

			return behaviorClassType != null ? behaviorClassType.GetFields() : new List<IField>();
		}
		
		private bool HasSpecificationMembers(IClass c) {
			return !c.IsAbstract
				&& c.Fields.Any(f => HasItReturnType(f) || HasBehavesLikeReturnType(f));
		}
		
		private bool HasBehavesLikeReturnType(IField field) {
			return MSpecBehavesLikeFQName.Equals(field.ReturnType.FullyQualifiedName);
		}
		
		private bool HasItReturnType(IField field) {
			return MSpecItFQName.Equals(field.ReturnType.FullyQualifiedName);
		}
		
		private bool HasBehaviorAttribute(IClass c) {
			return c.Attributes.Any(
				attribute => MSpecBehaviorsAttributeFQName.Equals(attribute.AttributeType.FullyQualifiedName));
		}
		
		private bool IsMSpecAssemblyReference(ProjectItem projectItem) {
			if (projectItem is ReferenceProjectItem) {
				ReferenceProjectItem refProjectItem = projectItem as ReferenceProjectItem;
				string name = refProjectItem.ShortName;
				return MSpecAssemblyName.Equals(name, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		
		const string MSpecAssemblyName = "Machine.Specifications";
		const string MSpecItFQName = MSpecAssemblyName + ".It";
		const string MSpecBehavesLikeFQName = MSpecAssemblyName + ".Behaves_like";
		const string MSpecBehaviorsAttributeFQName = MSpecAssemblyName + ".BehaviorsAttribute";
	}
}