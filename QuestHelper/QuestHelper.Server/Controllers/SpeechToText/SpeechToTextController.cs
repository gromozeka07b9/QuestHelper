using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Integration;
using QuestHelper.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.SpeechToText
{
    /// <summary>
    /// Методы обработки запросов на распознавание аудиофайлов
    /// </summary>
    //[Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class SpeechToTextController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private string _pathToMediaCatalog = string.Empty;
        private MediaManager _mediaManager;
        
        /// <summary>
        /// Распознавание аудио
        /// </summary>
        public SpeechToTextController()
        {
            _mediaManager = new MediaManager();
            _pathToMediaCatalog = _mediaManager.PathToMediaCatalog;
        }

        /// <summary>
        /// Запуск распознавания всех необработанных аудиофайлов
        /// </summary>
        /// <returns></returns>
        /*[HttpPost("parse/all")]
        public async Task TrySpeechParseAllAsync()
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            SpeechToTextProcess speachToTextProcess = new SpeechToTextProcess(_pathToMediaCatalog);
            var result = await speachToTextProcess.TrySpeechParseAllAsync();
            Response.StatusCode = result ? 200 : 500;
        }*/

        /// <summary>
        /// Распознавание одного файла (media)
        /// </summary>
        /// <param name="id">routepointmediaobjectId</param>
        /// <returns></returns>
        [HttpGet("parse/routepointmediaobject/{id}")]
        public async Task<IActionResult> TrySpeechParseAsync(string id)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            
            SpeechToTextProcess speachToTextProcess = new SpeechToTextProcess(_pathToMediaCatalog);
            var parseResult = await speachToTextProcess.TrySpeechParseMediaAsync(id);
            Response.StatusCode = parseResult.Result ? 200 : 500;

            return new ObjectResult(parseResult.Text);
        }

    }
}
