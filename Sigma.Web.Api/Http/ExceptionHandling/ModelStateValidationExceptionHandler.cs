namespace Sigma.Web.Http.ExceptionHandling
{
    using System.Net;
    using System;
    using System.Collections.Generic;
    using Sigma.Web.Http.Exceptions;

    public class ModelStateValidationExceptionHandler : ToEndUserExceptionHandler<ModelStateValidationException>
    {
        public ModelStateValidationExceptionHandler() : base(HttpStatusCode.BadRequest, "Validation constraints failed.")
        {
        }

        public override Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            var modelStateException = exception as ModelStateValidationException;
            System.Diagnostics.Contracts.Contract.Assert(modelStateException != null);

            var instance = (EndUserException)base.HandleException(exception, handlingInstanceId);

            var errors = new List<EndUserException.OperationErrorEntry>();
            foreach (var state in modelStateException.ModelState)
            {
                var key = state.Key;

                string code;
                if (string.IsNullOrEmpty(key))
                {
                    code = $"{instance.ErrorCode}.Model"; ;
                }
                else if (key.StartsWith("::"))
                {
                    code = $"{instance.ErrorCode}.Model.{key.Replace("::", "")}";
                }
                else
                {
                    code = $"{instance.ErrorCode}.Field.{key}";
                }

                foreach (var error in state.Value.Errors)
                {
                    var message = (error.ErrorMessage ?? error.Exception?.Message) ?? "unexpected error";

                    var errorEntry = new EndUserException.OperationErrorEntry
                    {
                        Code = code,
                        Message = message,
                        Exception = error.Exception
                    };

                    errors.Add(errorEntry);
                }
            }

            instance.Errors = errors.ToArray();

            return instance;
        }
    }
}