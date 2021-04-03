using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebToPdf.Util
{
  public static class CheckUrlIsValid
  {
    public static async Task<bool> Execute(string url)
    {
      bool isValid = false;
      var httpClient = new HttpClient();

      try
      {
        await httpClient.GetAsync(url);

        isValid = true;
      }
      catch (Exception)
      { }

      return isValid;
    }
  }
}