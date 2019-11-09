using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public class AudioManager
    {
        public async Task<bool> RecordAsync(string pathAudioFile)
        {
            bool result = false;
            try
            {
                DependencyService.Get<IRecordAudioService>().Start(pathAudioFile);
                result = !await App.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Audio, CommonResource.RoutePoint_AudioRecording, CommonResource.CommonMsg_Cancel, CommonResource.CommonMsg_OkAndSave);
            }
            catch (Exception e)
            {
                HandleError.Process("AudioManager", "Record", e, false);
            }
            finally
            {
                DependencyService.Get<IRecordAudioService>().Stop();
            }
            return result;
        }
    }
}
