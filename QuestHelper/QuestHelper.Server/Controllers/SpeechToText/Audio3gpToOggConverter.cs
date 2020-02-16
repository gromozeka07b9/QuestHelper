using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

namespace QuestHelper.Server.Controllers.SpeechToText
{
    /// <summary>
    /// Конвертер файлов 3gp в ogg
    /// </summary>
    public class Audio3gpToOggConverter
    {
        public Audio3gpToOggConverter()
        {
            try
            {
                FFmpeg.ExecutablesPath = System.Environment.GetEnvironmentVariable("GoshFFMpegPath");
            }
            catch
            {
                FFmpeg.ExecutablesPath = string.Empty;
            }
        }

        public async Task<bool> ConvertAsync(string inputFileName, string outputFileName)
        {
            bool result = false;
            try
            {
                IMediaInfo mediaInfo = await MediaInfo.Get(inputFileName).ConfigureAwait(false);
                var audioStream = mediaInfo.AudioStreams.First();
                var conversion = Conversion.New()
                    .AddStream(audioStream)
                    .SetOutputFormat(MediaFormat.Ogg)
                    //.SetOutputTime(TimeSpan.FromSeconds(14))
                    //.SetAudioBitrate("11K")
                    .SetOverwriteOutput(true)
                    .AddParameter("-ac 1")
                    .SetOutput(outputFileName);
                var convertResult = await conversion.Start().ConfigureAwait(false);
                result = convertResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result;
        }
    }
}
