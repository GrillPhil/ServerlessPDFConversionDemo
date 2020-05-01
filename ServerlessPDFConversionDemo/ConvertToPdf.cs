using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ServerlessPDFConversionDemo
{
    public class ConvertToPdf
    {
        private readonly FileService _fileService;
        private readonly PdfOptions _options;

        public ConvertToPdf(FileService fileService, IOptions<PdfOptions> options)
        {
            _fileService = fileService;
            _options = options.Value;
        }

        [FunctionName("ConvertToPdf")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "convert")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            if (req.Headers.ContentLength == 0)
            {
                return new BadRequestObjectResult("Please provide a file.");
            }

            var path = $"{_options.GraphEndpoint}sites/{_options.SiteId}/drive/items/";

            var fileId = await _fileService.UploadStreamAsync(path, req.Body, req.ContentType);
            var pdf = await _fileService.DownloadConvertedFileAsync(path, fileId, "pdf");
            await _fileService.DeleteFileAsync(path, fileId);

            return new FileContentResult(pdf, "application/pdf");
        }
    }
}
