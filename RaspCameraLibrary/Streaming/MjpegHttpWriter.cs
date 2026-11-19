using System.Text;

namespace RaspCameraLibrary.Streaming;

/// <summary>
/// Writes multipart MJPEG frames to any stream (e.g. ASP.NET response body).
/// </summary>
public static class MjpegHttpWriter
{
    public const string Boundary = "--frame";
    public const string ContentType = "multipart/x-mixed-replace; boundary=--frame";

    public static byte[] CreateHeader(int contentLength)
    {
        var header = $"{Boundary}\r\nContent-Type: image/jpeg\r\nContent-Length: {contentLength}\r\n\r\n";
        return Encoding.ASCII.GetBytes(header);
    }

    public static byte[] CreateFooter() => Encoding.ASCII.GetBytes("\r\n");

    public static async Task WriteFrameAsync(
        Stream output,
        byte[] jpegFrame,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(jpegFrame);

        await output.WriteAsync(CreateHeader(jpegFrame.Length), cancellationToken).ConfigureAwait(false);
        await output.WriteAsync(jpegFrame, cancellationToken).ConfigureAwait(false);
        await output.WriteAsync(CreateFooter(), cancellationToken).ConfigureAwait(false);
        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
