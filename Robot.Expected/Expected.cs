using System;

namespace Robot.Expected
{
    public class Expected<T>
    {
        public int? ErrorCode { get; }
        public string ErrorMessage { get; }

        public Exception Exception { get; set; }

        private T _value;
        public T Value {
            get {
                if (HasError)
                    throw new InvalidOperationException("Value is not avaliable. An error code, however, is!");
                return _value;
            }
        }

        #region Implicit Operators

        public static implicit operator T(Expected<T> exp) => exp.Value;

        public static implicit operator Expected<T>(T value) => new Expected<T>(value);

        #endregion

        #region Static Methods

        public static Expected<T> Error(int errorCode, string message = null)
        {
            return new Expected<T>(errorCode, message);
        }

        public static Expected<T> ErrorFormat(int errorCode, string message, params object[] values)
        {
            return Error(errorCode, string.Format(message, values));
        }

        public static Expected<T> Error(Exception exception, int errorCode, string message = null)
        {
            return new Expected<T>(errorCode, message, exception);
        }

        public static Expected<T> ErrorFormat(Exception exception, int errorCode, string message, params object[] values)
        {
            return Error(exception, errorCode, string.Format(message, values));
        }

        public static Expected<T> ConvertErrorFrom<S>(Expected<S> expected)
        {
            if (expected.HasValue)
                throw new InvalidOperationException("Object cannot be cast, for it is not an error.");

            return Error(expected.Exception, expected.ErrorCode.Value, expected.ErrorMessage);
        }

        #endregion

        #region Constructors

        public Expected(T value)
        {
            _value = value;
        }

        private Expected(int errorCode, string errorMsg = null, Exception exception = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMsg;
            Exception = exception;
        }

        #endregion

        #region Instance Methods

        public bool HasValue => !ErrorCode.HasValue;

        public bool HasError => ErrorCode.HasValue;

        #endregion
    }
}
