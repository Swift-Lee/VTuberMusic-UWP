using LitJson;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Network.GetTools;
using VTuberMusic.Tools;

namespace VTuberMusic.Modules
{
    class Account
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public object Birthday { get; set; }
        public string Avatar { get; set; }
        public int Sex { get; set; }
        public string SexText { get; set; }
        public object BirthdayText { get; set; }
        public string Level { get; set; }
        public int Exp { get; set; }
        public int TotalExp { get; set; }
        public int LikeMusicTotal { get; set; }
        public object LikeVtuber { get; set; }

        public static Account GetAccountInfo(string Token)
        {
            var client = new RestClient(GetTools.ApiUri);
            client.Authenticator = new JwtAuthenticator(Token);
            var request = new RestRequest("/v1/GetNavInfo");
            var response = client.Post(request); // 请求 Apiq
            if (response.IsSuccessful)
            {
                GetModules.UserGetModules jsonData = JsonMapper.ToObject<GetModules.UserGetModules>(response.Content); // 转 Json 为对象
                if (jsonData.Success)
                {
                    return jsonData.Data.UserInfo;
                }
                else
                {
                    Log.WriteLine("请求失败:\r\n" + response.Content, Tools.Level.Error);
                    throw new Exception(jsonData.Msg.ToString());
                }
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static string GetToken(string User, string Password)
        {
            string postJson = JsonMapper.ToJson(new GetModules.LoginPostModule { userName = User, password = Password });
            var response = GetTools.PostApi("/v1/SubmitLogin", postJson); // 请求 Api
            if (response.IsSuccessful)
            {
                GetModules.LoginGetModules jsonData = JsonMapper.ToObject<GetModules.LoginGetModules>(response.Content); // 转 Json 为对象
                if (jsonData.Success)
                {
                    return jsonData.Data;
                }
                else
                {
                    Log.WriteLine("请求失败:\r\n" + response.Content, Tools.Level.Error);
                    throw new Exception(jsonData.Msg.ToString());
                }
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}
