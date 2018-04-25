using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure.Infrastructure.Helpers
{
    public static class ReflectionHelper
    {
        // Sources:
        // https://github.com/mvbalaw/MvbaCore/blob/master/src/MvbaCore/Reflection.cs
        // https://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/

        // To get the name of a property without the need for calling a magic string
        // Can be deprecated in C# 6 in favour of "nameof(...)"

        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            var names = GetPropertyName(expression.Body as MemberExpression);
            if (names != null)
            {
                return names;
            }
            names = GetPropertyName(expression.Body as UnaryExpression);
            if (names != null)
            {
                return names;
            }
            throw new ArgumentException("expression must be in the form: (Thing instance) => instance.Property[.Optional.Other.Properties.In.Chain]");
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("expression must be in the form: () => instance.Property");
            }
            var names = GetNames(memberExpression);
            //var name = names.Count > 1 ? names.Skip(1).Join(".") : names.Join(".");
            var name = names.Count > 1 ? string.Join(".", names.Skip(1)) : string.Join(".", names);
            return name;
        }

        public static string GetPropertyName(MemberExpression memberExpression)
        {
            if (memberExpression == null)
            {
                return null;
            }
            var names = GetNames(memberExpression);
            //var name = names.Join(".");
            var name = string.Join(".", names);
            return name;
        }

        public static string GetPropertyName(UnaryExpression unaryExpression)
        {
            if (unaryExpression == null)
            {
                return null;
            }
            var memberExpression = unaryExpression.Operand as MemberExpression;
            return GetPropertyName(memberExpression);
        }

        private static List<string> GetNames(MemberExpression memberExpression)
        {
            var names = new List<string>
            {
                memberExpression.Member.Name
            };
            while (memberExpression.Expression is MemberExpression)
            {
                memberExpression = (MemberExpression)memberExpression.Expression;
                names.Insert(0, memberExpression.Member.Name);
            }
            return names;
        }
    }
}
