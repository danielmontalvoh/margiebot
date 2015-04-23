﻿using BazamWPF.UIHelpers;
using BazamWPF.ViewModels;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace MargieBot.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HubConnection _Hub = null;
        private string _WebSocketUrl = string.Empty;

        public ICommand CallCommand
        {
            get {
                return new RelayCommand(async (timeToParty) => {
                    _Hub = new HubConnection(_WebSocketUrl);
                    _Hub.Received += (string message) => { Message = message; };
                    await _Hub.Start();
                });
            }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { ChangeProperty<MainWindowViewModel>(vm => vm.Message, value); }
        }

        public MainWindowViewModel()
        {
            Message = "Starting up...";
            WebRequest request = WebRequest.Create("https://slack.com/api/rtm.start");
            byte[] body = Encoding.UTF8.GetBytes("token=xoxb-4597209409-Sy4JJEX6GblzmKrdF9mPngy7");
            request.Method = "POST";
            request.ContentLength = body.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            using(BinaryWriter writer = new BinaryWriter(request.GetRequestStream())) {
                writer.Write(body);
            }

            string responseJson = string.Empty;
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseJson = reader.ReadToEnd();
            }

            JObject jObject = JObject.Parse(responseJson);
            _WebSocketUrl = jObject["url"].Value<string>();
        }
    }
}