using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Com.GGIT.Security.Authorization
{
    public class AceAuthToken
    {
        private const string Auth_Username = "MSP_Notification_Auth_Username";
        private const string Auth_Password = "MSP_Notification_Auth_Password";
        private const string Auth_URL = "MSP_Notification_Auth_URL";

        #region Get Ace Token Methods
        public async Task<AuthorizationTokenResponse> GetAuthorizationTokenAsync()
        {
            AuthSettingValues authSettings = GetAuthSettings();
            var response = new AuthorizationTokenResponse();
            try
            {
                if (authSettings != null)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes(authSettings.AuthUsername + ":" + authSettings.AuthPassword)));

                        HttpResponseMessage result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, authSettings.AuthUrl)
                        {
                            Content = new FormUrlEncodedContent(new Dictionary<string, string> 
                            {
                                { "grant_type", "client_credentials" }
                            })
                        });

                        string respContent = await result.Content.ReadAsStringAsync();

                        TokenResponse jsonResponse = JsonConvert.DeserializeObject<TokenResponse>(respContent);

                        if (jsonResponse != null)
                        {
                            response.AccessToken = jsonResponse.AccessToken;
                            response.GetTokenSuccess = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Do nothing
            }
            return response;
        }
        #endregion

        #region Get settings from MSP_Setting
        private AuthSettingValues GetAuthSettings()
        {
            string[] settingStringArray = { Auth_Username, Auth_Password, Auth_URL };

            try
            {
                using (var session = new SessionDB().OpenSession())
                {
                    var setttingValue = session.Query<MSP_Setting>().Where(k => settingStringArray.Contains(k.SettingKey))
                        .Select(x => new
                        {
                            x.SettingKey,
                            x.SettingValue
                        }).Take(settingStringArray.Count())
                        .ToList();

                    AuthSettingValues request = new AuthSettingValues
                    {
                        AuthUsername = setttingValue.Where(o => o.SettingKey == Auth_Username).Select(o => o.SettingValue).FirstOrDefault(),
                        AuthPassword = setttingValue.Where(o => o.SettingKey == Auth_Password).Select(o => o.SettingValue).FirstOrDefault(),
                        AuthUrl = setttingValue.Where(o => o.SettingKey == Auth_URL).Select(o => o.SettingValue).FirstOrDefault(),
                    };

                    return request;
                }
            }catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }

    public class AuthorizationTokenResponse
    {
        public bool GetTokenSuccess { get; set; }
        public string AccessToken { get; set; }
    }

    public class AuthSettingValues
    {
        public string AuthUsername { get; set; }
        public string AuthPassword { get; set; }
        public string AuthUrl { get; set; }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public string ExporesIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
