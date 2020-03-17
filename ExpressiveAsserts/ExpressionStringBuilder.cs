using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressiveAsserts
{
    public class ExpressionStringBuilder
    {
        private StringBuilder Builder { get; } = new StringBuilder();
        public string Build(Expression expression, bool includeStartingLambda = false)
        {
            if (!includeStartingLambda && expression is LambdaExpression l)
            {
                Add(l.Body);
            }
            else
            {
                Add(expression);
            }
            
            return Builder.ToString();
        }

        public string Build(MemberBinding binding)
        {
            Builder.Append(binding.Member.Name);
            Builder.Append(" = ");
            if (binding is MemberAssignment a)
            {
                Add(a.Expression);
            }

            return Builder.ToString();
        }

        private bool Add(Expression expression)
        {
            if (expression is LambdaExpression l)
            {
                if (l.Parameters.Count == 0)
                {
                    Builder.Append("()");
                }
                else
                {
                    var para = l.Parameters.Select(p => p.Name).ToList();
                    if (para.Count > 1)
                    {
                        Builder.Append('(');
                        Builder.Append(string.Join(", ", para));
                        Builder.Append(')');
                    }
                    else
                    {
                        Builder.Append(string.Join(", ", para));
                    }
                }

                Builder.Append(" => ");
                Add(l.Body);
                return true;
            }
            if (expression is BinaryExpression b)
            {
                Add(b.Left);
                Builder.Append(" ");
                Builder.Append(NodeTypeToString(b.NodeType));
                Builder.Append(" ");
                Add(b.Right);
                return true;
            }
            else if (expression is MemberExpression m)
            {
                if (!(m.Expression is ConstantExpression) && Add(m.Expression))
                {
                    Builder.Append(".");
                }
                
                Builder.Append(m.Member.Name);
                return true;
            } else if (expression is ConstantExpression c)
            {
                if (c.Type.IsNested)
                {
                    return false;
                }

                
                if (c.Type == typeof(string))
                {
                    Builder.Append('"');
                    Builder.Append(c.Value);
                    Builder.Append('"');
                } 
                else if (c.Value is bool)
                {
                    Builder.Append((bool)c.Value ? "true" : "false");
                }
                else
                {
                    Builder.Append(c.Value);
                }
                return true;
            } else if (expression is ParameterExpression p)
            {
                Builder.Append(p.Name);
                return true;
            }

            if (expression is MethodCallExpression mce)
            {
                Add(mce.Object);
                Builder.Append('.');
                Builder.Append(mce.Method.Name);
                
                Builder.Append('(');

                var arguments = ArgumentsAsString(mce.Method, mce.Arguments);
                var argumentList = string.Join(", ", arguments);
                Builder.Append(argumentList);
                
                Builder.Append(')');
            }

            if (expression is NewExpression n)
            {
                Builder.Append("new ");
                Builder.Append(n.Type.Name);
                Builder.Append('(');
                Builder.Append(string.Join(", ", ArgumentsAsString(n.Constructor, n.Arguments)));
                Builder.Append(')');
            }

            if (expression is MemberInitExpression mie)
            {
                Add(mie.NewExpression);
                if (mie.Bindings.Any())
                {
                    Builder.Append(" { ");

                    var bindings = mie.Bindings.Select(b => new ExpressionStringBuilder().Build(b));
                    Builder.Append(string.Join(", ", bindings));
                    
                    Builder.Append(" }");
                }
            }

            if (expression is UnaryExpression u)
            {
                if (u.Method != null)
                {
                    Builder.Append(u.Method.Name);
                }

                Builder.Append(NodeTypeToString(u.NodeType, false));
                
                Add(u.Operand);
            }

            return false;
        }

        private IEnumerable<string> ArgumentsAsString(MethodBase method, IReadOnlyCollection<Expression> arguments)
        {
            if (arguments.Count == 0)
            {
                return new string[0];
            }
            
            if (HasParamParameter(method))
            {
                var normalParameters = arguments.Take(arguments.Count - 1);
                var normalParametersStrings = normalParameters.Select(p => new ExpressionStringBuilder().Build(p, true));
                var array = (NewArrayExpression)arguments.Last();
                return normalParametersStrings.Concat(array.Expressions.Select(e => new ExpressionStringBuilder().Build(e, true)));
            }
            
            return arguments.Select(a => new ExpressionStringBuilder().Build(a, true));
        }

        private static bool HasParamParameter(MethodBase method)
        {
            var parameters = method.GetParameters();
            var lastParameter = parameters.LastOrDefault();
            if (lastParameter == null)
            {
                return false;
            }

            var isParams = lastParameter.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
            return isParams;
        }

        private string NodeTypeToString(ExpressionType type, bool convertUnknown = true)
        {
            switch (type)
            {
                case ExpressionType.Equal: return "==";
                case ExpressionType.NotEqual: return "!=";
                
                case ExpressionType.Assign: return "=";
                case ExpressionType.Add: return "+";
                case ExpressionType.AddAssign: return "+=";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.SubtractAssign: return "-=";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.MultiplyAssign: return "*=";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.ModuloAssign: return "%=";
                case ExpressionType.Divide: return "/";
                case ExpressionType.DivideAssign: return "/=";
                case ExpressionType.AndAlso: return "&&";
                case ExpressionType.And: return "&";
                case ExpressionType.Not: return "!";
                case ExpressionType.Or: return "|";
                case ExpressionType.OrElse: return "||";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.Negate: return "-";
            }
            
            return convertUnknown ? type.ToString() : string.Empty;
        }
    }
}