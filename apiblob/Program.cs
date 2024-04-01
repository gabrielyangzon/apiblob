using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Azure;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




string storageAccounts = "";


builder.Services.AddAzureClients(options =>
{
    options.AddBlobServiceClient(storageAccounts);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/weatherforecast", async (BlobServiceClient _blobServiceClient) =>
{
    var ss = new List<BlobContainerItem>();
    var containers = _blobServiceClient.GetBlobContainersAsync();

    await foreach (var item in containers)
    {
        ss.Add(item);
    }

    var containerClient = _blobServiceClient.GetBlobContainerClient(ss[0].Name);

    

    string name = "test.json";

    var blobList = new List<BlobItem>();
    var blobs = containerClient.GetBlobsAsync();
    await foreach (var item in blobs)
    {
        blobList.Add(item);
    }

    /// upload
    //var jsonAuthor = JsonSerializer.Serialize(new AuthorCreateDto() { FirstName = "123" , LastName="234" });
    //var response = await containerClient.UploadBlobAsync(name,new MemoryStream(Encoding.UTF8.GetBytes(jsonAuthor)));

     var bb = containerClient.GetBlobClient(blobList[0].Name);

    var json =  bb.DownloadContentAsync().Result;
    var author = JsonSerializer.DeserializeAsync<AuthorCreateDto>(json.Value.Content.ToStream()).Result;
    /// download
   
    return author;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();


public class AuthorCreateDto
{

    public string FirstName { get; set; }
    public string LastName { get; set; }

}