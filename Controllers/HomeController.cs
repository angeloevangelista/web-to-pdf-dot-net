using System;
using WebToPdf.Util;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebToPdf.Errors;

namespace WebToPdf.Controllers
{
  [ApiController]
  [Route("")]
  public class HomeController : ControllerBase
  {
    [HttpGet]
    [Route("")]
    public async Task<Object> GetPdf(string url)
    {
      FileResult pdf = null;

      try
      {
        if (String.IsNullOrEmpty(url))
          throw new AppError("Envia a URL, imbecil.");

        if (!await CheckUrlIsValid.Execute(url))
          throw new AppError("URL inválida, imbecil.");

        pdf = await FetchPdf(url);
      }
      catch (Exception exception)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"An error occurred: ${exception}");
        Console.ForegroundColor = ConsoleColor.White;

        if (exception is AppError)
          return new
          {
            status = "ERROR",
            message = exception.Message
          };

        return new
        {
          status = "ERROR",
          message = "Internal server error."
        };
      }

      return pdf;
    }

    private async Task<FileContentResult> FetchPdf(string url)
    {
      await new BrowserFetcher().DownloadAsync(
         BrowserFetcher.DefaultChromiumRevision
       );

      var browser = await Puppeteer.LaunchAsync(new LaunchOptions
      {
        Headless = true,
        Args = new string[] {
          "--no-sandbox",
          "--disable-setuid-sandbox"
        }
      });

      var page = await browser.NewPageAsync();

      await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);

      var pdfData = await page.PdfDataAsync(new PdfOptions()
      {
        Format = PaperFormat.A4
      });

      await browser.CloseAsync();

      var response = File(pdfData, "application/pdf");

      return response;
    }
  }
}
