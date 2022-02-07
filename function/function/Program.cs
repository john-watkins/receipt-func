using AwsApi;
using GoogleApi.mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenFaas.Secrets;
using OpenFaaS;
using OpenFaaS.Hosting;
using System;
using System.Net;

Runner.Run(args, builder =>
{
    builder.Services.ConfigureFunction().AddNewtonsoftJson();
    builder.Logging.AddJsonConsole();
    builder.Configuration
        .AddUserSecrets<Program>(true)
        .AddOpenFaaSSecrets();

    // add your services to the container
    builder.Services.AddFuncServices();
}, app =>
{
    // configure the HTTP request pipeline

    app.MapPost("/", async (HttpContext context, IMailRepo mailRepo, IAwsS3 s3) =>
    {
        if (!context.Request.HasJsonContentType())
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
            return;
        }
        var request = await context.Request.ReadFromJsonAsync<GmailQuery>();
        if (request == null)
        {
            throw new System.Exception("Error reading gmail query json");
        }
        DateTime dtNow = DateTime.Now;
        DateTime lastSync = LastSyncHelper.GetLastSync();
        var msgs = await mailRepo.GetEmailMessages(request.Email, request.GmailLabel, lastSync);
        foreach (var item in msgs)
        {
            await s3.UploadFileToS3(item.Raw, item.FileName, item.Created);
        }
        LastSyncHelper.WriteLastSync(dtNow);
        await context.Response.WriteAsJsonAsync(new
        {
            Message = $"Sucessfully uploaded email files (.eml) to S3 bucket for {request.Email}, label: {request.GmailLabel}"
        });
    });

    app.MapGet("/mail", async (HttpContext context, IMailRepo mailRepo, IAwsS3 s3) =>
    {
        DateTime dtNow = DateTime.Now;
        DateTime lastSync = LastSyncHelper.GetLastSync();
        var msgs = await mailRepo.GetEmailMessages("john.watkins@springdox.com", "receipts", lastSync);
        foreach (var item in msgs)
        {
            await s3.UploadFileToS3(item.Raw, item.FileName, item.Created);
        }
        LastSyncHelper.WriteLastSync(dtNow);
        return msgs;
    });

});
