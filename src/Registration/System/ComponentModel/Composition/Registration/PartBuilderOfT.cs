using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.Internal;


namespace System.ComponentModel.Composition.Registration
{
    public class PartBuilder<T> : PartBuilder
    {
        private class PropertyExpressionAdapter
        {
            PropertyInfo _propertyInfo;
            Action<ImportBuilder> _configureImport;
            Action<ExportBuilder> _configureExport;

            public  PropertyExpressionAdapter(
                Expression<Func<T, object>> propertyFilter,
                Action<ImportBuilder> configureImport = null,
                Action<ExportBuilder> configureExport = null)
            {
                this._propertyInfo = SelectProperties(propertyFilter);
                this._configureImport = configureImport;
                this._configureExport = configureExport;
            }

            public bool VerifyPropertyInfo(PropertyInfo pi)
            {
                return pi == this._propertyInfo;
            }

            public void ConfigureImport(PropertyInfo propertyInfo, ImportBuilder importBuilder)
            {
                if(this._configureImport != null)
                {
                    this._configureImport(importBuilder);
                }
            }

            public void ConfigureExport(PropertyInfo propertyInfo, ExportBuilder exportBuilder)
            {
                if(this._configureExport != null)
                {
                    this._configureExport(exportBuilder);
                }
            }

            private static PropertyInfo SelectProperties(Expression<Func<T, object>> propertyFilter)
            {
                Requires.NotNull(propertyFilter, "propertyFilter");

                var expr = Reduce(propertyFilter).Body;
                if(expr.NodeType == ExpressionType.MemberAccess)
                {
                    var memberInfo = ((MemberExpression)expr).Member;
                    if(memberInfo.MemberType == MemberTypes.Property)
                    {
                        return (PropertyInfo)memberInfo;
                    }
                }

                // An error occured the expression must be a Property Member Expression
                throw ExceptionBuilder.Argument_ExpressionMustBePropertyMember("propertyFilter");
            }

            protected static Expression<Func<T, object>> Reduce(Expression<Func<T, object>> expr)
            {
                while(expr.CanReduce)
                {
                    expr = (Expression<Func<T, object>>)expr.Reduce();
                }
                return expr;
            }
        }

        private class ConstructorExpressionAdapter
        {
            ConstructorInfo _constructorInfo = null;
            Dictionary<ParameterInfo, Action<ImportBuilder>> _importBuilders = null;

            public ConstructorExpressionAdapter(Expression<Func<ParameterImportBuilder, T>> selectConstructor)
            {
                ParseSelectConstructor(selectConstructor);
            }

            public ConstructorInfo SelectConstructor(ConstructorInfo[] constructorInfos)
            {
                return _constructorInfo;
            }

            public void ConfigureConstructorImports(ParameterInfo parameterInfo, ImportBuilder importBuilder)
            {
                if(this._importBuilders != null)
                {
                    Action<ImportBuilder> parameterImportBuilder;
                    if(this._importBuilders.TryGetValue(parameterInfo, out parameterImportBuilder))
                    {
                        parameterImportBuilder(importBuilder);
                    }
                }

                return;
            }

            private void ParseSelectConstructor(Expression<Func<ParameterImportBuilder, T>> constructorFilter)
            {
                Requires.NotNull(constructorFilter, "constructorFilter");

                var expr = Reduce(constructorFilter).Body;
                if(expr.NodeType != ExpressionType.New)
                {
                    throw ExceptionBuilder.Argument_ExpressionMustBeNew("constructorFilter");
                }
                var newExpression = (NewExpression)expr;
                this._constructorInfo = newExpression.Constructor;

                int index = 0;
                var parameterInfos = this._constructorInfo.GetParameters();

                foreach(var argument in newExpression.Arguments)
                {
                    if(argument.NodeType == ExpressionType.Call)
                    {
                        var methodCallExpression = (MethodCallExpression)argument;
                        if(methodCallExpression.Arguments.Count() == 1)
                        {
                            var parameter = methodCallExpression.Arguments[0];
                            if(parameter.NodeType == ExpressionType.Lambda)
                            {
                                var lambdaExpression = (LambdaExpression)parameter;
                                var importDelegate = lambdaExpression.Compile();
                                if(this._importBuilders == null)
                                {
                                    this._importBuilders = new Dictionary<ParameterInfo, Action<ImportBuilder>>();
                                }
                                this._importBuilders.Add(parameterInfos[index], (Action<ImportBuilder>)importDelegate);
                                ++index;
                            }
                        }
                    }
                }
            }

            private static Expression<Func<ParameterImportBuilder, T>> Reduce(Expression<Func<ParameterImportBuilder, T>> expr)
            {
                while(expr.CanReduce)
                {
                    expr.Reduce();
                }
                return expr;
            }
        }

        internal PartBuilder(Predicate<Type> selectType) : base(selectType)
        {
        }

        public PartBuilder<T> SelectConstructor(Expression<Func<ParameterImportBuilder, T>> constructorFilter)
        {
            Requires.NotNull(constructorFilter, "constructorFilter");

            var adapter = new ConstructorExpressionAdapter(constructorFilter);
            base.SelectConstructor(adapter.SelectConstructor, adapter.ConfigureConstructorImports);
            return this;
        }


        public PartBuilder<T> ExportProperty(Expression<Func<T, object>> propertyFilter)
        {
            return ExportProperty(propertyFilter, null);
        }

        public PartBuilder<T> ExportProperty(
            Expression<Func<T, object>> propertyFilter, 
            Action<ExportBuilder> exportConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            var adapter = new PropertyExpressionAdapter(propertyFilter, null, exportConfiguration);
            base.ExportProperties(adapter.VerifyPropertyInfo, adapter.ConfigureExport);
            return this;
        }

        public PartBuilder<T> ExportProperty<TContract>(Expression<Func<T, object>> propertyFilter)
        {
            return ExportProperty<TContract>(propertyFilter, null);
        }

        public PartBuilder<T> ExportProperty<TContract>(
            Expression<Func<T, object>> propertyFilter, 
            Action<ExportBuilder> exportConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            var adapter = new PropertyExpressionAdapter(propertyFilter, null, exportConfiguration);
            base.ExportProperties<TContract>(adapter.VerifyPropertyInfo, adapter.ConfigureExport);
            return this;
        }

        public PartBuilder<T> ImportProperty(Expression<Func<T, object>> propertyFilter) 
        {
            return ImportProperty(propertyFilter, null);
        }

        public PartBuilder<T> ImportProperty(
            Expression<Func<T, object>> propertyFilter, 
            Action<ImportBuilder> importConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            var adapter = new PropertyExpressionAdapter(propertyFilter, importConfiguration, null);
            base.ImportProperties(adapter.VerifyPropertyInfo, adapter.ConfigureImport);
            return this;
        }

        public PartBuilder<T> ImportProperty<TContract>(Expression<Func<T, object>> propertyFilter)
        {
            return ImportProperty<TContract>(propertyFilter, null);
        }

        public PartBuilder<T> ImportProperty<TContract>(
            Expression<Func<T, object>> propertyFilter, 
            Action<ImportBuilder> importConfiguration)
        {
            Requires.NotNull(propertyFilter, "propertyFilter");

            var adapter = new PropertyExpressionAdapter(propertyFilter, importConfiguration, null);
            base.ImportProperties<TContract>(adapter.VerifyPropertyInfo, adapter.ConfigureImport);
            return this;
        }
    }
}
