namespace Sigma.Web.Http.Exceptions
{
    using System;
    using System.Web.Http.ModelBinding;

    [Serializable]
    public class ModelStateValidationException : HttpRootException
    {
        public ModelStateValidationException(ModelStateDictionary modelState)
        {
            this.ModelState = modelState;
        }

        public ModelStateValidationException() { }
        public ModelStateValidationException(string message) : base(message) { }
        public ModelStateValidationException(string message, Exception inner) : base(message, inner) { }
        protected ModelStateValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public ModelStateDictionary ModelState { get; }
    }
}