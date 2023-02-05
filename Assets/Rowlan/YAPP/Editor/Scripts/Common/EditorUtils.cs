using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Get unity class properties via reflection
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// Retrieve string path of serialized properties
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expression)
        {
            MemberExpression memberExpression;
            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    memberExpression = expression.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();

            while (memberExpression != null)
            {
                members.Add(memberExpression.Member.Name);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                stringBuilder.Append(members[i]);
                if (i > 0) stringBuilder.Append('.');
            }

            return stringBuilder.ToString();
        }

    }
}

