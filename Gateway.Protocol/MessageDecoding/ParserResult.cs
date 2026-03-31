namespace Gateway.Protocol.MessageDecoding;

public readonly record struct Result<TPayload>(TPayload Payload, bool Ok, string ErrorMessage)
{
    public static Result<TPayload> Success(TPayload payload)
        => new(Payload: payload, Ok: true, ErrorMessage: string.Empty);

    public static Result<TPayload?> Failure(string errorMessage)
        => new(Payload: default, Ok: false, errorMessage);
}