using agsXMPP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HTChat.Models
{
    static class ChatRenderer
    {
        public static string[] AllowedImageExtensions = new[] { ".bmp", ".jpeg", ".jpg", ".png", ".tiff", ".gif", ".ico" };
        public static readonly Regex WebLinkRegex = new Regex(@"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?");
        public static readonly Dictionary<string, ImageSource> ImageCache = new Dictionary<string, ImageSource>();
        private static NetworkCredential _ftpCredentials;
        
        public static IEnumerable<Inline> RenderMessageHeader(Jid author, DateTime timestamp)
        {
            var contact = ChatClient.Instance.JidToContact(author);
            yield return new InlineUIContainer(new Image() {
                Source = contact.Avatar,
                Width = 16,
                Height = 16,
                Margin = new Thickness(64 + 5, 0, 5, 0) });

            yield return new Run(contact.DisplayName) { FontWeight =  FontWeights.Bold };
            yield return new LineBreak();
        }

        public static Block RenderChatMessage(Jid author, string message, DateTime timestamp)
        {
            var paragraph = new Paragraph();
            paragraph.TextIndent = -(64 + 5);
            paragraph.Inlines.AddRange(RenderMessageHeader(author, timestamp));
            //paragraph.Inlines.Add(new Run(message));
            var text = message;
            Run run;
            int lastIndex = 0;
            // add links, if available
            foreach (Match match in WebLinkRegex.Matches(text))
            {
                if (match.Index > lastIndex)
                {
                    run = new Run(text.Substring(lastIndex, match.Index - lastIndex));
                    paragraph.Inlines.Add(run);
                }

                run = new Run(match.Value);
                Hyperlink hlink = new Hyperlink(run);
                hlink.NavigateUri = new System.Uri(match.Value);
                hlink.IsEnabled = true;
                hlink.RequestNavigate += (o, e) =>
                {
                    Process.Start(new ProcessStartInfo(match.Value));
                    e.Handled = true;
                };
                paragraph.Inlines.Add(run);
                var progressRing = new InlineUIContainer(new MahApps.Metro.Controls.ProgressRing
                {
                    Height = 16,
                    Width = 16,
                    Margin = new Thickness(5, 0, 0, 0)
                });

                paragraph.Inlines.Add(progressRing);

                if (ImageCache.TryGetValue(match.Value, out var bmp))
                    RenderImage(bmp, paragraph, progressRing);
                else
                    Task.Run(() => RenderWebLink(match.Value, paragraph, progressRing));

                lastIndex = match.Index + match.Value.Length;
            }
            if (lastIndex < text.Length)
            {
                run = new Run(text.Substring(lastIndex));
                //run.Tag = tag;
                paragraph.Inlines.Add(run);
            }

            return paragraph;
        }

        internal static void TryCacheFile(string uri, string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            if (AllowedImageExtensions.Contains(ext)) {
                ImageCache[uri] = new BitmapImage(new System.Uri(filename));
            }
        }

        public static async Task RenderWebLink(string value, Paragraph paragraph, InlineUIContainer progressRing)
        {
            try
            {
                _ftpCredentials = _ftpCredentials ?? new NetworkCredential(Properties.Settings.Default.FtpUsername, Properties.Settings.Default.FtpPassword);

                var request = WebRequest.Create(value);

                if (request is FtpWebRequest req)
                {
                    req.Method = WebRequestMethods.Ftp.DownloadFile;
                    string extension = Path.GetExtension(req.RequestUri.LocalPath).ToLowerInvariant();
                    if (AllowedImageExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
                    {
                        if (value.StartsWith(Properties.Settings.Default.FtpHost))
                            req.Credentials = _ftpCredentials;
                        using (FtpWebResponse response = (FtpWebResponse)await req.GetResponseAsync())
                        {
                            Stream responseStream = response.GetResponseStream();
                            var outputPath = Path.Combine(App.ReceivedFilesFolder, Path.GetFileName(req.RequestUri.LocalPath));
                            var outputPathFilename = Path.GetFileNameWithoutExtension(outputPath);
                            var dir = Path.GetDirectoryName(outputPath);
                            int tries = 1;
                            while (File.Exists(outputPath))
                            {
                                outputPath = Path.Combine(dir, outputPathFilename) + tries.ToString() + extension;
                                tries++;
                            }

                            using (var outFile = new FileStream(outputPath, FileMode.CreateNew))
                            {
                                await responseStream.CopyToAsync(outFile);
                            }

                            Dispatch.InvokeUI(() =>
                            {
                                if (!File.Exists(outputPath))
                                {
                                    paragraph.Inlines.Remove(progressRing);
                                    return;
                                }
                                var bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.UriSource = new System.Uri(outputPath, UriKind.Absolute);
                                bitmapImage.EndInit();
                                ImageCache[value] = bitmapImage;
                                RenderImage(bitmapImage, paragraph, progressRing);
                            });
                        }
                    }
                    else
                    {
                        Dispatch.InvokeUI(() => paragraph.Inlines.Remove(progressRing));
                    }
                }
            }
            catch (Exception)
            {
                Dispatch.InvokeUI(() => paragraph.Inlines.Remove(progressRing));
            }

        }

        public static void RenderImage(ImageSource bitmapImage, Paragraph paragraph, InlineUIContainer progressRing)
        {
            try
            {
                var image = new Image();
                image.Source = bitmapImage;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Stretch = Stretch.Uniform;

                var lb1 = new LineBreak();
                var iui = new InlineUIContainer(image);
                var lb2 = new LineBreak();

                paragraph.Inlines.InsertAfter(progressRing, lb1);
                paragraph.Inlines.InsertAfter(lb1, iui);
                paragraph.Inlines.InsertAfter(iui, lb2);
            }
            catch (Exception)
            {

            }
            finally
            {
                paragraph.Inlines.Remove(progressRing);
            }
        }
    }
}
