using System.Text.Json;

namespace PereViader.Scrolller;

public interface IScrolllerExtractor<T>
{
    T Extract(JsonDocument jsonDocument);
}