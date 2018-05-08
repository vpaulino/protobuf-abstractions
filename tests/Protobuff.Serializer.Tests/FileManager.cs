using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VBAnalysisTests
{
    public class FileManager : IFileManager
    {
        Uri filePath;
        



        public FileManager(Uri filePath)
        {
            this.filePath = filePath;
            
        }

        public FileManager()
        {
                

        }

        public async Task WriteAsync(string output)
        {
            using (StreamWriter fileStream = new StreamWriter(this.filePath.AbsolutePath, true))
            {
                await fileStream.WriteAsync(output);
                await fileStream.FlushAsync();

            }
        }

        public async Task WriteAsync(byte[] payload)
        {
            using (FileStream file = new FileStream(this.filePath.AbsolutePath, FileMode.Create))
            {
                await file.WriteAsync(payload, 0, payload.Length);
                await file.FlushAsync();
            }
            
        }

        public async Task<JObject> ReadJsonAsync(Stream stream, CancellationToken cancelationToken)
        {
           
            var serializer = new JsonSerializer();
            string fileText = null;
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
               fileText = await jsonTextReader.ReadAsStringAsync(cancelationToken);              
            }

            return JObject.Parse(fileText);
        }

        public async Task<T> ReadObjectAsync<T>(Stream stream, CancellationToken cancelationToken)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
               return serializer.Deserialize<T>(jsonTextReader);
            }

        }



    }
}
