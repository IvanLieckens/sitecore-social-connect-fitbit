using System.Collections.Generic;

namespace Sitecore.Social.Fitbit.Model
{
    public class ErrorsResponse
    {
        public List<Error> Errors { get; set; }

        public class Error
        {
            public string ErrorType { get; set; }

            public string Message { get; set; }
        }
    }
}
