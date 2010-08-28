using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Namespace : IDependency, INode
    {
        private DependencyGraph _graphCache;

        /// <summary>
        /// Types within namespace
        /// </summary>
        public ISet<Type> Types { get; private set; }

        /// <summary>
        /// Name of namespace
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Module where is namespace located
        /// </summary>
        public Module Module { get; set; }

        public Namespace()
        {
            Types = new HashSet<Type>();

            Dependency = this;
        }

        public DependencyGraph BuildDependencyGraph()
        {
            if (_graphCache != null)
                return _graphCache;

            var g = new DependencyGraph();

            foreach (var type in Types)
            {
                g.AddVertex(new DependencyVertex(type));
            }

            foreach (var type in Types)
            {
                var types = type.GetUses();

                foreach (var dependType in types)
                {
                    if (dependType != type && dependType.Namespace == type.Namespace)
                        g.AddEdge(new DependencyEdge(new DependencyVertex(type), new DependencyVertex(dependType)));
                }
            }

            _graphCache = g;
            return g;
        }
        
        public Relationship GetRelationship(INode node)
        {
        	Relationship relationship = new Relationship();
        	
        	if (node == this) {
        		relationship.Relationships.Add(RelationshipType.Same);
        		return relationship;
        	}
        	
        	if (node is Namespace) {
        		Namespace ns = (Namespace)node;
        		
        		foreach (var type in ns.Types)
        		{
        			if (Types.Contains(type)) {
        		    	relationship.NumberOfOccurrences++;
        		    	relationship.Relationships.Add(RelationshipType.UseThis);
    		    	}
        		}
        	}

        	
        	if (node is Type) {
        		Type type = (Type)node;
        		
        		if (this.Types.Contains(type.BaseType)) {
        		    relationship.NumberOfOccurrences++;
        		    relationship.Relationships.Add(RelationshipType.UseThis);
    		    }
        		
        		foreach (var thisType in type.GenericImplementedInterfacesTypes) {
    		    	if (this.Types.Contains(thisType)) {
    		    		relationship.NumberOfOccurrences++;
    		    		relationship.Relationships.Add(RelationshipType.UseThis);
    		    	}
    		    }
        		
        		foreach (var thisType in type.ImplementedInterfaces) {
    		    	if (this.Types.Contains(thisType)) {
    		    		relationship.NumberOfOccurrences++;
    		    		relationship.Relationships.Add(RelationshipType.UseThis);
    		    	}
    		    }
        		
        		if (this.Types.Contains(type)) {
        		    relationship.Relationships.Add(RelationshipType.Contains);
    		    }
        	}
        	
        	if (node is Method) {
        		Method method = (Method)node;
        		
        		if (this.Types.Contains(method.ReturnType)) {
        		    relationship.NumberOfOccurrences++;
        		    relationship.Relationships.Add(RelationshipType.UseThis);
    		    }
    		    
        		foreach (var type in method.GenericReturnTypes) {
    		    	if (this.Types.Contains(type)) {
    		    		relationship.NumberOfOccurrences++;
    		    		relationship.Relationships.Add(RelationshipType.UseThis);
    		    	}
    		    }
        		
        		foreach (var parameter in method.Parameters) {
        			
        			if (this.Types.Contains(parameter.ParameterType)) {
        				relationship.NumberOfOccurrences++;
    		    		relationship.Relationships.Add(RelationshipType.UseThis);
        			}
        			
        			foreach (var type in parameter.GenericTypes) {
        				if (this.Types.Contains(type)) {
		    				relationship.NumberOfOccurrences++;
				    		relationship.Relationships.Add(RelationshipType.UseThis);
		    			}
        			}
        		}
        		
        		foreach (var type in method.TypeUses) {
        			if (this.Types.Contains(type)) {
	    				relationship.NumberOfOccurrences++;
			    		relationship.Relationships.Add(RelationshipType.UseThis);
	    			}
        		}
        		
        		foreach (var type in method.TypeUses) {
        			if (this.Types.Contains(type)) {
	    				relationship.NumberOfOccurrences++;
			    		relationship.Relationships.Add(RelationshipType.UseThis);
	    			}
        		}
        		
        		foreach (var field in method.FieldUses) {
        			foreach (var type in this.Types) {
        				if (type.Fields.Contains(field)) {
		    				relationship.NumberOfOccurrences++;
				    		relationship.Relationships.Add(RelationshipType.UseThis);
        				}
	    			}
        		}
        		
        		foreach (var meth in method.MethodUses) {
        			foreach (var type in this.Types) {
        				if (type.Methods.Contains(meth)) {
		    				relationship.NumberOfOccurrences++;
				    		relationship.Relationships.Add(RelationshipType.UseThis);
        				}
	    			}
        		}
        		
        		foreach (var type in method.TypeUses) {
        			if (this.Types.Contains(type)) { 
	    				relationship.NumberOfOccurrences++;
			    		relationship.Relationships.Add(RelationshipType.UseThis);
    				}
        				
        		}
        		
        		if (this.Types.Contains(method.DeclaringType))
    		    	relationship.Relationships.Add(RelationshipType.Contains);
        	}
        	
        	if (node is Field) {
        		Field field = (Field)node;
        		if (this.Types.Contains(field.FieldType)) {
        		    relationship.NumberOfOccurrences++;
        		    relationship.Relationships.Add(RelationshipType.UseThis);
    		    }
    		    
    		    foreach (var type in field.GenericTypes) {
    		    	if (this.Types.Contains(type)) {
    		    		relationship.NumberOfOccurrences++;
    		    		relationship.Relationships.Add(RelationshipType.UseThis);
    		    	}
    		    }
    		    
    		    if (this.Types.Contains(field.DeclaringType))
    		    	relationship.Relationships.Add(RelationshipType.Contains);
        	}
        	
        	return relationship;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Namespace Summary");
            builder.Append(Environment.NewLine);
            builder.AppendLine(String.Format("Name: {0}", Name));
            builder.AppendLine(String.Format("Methods: {0}", Types.Sum(type => type.Methods.Count)));
            builder.AppendLine(String.Format("Fields: {0}", Types.Sum(type => type.Fields.Count)));
            builder.AppendLine(String.Format("Types: {0}", Types.Count));
            // more to come
            
            return builder.ToString();
        }
        
        public BitmapSource Icon { get { return NodeIconService.GetIcon(this); } }
    }
}
