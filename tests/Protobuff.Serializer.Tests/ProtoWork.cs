using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serialization.Proto.Schemas;
using Serialization.Proto.Schemas.Settings;
using Serialization.Proto.Serializer;


namespace VBAnalysisTests
{
    public static class ProtoWork
    {

        public static async Task<Tuple<string, string>> CreateSchemaAndSerializeObject<T>(string configObjectName) where T : class
        {

            var assemblyTypesPath = ConfigurationManager.AppSettings.Get("assembliesPath");
            string schemaOutputDirectory = SetupDirectories(ConfigurationManager.AppSettings.Get("outputSchemasPath"));
            string payloadOutputDirectory =SetupDirectories(ConfigurationManager.AppSettings.Get("outputBytesPath"));

            IEnumerable<TypeInfo> typesInfo = FetchTypes(assemblyTypesPath, filename => filename.EndsWith(".dll")).Where(type => EvaluatePredicates(type.FullName));
            
            string schema = GenerateSchema(typesInfo);

            var schemaFilePath = await SaveSchemaToFile(schemaOutputDirectory, configObjectName, schema);

            var bytesFilePath = await SavePayloadToFile<T>(payloadOutputDirectory, configObjectName);

            return new Tuple<string, string>(schemaFilePath, bytesFilePath);
        }

        private static async Task<string> SavePayloadToFile<T>(string payloadOutputDirectory, string configName) where T : class
        {
            var instanceJsonPath = ConfigurationManager.AppSettings.Get(configName);

            T instance = await GetInstance<T>(instanceJsonPath);
            

            Console.WriteLine("Serialize VBSubject object instance to protobuf... ");

            byte[] payload = Serialize<T>(instance);

            await SerializeInstancesToFile(payload, payloadOutputDirectory, $"{instance.GetType().Name}.dat");

            return payloadOutputDirectory + $"{instance.GetType().Name}.dat";
        }
        

        private static async Task<T> GetInstance<T>(string instanceJsonPath)
        {

            FileManager jsonFileReader = new FileManager();

            T instance = await jsonFileReader.ReadObjectAsync<T>(new FileStream(instanceJsonPath, FileMode.Open), default(CancellationToken));

            return instance;

        }

      

        private static byte[] Serialize<T>(T instance) where T : class
        {
            Serializer serializer = new Serializer();
            byte[] payload = serializer.Serialize<T>(instance);
            return payload;
        }

        private static async Task SerializeInstancesToFile(byte[] payload, string outputDirectory, string fileName = "/rootPayload.dat")
        {
            IFileManager fileoutput = new FileManager(new System.Uri(outputDirectory + fileName));
            await fileoutput.WriteAsync(payload);
        }

        private static async Task<string> SaveSchemaToFile(string outputDirectory, string name, string schema)
        {
            IFileManager fileoutput = new FileManager(new System.Uri(outputDirectory + $"/{name}.proto"));
            await fileoutput.WriteAsync(schema.ToString());

            return outputDirectory + $"/{name}.proto";
        }

        private static string GenerateSchema(IEnumerable<TypeInfo> typesInfo)
        {

            BuilderSettings settings = new BuilderSettings();
            //settings.Rules.Add(new BuilderRule()
            //{
            //    Predicate = (type) => type.CustomAttributes.FirstOrDefault((attr) => attr.AttributeType.Equals(typeof(VisionBox.ComponentModel.TypeIdAttribute))) == null,
            //    Type = RuleType.Exclude
            //});

            var generator = new ProtoSchemaBuilder(new ProtobuffSchemaRender(ProtoBuf.Meta.ProtoSyntax.Proto2), settings);
            string schema = generator.BuildSchema(typesInfo);
            return schema;
        }

        //private static async Task DoFluent(string assemblyTypesPath, string outputDirectory)
        //{
        //    await ProtoSchemaBuilder.Start()
        //                            .FetchTypes(assemblyTypesPath)
        //                            .BuildSchema()
        //                            .WriteSchemaAsync(outputDirectory);
        //}

       


        private static string SetupDirectories(string basePath, string outputPath)
        {
            //string outputDirectory = Environment.CurrentDirectory + "\\schemas\\";
            StringBuilder outputDirectory = new StringBuilder();// + "\\schemas\\";
            outputDirectory = outputDirectory.Append(basePath).Append("\\").Append(outputPath).Append("\\");
            if (!Directory.Exists(outputDirectory.ToString()))
            {
                Directory.CreateDirectory(outputDirectory.ToString());

            }

            return outputDirectory.ToString();

        }

        private static string SetupDirectories(string fullPath)
        {
            //string outputDirectory = Environment.CurrentDirectory + "\\schemas\\";
            StringBuilder outputDirectory = new StringBuilder();// + "\\schemas\\";

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);

            }

            return fullPath;

        }


        public static IEnumerable<TypeInfo> FetchTypes(string assembliesPath, Func<string, bool> predicate)
        {
            //string assembliesPath = ConfigurationManager.AppSettings.Get("assembliesPath");

            //var assemblyFiles = Directory.GetFiles(assembliesPath).Where(filename => filename.EndsWith(".dll")).Where(filename => EvaluatePredicates(filename));

            var assemblyFiles = Directory.GetFiles(assembliesPath).Where(predicate);
            List<TypeInfo> types = new List<TypeInfo>();

            foreach (var assemblyFileName in assemblyFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyFileName);

                    types.AddRange(assembly.DefinedTypes);
                }
                catch (Exception ex)
                {


                }

            }

            return types;
        }

        public static bool EvaluatePredicates(string fileName)
        {
            Dictionary<string, Func<string, string, bool>> predicates = new Dictionary<string, Func<string, string, bool>>();

            predicates.Add("assembly.filter.startWith", (value, match) => value.StartsWith(match));
            predicates.Add("assembly.filter.contains", (value, match) => value.Contains(match));
            predicates.Add("assembly.filter.endsWith", (value, match) => value.EndsWith(match));

            bool result = false;
            //var startWithRule = ConfigurationManager.AppSettings.GetValues("assembly.filter.startWith").FirstOrDefault();
            //var contains = ConfigurationManager.AppSettings.GetValues("assembly.filter.contains").FirstOrDefault();
            //var endsWith = ConfigurationManager.AppSettings.GetValues("assembly.filter.endsWith").FirstOrDefault();

            string[] rules = new string[] { "assembly.filter.contains", "assembly.filter.startWith", "assembly.filter.endsWith" };
            foreach (var rule in rules)
            {


                Func<string, string, bool> predicate = null;
                predicates.TryGetValue(rule, out predicate);
                if (!ConfigurationManager.AppSettings.AllKeys.Contains(rule))
                    continue;

                result = predicate == null ? false : (result || predicate(fileName, ConfigurationManager.AppSettings[rule]));

            }

            return result;
        }

    }
}
