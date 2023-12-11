using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Converters;

namespace CustomBinding.SFTP.Worker;

internal sealed partial class SFTPControllerConverter : IInputConverter
{
    private readonly TelemetryClient _telemetryClient;

    public SFTPControllerConverter(TelemetryConfiguration telemetryConfiguration)
    {        
        this._telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.TargetType != typeof(ISFTPController))
        {
            return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
        }

        if (context.Source is not string controllerConfigText)
        {
            return new ValueTask<ConversionResult>(ConversionResult.Failed(new InvalidOperationException(
                $"Expected the SFTP extension to send a string payload for {nameof(SFTPBindingAttribute)}.")));
        }

        try
        {
            var data = JsonSerializer.Deserialize<ControllerConfig>(controllerConfigText);
            var controller = new SFTPController(data, context.FunctionContext.GetLogger<SFTPController>(), _telemetryClient);
            return new ValueTask<ConversionResult>(ConversionResult.Success(controller));
        }
        catch (Exception innerException)
        {
            InvalidOperationException exception = new(
                $"Failed to convert the input binding context data into a {nameof(ISFTPController)} object. The data may have been delivered in an invalid format.",
                innerException);
            return new ValueTask<ConversionResult>(ConversionResult.Failed(exception));
        }
    }
}
