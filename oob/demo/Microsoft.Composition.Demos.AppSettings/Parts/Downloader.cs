using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSettingsExtensionDemo.Extension;

namespace AppSettingsExtensionDemo.Parts
{
    [Export]
    public class Downloader
    {
        readonly string _serverUrl;
        readonly string _username;

        [ImportingConstructor]
        public Downloader(
            [Setting("serverUrl")] string serverUrl,
            [Setting("username")] string username)
        {
            _serverUrl = serverUrl;
            _username = username;
        }

        public void Download()
        {
            Console.WriteLine("Downloading from {0} as {1}...", _serverUrl, _username);
        }
    }
}
