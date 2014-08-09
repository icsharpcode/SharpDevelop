// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Collections.Generic;
	
namespace CSharpBinding.OutlinePad
{
	internal static class AstNodeHelper {
		
		static bool showExtendedInfos = true;
		static bool colorizeNode = false;
		static HighlightingColor nameSpaceHighLighting, regionHighLighting, classHighLighting, methodHighLighting, interfaceHighLighting;
		
		static AstNodeHelper() {
			var highlightingDefinition 	= HighlightingManager.Instance.GetDefinition("C#");
			
			nameSpaceHighLighting 		= highlightingDefinition.NamedHighlightingColors.First( x => x.Name == "NamespaceKeywords");
			regionHighLighting 			= highlightingDefinition.NamedHighlightingColors.First( x => x.Name == "Preprocessor");
			classHighLighting 			= highlightingDefinition.NamedHighlightingColors.First( x => x.Name == "ReferenceTypeKeywords");
			interfaceHighLighting 		= highlightingDefinition.NamedHighlightingColors.First( x => x.Name == "ReferenceTypeKeywords");
			methodHighLighting 			= highlightingDefinition.NamedHighlightingColors.First( x => x.Name == "MethodCall");
		}
				
		private static Modifiers GetAccessModifier(AstNode node) {
            var accessModifier = node.Children.FirstOrDefault(x => x is CSharpModifierToken &&
                                                            ((x as CSharpModifierToken).Modifier == Modifiers.Public ||
                                                            (x as CSharpModifierToken).Modifier == Modifiers.Protected ||
                                                            (x as CSharpModifierToken).Modifier == Modifiers.Private ||
															(x as CSharpModifierToken).Modifier == Modifiers.Internal )) as CSharpModifierToken;
			
			// special case: All members in an interface are public, although they have no access modifier
			if (accessModifier == null && IsInterface(node.Parent)) {
				return Modifiers.Public;
			}
			return accessModifier == null 
						? Modifiers.None 
						: accessModifier.Modifier;
        }
		
		private static string GetParameterDeclsAsString(AstNodeCollection<ParameterDeclaration> parameterDecls) {
            var parameterString = new StringBuilder();
            int current = 0;
            foreach (var paramDecl in parameterDecls) {
				if (paramDecl.ParameterModifier != ParameterModifier.None) {
					parameterString.Append(paramDecl.ParameterModifier.ToString().ToLower());
					parameterString.Append(" ");
				}
                parameterString.Append(paramDecl.Type.ToString());
                parameterString.Append(" ");
                parameterString.Append(paramDecl.Name);

                if (current < (parameterDecls.Count - 1)) {
                    parameterString.Append(", ");
                }
                current++;
            }

            return parameterString.ToString();
        }
		
		private static string GetTypeParameterDeclsAsString(AstNodeCollection<TypeParameterDeclaration> parameterDecls) {
            var parameterString = new StringBuilder();
            int current = 0;
            foreach (var paramDecl in parameterDecls) {
                parameterString.Append(paramDecl.NameToken.ToString());

                if (current < (parameterDecls.Count - 1)) {
                    parameterString.Append(", ");
                }
                current++;
            }

            return parameterString.ToString();
        }
		
		public static bool IsAllowedNode(AstNode node)  {
            return (IsRegionStart(node)
				|| IsRegionEnd(node) 
                || IsClass(node)
				|| IsInterface(node) 
                || IsMethod(node) 
                || IsField(node) 
                || IsProperty(node)
				|| IsNameSpace(node)
				|| IsConstructor(node)
				|| IsEvent(node)
				|| IsDelegate(node)
				|| IsIndexer(node)
				|| IsEnum(node)
				|| IsEnumMember(node)
				|| IsStruct(node)
				|| IsOperator(node)
			);
        }
		
        public static bool IsRegionStart(AstNode node) {
            return (node is PreProcessorDirective && (node as PreProcessorDirective).Type == PreProcessorDirectiveType.Region);
        }

		internal static void SetRegionStartInfos(CSharpOutlineNode node, AstNode dataNode)  {
            var preProcessorNode = (PreProcessorDirective)dataNode;
			node.Name 			 = "*****" + preProcessorNode.Argument + "****";
			node.ForegroundBrush = (regionHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
//			node.Weight = regionHighLighting.FontWeight != null ? regionHighLighting.FontWeight.Value : FontWeights.Normal;          
		}
		
        public static bool IsRegionEnd(AstNode node) {
            return (node is PreProcessorDirective && (node as PreProcessorDirective).Type == PreProcessorDirectiveType.Endregion);
        }

		internal static void SetRegionEndInfos(CSharpOutlineNode node, AstNode dataNode)  {
            var preProcessorNode = (PreProcessorDirective)dataNode;
            node.Name 			 = "********************";
			node.ForegroundBrush = (regionHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
//			node.Weight = regionHighLighting.FontWeight != null ? regionHighLighting.FontWeight.Value : FontWeights.Normal;          
        }
		
		public static bool IsDelegate(AstNode node) {
            return (node is DelegateDeclaration);
        }
		
		internal static void SetDelegateNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var typeNode 	= (DelegateDeclaration)dataNode;
			node.Name 		= typeNode.Name;
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
//			node.Weight = classHighLighting.FontWeight != null ? classHighLighting.FontWeight.Value : FontWeights.Normal;
			
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Delegate";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedDelegate";
					break;	
				case Modifiers.Private:
					node.IconName = "Icons.16x16.PrivateDelegate";
					break;					
				default:
					node.IconName = "Icons.16x16.InternalDelegate";
					break;
			}       
        }

		public static bool IsStruct(AstNode node) {
            return (node is TypeDeclaration && (node as TypeDeclaration).ClassType == ClassType.Struct);
        }
		
		internal static void SetStructNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var typeNode 	= (TypeDeclaration)dataNode;
			node.Name 		= typeNode.Name;
			node.IsExpanded = true;
			
			if (colorizeNode)
				node.ForegroundBrush = (classHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
//			node.Weight = classHighLighting.FontWeight != null ? classHighLighting.FontWeight.Value : FontWeights.Normal;
			
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Struct";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedStruct";
					break;	
				case Modifiers.Private:
					node.IconName = "Icons.16x16.PrivateStruct";
					break;					
				default:
					node.IconName = "Icons.16x16.InternalStruct";
					break;
			}       
        }

		public static bool IsEnumMember(AstNode node) {
            return (node is EnumMemberDeclaration);
        }
		
		internal static void SetEnumMemberNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var enumMemberNode 	= (EnumMemberDeclaration)dataNode;
			node.Name 			= enumMemberNode.Name;
			
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
			node.IconName = "Icons.16x16.Enum";			      
        }

		public static bool IsEnum(AstNode node) {
            return (node is TypeDeclaration && (node as TypeDeclaration).ClassType == ClassType.Enum);
        }
		
		internal static void SetEnumNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var typeNode 	= (TypeDeclaration)dataNode;
			node.Name 		= typeNode.Name;
			node.IsExpanded = true;
			
			if (colorizeNode)
				node.ForegroundBrush = (classHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
						
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Enum";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedEnum";
					break;	
				case Modifiers.Private:
					node.IconName = "Icons.16x16.PrivateEnum";
					break;					
				default:
					node.IconName = "Icons.16x16.InternalEnum";
					break;
			}       
        }
		
        public static bool IsClass(AstNode node) {
            return (node is TypeDeclaration && (node as TypeDeclaration).ClassType == ClassType.Class);
        }

		internal static void SetClassNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var typeNode 	= (TypeDeclaration)dataNode;
			node.Name 		= typeNode.Name 
								+ typeNode.LChevronToken 
								+ GetTypeParameterDeclsAsString(typeNode.TypeParameters) 
								+ typeNode.RChevronToken;
			node.IsExpanded = true;
			
			if (colorizeNode)
				node.ForegroundBrush = (classHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
//			node.Weight = classHighLighting.FontWeight != null ? classHighLighting.FontWeight.Value : FontWeights.Normal;
						
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Class";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedClass";
					break;	
				case Modifiers.Private:
					node.IconName = "Icons.16x16.PrivateClass";
					break;					
				default:
					node.IconName = "Icons.16x16.InternalClass";
					break;
			}       
        }
		
		public static bool IsEvent(AstNode node) {
            return (node is EventDeclaration);
        }

		internal static void SetEventNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var eventNode 		= (EventDeclaration)dataNode;
			var varInitializer 	= eventNode.Children.FirstOrDefault(x => x is VariableInitializer) as VariableInitializer;
			node.Name 			= (varInitializer != null) 
										? varInitializer.Name 
										: "<unknown>";
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
			
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Event";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedEvent";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateEvent";
					break;
			}       
        }
		
		public static bool IsInterface(AstNode node) {
            return (node is TypeDeclaration && (node as TypeDeclaration).ClassType == ClassType.Interface);
        }

		internal static void SetInterfaceNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var typeNode 	= (TypeDeclaration)dataNode;
			node.Name 		= typeNode.Name;
			node.IsExpanded = true;
			
			if (colorizeNode)
				node.ForegroundBrush = (interfaceHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
			
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Interface";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedInterface";
					break;	
				case Modifiers.Private:
					node.IconName = "Icons.16x16.PrivateInterface";
					break;					
				default:
					node.IconName = "Icons.16x16.InternalInterface";
					break;
			}          
        }

		public static bool IsOperator(AstNode node) {
            return (node is OperatorDeclaration);
        }

		internal static void SetOperatorNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var operatorNode 	= (OperatorDeclaration)dataNode;			
            node.Name 			= operatorNode.OperatorToken 
								+ " " 
            					+ operatorNode.OperatorTypeToken;

			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
            
			if (showExtendedInfos) {
				node.Name += " " 
						   + operatorNode.LParToken
						   + GetParameterDeclsAsString(operatorNode.Parameters)
						   + operatorNode.RParToken;
			}			
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Method";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedMethod";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateMethod";
					break;
			}
        }
		
        public static bool IsMethod(AstNode node) {
            return (node is MethodDeclaration);
        }

		internal static void SetMethodNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var methodNode 	= (MethodDeclaration)dataNode;			
            node.Name 		= methodNode.Name;
					
			node.Name += " " 
					   + methodNode.LParToken
					   + (showExtendedInfos ? GetParameterDeclsAsString(methodNode.Parameters) : "")
					   + methodNode.RParToken;

			if (colorizeNode)
				node.ForegroundBrush = (methodHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
//			node.Weight = methodHighLighting.FontWeight != null ? methodHighLighting.FontWeight.Value : FontWeights.Normal;
			
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Method";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedMethod";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateMethod";
					break;
			}
        }
		
        public static bool IsField(AstNode node) {
            return (node is FieldDeclaration);
        }

		internal static void SetFieldNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            
			var fieldNode = (FieldDeclaration)dataNode;
			var fieldName =  new StringBuilder();
			int current = 0;
			foreach (var varInitializer in fieldNode.Variables) {
                fieldName.Append(varInitializer.Name);
				
                if (current < (fieldNode.Variables.Count - 1)) {
                    fieldName.Append(", ");
                }
                current++;
            }

            node.Name = fieldName.ToString();
            
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;

			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Field";
					break;	
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedField";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateField";
					break;
			}					          
        }
		
        public static bool IsProperty(AstNode node) {
            return (node is PropertyDeclaration);
        }
		
		internal static void SetPropertyNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var propertyNode = (PropertyDeclaration)dataNode;
			node.Name 		 = propertyNode.Name;
			
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
			
			if (showExtendedInfos) {
				node.Name += " " + propertyNode.LBraceToken 
						   + " " + propertyNode.Getter.Keyword + (!string.IsNullOrEmpty(propertyNode.Getter.Keyword.ToString()) ? ";" : "")
						   + " " + propertyNode.Setter.Keyword + (!string.IsNullOrEmpty(propertyNode.Setter.Keyword.ToString()) ? ";" : "")
						   + " " + propertyNode.RBraceToken;
			}						
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Property";
					break;	
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedProperty";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateProperty";
					break;
			}    
	    }				
		
		public static bool IsIndexer(AstNode node) {
            return (node is IndexerDeclaration);
        }
		
		internal static void SetIndexerNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var indexerNode = (IndexerDeclaration)dataNode;
			node.Name 		= indexerNode.ReturnType.ToString() 
						    + " " 
						    + indexerNode.ThisToken.ToString()
						    + indexerNode.LBracketToken
						    + GetParameterDeclsAsString(indexerNode.Parameters)
						    + indexerNode.RBracketToken;		
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
			
			if (showExtendedInfos) {
				node.Name += " " + indexerNode.LBraceToken 
						   + " " + indexerNode.Getter.Keyword + (!string.IsNullOrEmpty(indexerNode.Getter.Keyword.ToString()) ? ";" : "")
						   + " " + indexerNode.Setter.Keyword + (!string.IsNullOrEmpty(indexerNode.Setter.Keyword.ToString()) ? ";" : "")
						   + " " + indexerNode.RBraceToken;
			}
			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Property";
					break;	
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedProperty";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateProperty";
					break;
			}    
	    }
		
		public static bool IsNameSpace(AstNode node) {
            return (node is NamespaceDeclaration);
        }
		
		internal static void SetNameSpaceNodeInfos(CSharpOutlineNode node, AstNode dataNode) {            
            var nameSpaceNode 	= (NamespaceDeclaration)dataNode;
			node.Name 			= nameSpaceNode.Name;
			node.IconName 		= "Icons.16x16.NameSpace";  
			node.IsExpanded 	= true;
			
			if (colorizeNode)
				node.ForegroundBrush = (nameSpaceHighLighting.Foreground as SimpleHighlightingBrush).GetBrush(null);
//			node.Weight = nameSpaceHighLighting.FontWeight != null ? nameSpaceHighLighting.FontWeight.Value : FontWeights.Normal;		      
        }
		
		public static bool IsConstructor(AstNode node) {
            return (node is ConstructorDeclaration);
        }
		
		internal static void SetConstructorNodeInfos(CSharpOutlineNode node, AstNode dataNode) {
            var constructorNode = (ConstructorDeclaration)dataNode;
            node.Name 			= constructorNode.Name;
            
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;

			node.Name += " " 
					   + constructorNode.LParToken 
					   + (showExtendedInfos ? GetParameterDeclsAsString(constructorNode.Parameters) : "")
					   + constructorNode.RParToken;

			switch(GetAccessModifier(dataNode)) {
				case Modifiers.Public:
					node.IconName = "Icons.16x16.Method";
					break;					
				case Modifiers.Protected:
					node.IconName = "Icons.16x16.ProtectedMethod";
					break;					
				default:
					node.IconName = "Icons.16x16.PrivateMethod";
					break;
			}
        }
		
		public static bool IsDocument(AstNode node) {
            return (node is SyntaxTree);
        }
		
		internal static void SetDocumentNodeInfos(CSharpOutlineNode node, AstNode dataNode) {            
			node.Name 		= Path.GetFileName(((SyntaxTree)dataNode).FileName);
			node.IconName 	= "C#.File.FullFile";    
			node.IsExpanded = true;  
			node.Weight 	= FontWeights.Bold;
			
			if (colorizeNode)
				node.ForegroundBrush = Brushes.Black;
        }
    }
}
