namespace Sigma.Web.Frontend.Core.Filters
{
    using System;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Web.Http.ModelBinding;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateActionParametersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var descriptor = context.ActionDescriptor as ReflectedHttpActionDescriptor;

            if (descriptor != null)
            {
                var parameters = descriptor.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    var argument = context.ActionArguments[parameter.Name];

                    EvaluateValidationAttributes(parameter, argument, context.ModelState);
                }
            }

            base.OnActionExecuting(context);
        }

        private void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            var validationAttributes = parameter.CustomAttributes;

            foreach (var attributeData in validationAttributes)
            {
                var attributeInstance = CustomAttributeExtensions.GetCustomAttribute(parameter, attributeData.AttributeType);

                var validationAttribute = attributeInstance as ValidationAttribute;

                if (validationAttribute != null)
                {
                    var isValid = validationAttribute.IsValid(argument);
                    if (!isValid)
                    {
                        modelState.AddModelError($"::{parameter.Name}", validationAttribute.FormatErrorMessage(parameter.Name));
                    }
                }
            }
        }
    }
}