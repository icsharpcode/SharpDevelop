/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-09-23
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using developwithpassion.specifications.extensions;	
using developwithpassion.specifications.rhinomocks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using Machine.Specifications;
using Machine.Fakes.Adapters.Rhinomocks;

namespace ICSharpCode.MachineSpecifications.Tests
{
	[Subject(typeof(ClassFilterBuilder))]
	public class When_building_class_filter_from_test_selection : Observes<ClassFilterBuilder>
	{
		const string NAMESPACE_FILTER = "Namespace";
		static IClass classAddedExplicitly, classInNamespace, classOutsideNamespace;
		static SelectedTests selectedTests;	
		static IProjectContent projectContent;
		static IList<string> result;
		
		Establish ctx = () => {
			classAddedExplicitly = fake.an<IClass>();			
			classAddedExplicitly.setup(x => x.Namespace).Return("");
			classAddedExplicitly.setup(x => x.FullyQualifiedName).Return("ClassAddedExplicitly");
			classInNamespace = fake.an<IClass>();
			classInNamespace.setup(x => x.Namespace).Return("Namespace.OtherNamespace");
			classInNamespace.setup(x => x.FullyQualifiedName).Return("Namespace.OtherNamespace.ClassInNamespace");
			classOutsideNamespace = fake.an<IClass>();
			classOutsideNamespace.setup(x => x.Namespace).Return("Namespace2");
			classOutsideNamespace.setup(x => x.FullyQualifiedName).Return("Namespace2.ClassOutsideNamespac");
			
			var project = fake.an<IProject>();			
			projectContent = fake.an<IProjectContent>();
			projectContent.setup(x => x.Classes).Return(new[]{classInNamespace, classOutsideNamespace});
			
			selectedTests = new SelectedTests(project, NAMESPACE_FILTER, classAddedExplicitly, null);
		};
		
		Because of = () =>
			result = sut.BuildFilterFor(selectedTests, projectContent);			
		
		It should_add_fully_qualified_name_of_selected_test_class = () =>
			result.ShouldContain(classAddedExplicitly.FullyQualifiedName);
		
		It should_add_class_included_in_selected_namespace = () =>
			result.ShouldContain(classInNamespace.FullyQualifiedName);
		
		It should_not_include_class_not_included_in_namespace = () =>
			result.ShouldNotContain(classOutsideNamespace.FullyQualifiedName);
	}
}
