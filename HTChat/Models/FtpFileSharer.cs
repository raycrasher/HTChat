using agsXMPP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Models
{
    public class FtpFileSharer: INotifyPropertyChanged
    {
        private NetworkCredential _credentials;
        private WebClient _client;

        public string Username => Properties.Settings.Default.FtpUsername;
        public string Password => Properties.Settings.Default.FtpPassword;
        public string Host => Properties.Settings.Default.FtpHost;

        public long ProgressBytes { get; private set; }
        public long ProgressTotalBytes { get; private set; }
        public int ProgressPercent { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<(bool result, string link)> ShareFile(Jid jid, string filename)
        {
            string dir = (Host + "/" + jid.Bare).Replace(@"\20", "");

            _credentials = _credentials ?? new NetworkCredential(Username, Password);
            _client = new WebClient
            {
                Credentials = _credentials
            };
            _client.UploadProgressChanged += UploadProgressChanged;

            try
            {
                var request = WebRequest.Create(Host);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = _credentials;
                using(var response = await request.GetResponseAsync())
                {                    
                }
            }
            catch (WebException ex) { }

            try
            {
                
                var request = WebRequest.Create(dir);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = _credentials;
                using (var response = await request.GetResponseAsync())
                {
                }
            }
            catch (WebException ex) { }
            
            string uri = dir +"/" + Path.GetFileName(filename);
            try
            {
                var request = WebRequest.Create(uri);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = _credentials;
                using (var response = await request.GetResponseAsync())
                {
                }
            }
            catch(WebException ex)
            {
            }            

            try
            {
                await _client.UploadFileTaskAsync(new System.Uri(uri), "STOR", filename);
                ChatRenderer.TryCacheFile(uri, filename);
                return (true, uri);
                
            }
             catch(WebException ex)
            {
                Debug.Print(ex.ToString());
                return (false, null);
            }
        }

        private void UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            ProgressBytes = e.BytesSent;
            ProgressTotalBytes = e.TotalBytesToSend;
            ProgressPercent = e.ProgressPercentage;
        }
    }
}
