using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using MinioWebClient.Models;

namespace MinioWebClient.Controllers;

public class HomeController : Controller
{
    private readonly IAmazonS3 _storage;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IAmazonS3 storage,
        ILogger<HomeController> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var request = new ListObjectsRequest
        {
            BucketName = Config.MinioBucket
        };
        var listObjects = await _storage.ListObjectsAsync(request);
        var viewModel = new FileListViewModel
        {
            Files = listObjects.S3Objects
                .Select(x => new FileModel(x))
                .ToList()
        };
        
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] UploadForm form)
    {
        var formFile = form.File;
        _logger.LogInformation("Uploading file {FileName}", formFile.FileName);

        await using var fileStream = formFile.OpenReadStream();
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = Config.MinioBucket,
            Key = formFile.FileName,
            ContentType = formFile.ContentType,
            InputStream = fileStream
        };
        await _storage.PutObjectAsync(putObjectRequest);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Download([FromQuery] string key)
    {
        _logger.LogInformation("Requested downloading file {Key}", key);
        
        var getObjectMetadataRequest = new GetObjectMetadataRequest
        {
            BucketName = Config.MinioBucket,
            Key = key
        };
        var getObjectMetadata = await _storage.GetObjectMetadataAsync(getObjectMetadataRequest);
        var objectStream = await _storage.GetObjectStreamAsync(Config.MinioBucket, key, null);
        var fileName = Path.GetFileName(key);

        return File(objectStream, getObjectMetadata.Headers.ContentType, fileName);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}