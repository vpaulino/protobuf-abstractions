using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VBAnalysisTests
{
    public interface IFileManager
    {

        Task<T> ReadObjectAsync<T>(Stream stream, CancellationToken cancelationToken);

        Task<JObject> ReadJsonAsync(Stream stream, CancellationToken cancelationToken);

        Task WriteAsync(string output);

        Task WriteAsync(byte[] payload);
    }
}