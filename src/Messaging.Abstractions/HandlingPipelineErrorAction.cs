namespace ArchiScrapper.Messaging.Abstractions;

public enum HandlingPipelineErrorAction
{
    Retry = 0,
    Continue = 1,
    Stop = 2,
}