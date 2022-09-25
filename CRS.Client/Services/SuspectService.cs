using CRS.Domain;

using Newtonsoft.Json;

using SourceAFIS.Simple;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRS.Client.Services
{
    public class SuspectService
    {
        HttpClient httpClient;
        private AfisEngine _afis;
        List<CitizenResponse> _citizenResponses = new();
        public SuspectService()
        {
            _afis = new AfisEngine();
            httpClient = new()
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
            Load();
        }
        async void Load()
        {
            try
            {
                var response = await httpClient.GetAsync("api/get-all");
                var result = await response.ToResult<List<CitizenResponse>>();
                _citizenResponses = result.Data;
            }
            catch (Exception)
            {

                _citizenResponses = new();
            }

        }
        async Task<IResult<CitizenResponse>> Identify(Bitmap bmp)
        {

            if (!_citizenResponses.Any())
            {
                return Result<CitizenResponse>.Fail();
            }
            try
            {

                var allPersons = new List<Person>();
                foreach (var citizen in _citizenResponses)
                {
                    var person = new Person
                    {
                        Id = citizen.Id,
                    };

                    var ms = new MemoryStream(citizen.FingerPrintData);
                    Bitmap bitmap = new(ms);
                    Fingerprint fp = new()
                    {
                        AsBitmap = bitmap
                    };
                    person.Fingerprints.Add(fp);

                    _afis.Extract(person);
                    allPersons.Add(person);


                }
                var match = ValidateFingerprint(bmp, allPersons);
                if (match != null)
                {
                    return Result<CitizenResponse>.Success(match);
                }
                return Result<CitizenResponse>.Fail("No Match Found");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                return Result<CitizenResponse>.Fail(ex.Message + ex.StackTrace);
            }
        }
        public async Task<CitizenResponse> GetDetails(Bitmap bitmap)
        {
            var result = await Identify(bitmap);
            if(result.Succeeded)
              return result.Data;
            return null;
        }
        private CitizenResponse ValidateFingerprint(Bitmap bitmap, List<Person> allPersons)
        {
            var unknownPerson = new Person();
            var fingerprint = new Fingerprint();
            fingerprint.AsBitmap = bitmap;

            unknownPerson.Fingerprints.Add(fingerprint);
            _afis.Extract(unknownPerson);

            var match = _afis.Identify(unknownPerson, allPersons).FirstOrDefault();
            if (match == null)
            {
                return null;
            }
            return _citizenResponses.FirstOrDefault(r => r.Id == match.Id);

        }
        public async Task<IResult> Add(SuspectRequest suspect)
        {
            httpClient.BaseAddress = new Uri("");
            var response = await httpClient.PostAsJsonAsync("api/suspects", suspect);
            if(response.IsSuccessStatusCode)
            {
                return await response.ToResult();
            }
            return Result.Fail("Could not connect to server");
        }
    }
}
