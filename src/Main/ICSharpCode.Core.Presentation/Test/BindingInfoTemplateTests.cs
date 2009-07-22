using System;
using NUnit.Framework;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.Core.Presentation.Tests
{
	[TestFixture]
	public class BindingInfoTemplateTests
	{
		[TestAttribute]
	    public void IsTemplateForSupersetsTests()
	    {
	    	var source = new InputBindingInfo();
	    	source.RoutedCommandName = "TestCommand";
	    	source.OwnerTypeName = "TestOwnerType";
	    	
	    	var emptyTemplate = new BindingInfoTemplate();
	    	Assert.IsTrue(emptyTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet));
	    	
	    	var matchingTemplate = new BindingInfoTemplate();
	    	matchingTemplate.RoutedCommandName = "TestCommand";
	    	Assert.IsTrue(matchingTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet));
	    	
	    	var unmatchingTemplate = new BindingInfoTemplate();
	    	unmatchingTemplate.RoutedCommandName = "OtherTestCommand";
	    	Assert.IsFalse(unmatchingTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet));
	    	
	    	var overlappingTemplate = new BindingInfoTemplate();
	    	overlappingTemplate.RoutedCommandName = "TestCommand";
	    	overlappingTemplate.OwnerInstanceName = "TestOwnerInstance";
	    	Assert.IsFalse(overlappingTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet));
	    	
			var biggerTemplate = new BindingInfoTemplate();
			biggerTemplate.RoutedCommandName = "TestCommand";
			biggerTemplate.OwnerTypeName = "TestOwnerType";
			biggerTemplate.OwnerInstanceName = "TestOwnerInstance";
			Assert.IsFalse(biggerTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet));
			
	    	var exactTemplate = new BindingInfoTemplate();
	    	exactTemplate.RoutedCommandName = "TestCommand";
	    	exactTemplate.OwnerTypeName = "TestOwnerType";
	    	Assert.IsTrue(exactTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet));
	    }
	    
		[TestAttribute]
	    public void IsTemplateForSubsetsTests()
	    {
			var source = new InputBindingInfo();
			source.RoutedCommandName = "TestCommand";
			source.OwnerTypeName = "TestOwnerType";
			
	    	var emptyTemplate = new BindingInfoTemplate();
	    	Assert.IsFalse(emptyTemplate.IsTemplateFor(source, BindingInfoMatchType.SubSet));
			
			var matchingTemplate = new BindingInfoTemplate();
			matchingTemplate.RoutedCommandName = "TestCommand";
			Assert.IsTrue(matchingTemplate.IsTemplateFor(source, BindingInfoMatchType.SubSet));
			
			var unmatchingTemplate = new BindingInfoTemplate();
			unmatchingTemplate.RoutedCommandName = "OtherTestCommand";
			Assert.IsFalse(unmatchingTemplate.IsTemplateFor(source, BindingInfoMatchType.SubSet));
			
			var overlappingTemplate = new BindingInfoTemplate();
			overlappingTemplate.RoutedCommandName = "TestCommand";
			overlappingTemplate.OwnerInstanceName = "TestOwnerInstance";
			Assert.IsTrue(overlappingTemplate.IsTemplateFor(source, BindingInfoMatchType.SubSet));
			
			var biggerTemplate = new BindingInfoTemplate();
			biggerTemplate.RoutedCommandName = "TestCommand";
			biggerTemplate.OwnerTypeName = "TestOwnerType";
			biggerTemplate.OwnerInstanceName = "TestOwnerInstance";
			Assert.IsTrue(biggerTemplate.IsTemplateFor(source, BindingInfoMatchType.SubSet));
			
			var exactTemplate = new BindingInfoTemplate();
			exactTemplate.RoutedCommandName = "TestCommand";
			exactTemplate.OwnerTypeName = "TestOwnerType";
			Assert.IsTrue(exactTemplate.IsTemplateFor(source, BindingInfoMatchType.SubSet));
	    }
	    
		[TestAttribute]
	    public void IsTemplateForPartlyMatchingTests()
	    {
			var source = new InputBindingInfo();
			source.RoutedCommandName = "TestCommand";
			source.OwnerTypeName = "TestOwnerType";
			
	    	var emptyTemplate = new BindingInfoTemplate();
	    	Assert.IsTrue(emptyTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet | BindingInfoMatchType.SubSet));
			
			var matchingTemplate = new BindingInfoTemplate();
			matchingTemplate.RoutedCommandName = "TestCommand";
			Assert.IsTrue(matchingTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet | BindingInfoMatchType.SubSet));
			
			var unmatchingTemplate = new BindingInfoTemplate();
			unmatchingTemplate.RoutedCommandName = "OtherTestCommand";
			Assert.IsFalse(unmatchingTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet | BindingInfoMatchType.SubSet));
			
			var overlappingTemplate = new BindingInfoTemplate();
			overlappingTemplate.RoutedCommandName = "TestCommand";
			overlappingTemplate.OwnerInstanceName = "TestOwnerInstance";
			Assert.IsTrue(overlappingTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet | BindingInfoMatchType.SubSet));
			
			var biggerTemplate = new BindingInfoTemplate();
			biggerTemplate.RoutedCommandName = "TestCommand";
			biggerTemplate.OwnerTypeName = "TestOwnerType";
			biggerTemplate.OwnerInstanceName = "TestOwnerInstance";
			Assert.IsTrue(biggerTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet | BindingInfoMatchType.SubSet));
			
			var exactTemplate = new BindingInfoTemplate();
			exactTemplate.RoutedCommandName = "TestCommand";
			exactTemplate.OwnerTypeName = "TestOwnerType";
			Assert.IsTrue(exactTemplate.IsTemplateFor(source, BindingInfoMatchType.SuperSet | BindingInfoMatchType.SubSet));
	    }
	}
}
