/*
    Name: Ross Taggart
    ID: S1828840
*/

namespace Honours_Project.Models
{
    /// <summary>
    /// Model to track general errors
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID of the request
        /// </summary>
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}