namespace Services.HandleResponses
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultMessageForStatusCode(int code)
            => code switch
            {
                400 => "Bad Request",
                401 => "You're not authorized",
                404 => "Resourse not found",
                500 => "Enternal Server Error",
                _ => null
            };
    }
}
