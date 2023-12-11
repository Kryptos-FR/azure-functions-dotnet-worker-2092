using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Converters;

namespace CustomBinding.GooglePubSub.Worker;

internal sealed partial class GPubSubControllerConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.TargetType != typeof(IGPubSubController))
        {
            return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
        }

        if (context.Source is not string controllerConfigText)
        {
            return new ValueTask<ConversionResult>(ConversionResult.Failed(new InvalidOperationException(
                $"Expected the GooglePubSub extension to send a string payload for {nameof(GPubSubBindingAttribute)}.")));
        }

        try
        {
            var data = JsonSerializer.Deserialize<ControllerConfig>(controllerConfigText);
            var controller = new GPubSubController(data, context.FunctionContext.GetLogger<GPubSubController>(), /*FIXME*/null);
            return new ValueTask<ConversionResult>(ConversionResult.Success(controller));
        }
        catch (Exception innerException)
        {
            InvalidOperationException exception = new(
                $"Failed to convert the input binding context data into a {nameof(IGPubSubController)} object. The data may have been delivered in an invalid format.",
                innerException);
            return new ValueTask<ConversionResult>(ConversionResult.Failed(exception));
        }
    }
}
