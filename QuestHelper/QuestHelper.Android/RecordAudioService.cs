using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(RecordAudioService))]
namespace QuestHelper.Droid
{
    public class RecordAudioService : IRecordAudioService
    {
        protected MediaRecorder recorder;

        public void Start(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                if (recorder == null)
                {
                    recorder = new MediaRecorder(); // Initial state.
                }
                recorder.Reset();
                recorder.SetAudioSource(AudioSource.Mic);
                recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                recorder.SetAudioEncoder(AudioEncoder.Aac);
                recorder.SetAudioChannels(2);
                recorder.SetAudioEncodingBitRate(96000);
                recorder.SetAudioSamplingRate(44100);
                recorder.SetOutputFile(filePath);
                recorder.Prepare(); // Prepared state
                //recorder.SetMaxDuration(30000);
                recorder.Start(); // Recording state.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Stop()
        {
            try
            {
                recorder.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}