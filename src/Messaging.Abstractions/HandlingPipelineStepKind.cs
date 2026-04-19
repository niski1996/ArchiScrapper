namespace ArchiScrapper.Messaging.Abstractions;

public enum HandlingPipelineStepKind
{
    Infrastructure = 0,
    Business = 1,
    Consumer = 2,
}