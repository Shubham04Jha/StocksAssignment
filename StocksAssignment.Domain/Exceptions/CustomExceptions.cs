using System;

namespace StocksAssignment.Domain.Exceptions
{
    // Thrown at API while mapping 
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    // Thrown when external Services like gRPC are unavailable
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    // Thrown when database connection or execution queries fail
    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
