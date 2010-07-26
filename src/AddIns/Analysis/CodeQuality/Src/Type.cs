using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Type : IDependency, INode
    {
        private DependencyGraph _graphCache;

        /// <summary>
        /// Nested types like inner classes, interfaces and so on.
        /// </summary>
        public ISet<Type> NestedTypes { get; private set; }

        /// <summary>
        /// Type which owns this type. If this isn't nested type so Owner is null.
        /// </summary>
        public Type Owner { get; set; }

        /// <summary>
        /// Inherited type. If there is no inheritance so it is null.
        /// </summary>
        public Type BaseType { get; set; }

        /// <summary>
        /// Interfaces which are implemented by this type
        /// </summary>
        public ISet<Type> ImplementedInterfaces { get; private set; }

        /// <summary>
        /// Methods within type
        /// </summary>
        public ISet<Method> Methods { get; private set; }

        /// <summary>
        /// Fields within type
        /// </summary>
        public ISet<Field> Fields { get; private set; }
        
        /// <summary>
        /// Members of type such as methods and fields
        /// </summary>
        public IEnumerable<INode> Members
        {
        	get
        	{
        		return new HashSet<INode>(Methods).Union(Fields);
        	}
        }

        /// <summary>
        /// Events within type
        /// </summary>
        public ISet<Event> Events { get; private set; }

        /// <summary>
        /// Name of type with a name of namespace.
        /// </summary>
        public string FullName
        {
            get
            {
                return Namespace.Name + "." + Name;
            }
        }

        /// <summary>
        /// Name of type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Namespace where type is located
        /// </summary>
        public Namespace Namespace { get; set; }

        /// <summary>
        /// Whether the type is interface
        /// </summary>
        public bool IsInterface { get; set; }

        /// <summary>
        /// Whether the type is enum
        /// </summary>
        public bool IsEnum { get; set; }

        /// <summary>
        /// Whether the type is class
        /// </summary>
        public bool IsClass { get; set; }

        /// <summary>
        /// Whether the type is sealed
        /// </summary>
        public bool IsSealed { get; set; }

        /// <summary>
        /// Whether the type is abstract
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Whether the type is public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the type is struct
        /// </summary>
        public bool IsStruct { get; set; }

        /// <summary>
        /// Whether the type is internal
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// Whether the type is delagate
        /// </summary>
        public bool IsDelegate { get; set; }

        /// <summary>
        /// If it is nested type whether is private
        /// </summary>
        public bool IsNestedPrivate { get; set; }

        /// <summary>
        /// If it is nested type whether is public
        /// </summary>
        public bool IsNestedPublic { get; set; }

        /// <summary>
        /// If it is nested type whether is protected
        /// </summary>
        public bool IsNestedProtected { get; set; }

        /// <summary>
        /// Whether the base type is generic instance
        /// </summary>
        public bool IsBaseTypeGenericInstance { get; set; }

        /// <summary>
        /// If the base type is generic instance so all types used in generic are presented in this set.
        /// </summary>
        public ISet<Type> GenericBaseTypes { get; private set; }

        /// <summary>
        /// If one of implemented interfaces is generic instance so all types used in generic are presented in this set.
        /// </summary>
        public ISet<Type> GenericImplementedInterfacesTypes { get; private set; }

        public Type()
        {
            Methods = new HashSet<Method>();
            Fields = new HashSet<Field>();
            Events = new HashSet<Event>();
            NestedTypes = new HashSet<Type>();
            ImplementedInterfaces = new HashSet<Type>();
            GenericBaseTypes = new HashSet<Type>();
            GenericImplementedInterfacesTypes = new HashSet<Type>();

            Owner = null;
            BaseType = null;

            IsBaseTypeGenericInstance = false;

            Dependency = this;
        }

        /// <summary>
        /// Returns all types which are used in this type
        /// </summary>
        /// <returns>A set of types</returns>
        public ISet<Type> GetUses()
        {
            var set = new HashSet<Type>();

            foreach (var method in Methods)
            {
                set.UnionWith(method.TypeUses);

                foreach (var parameter in method.Parameters)
                {
                    if (parameter.IsGenericInstance)
                        set.UnionWith(parameter.GenericTypes);

                    if (parameter.ParameterType != null)
                        set.Add(parameter.ParameterType);
                }

                if (method.IsReturnTypeGenericInstance)
                    set.UnionWith(method.GenericReturnTypes);

                if (method.ReturnType != null)
                    set.Add(method.ReturnType);
            }

            foreach (var field in Fields)
            {
                if (field.IsGenericInstance)
                    set.UnionWith(field.GenericTypes);

                if (field.FieldType != null) // if it is null so it is type from outside of this assembly
                    set.Add(field.FieldType); // TODO: find solution to handle them
            }

            if (BaseType != null)
                set.Add(BaseType);

            if (IsBaseTypeGenericInstance)
                set.UnionWith(GenericBaseTypes);

            set.UnionWith(ImplementedInterfaces);
            set.UnionWith(GenericImplementedInterfacesTypes);

            return set;
        }

        public DependencyGraph BuildDependencyGraph()
        {
            if (_graphCache != null)
                return _graphCache;

            var g = new DependencyGraph();

            foreach (var method in Methods)
            {
                g.AddVertex(new DependencyVertex(method));
            }

            foreach (var field in Fields)
            {
                g.AddVertex(new DependencyVertex(field));
            }

            foreach (var method in Methods)
            {
                foreach (var methodUse in method.MethodUses)
                {
                    g.AddEdge(new DependencyEdge(new DependencyVertex(method), new DependencyVertex(methodUse)));
                }

                foreach (var fieldUse in method.FieldUses)
                {
                    g.AddEdge(new DependencyEdge(new DependencyVertex(method), new DependencyVertex(fieldUse)));
                }
            }

            _graphCache = g;
            return g;
        }
        
        public Relationship GetRelationship(INode node)
        {
//        	if (node is Namespace) {
//        		Namespace ns = (Namespace)node;
//        		if (ns.Types.Contains(this)
//        		    return RelationshipType.Contains;
//        	}

			return new Relationship();
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Type Summary");
            builder.Append(Environment.NewLine);
            builder.AppendLine(String.Format("Name: {0}", Name));
            builder.AppendLine(String.Format("Methods: {0}", Methods.Count));
            builder.AppendLine(String.Format("Fields: {0}", Fields.Count));
            builder.AppendLine(String.Format("Nested types: {0}", NestedTypes.Count));
            builder.AppendLine(String.Format("Implemented interfaces: {0}", ImplementedInterfaces.Count));
            // more to come

            builder.Append(Environment.NewLine);

            if (IsAbstract)
                builder.AppendLine("IsAbstract");
            if (IsClass)
                builder.AppendLine("IsClass");
            if (IsDelegate)
                builder.AppendLine("IsDelegate");
            if (IsEnum)
                builder.AppendLine("IsEnum");
            if (IsInterface)
                builder.AppendLine("IsInterface");
            if (IsInternal)
                builder.AppendLine("IsInternal");
            if (IsNestedPrivate)
                builder.AppendLine("IsNestedPrivate");
            if (IsNestedProtected)
                builder.AppendLine("IsNestedProtected");
            if (IsNestedPublic)
                builder.AppendLine("IsNestedPublic");
            if (IsPublic)
                builder.AppendLine("IsPublic");
            if (IsSealed)
                builder.AppendLine("IsSealed");
            if (IsStruct)
                builder.AppendLine("IsStruct");
            
            return builder.ToString();
        }
        
        public BitmapSource Icon { get { return NodeIconService.GetIcon(this); } }
    }
}
