namespace OpenCqs2.Abstractions
{
    public interface IHandler
    {
    }

    public class HandlerResult<T>
    {
        public enum HandlerResults
        {
            Success,
            Fail
        }

        public HandlerResult()
        {
            this.Code = HandlerResults.Success;
        }

        public HandlerResult(HandlerResults code, T? result = default)
        {
            this.Code = code;
            this.Result = result;
        }

        public static HandlerResult<T> OK => new(HandlerResults.Success);

        public HandlerResults Code { get; init; }

        public T? Result { get; init; }

        public override string ToString()
        {
            return $"{(this.Code == HandlerResults.Fail ? "ERROR: " : string.Empty)}{this.Result}";
        }
    }

    public class HandlerResult : HandlerResult<string>
    {
        public HandlerResult(HandlerResults code, string? message = default) : base(code, message)
        {
        }

        public new static HandlerResult OK => new(HandlerResults.Success);
    }
}