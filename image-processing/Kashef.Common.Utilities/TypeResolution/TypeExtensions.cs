using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kashef.Common.Utilities.TypeResolution
{    
    /// <summary>
    /// Extension Methods for Types.
    /// </summary>
    public static class TypeExtensions
    {
        internal static readonly Type[] PredefinedTypes = {
        typeof(Object),
        typeof(Boolean),
        typeof(Char),
        typeof(String),
        typeof(SByte),
        typeof(Byte),
        typeof(Int16),
        typeof(UInt16),
        typeof(Int32),
        typeof(UInt32),
        typeof(Int64),
        typeof(UInt64),
        typeof(Single),
        typeof(Double),
        typeof(Decimal),
        typeof(DateTime),
        typeof(TimeSpan),
        typeof(Guid),
        typeof(System.Math),
        typeof(Convert)
    };

        /// <summary>
        /// Checks if this type is already predefined.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPredefinedType(this Type type)
        {
            foreach (Type t in PredefinedTypes)
            {
                if (t == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the type is Nullable.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Gets non nullable type of the current type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Gets type name.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeName(this Type type)
        {
            Type baseType = GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

#if WPF40 || SILVERLIGHT
	public static bool IsDynamic(this Type type)
	{
		return type.IsCompatibleWith<System.Dynamic.IDynamicMetaObjectProvider>();
	}
#endif
        /// <summary>
        /// Checks if the type is numeric.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        /// <summary>
        /// Checks if the type is signed integral.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSignedIntegralType(this Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        /// <summary>
        /// Checks if the type is unsigned integral.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnsignedIntegralType(this Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        /// <summary>
        /// Checks if the numeric type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetNumericTypeKind(this Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets info about indexer property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="indexerArguments"></param>
        /// <returns></returns>
        public static PropertyInfo GetIndexerPropertyInfo(this Type type, params Type[] indexerArguments)
        {
            return GetIndexerPropertyInfo(type, (IEnumerable<Type>)indexerArguments);
        }

        /// <summary>
        /// Gets info about indexer property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="indexerArguments"></param>
        /// <returns></returns>
        public static PropertyInfo GetIndexerPropertyInfo(this Type type, IEnumerable<Type> indexerArguments)
        {
            // We will lookup first the generic interface implementations, in order to have 
            // richer type information in scenarios like IList and IList<T>
            var implementedInterfacesProperties =
                type.GetInterfaces().OrderBy(i => !i.IsGenericType).SelectMany(i => i.GetProperties());

            return
                (from p in type.GetProperties().Concat(implementedInterfacesProperties)
                    where AreArgumentsApplicable(indexerArguments, p.GetIndexParameters())
                    select p).FirstOrDefault();
        }

        /// <summary>
        /// Checks if the arguments are applicable for the passed parameters.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool AreArgumentsApplicable(IEnumerable<Type> arguments, IEnumerable<ParameterInfo> parameters)
        {
            var argumentList = arguments.ToList();
            var parameterList = parameters.ToList();

            if (argumentList.Count != parameterList.Count)
            {
                return false;
            }

            for (int i = 0; i < argumentList.Count; i++)
            {
                if (parameterList[i].ParameterType != argumentList[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the type is Enum.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumType(this Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        /// <summary>
        /// Checks if the type is compatible with template.
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsCompatibleWith<TTargetType>(this Type source)
        {
            return source.IsCompatibleWith(typeof(TTargetType));
        }

        /// <summary>
        /// Checks if the two types are compatible.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static bool IsCompatibleWith(this Type source, Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = source.GetNonNullableType();
            Type tt = target.GetNonNullableType();
            if (st != source && tt == target) return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Gets the generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static Type FindGenericType(this Type type, Type genericType)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType) return type;
                if (genericType.IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = intfType.FindGenericType(genericType);
                        if (found != null) return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets default value of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        /// <summary>
        /// Finds member information by member name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static MemberInfo FindPropertyOrField(this Type type, string memberName)
        {
            MemberInfo memberInfo = type.FindPropertyOrField(memberName, false);

            if (memberInfo == null)
            {
                memberInfo = type.FindPropertyOrField(memberName, true);
            }

            return memberInfo;
        }

        /// <summary>
        /// Finds member information by member name checking for static declaration. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <param name="staticAccess"></param>
        /// <returns></returns>
        public static MemberInfo FindPropertyOrField(this Type type, string memberName, bool staticAccess)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (Type t in type.SelfAndBaseTypes())
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field,
                    flags, Type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0) return members[0];
            }
            return null;
        }

        /// <summary>
        /// Gets base types, including current type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> SelfAndBaseTypes(this Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        /// <summary>
        /// Gets base classes of the current type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> SelfAndBaseClasses(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        /// <summary>
        /// Adds interface to list of types.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="type"></param>
        public static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type t in type.GetInterfaces()) AddInterface(types, t);
            }
        }
        
        /// <summary>
        /// Checks if the current type allows sorting.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CanSort(this Type source)
        {
            if (source == null) return false;

            bool isSortable = false;
            if (typeof(IComparable).IsAssignableFrom(source))
            {
                isSortable = true;
            }
            else
            {
                Type underlyingTypeOfNullable = Nullable.GetUnderlyingType(source);
                if (underlyingTypeOfNullable != null && typeof(IComparable).IsAssignableFrom(underlyingTypeOfNullable))
                {
                    isSortable = true;
                }
            }
            return isSortable;
        }

        /// <summary>
        /// Checks if the current type allows grouping.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CanGroup(this Type source)
        {
            if (source == null) return false;

            Type typeToCheck = Nullable.GetUnderlyingType(source) ?? source;
            return ImplementsIEquatable(typeToCheck) || ImplementsIComparable(typeToCheck);
        }

        /// <summary>
        /// Checks if the current type allows filtering.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CanFilter(this Type source)
        {
            if (source == null) return false;

            Type typeToCheck = Nullable.GetUnderlyingType(source) ?? source;
            return ImplementsIEquatable(typeToCheck) || ImplementsIComparable(typeToCheck);
        }

        /// <summary>
        /// Checks if the type implements IEquatable.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ImplementsIEquatable(this Type source)
        {
            if (source == null) return false;

            Type equatableType = typeof(IEquatable<>);
            Type genericIEquatableType = equatableType.MakeGenericType(source);

            return genericIEquatableType.IsAssignableFrom(source);
        }

        /// <summary>
        /// Checks if the type implements IComparable.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ImplementsIComparable(this Type source)
        {
            if (source == null) return false;

            foreach (Type @interface in source.GetInterfaces())
            {
                if (@interface == typeof(IComparable))
                {
                    return true;
                }
            }
            return false;
        }        

        /// <summary>
        /// Checks if the current type is primitive or value type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitiveOrValueType(this Type type)
        {
            return type.IsPrimitive || type.IsValueType;
        }
    }    
}
