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
		static DefaultClass classAddedExplicitly, classInNamespace, classOutsideNamespace, classNestedInAddedExplicitly, classNestedInClassInNamespace;
		static SelectedTests selectedTests;
		static IProjectContent projectContent;
		static IList<string> result;
		
		Establish ctx = () => {
			projectContent = fake.an<IProjectContent>();
			projectContent.setup(x => x.SystemTypes).Return(new SystemTypes(projectContent));
			var compilationUnit = new DefaultCompilationUnit(projectContent);

			classAddedExplicitly = new DefaultClass(compilationUnit, "ClassAddedExplicitly");
			classNestedInAddedExplicitly = new DefaultClass(compilationUnit, classAddedExplicitly);
			classNestedInAddedExplicitly.FullyQualifiedName = "ClassAddedExplicitly.InnerClass";
			classAddedExplicitly.InnerClasses.Add(classNestedInAddedExplicitly);

			classInNamespace = new DefaultClass(compilationUnit, "Namespace.OtherNamespace.ClassInNamespace");
			classNestedInClassInNamespace = new DefaultClass(compilationUnit, classInNamespace);
			classNestedInClassInNamespace.FullyQualifiedName = "Namespace.OtherNamespace.ClassInNamespace.InnerClass";
			classInNamespace.InnerClasses.Add(classNestedInClassInNamespace);
			classOutsideNamespace = new DefaultClass(compilationUnit, "Namespace2.ClassOutsideNamespac");
			
			var project = fake.an<IProject>();
			projectContent.setup(x => x.Classes).Return(new[]{classInNamespace, classOutsideNamespace});
			
			selectedTests = new SelectedTests(project, NAMESPACE_FILTER, classAddedExplicitly, null);
		};
		
		Because of = () =>
			result = sut.BuildFilterFor(selectedTests, projectContent);
		
		It should_add_dotnet_name_of_selected_test_class = () =>
			result.ShouldContain(classAddedExplicitly.DotNetName);
		
		It should_add_class_included_in_selected_namespace = () =>
			result.ShouldContain(classInNamespace.DotNetName);
		
		It should_not_include_class_not_included_in_namespace = () =>
			result.ShouldNotContain(classOutsideNamespace.DotNetName);

		It should_not_include_class_nested_in_selected_test_class = () =>
			result.ShouldNotContain(classNestedInAddedExplicitly.DotNetName);

		It should_include_class_nested_in_class_from_selected_namespace = () =>
			result.ShouldContain(classNestedInClassInNamespace.DotNetName);
	}
}
