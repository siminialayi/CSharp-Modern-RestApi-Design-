using System.Net;

namespace Blog.Api.Models.ErrorResponses
    {
        /// <summary>
        /// Standardized API error response model 
        /// </summary>
    public class ErrorResponse
        {
            public HttpStatusCode Status { get; set; }
            public string Title { get; set; } = "An error occurred";
            public string Detail { get; set; }
            public Dictionary<string, string[]> Errors { get; set; } = [];

            
        }
    }
