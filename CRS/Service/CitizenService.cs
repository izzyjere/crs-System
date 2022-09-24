using CRS.Domain;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRS.Service
{
    public class CitizenService
    {
        HttpClient httpClient = new();
        public CitizenService()
        {
            httpClient.BaseAddress = new( "https://localhost:5001/");
        }
        public async Task<string> GetNRCNumber()
        {
            var response = await httpClient.GetAsync("api/nrc-number");
            var res = await response.ToResult<string>();
            if(res.Succeeded)
            {
                return res.Data;
            }
            return string.Empty;
        }
        public async Task<IResult> CreateCitizen(CitizenRequest citizen)
        {
            var response = await httpClient.PostAsJsonAsync("api/create", citizen);
            if (response.IsSuccessStatusCode)
            {
                return await response.ToResult();
            }
            return Result.Fail("Could not connect to server.");
        }         
    }
}
