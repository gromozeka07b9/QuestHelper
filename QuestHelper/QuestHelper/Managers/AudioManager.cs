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

        static IRecordAudioService srvInstance = DependencyService.Get<IRecordAudioService>();

        public bool RecordStart(string pathAudioFile)
        {
            bool started = false;
            try
            {
                srvInstance.Start(pathAudioFile);
                started = true;
            }
            catch (Exception e)
            {
                HandleError.Process("AudioManager", "RecordStart", e, false);
            }
            return started;
        }
        public void RecordStop()
        {
            try
            {
                srvInstance.Stop();
            }
            catch (Exception e)
            {
                HandleError.Process("AudioManager", "RecordStop", e, false);
            }
        }
    }
}
