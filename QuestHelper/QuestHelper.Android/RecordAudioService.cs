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
                recorder.SetOutputFormat(OutputFormat.Default);
                recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                recorder.SetAudioChannels(1);
                recorder.SetAudioEncodingBitRate(96000);
                //recorder.SetAudioSamplingRate(44100);
                //recorder.SetAudioEncodingBitRate(48000);
                //recorder.SetAudioSamplingRate(22050);
                recorder.SetOutputFile(filePath);
                recorder.Prepare(); // Prepared state
                //recorder.SetMaxDuration(30000);
                recorder.Start(); // Recording state.
            }
            catch (Exception e)
            {
                HandleError.Process("RecordAudioService", "Start", e, false);
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
                HandleError.Process("RecordAudioService", "Stop", e, false);
            }
        }
    }
}