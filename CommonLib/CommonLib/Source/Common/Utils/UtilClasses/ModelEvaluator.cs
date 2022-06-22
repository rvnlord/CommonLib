using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public static class ModelEvaluator
    {
        private static (TModel, string, TProperty, string) GetModelAndPropertyInternal<TModel, TProperty>(this Expression accessorBody, TModel model = default)
        {
            if (accessorBody is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert && unaryExpression.Type == typeof(object))
                accessorBody = unaryExpression.Operand;

            if (!(accessorBody is MemberExpression memberExpression))
                throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. Only simple member accessors (fields, properties) of an object are supported.");

            var propertyName = memberExpression.Member.Name;

            if (model == null)
            {
                switch (memberExpression.Expression)
                {
                case ConstantExpression constantExpression:
                    {
                        model = (TModel)constantExpression.Value;
                        break;
                    }
                default:
                    {
                        var modelLambda = Expression.Lambda(memberExpression.Expression);
                        var modelLambdaCompiled = (Func<object>)modelLambda.Compile();
                        model = (TModel)modelLambdaCompiled();
                        break;
                    }
                }
            }

            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var propertyValue = (TProperty)propertyInfo.GetValue(model);
            var displayName = ((DisplayNameAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName;

            if (displayName != null)
                return (model, propertyName, propertyValue, displayName);

            var metadataType = (MetadataTypeAttribute)typeof(TModel).GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();

            if (metadataType == null)
                return (model, propertyName, propertyValue, propertyName);

            var property = metadataType.MetadataClassType.GetProperty(propertyName);
            if (property != null)
                displayName = ((DisplayNameAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName ?? propertyName;

            return (model, propertyName, propertyValue, displayName);
        }

        private static (TModel, string, TProperty, string) GetModelAndPropertyInternal<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model)
        {
            if (expr == null)
                throw new ArgumentNullException(nameof(expr));

            var type = model.GetType(); // typeof(TSource)
            if (!(expr.Body is MemberExpression memberExpression))
                throw new ArgumentException($"Expression '{expr}' refers to a method, not a property.");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException($"Expression '{expr}' refers to a field, not a property.");
            if (type != propertyInfo.ReflectedType && propertyInfo.ReflectedType != null && !type.IsSubclassOf(propertyInfo.ReflectedType))
                throw new ArgumentException($"Expression '{expr}' refers to a property that is not from type {type}.");

            var propertyName = memberExpression.Member.Name;
            var propertyValue = (TProperty)propertyInfo.GetValue(model);
            var displayName = ((DisplayNameAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName;

            if (displayName != null)
                return (model, propertyName, propertyValue, displayName);

            var metadataType = (MetadataTypeAttribute)typeof(TModel).GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();

            if (metadataType == null)
                return (model, propertyName, propertyValue, propertyName);

            var property = metadataType.MetadataClassType.GetProperty(propertyName);
            if (property != null)
                displayName = ((DisplayNameAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName ?? propertyName;

            return (model, propertyName, propertyValue, displayName);
        }

        private static (object, string, object, string) GetModelAndPropertyInternal(object model, string propertyPath)
        {
            if (model == null)
                throw new NullReferenceException(nameof(model));

            var (propertyName, propertyValue, propertyInfo) = model.GetPropertyNameFromPropertyPathInternal(propertyPath);

            var displayName = ((DisplayNameAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName;

            if (displayName != null)
                return (model, propertyName, propertyValue, displayName);

            var metadataType = (MetadataTypeAttribute)model.GetType().GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();

            if (metadataType == null)
                return (model, propertyName, propertyValue, propertyName);

            var property = metadataType.MetadataClassType.GetProperty(propertyName);
            if (property != null)
                displayName = ((DisplayNameAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault())?.DisplayName ?? propertyName;

            return (model, propertyName, propertyValue, displayName);
        }

        private static (string propertyName, object propertyValue, PropertyInfo pi) GetPropertyNameFromPropertyPathInternal(this object model, string propertyPath) // FluentValidation Error PropertyName can be something like "ObjectA.ObjectB.PropertyX", however, Blazor does NOT recognize nested FieldIdentifier. Instead, the FieldIdentifier is assigned to the object in question. (Model + Property Name). Therefore, we need to traverse the object graph to acquire them!
        {
            var pi = model.GetType().GetProperty(propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (pi == null)
                throw new NullReferenceException(nameof(pi));

            if (!propertyPath.ContainsInvariant("."))
                return (propertyPath, pi.GetValue(model), pi);

            var walker = model;
            var modelObjectPath = "";
            var objectParts = propertyPath.Split('.');
            var fieldName = objectParts[^1];
            
            for (var i = 0; i < objectParts.Length - 1; i++)
            {
                var propertyName = objectParts[i];
                var isArray = false;
                var arrayIndex = 0;
                if (propertyName.ContainsInvariant("[") && propertyName.ContainsInvariant("]"))
                {
                    var indexedPropertyName = propertyName.Split('[', ']'); // propertyName = "A[22]" --> ["A", "22"]
                    propertyName = indexedPropertyName[0];
                    isArray = true;
                    arrayIndex = indexedPropertyName[1].ToInt();
                }

                if (string.IsNullOrEmpty(modelObjectPath)) // Constructing model object path here allows capturing the same array objects without the index!
                    modelObjectPath = propertyName;
                else
                    modelObjectPath += "." + propertyName;

                pi = walker.GetType().GetProperty(propertyName);
                walker = pi?.GetValue(walker);

                if (isArray && walker is IList array) // System.Array implements IList https://docs.microsoft.com/en-us/dotnet/api/system.array?view=netcore-3.0
                    walker = array[arrayIndex];
                if (walker == null)
                    break;
            }

            return (fieldName, walker, pi);
        }

        #region generic expressions - property only

        public static (object, string, TProperty, string) GetModelAndProperty<TProperty>(this Expression<Func<TProperty>> expr)
        {
            if (expr == null)
                throw new NullReferenceException(nameof(expr));

            var accessorBody = expr.Body;
            return accessorBody.GetModelAndPropertyInternal<object, TProperty>();
        }

        public static object GetModel<TProperty>(this Expression<Func<TProperty>> expr)
        {
            var (model, _, _, _) = expr.GetModelAndProperty();
            return model;
        }

        public static string GetPropertyName<TProperty>(this Expression<Func<TProperty>> expr)
        {
            var (_, propertyName, _, _) = expr.GetModelAndProperty();
            return propertyName;
        }

        public static TProperty GetPropertyValue<TProperty>(this Expression<Func<TProperty>> expr)
        {
            var (_, _, propertyValue, _) = expr.GetModelAndProperty();
            return propertyValue;
        }

        public static string GetPropertyDisplayName<TProperty>(this Expression<Func<TProperty>> expr)
        {
            var (_, _, _, displayName) = expr.GetModelAndProperty();
            return displayName;
        }

        public static TProperty SetPropertyValue<TProperty>(this Expression<Func<TProperty>> expr, TProperty value)
        {
            var (model, propName, _, _) = expr.GetModelAndProperty();
            model.GetType().GetProperty(propName)?.SetValue(model, value);
            return value;
        }

        #endregion

        #region generic expressions - model and property

        public static (TModel, string, TProperty, string) GetModelAndProperty<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model)
        {
            return expr.GetModelAndPropertyInternal(model);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static object GetModel<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model)
        {
            return model;
        }

        public static string GetPropertyName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model)
        {
            var (_, propertyName, _, _) = expr.GetModelAndProperty(model);
            return propertyName;
        }

        public static TProperty GetPropertyValue<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model)
        {
            var (_, _, propertyValue, _) = expr.GetModelAndProperty(model);
            return propertyValue;
        }

        public static string GetPropertyDisplayName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model)
        {
            var (_, _, _, displayName) = expr.GetModelAndProperty(model);
            return displayName;
        }

        public static TProperty SetPropertyValue<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expr, TModel model, TProperty value)
        {
            var (_, propName, _, _) = expr.GetModelAndProperty(model);
            model.GetType().GetProperty(propName)?.SetValue(model, value);
            return value;
        }

        #endregion

        #region generic model, expression with model and property

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static (TModel, string, TProperty, string) GetModelAndProperty<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expr)
        {
            return expr.GetModelAndProperty(model);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "extending object on purpose")]
        public static object GetModel<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expr)
        {
            return model;
        }

        public static string GetPropertyName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expr)
        {
            var (_, propertyName, _, _) = expr.GetModelAndProperty(model);
            return propertyName;
        }

        public static TProperty GetPropertyValue<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expr)
        {
            var (_, _, propertyValue, _) = expr.GetModelAndProperty(model);
            return propertyValue;
        }

        public static string GetPropertyDisplayName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expr)
        {
            var (_, _, _, displayName) = expr.GetModelAndProperty(model);
            return displayName;
        }

        public static TProperty SetPropertyValue<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expr, TProperty value)
        {
            var (_, propName, _, _) = expr.GetModelAndProperty(model);
            model.GetType().GetProperty(propName)?.SetValue(model, value);
            return value;
        }

        #endregion

        #region generic model, expression with property only

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "extending object on purpose")]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static (object, string, TProperty, string) GetModelAndProperty<TProperty>(this object model, Expression<Func<TProperty>> expr)
        {
            return expr.GetModelAndProperty();
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "extending object on purpose")]
        public static object GetModel<TProperty>(this object model, Expression<Func<TProperty>> expr)
        {
            return model;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "extending object on purpose")]
        public static string GetPropertyName<TProperty>(this object model, Expression<Func<TProperty>> expr)
        {
            var (_, propertyName, _, _) = expr.GetModelAndProperty();
            return propertyName;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "extending object on purpose")]
        public static TProperty GetPropertyValue<TProperty>(this object model, Expression<Func<TProperty>> expr)
        {
            var (_, _, propertyValue, _) = expr.GetModelAndProperty();
            return propertyValue;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "extending object on purpose")]
        public static string GetPropertyDisplayName<TProperty>(this object model, Expression<Func<TProperty>> expr)
        {
            var (_, _, _, displayName) = expr.GetModelAndProperty();
            return displayName;
        }

        public static TProperty SetPropertyValue<TProperty>(this object model, Expression<Func<TProperty>> expr, TProperty value)
        {
            var (_, propName, _, _) = expr.GetModelAndProperty();
            model?.GetType().GetProperty(propName)?.SetValue(model, value);
            return value;
        }

        #endregion

        #region non-generic model

        public static (object, string, object, string) GetModelAndProperty(this object model, string propertyname)
        {
            return GetModelAndPropertyInternal(model, propertyname);
        }

        public static object GetModel(this object model)
        {
            return model;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "For cohereence")]
        public static string GetPropertyName(this object model, string propertyName)
        {
            return propertyName;
        }

        public static object GetPropertyValue(this object model, string propertyName)
        {
            var (_, _, propertyValue, _) = GetModelAndPropertyInternal(model, propertyName);
            return propertyValue;
        }

        public static string GetPropertyDisplayName(this object model, string propertyName)
        {
            var (_, _, _, displayName) = GetModelAndPropertyInternal(model, propertyName);
            return displayName;
        }

        public static object SetPropertyValue(this object model, string propertyName, object newValue)
        {
            var (_, propName, _, _) = GetModelAndProperty(model, propertyName);
            model?.GetType().GetProperty(propName)?.SetValue(model, newValue);
            return newValue;
        }

        #endregion
    }
}
